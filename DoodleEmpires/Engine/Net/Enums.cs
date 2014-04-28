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
        BlockUpdate,
        RequestBlockChanged,
        RequestJoin,
        AcceptedJoin,
        PlayerJoined,
        PlayerLeft,
        ConnectionFailed
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
