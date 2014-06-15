using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Economy;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Called when a new server has been found
    /// </summary>
    /// <param name="serverInfo">The server's info</param>
    public delegate void FoundServerEvent(ServerInfo serverInfo);

    /// <summary>
    /// Called when a player has joined a server
    /// </summary>
    /// <param name="serverInfo">The player's info</param>
    public delegate void PlayerJoinedEvent(PlayerInfo serverInfo);

    /// <summary>
    /// Called when a player has left a server
    /// </summary>
    /// <param name="serverInfo">The player's info</param>
    public delegate void PlayerLeftEvent(PlayerInfo serverInfo);

    /// <summary>
    /// Called when a terrain block's ID has been set
    /// </summary>
    /// <param name="x">The x coord of the tile</param>
    /// <param name="y">The y coord of the tile</param>
    /// <param name="ID">The ID of the tile</param>
    public delegate void TerrainSetEvent(int x, int y, byte ID);

    /// <summary>
    /// Called by a client when it joins a server
    /// </summary>
    /// <param name="info">The server's information</param>
    public delegate void JoinedServerEvent(ServerInfo info);

    /// <summary>
    /// Called when a client has failed to connect to a server
    /// </summary>
    /// <param name="reason">The reason for failure</param>
    public delegate void ServerConnectionFailed(string reason);

    /// <summary>
    /// Called when a zone is added to the world
    /// </summary>
    /// <param name="zone">The zone being added</param>
    public delegate void ZoneAddedEvent(Zoning zone);

    /// <summary>
    /// Called when a zone is removed from the world
    /// </summary>
    /// <param name="zone">The zone being removed</param>
    public delegate void ZoneRemovedEvent(Zoning zone);
}
