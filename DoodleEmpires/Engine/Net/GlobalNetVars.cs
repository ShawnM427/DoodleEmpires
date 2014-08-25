using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// A collection of global networking constants
    /// </summary>
    public static class GlobalNetVars
    {
        /// <summary>
        /// The default port for servers to bind to
        /// </summary>
        public const int DEFAULT_PORT = 14245;
        /// <summary>
        /// The port of the master server
        /// </summary>
        public const int MASTER_SERVER_PORT = 14556;
        /// <summary>
        /// A string representation of the master server's IP adress
        /// </summary>
        public const string MASTER_SERVER_IP = "localhost";
        /// <summary>
        /// The minimum port
        /// </summary>
        public const int MIN_PORT = 14240;
        /// <summary>
        /// The maximum port
        /// </summary>
        public const int MAX_PORT = 14250;
        /// <summary>
        /// The default name for an unnamed server
        /// </summary>
        public const string DEFAULT_SERVER_NAME = "<noName>";
        /// <summary>
        /// The default message for a server
        /// </summary>
        public const string DEFAULT_SERVER_MESSAGE = "Welcome!";
        /// <summary>
        /// Time delay between polling for servers (seconds)
        /// </summary>
        public const double SERVER_POLLING_RATE = 5.0;
        /// <summary>
        /// Time delay between fetching new pings for servers (seconds)
        /// </summary>
        public const double SERVER_PING_RATE = 2.5;
    }
}
