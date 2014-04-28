using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public delegate void FoundServerEvent(ServerInfo serverInfo);

    public delegate void PlayerJoinedEvent(PlayerInfo serverInfo);

    public delegate void PlayerLeftEvent(PlayerInfo serverInfo);

    public delegate void TerrainIDSetEvent(int x, int y, byte ID);

    public delegate void JoinedServerEvent(ServerInfo info);

    public delegate void ServerConnectionFailed(string reason);
}
