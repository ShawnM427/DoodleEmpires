using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom;

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeToHandle"></param>
        /// <returns></returns>
        public static CodeMemberMethod BuildPacketHandler_DOM(Type typeToHandle, string name, MemberAttributes attributes)
        {
            if (typeToHandle.IsSubclassOf(typeof(IPacket)))
            {
                Type baseType = typeof(IPacket);

                PropertyInfo[] properties = typeToHandle.GetProperties();

                CodeMemberMethod writeMethod = new CodeMemberMethod();
                writeMethod.Name = name;
                writeMethod.Attributes = attributes;
                writeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NetOutgoingMessage), "msg"));
                writeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeToHandle, "packet"));
                writeMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                writeMethod.Comments.Add(new CodeCommentStatement(string.Format("Writes a {0} to an outgoing packet", typeToHandle.Name), true));
                writeMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                writeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name = \"{0}\">{1}</param>", "msg", "The outgoing message to write to"), true));
                writeMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name = \"{0}\">{1}</param>", "packet", "The packet to write to the message"), true));

                foreach (PropertyInfo property in properties)
                {
                    if (baseType.GetProperty(property.Name) == null)
                    {
                        Type returnType = property.PropertyType;

                        if (returnType.GetInterface("INetworkable") != null)
                        {
                            writeMethod.Statements.Add(new CodeCommentStatement(string.Format("Writes {0}.{1} to the message", typeToHandle.Name, property.Name)));
                            CodeMethodInvokeExpression writeToPacket = new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("packet"), property.Name), "Write", new CodeArgumentReferenceExpression("msg"));
                            writeMethod.Statements.Add(writeToPacket);
                        }
                        else
                        {
                            Type msgType = typeof(NetOutgoingMessage);
                            MethodInfo[] methods = msgType.GetMethods();

                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.Equals("Write") && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == returnType)
                                {
                                    writeMethod.Statements.Add(new CodeCommentStatement(string.Format("Writes {0}.{1} to the message", typeToHandle.Name, property.Name)));
                                    writeMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("msg"), method.Name), new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("packet"), property.Name)));
                                    break;
                                }
                            }
                        }
                    }
                }
                return writeMethod;
            }
            else
                return null;
        }

        public virtual void Write(NetOutgoingMessage msg, IPacket packet)
        {
            if (packet.GetType() == _myType)
                _writeAction(msg, packet);
        }
    }
}
