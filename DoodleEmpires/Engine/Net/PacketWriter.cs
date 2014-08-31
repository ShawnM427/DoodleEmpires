using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DoodleEmpires.Engine.Net
{
    public class PacketWriter
    {
        private Type _myType;
        private uint _packetID = 0;
        private Action<NetOutgoingMessage, IPacket> _writeAction;

        /// <summary>
        /// gets or sets the delivery method to use for this packet type
        /// </summary>
        public NetDeliveryMethod DeliveryMethod { get; set; }
        /// <summary>
        /// Gets or sets the target reciepient for this packet
        /// </summary>
        public NetConnection Reciepient { get; set; }
        /// <summary>
        /// Gets or sets wheter this packet should be sent to all clients
        /// </summary>
        public bool SendToAll { get; set; }
                
        public uint PacketID
        {
            get
            {
                return _packetID;
            }
            set
            {
                _packetID = value;
            }
        }
        public Type HandledType
        {
            get { return _myType; }
        }

        public Action<NetOutgoingMessage, IPacket> WriteAction
        {
            get { return _writeAction; }
        }

        public PacketWriter(Type typeToHandle)
        {
            DeliveryMethod = NetDeliveryMethod.ReliableOrdered;
            Reciepient = null;
            SendToAll = true;

            BuildPacketHandler(typeToHandle);
        }

        protected virtual void BuildPacketHandler(Type typeToHandle)
        {
            if (typeToHandle.GetInterface("IPacket") != null)
            {
                _myType = typeToHandle;
                Type baseType = typeof(IPacket);

                PropertyInfo[] properties = typeToHandle.GetProperties();

                _writeAction = new Action<NetOutgoingMessage, IPacket>((msg, item) => { });

                foreach (PropertyInfo property in properties)
                {
                    if (baseType.GetProperty(property.Name) == null)
                    {
                        Type returnType = property.PropertyType;

                        if (returnType.GetInterface("INetworkable") != null)
                        {
                            _writeAction += (msg, item) => ((INetworkable)property.GetValue(item)).Write(msg);
                        }
                        else
                        {
                            Type msgType = typeof(NetIncomingMessage);
                            MethodInfo[] methods = msgType.GetMethods();

                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.StartsWith("Write") &&  method.GetParameters().Length == 1 && method.ReturnType == returnType)
                                {
                                    _writeAction += (msg, item) => ((INetworkable)property.GetValue(item)).Write(msg); ;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Write(NetOutgoingMessage msg, IPacket packet)
        {
            if (packet.GetType() == _myType)
                _writeAction(msg, packet);
        }
    }
}
