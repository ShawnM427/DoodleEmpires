using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Net
{
    public class NetManager : IDisposable
    {
        Action<NetIncomingMessage> _noAction;
        Action<NetIncomingMessage> _unknownAction;
        Action<NetIncomingMessage>[] _packetHandlers;

        Action<NetIncomingMessage> _onDiscoveryRequest;
        Action<NetIncomingMessage> _onDiscoveryResponse;
        Action<NetIncomingMessage> _onStatusChanged;
        Action<NetIncomingMessage> _onConnectionApproval;

        NetPacketIdentifierSize _idSize;
        bool _running = true;
        bool _handleUnconnectedData = false;
        NetPeer _peer;
        Thread _mainThread;

        Dictionary<Type, PacketWriter> _writers;

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

            _packetHandlers = new Action<NetIncomingMessage>[(int)idSize];
            _unknownAction = OnUnknowPacketReceived;
            _noAction = OnNoActionAvailable;
            _writers = new Dictionary<Type, PacketWriter>();

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
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            (_onConnectionApproval ?? _noAction).Invoke(msg);
                            break;

                        case NetIncomingMessageType.DiscoveryRequest:
                            (_onDiscoveryRequest ?? _noAction).Invoke(msg);
                            break;

                        case NetIncomingMessageType.DiscoveryResponse:
                            (_onDiscoveryResponse ?? _noAction).Invoke(msg);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            (_onStatusChanged ?? _noAction).Invoke(msg);
                            break;
                            
                        case NetIncomingMessageType.Data:
                            HandlePacket(msg);
                            break;

                        case NetIncomingMessageType.UnconnectedData:
                            if (_handleUnconnectedData) HandlePacket(msg);
                            break;

                        default:
                            _noAction.Invoke(msg);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles a packet by reading its packetID and passing it to the correct Packet handler
        /// </summary>
        /// <param name="msg">The incomming message to parse</param>
        public virtual void HandlePacket(NetIncomingMessage msg)
        {
            switch (_idSize) // how we read the ID depends on the NetPacketIdentifierSize
            {
                case NetPacketIdentifierSize.Byte:
                    (_packetHandlers[msg.ReadByte()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                    break;
                case NetPacketIdentifierSize.Short:
                    (_packetHandlers[msg.ReadUInt16()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                    break;
                case NetPacketIdentifierSize.Int:
                    (_packetHandlers[msg.ReadUInt32()] ?? _unknownAction).Invoke(msg); // handle the packet with the given ID, if none exist perform default action
                    break;
                default:
                    throw new InvalidOperationException(string.Format("No declared handler for ID size {0}", _idSize));
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

        /// <summary>
        /// Called when an action is not defined for the message type
        /// </summary>
        protected virtual void OnNoActionAvailable(NetIncomingMessage msg)
        {
            Debug.WriteLine("Did not handle message with type \"{0}\"", new[] { msg.MessageType });
        }
        
        /// <summary>
        /// Adds a packet to the list of packets to handle
        /// </summary>
        /// <param name="type">The packet to handle</param>
        public virtual void ImportPacketType(Type type, EventHandler<IPacket> onCalled)
        {
            if (_packetHandlers.Contains(null) && type.GetInterface("IPacket") != null)
            {
                uint ID = (uint)Array.FindIndex(_packetHandlers, packet => packet == null);
                PacketReader reader = new PacketReader(type);
                reader.OnReceived += onCalled;
                _writers.Add(type, new PacketWriter(type) { PacketID = ID});
                reader.PacketID = ID;
                _packetHandlers[ID] = reader.Handle;
            }
            else
                throw new InvalidOperationException("The maximum number of packets has been reached!");
        }

        /// <summary>
        /// Sends a packet using this net manager
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public virtual void SendMessage(IPacket packet) 
        {
            NetOutgoingMessage msg = _peer.CreateMessage();
            PacketWriter writer = _writers[packet.GetType()];

            switch (_idSize) // how we read the ID depends on the NetPacketIdentifierSize
            {
                case NetPacketIdentifierSize.Byte:
                    msg.Write((byte)writer.PacketID);
                    break;
                case NetPacketIdentifierSize.Short:
                    msg.Write((ushort)writer.PacketID);
                    break;
                case NetPacketIdentifierSize.Int:
                    msg.Write((uint)writer.PacketID);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("No declared handler for ID size {0}", _idSize));
            }

            writer.Write(msg, packet);

            if (writer.SendToAll)
                _peer.SendMessage(msg, _peer.Connections, writer.DeliveryMethod, 1);
            else
                _peer.SendMessage(msg, writer.Reciepient, writer.DeliveryMethod, 0);
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _unknownAction = null;
            _packetHandlers = null;
            _peer.Shutdown("Listener closed");
        }
    }
}
