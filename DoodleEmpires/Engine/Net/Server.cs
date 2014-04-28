using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;
using System.Threading;
using System.Net;

namespace DoodleEmpires.Engine.Net
{
    public class GameServer
    {
        const int _DEFAULTPORT = 14245;
        const string _DEFAULTNAME = "Unnamed Server";

        NetServer _server;
        ServerInfo _serverInfo;

        List<PlayerInfo> _players = new List<PlayerInfo>();

        public void Run(string[] args)
        {
            int port = _DEFAULTPORT;
            _serverInfo = 
                new ServerInfo(args.Length > 0 ? args[0] : _DEFAULTNAME, 
                new IPEndPoint(IPAddress.Any, args.Length > 1 ? int.Parse(args[0]) : _DEFAULTPORT));
            
            Console.WriteLine("Opening server");
            Console.WriteLine("Server name set to \"{0}\"", _serverInfo.Name);
            NetPeerConfiguration config = new NetPeerConfiguration("nettest");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;
            Console.WriteLine("Net configuration complete");

            try
            {
                Console.WriteLine("Attemting to bind to port {0}", port);

                _server = new NetServer(config);
                _server.Start();

                Console.WriteLine("Server started at {0}:{1}", _server.Configuration.BroadcastAddress, _server.Port);
            }
            catch
            {
                Console.WriteLine("Failed to bind to port {0}\n", port);
                Console.WriteLine("Aborting");
                return;
            }
            
            // schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            // run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = _server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:

                            NetOutgoingMessage om = _server.CreateMessage();
                            _serverInfo.WriteToPacket(om);
                            _server.SendDiscoveryResponse(om, msg.SenderEndpoint);

                            Console.WriteLine("Pinged discovery request");
                            break;

                        case NetIncomingMessageType.StatusChanged: //a client's status has changed

                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            //if (status == NetConnectionStatus.Disconnected)

                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;
                            
                        case NetIncomingMessageType.Data:
                            NetPacketType pType = (NetPacketType)msg.ReadByte();

                            switch (pType)
                            {
                                default:

                                    break;
                            }
                            break;
                    }
                    _server.Recycle(msg);
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            _server.Shutdown("app exiting");
        }
    }
}
