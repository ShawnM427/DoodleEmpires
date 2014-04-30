using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public enum PlayerID : byte
    {
        One,
        Two,
        Three,
        Four
    }

    public enum NetPacketType : byte
    {
        RequestJoin = 0,
        AcceptedJoin = 1,
        ConnectionFailed = 2,
        PlayerJoined = 3,
        PlayerLeft = 4,
        RequestBlockChanged = 5,
        BlockUpdate = 6,
        MetaChanged = 7,
        ReqZoneadded = 8,
        ZoneAdded = 9,
        ZoneRemoved = 10,
        MapChanged = 11
    }

    public enum GameState : byte
    {
        MainMenu,
        ServerList,
        Lobby,
        JoiningGame,
        InGame
    }
}
