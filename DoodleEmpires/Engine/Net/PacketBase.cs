using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public class PacketBase : IPacket
    {
        public event EventHandler<IPacket> OnReceived;

        public Lidgren.Network.NetDeliveryMethod DeliveryMethod
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Lidgren.Network.NetConnection Reciepient
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SendToAll
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public uint PacketID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //protected virtual void BuildPacketHandlers()
        //{
        //    Type thisType = this.GetType();
        //    Type baseType = typeof(IPacket);

        //    PropertyInfo[] properties = thisType.GetProperties();

        //    _writeAction = new Action<NetOutgoingMessage>(X => { });
        //    _readAction = new Action<NetIncomingMessage>(X => { });

        //    foreach (PropertyInfo property in properties)
        //    {
        //        if (baseType.GetProperty(property.Name) == null)
        //        {
        //            Type returnType = property.PropertyType;

        //            if (returnType.GetInterface("INetworkable") != null)
        //            {
        //                _writeAction += (msg => ((INetworkable)property.GetValue(this)).Write(msg));
        //                _readAction += (msg => property.SetValue(this, ((INetworkable)property).Read(msg)));
        //            }
        //            else
        //            {
        //                Type msgType = typeof(NetOutgoingMessage);
        //                MethodInfo[] methods = msgType.GetMethods();

        //                foreach (MethodInfo method in methods)
        //                {
        //                    if (method.Name == "Write" && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == returnType)
        //                    {
        //                        _writeAction += (msg => method.Invoke(msg, new[] { property.GetValue(this) }));
        //                        break;
        //                    }
        //                }

        //                msgType = typeof(NetIncomingMessage);
        //                methods = msgType.GetMethods();

        //                foreach (MethodInfo method in methods)
        //                {
        //                    if (method.Name.StartsWith("Read") && !method.Name.StartsWith("ReadVariable") && method.GetParameters().Length == 0 && method.ReturnType == returnType)
        //                    {
        //                        _readAction += (msg => property.SetValue(this, method.Invoke(msg, new object[] { })));
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public void Handle(Lidgren.Network.NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        public void Send(Lidgren.Network.NetOutgoingMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
