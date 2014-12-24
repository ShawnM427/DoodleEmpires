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
using DoodleEmpires.Engine.Net;
using System.Threading;

namespace MasterServer
{
    public partial class MainWindow : Form
    {
        volatile bool _quitingServer;
        Thread _mainThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        List<ServerInfo> _hosts = new List<ServerInfo>();
        List<User> _users = new List<User>();

        NetPeerConfiguration config;

        public void RemoveListItem(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(RemoveListItem), new object[] { text });
                return;
            }
            lst_knownServers.Items.RemoveByKey(text);            
        }

        public void AddListItem(ListViewItem item)
        {

            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<ListViewItem>(AddListItem), new object[] { item });
                return;
            }
            lst_knownServers.Items.Add(item);
        }
        
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            rtb_info.Text += value + "\r";

            rtb_info.SelectionStart = rtb_info.Text.Length;
            rtb_info.ScrollToCaret();
        }

        public void AppendTextBox(string value, object arg1)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string, object>(AppendTextBox), new object[] { value, arg1 });
                return;
            }
            rtb_info.Text += string.Format(value, arg1) + "\r";

            rtb_info.SelectionStart = rtb_info.Text.Length;
            rtb_info.ScrollToCaret();
        }

        public void AppendTextBox(string value, object arg1, object arg2)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string, object, object>(AppendTextBox), new object[] { value, arg1, arg2 });
                return;
            }
            rtb_info.Text += string.Format(value, arg1, arg2) + "\r";

            rtb_info.SelectionStart = rtb_info.Text.Length;
            rtb_info.ScrollToCaret();
        }

        private void RunServer()
        {
            config = new NetPeerConfiguration("DoodleEmpiresMaster");
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            config.Port = GlobalNetVars.MASTER_SERVER_PORT;
            config.EnableUPnP = chk_upnp.Checked;
            NetPeer peer = new NetPeer(config);
            peer.Start();

            if (chk_upnp.Checked)
            {
                IPAddress external = peer.UPnP.GetExternalIP();
                peer.UPnP.ForwardPort(config.Port, "DoodleEmpiresMaster");
            }

            AppendTextBox("Starting master sever on {0}:{1}", peer.Configuration.BroadcastAddress, peer.Port);

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
                            switch ((NetPacketType)msg.ReadByte())
                            {
                                #region Request the public encryption key
                                case NetPacketType.MASTER_RequestPublicKey:
                                    // send registered host to client
                                    NetOutgoingMessage ret = peer.CreateMessage();
                                    ret.Write((byte)NetPacketType.MASTER_ReturnPublicKey, 8);
                                    ret.Write(Encryption._publicKey);
                                    peer.SendUnconnectedMessage(ret, msg.SenderEndPoint);
                                    break;
                                #endregion

                                #region Register a new host
                                case NetPacketType.MASTER_RegisterHost:
                                    // It's a host wanting to register its presence
                                    //IPEndPoint[] eps = new IPEndPoint[]
                                    //                                    {
                                    //                                            msg.ReadIPEndpoint(), // internal
                                    //                                            msg.SenderEndpoint // external
                                    //                                    };
                                    ServerInfo newInfo = ServerInfo.ReadFromPacket(msg);

                                    if (!_hosts.Contains(newInfo))
                                    {
                                        newInfo.ExternalEndPoint = msg.SenderEndPoint;
                                        _hosts.Add(newInfo);
                                        AppendTextBox("Got registration for host \"{0}\"", _hosts.Last().Name);
                                        AddListItem(new ListViewItem(_hosts.Last().Name) { Tag = _hosts.Last(), Name = newInfo.Name });
                                    }
                                    //registeredHosts.Add(eps);
                                    break;
                                #endregion

                                #region Remove a host
                                case NetPacketType.MASTER_HostEnded:
                                    // It's a host that has disconnected (but not crashed)
                                    ServerInfo hostInfo = ServerInfo.ReadFromPacket(msg);
                                    AppendTextBox("Host \"{0}\" has ended", hostInfo.Name);
                                    _hosts.Remove(hostInfo);
                                    RemoveListItem(hostInfo.Name);
                                    //registeredHosts.Add(eps);
                                    break;
                                #endregion

                                #region Update a host
                                case NetPacketType.ServerInfoChanged:
                                    // It's a host that has disconnected (but not crashed)
                                    string hostName = msg.ReadString();
                                    ServerInfo info = ServerInfo.ReadFromPacket(msg);
                                    info.ExternalEndPoint = msg.SenderEndPoint;
                                    _hosts[_hosts.IndexOf(_hosts.Find(X => X.Name == hostName))] = info;
                                    RemoveListItem(info.Name);
                                    AddListItem(new ListViewItem(info.Name) { Tag = info, Name = info.Name });
                                    //registeredHosts.Add(eps);
                                    break;
                                #endregion

                                #region Requesting Host List
                                case NetPacketType.MASTER_RequestHostList:
                                    // It's a client wanting a list of registered hosts
                                    AppendTextBox("Sending list of {0} hosts to client {1}", _hosts.Count, msg.SenderEndPoint);
                                    foreach (ServerInfo ep in _hosts)
                                    {
                                        // send registered host to client
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)NetPacketType.MASTER_SentHostInfo, 8);
                                        ep.WriteToPacket(om);
                                        om.Write(ep.ExternalEndPoint);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                    }
                                    break;
                                #endregion

                                #region Requesting Introduction
                                case NetPacketType.MASTER_RequestIntroduction:
                                    // It's a client wanting to connect to a specific (external) host
                                    IPEndPoint clientInternal = msg.ReadIPEndPoint();
                                    IPEndPoint hostExternal = msg.ReadIPEndPoint();
                                    string token = msg.ReadString();

                                    AppendTextBox(msg.SenderEndPoint + " requesting introduction to " + hostExternal + " (token " + token + ")");

                                    // find in list
                                    foreach (ServerInfo elist in _hosts)
                                    {
                                        if (elist.ExternalEndPoint.Equals(hostExternal))
                                        {
                                            // found in list - introduce client and host to eachother
                                            AppendTextBox("Sending introduction...");
                                            peer.Introduce(
                                                    elist.InternalEndPoint, // host internal
                                                    elist.ExternalEndPoint, // host external
                                                    clientInternal, // client internal
                                                    msg.SenderEndPoint, // client external
                                                    token // request token
                                            );
                                            break;
                                        }
                                    }
                                    break;

                                #endregion

                                #region Requesting profile registration

                                case NetPacketType.MASTER_RequestRegister:
                                    string encryptedUserName = msg.ReadString();
                                    string encryptedPassword = msg.ReadString();

                                    string username = Encryption.Decrypt(encryptedUserName);
                                    string password = Encryption.Decrypt(encryptedPassword);

                                    if (_users.Find(X => X.Name.ToLower() == username.ToLower()) == null)
                                    {
                                        _users.Add(new User(username, password));

                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)NetPacketType.MASTER_SuccesfullRegistration);
                                        om.Write(_users.LongCount() - 1);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                    }
                                    else
                                    {
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)NetPacketType.MASTER_FailedRegistration);
                                        om.Write("User with that name already exists!");
                                        peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                    }
                                    break;

                                #endregion

                                #region Requesting Login and AUTH token

                                case NetPacketType.MASTER_RequestLogin:
                                    encryptedUserName = msg.ReadString();
                                    encryptedPassword = msg.ReadString();

                                    username = Encryption.Decrypt(encryptedUserName);
                                    password = Encryption.Decrypt(encryptedPassword);

                                    if (_users.Find(X => X.Name == username) != null)
                                    {
                                        bool valid = _users.Find(X => X.Name == username).IsUser(username, encryptedPassword);

                                        if (valid)
                                        {
                                            NetOutgoingMessage om = peer.CreateMessage();
                                            om.Write((byte)NetPacketType.MASTER_SuccesfullLogin);
                                            om.Write("authtoken would go here");
                                            peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                        }
                                        else
                                        {
                                            NetOutgoingMessage om = peer.CreateMessage();
                                            om.Write((byte)NetPacketType.MASTER_FailedLogin);
                                            peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                        }
                                    }
                                    else
                                    {
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write((byte)NetPacketType.MASTER_FailedLogin);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
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
                            AppendTextBox(msg.ReadString());
                            break;
                    }
                }
            }

            peer.Shutdown("shutting down");

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _quitingServer = true;
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
            /// <param name="password">The unencrypted user password</param>
            /// <returns>True if this is the user</returns>
            public bool IsUser(string name, string password)
            {
                return name == _name && password == _password;
            }
        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            _mainThread = new Thread(RunServer);
            _mainThread.Start();
        }
    }

}
