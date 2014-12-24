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

            if (args.Contains("-t"))
            {
                using (var game = new Tests(playerName))
                    game.Run();
            }
            else
            {
                using (var game = new NetGame(playerName))
                    game.Run();
            }
        }
    }
#endif
}
