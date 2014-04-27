using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Lidgren.Network;

namespace MasterServer
{
    public partial class Form1 : Form
    {
        volatile bool _quitingServer;
        public Form1()
        {
            InitializeComponent();
        }

        List<IPEndPoint[]> _hosts = new List<IPEndPoint[]>();
        List<User> _users = new List<User>();

        NetPeerConfiguration config = new NetPeerConfiguration("masterserver");

        private void RunServer()
        {
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            config.Port = 14542;
            NetPeer peer = new NetPeer(config);
            peer.Start();

            while (!_quitingServer)
            {
                NetIncomingMessage msg;
                while ((msg = peer.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.UnconnectedData:
                            //
                            // We've received a message from a client or a host
                            //

                            // by design, the first byte always indicates action
                            switch ((MasterServerMessageType)msg.ReadByte())
                            {
                                #region Register a new host
                                case MasterServerMessageType.RegisterHost:
                                    // It's a host wanting to register its presence
                                    IPEndPoint[] eps = new IPEndPoint[]
                                                                        {
                                                                                msg.ReadIPEndpoint(), // internal
                                                                                msg.SenderEndpoint // external
                                                                        };
                                    Console.WriteLine("Got registration for host " + eps[1]);
                                    _hosts.Add(eps);
                                    break;
                                #endregion

                                #region Requesting a list of hosts
                                case MasterServerMessageType.RequestHostList:
                                    // It's a client wanting a list of registered hosts
                                    Console.WriteLine("Sending list of " + _hosts.Count + " hosts to client " + msg.SenderEndpoint);
                                    foreach (IPEndPoint[] ep in _hosts)
                                    {
                                        // send registered host to client
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write(ep[0]);
                                        om.Write(ep[1]);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                    }
                                    break;
                                #endregion

                                #region Requesting NAT introduction
                                case MasterServerMessageType.RequestIntroduction:
                                    // It's a client wanting to connect to a specific (external) host
                                    IPEndPoint clientInternal = msg.ReadIPEndpoint();
                                    IPEndPoint hostExternal = msg.ReadIPEndpoint();
                                    string token = msg.ReadString();

                                    Console.WriteLine(msg.SenderEndpoint + " requesting introduction to " + hostExternal + " (token: " + token + ")");

                                    // find in list
                                    foreach (IPEndPoint[] elist in _hosts)
                                    {
                                        if (elist[1].Equals(hostExternal))
                                        {
                                            // found in list - introduce client and host to eachother
                                            Console.WriteLine("Sending introduction...");
                                            peer.Introduce(
                                                    elist[0], // host internal
                                                    elist[1], // host external
                                                    clientInternal, // client internal
                                                    msg.SenderEndpoint, // client external
                                                    token // request token
                                            );
                                            break;
                                        }
                                    }
                                    break;
                                #endregion

                                #region Requesting profile registration
                                case MasterServerMessageType.RequestRegister:
                                    string UNE = msg.ReadString();
                                    string PE = msg.ReadString();

                                    string UND = UNE;
                                    string PD = PE;

                                    if (_users.Find(X => X.Name == UND) == null)
                                    {
                                        _users.Add(new User(UND, PD));

                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)MasterServerMessageType.SuccesfullRegistration);
                                        om.Write(_users.LongCount() - 1);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                    }
                                    else
                                    {
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)MasterServerMessageType.FailedRegistration);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                    }
                                    break;
                                #endregion

                                #region Requesting Login and AUTH token
                                case MasterServerMessageType.RequestLogin:
                                    UNE = msg.ReadString();
                                    PE = msg.ReadString();

                                    UND = UNE;
                                    PD = PE;

                                    if (_users.Find(X => X.Name == UND) != null)
                                    {
                                        bool valid = _users.Find(X => X.Name == UND).IsUser(UND, PE);

                                        if (valid)
                                        {
                                            NetOutgoingMessage om = peer.CreateMessage();
                                            om.Write((byte)MasterServerMessageType.SuccesfullLogin);
                                            peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                        }
                                        else
                                        {
                                            NetOutgoingMessage om = peer.CreateMessage();
                                            om.Write((byte)MasterServerMessageType.FailedLogin);
                                            peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                        }
                                    }
                                    else
                                    {
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)MasterServerMessageType.FailedLogin);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndpoint);
                                    }
                                    break;
                                #endregion
                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            // print diagnostics message
                            Console.WriteLine(msg.ReadString());
                            break;
                    }
                }
            }

            peer.Shutdown("shutting down");

        }

        private class User
        {
            string _name;
            string _password;

            /// <summary>
            /// Gets a user's name
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            public User(string name, string password)
            {
                _name = name;
                _password = password;
            }

            /// <summary>
            /// Checks if the given name and password matches this user profile
            /// </summary>
            /// <param name="name">The name of the user</param>
            /// <param name="encryptedPassword">The encrypted user password</param>
            /// <returns>True if this is the user</returns>
            public bool IsUser(string name, string encryptedPassword)
            {
                return name == _name ? encryptedPassword == _password : false;
            }
        }

        public enum MasterServerMessageType : byte
        {
            RegisterHost = 0,
            RequestHostList = 1,
            RequestIntroduction = 2,
            RequestRegister = 3,
            RequestLogin = 4,
            SuccesfullRegistration = 5,
            FailedRegistration = 6,
            SuccesfullLogin = 7,
            FailedLogin = 8
        }
    }

}
