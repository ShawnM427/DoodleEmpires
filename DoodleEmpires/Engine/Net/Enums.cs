using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents the player's ID
    /// </summary>
    public enum PlayerID : byte
    {
        /// <summary>
        /// The first player
        /// </summary>
        One,
        /// <summary>
        /// The second player
        /// </summary>
        Two,
        /// <summary>
        /// The third player
        /// </summary>
        Three,
        /// <summary>
        /// The fourth player
        /// </summary>
        Four
    }

    /// <summary>
    /// The packet type of a packet
    /// </summary>
    public enum NetPacketType : byte
    {
        /// <summary>
        /// The packet is to request a join from a client
        /// </summary>
        RequestJoin = 0,
        /// <summary>
        /// The packet is confirming a request to join from a client
        /// </summary>
        AcceptedJoin = 1,
        /// <summary>
        /// The packet is to deny request to join from a client
        /// </summary>
        ConnectionFailed = 2,
        /// <summary>
        /// The packet is to state that a client has joined a server
        /// </summary>
        PlayerJoined = 3,
        /// <summary>
        /// The packet is to state that a client has left a server
        /// </summary>
        PlayerLeft = 4,
        /// <summary>
        /// The packet is to make a request for a block change
        /// </summary>
        RequestBlockChanged = 5,
        /// <summary>
        /// The packet is to state that a block has been changed
        /// </summary>
        BlockUpdate = 6,
        /// <summary>
        /// The packet is to state that a block's metadata has been changed
        /// </summary>
        MetaChanged = 7,
        /// <summary>
        /// The packet is to make a request for a zone to be defined
        /// </summary>
        ReqZoneadded = 8,
        /// <summary>
        /// The packet is to make a request for a zone to be removed
        /// </summary>
        ReqZoneRemoved = 9,
        /// <summary>
        /// The packet is to state a zone has been added
        /// </summary>
        ZoneAdded = 10,
        /// <summary>
        /// The packet is to state a zone hs been removed
        /// </summary>
        ZoneRemoved = 11,
        /// <summary>
        /// This packet states that the map has changed and needs to be re-downloaded
        /// </summary>
        MapChanged = 12,
        /// <summary>
        /// Represents a packet used for pinging servers or clients
        /// </summary>
        PingMessage = 13
    }

    /// <summary>
    /// Represents the state of the game
    /// </summary>
    public enum GameState : byte
    {
        /// <summary>
        /// The game is currently in the main menu
        /// </summary>
        MainMenu,
        /// <summary>
        /// The game is currently in the server list
        /// </summary>
        ServerList,
        /// <summary>
        /// The game is currently in the server lobby
        /// </summary>
        Lobby,
        /// <summary>
        /// The game is currently joining a server
        /// </summary>
        JoiningGame,
        /// <summary>
        /// The game is currently in the main game
        /// </summary>
        InGame
    }
}
