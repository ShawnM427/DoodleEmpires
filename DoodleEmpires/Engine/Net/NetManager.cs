using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Net
{
    public class NetManager : IDisposable
    {
        Action<NetIncomingMessage> _unknownAction;
        Action<NetIncomingMessage>[] _packetActions;
        NetPacketIdentifierSize _idSize;
        bool _running = true;
        NetPeer _peer;
        Thread _mainThread;

        /// <summary>
        /// Gets or sets the netpeer to listen to
        /// </summary>
        public virtual NetPeer Peer
        {
            get { return _peer; }
            set
            {
                if (_running == false)
                    _peer = value;
                else
                    throw new InvalidOperationException("Cannot set peer while listener is running!");
            }
        }

        /// <summary>
        /// Creates a new net manager
        /// </summary>
        /// <param name="idSize">The size of packet ID's to read/write</param>
        /// <param name="peer">The peer to listen to</param>
        public NetManager(NetPacketIdentifierSize idSize, NetPeer peer)
        {
            _idSize = idSize;
            _peer = peer;

            _packetActions = new Action<NetIncomingMessage>[(int)idSize];
            _unknownAction = OnUnknowPacketReceived;

            _mainThread = new Thread(PerformRun);
            _mainThread.IsBackground = true;
        }

        /// <summary>
        /// Starts running this net manager, note that this call will run the networking on a new thread
        /// </summary>
        public void Run()
        {
            if (_peer != null)
                _mainThread.Start();
            else
                throw new ArgumentNullException("Peer to listen to cannot be null!");
        }

        /// <summary>
        /// The backend method for running the listener
        /// </summary>
        protected virtual void PerformRun()
        {
            while (_running) // as long as the listener is active
            {
                NetIncomingMessage msg;
                if ((msg = _peer.ReadMessage()) != null) // if there is a message available
                {
                    switch (_idSize) // how we read the ID depends on the NetPacketIdentifierSize
                    {
                        case NetPacketIdentifierSize.Byte:
                            (_packetActions[msg.ReadByte()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                            break;
                        case NetPacketIdentifierSize.Short:
                            (_packetActions[msg.ReadUInt16()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                            break;
                        case NetPacketIdentifierSize.Int:
                            (_packetActions[msg.ReadUInt32()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("No declared handler for ID size {0}", _idSize));
                    }
                }
            }
        }

        /// <summary>
        /// Called when an unknown packet was received, default is to shut down the peer
        /// </summary>
        protected virtual void OnUnknowPacketReceived(NetIncomingMessage msg)
        {
            _running = false;
            _peer.Shutdown(string.Format("Could not parse packet"));
            throw new InvalidOperationException("Unknown packetID received!");
        }

        public virtual void ImportPacketType(IPacket type) 
        {
            int ID = Array.FindIndex(_packetActions, packet => packet == null);
            type.PacketID = ID;
            _packetActions[ID] = type.Handle;
        }

        public virtual void SendMessage(IPacket packet) 
        {
            NetOutgoingMessage msg = _peer.CreateMessage();

            switch (_idSize) // how we read the ID depends on the NetPacketIdentifierSize
            {
                case NetPacketIdentifierSize.Byte:
                    msg.Write((byte)packet.PacketID);
                    break;
                case NetPacketIdentifierSize.Short:
                    msg.Write((ushort)packet.PacketID);
                    break;
                case NetPacketIdentifierSize.Int:
                    msg.Write((uint)packet.PacketID);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("No declared handler for ID size {0}", _idSize));
            }

            packet.Send(msg);

            if (packet.SendToAll)
                _peer.SendMessage(msg, _peer.Connections, packet.DeliveryMethod, 0);
            else
                _peer.SendMessage(msg, packet.Reciepient, packet.DeliveryMethod, 0);
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _unknownAction = null;
            _packetActions = null;
            _peer.Shutdown("Listener closed");
        }
    }
}
