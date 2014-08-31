#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DoodleEmpires.Engine.Net;
#endregion

namespace DoodleEmpires
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string playerName = "unknown";

            if (args.Length > 0)
                playerName = args[0];
            
            using (var game = new Tests(playerName))
                game.Run();
        }

        /// <summary>
        /// Runs a game server
        /// </summary>
        public static void RunServer()
        {
            Thread thread = new Thread(StartServer);
            thread.Start();
        }

        private static void StartServer()
        {
            GameServer server = new GameServer();
            server.Run(new string[0]);
        }
    }
#endif
}
