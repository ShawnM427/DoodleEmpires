using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public enum MasterServerMessageType : byte
    {
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
        PingMessage = 13,
        /// <summary>
        /// Called when a server's information has updated
        /// </summary>
        ServerInfoChanged = 14,

        MASTER_HostEnded = 243,
        MASTER_ReturnPublicKey = 244,
        MASTER_RequestPublicKey = 245,
        MASTER_RegisterHost = 246,
        MASTER_RequestHostList = 247,
        MASTER_SentHostInfo = 248,
        MASTER_RequestIntroduction = 249,
        MASTER_RequestRegister = 250,
        MASTER_RequestLogin = 251,
        MASTER_SuccesfullRegistration = 252,
        MASTER_FailedRegistration = 253,
        MASTER_SuccesfullLogin = 254,
        MASTER_FailedLogin = 255
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

    /// <summary>
    /// Represents the size of a newtwork packet identifier, the value of this enum contains the max number of packet types
    /// </summary>
    public enum NetPacketIdentifierSize : uint
    {
        /// <summary>
        /// The packet identifiers are 1 byte (8 bits)
        /// </summary>
        Byte = byte.MaxValue,
        /// <summary>
        /// The packet identifiers are 2 bytes read as a short (16 bits)
        /// </summary>
        Short = ushort.MaxValue,
        /// <summary>
        /// The packet identifiers are 4 bytes read as an int (32 bits)
        /// </summary>
        Int = uint.MaxValue
    }
}
