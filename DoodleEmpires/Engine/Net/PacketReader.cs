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
    public class PacketReader
    {
        //private uint _packetID = 0;
        //private Func<NetIncomingMessage, IPacket> _readAction;

        //public event EventHandler<IPacket> OnReceived;

        //public uint PacketID
        //{
        //    get
        //    {
        //        return _packetID;
        //    }
        //    set
        //    {
        //        _packetID = value;
        //    }
        //}

        //public Func<NetIncomingMessage, IPacket> ReadAction
        //{
        //    get { return _readAction; }
        //}

        //public PacketReader(Type typeToHandle)
        //{
        //    BuildPacketHandler(typeToHandle);
        //}

        //protected virtual void BuildPacketHandler(Type typeToHandle)
        //{
        //    if (typeToHandle.GetInterface("IPacket") != null)
        //    {
        //        Type baseType = typeof(IPacket);

        //        PropertyInfo[] properties = typeToHandle.GetProperties();

        //        Action<NetIncomingMessage, IPacket> loadProperties = new Action<NetIncomingMessage, IPacket>((msg, item) => { });
        //        CodeMemberMethod readMethod = new CodeMemberMethod();
        //        readMethod.ReturnType = new CodeTypeReference(typeToHandle);
        //        readMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NetIncomingMessage), "msg"));

        //        foreach (PropertyInfo property in properties)
        //        {
        //            if (baseType.GetProperty(property.Name) == null)
        //            {
        //                Type returnType = property.PropertyType;

        //                if (returnType.GetInterface("INetworkable") != null)
        //                {
        //                    loadProperties += (msg, item) => property.SetValue(item, ((INetworkable)Activator.CreateInstance(property.PropertyType)).Read(msg));
        //                    readMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(returnType), "Read", new CodeArgumentReferenceExpression("msg")));
        //                }
        //                else
        //                {
        //                    Type msgType = typeof(NetIncomingMessage);
        //                    MethodInfo[] methods = msgType.GetMethods();

        //                    foreach (MethodInfo method in methods)
        //                    {
        //                        if (method.Name.StartsWith("Read") && !method.Name.StartsWith("ReadVariable") && method.GetParameters().Length == 0 && method.ReturnType == returnType)
        //                        {
        //                            loadProperties += (msg, item) => property.SetValue(item, method.Invoke(msg, new object[] { }));
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        _readAction = new Func<NetIncomingMessage, IPacket>(msg => 
        //        {
        //            IPacket packet = (IPacket)Activator.CreateInstance(typeToHandle);
        //            loadProperties(msg, packet);
        //            return packet; 
        //        });
        //    }
        //}

        //public virtual void Handle(NetIncomingMessage msg)
        //{
        //    IPacket data = _readAction(msg);

        //    if (OnReceived != null)
        //        OnReceived.Invoke(this, data);
        //}

        /// <summary>
        /// Builds a static CodeDOM packet reader for the given type
        /// </summary>
        /// <param name="typeToHandle">The type to build a reader for</param>
        /// <returns>A CodeMemberMethod that can be compiled to read a packet from a NetIncomingMessage</returns>
        public static CodeMemberMethod BuildPacketHandler_DOM(Type typeToHandle, string name, MemberAttributes attributes)
        {
            if (typeToHandle.IsSubclassOf(typeof(IPacket))) // Ensure that this type is a packet
            {
                // cache the packet type
                Type baseType = typeof(IPacket);

                // Get all the properties in the packet
                PropertyInfo[] properties = typeToHandle.GetProperties();

                // Create the method
                CodeMemberMethod readMethod = new CodeMemberMethod();
                readMethod.Name = name;
                readMethod.Attributes = attributes;
                // Set the return type to the packet's type
                readMethod.ReturnType = new CodeTypeReference(typeToHandle);
                // Add the incomming message to the parameters as "msg"
                readMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NetIncomingMessage), "msg"));

                // Add XML documentation to this method
                readMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
                readMethod.Comments.Add(new CodeCommentStatement(string.Format("Reads a {0} from an incoming packet", typeToHandle.Name), true));
                readMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
                readMethod.Comments.Add(new CodeCommentStatement(string.Format("<param name = \"{0}\">{1}</param>", "msg", "The message to read from"), true));
                readMethod.Comments.Add(new CodeCommentStatement(string.Format("<returns>{0}</returns>", "The packet read from the message"), true));

                // Define a variable to store the packet currently being read in
                readMethod.Statements.Add(new CodeVariableDeclarationStatement(typeToHandle, "packet"));
                // Assign a new value to the packet
                readMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("packet"), new CodeObjectCreateExpression(typeToHandle)));

                // Iterate over all of the packet's properties
                foreach (PropertyInfo property in properties)
                {
                    if (baseType.GetProperty(property.Name) == null) // Make sure this is not an inherited member
                    {
                        // Get the type of the current property
                        Type returnType = property.PropertyType;

                        // Get a code snippet to access the property in the packet
                        CodePropertyReferenceExpression packetVariable = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("packet"), property.Name);

                        if (returnType.GetInterface("INetworkable") != null) // If the type implements the INetworkable interface
                        {
                            // Create a code snippet to read the object from the packet
                            CodeMethodInvokeExpression readINetworkable = new CodeMethodInvokeExpression(new CodeObjectCreateExpression(returnType), "Read", new CodeArgumentReferenceExpression("msg"));
                            // Cast the INetworkable instance to the propertie's type
                            CodeCastExpression castExpression = new CodeCastExpression(returnType, readINetworkable);
                            // Add the statements to the method
                            readMethod.Statements.Add(new CodeAssignStatement(packetVariable, castExpression));
                        }
                        else // If the type does not implement the INetworkable interface
                        {
                            // Get the type for the incoming message
                            Type msgType = typeof(NetIncomingMessage);
                            // Get the methods in NetIncomingMessage
                            MethodInfo[] methods = msgType.GetMethods();

                            foreach (MethodInfo method in methods) // Iterate over all methods
                            {
                                // If the method will read the correct variable type out, use it then break out of the loop
                                if (method.Name.StartsWith("Read") && !method.Name.StartsWith("ReadVariable") && method.GetParameters().Length == 0 && method.ReturnType == returnType)
                                {
                                    // Create a code snippet to read the variable out of the message
                                    CodeMethodInvokeExpression read = new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("msg"), method.Name);
                                    // Add the statement to the method
                                    readMethod.Statements.Add(new CodeAssignStatement(packetVariable, read));
                                    //Exit the loop
                                    break;
                                }
                            }
                        }
                    }
                }
                // Create a method to invoke the OnReveived method
                CodeMethodInvokeExpression eventInvoke = new CodeMethodInvokeExpression(new CodeEventReferenceExpression(null, "OnReceived"), "Invoke", new CodeThisReferenceExpression(), new CodeVariableReferenceExpression("packet"));
                
                // Make an if statement to check if the event is null
                CodeConditionStatement tryInvokeEvent = new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(new CodeEventReferenceExpression(null, "OnReceived"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)));
                // If the event is not equal to null, invoke the event
                tryInvokeEvent.TrueStatements.Add(eventInvoke);

                // Add the code statement to the method
                readMethod.Statements.Add(tryInvokeEvent);

                // Add a return statement to return the packet
                readMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("packet")));
                return readMethod;
            }
            else
                return null;
        }
    }
}
