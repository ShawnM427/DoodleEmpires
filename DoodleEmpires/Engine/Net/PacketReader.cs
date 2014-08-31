using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DoodleEmpires.Engine.Net
{
    public class PacketReader
    {
        private uint _packetID = 0;
        private Func<NetIncomingMessage, IPacket> _readAction;
        
        public event EventHandler<IPacket> OnReceived;
        
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

        public Func<NetIncomingMessage, IPacket> ReadAction
        {
            get { return _readAction; }
        }

        public PacketReader(Type typeToHandle)
        {
            BuildPacketHandler(typeToHandle);
        }

        protected virtual void BuildPacketHandler(Type typeToHandle)
        {
            if (typeToHandle.GetInterface("IPacket") != null)
            {
                Type baseType = typeof(IPacket);

                PropertyInfo[] properties = typeToHandle.GetProperties();

                Action<NetIncomingMessage, IPacket> loadProperties = new Action<NetIncomingMessage, IPacket>((msg, item) => { });

                foreach (PropertyInfo property in properties)
                {
                    if (baseType.GetProperty(property.Name) == null)
                    {
                        Type returnType = property.PropertyType;

                        if (returnType.GetInterface("INetworkable") != null)
                        {
                            loadProperties += (msg, item) => property.SetValue(item, ((INetworkable)Activator.CreateInstance(property.PropertyType)).Read(msg));
                        }
                        else
                        {
                            Type msgType = typeof(NetIncomingMessage);
                            MethodInfo[] methods = msgType.GetMethods();

                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.StartsWith("Read") && !method.Name.StartsWith("ReadVariable") && method.GetParameters().Length == 0 && method.ReturnType == returnType)
                                {
                                    loadProperties += (msg, item) => property.SetValue(item, method.Invoke(msg, new object[] { }));
                                    break;
                                }
                            }
                        }
                    }
                }
                _readAction = new Func<NetIncomingMessage, IPacket>(msg => 
                {
                    IPacket packet = (IPacket)Activator.CreateInstance(typeToHandle);
                    loadProperties(msg, packet);
                    return packet; 
                });
            }
        }

        public virtual void Handle(NetIncomingMessage msg)
        {
            IPacket data = _readAction(msg);

            if (OnReceived != null)
                OnReceived.Invoke(this, data);
        }
    }
}
