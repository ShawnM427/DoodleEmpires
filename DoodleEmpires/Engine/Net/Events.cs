using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Economy;

namespace DoodleEmpires.Engine.Net
{
    public delegate void FoundServerEvent(ServerInfo serverInfo);

    public delegate void PlayerJoinedEvent(PlayerInfo serverInfo);

    public delegate void PlayerLeftEvent(PlayerInfo serverInfo);

    public delegate void TerrainSetEvent(int x, int y, byte ID);

    public delegate void JoinedServerEvent(ServerInfo info);

    public delegate void ServerConnectionFailed(string reason);

    public delegate void ZoneAddedEvent(Zoning zone);

    public delegate void ZoneRemovedEvent(Zoning zone);
}
