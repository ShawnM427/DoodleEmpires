using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Economy;
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Entities;
using System.IO;
using Lidgren.Network;
using DoodleEmpires.Engine.Net;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Represents a zone that applies unit buffs and allows new tech options
    /// </summary>
    public class Zoning
    {
        Rectangle _bounds;
        /// <summary>
        /// Gets or sets the bounds for this zone
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                _totalBuff = Info.EconomyBuff *
                ((_bounds.Width / SPMap.TILE_WIDTH) * (_bounds.Height / SPMap.TILE_HEIGHT));
            }
        }

        EconomyBuff _totalBuff = EconomyBuff.Empty;
        /// <summary>
        /// Gets the total economy buff for this zone
        /// </summary>
        public EconomyBuff TotalEconomyBuff
        {
            get { return _totalBuff; }
        }

        ZoneInfo _info;
        /// <summary>
        /// Gets the zoning information about this zone
        /// </summary>
        public ZoneInfo Info
        {
            get { return _info; }
        }

        byte _playerID;
        /// <summary>
        /// Gets the ID of the player associated with this zone
        /// </summary>
        public byte PlayerID
        {
            get { return _playerID; }
        }

        /// <summary>
        /// Creates a new zone
        /// </summary>
        /// <param name="bounds">The bounds of the zone</param>
        /// <param name="playerID">The ID of the player to bind to</param>
        /// <param name="info">The zone info to use</param>
        public Zoning(Rectangle bounds, byte playerID, ZoneInfo info)
        {
            _playerID = playerID;
            _bounds = bounds;
            _info = info;
        }
                
        /// <summary>
        /// Writes basic zone data to stream (TODO Actually save zone info)
        /// </summary>
        /// <param name="writer">The stream to write to</param>
        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write(Bounds.X);
            writer.Write(Bounds.Y);
            writer.Write(Bounds.Width);
            writer.Write(Bounds.Height);

            writer.Write(_playerID);

            writer.Write(_info.ZoneID);
        }

        /// <summary>
        /// Reads basic zone data from stream (TODO Actually load zone info)
        /// </summary>
        /// <param name="reader">The stream to read from</param>
        /// <returns>A basic zone loaded from teh memory stream</returns>
        public static Zoning ReadFromStream(BinaryReader reader)
        {
            Rectangle bounds = 
                new Rectangle(reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32());

            byte playerID = reader.ReadByte();

            short zoneID = reader.ReadInt16();
            
            return new Zoning(bounds, playerID, GlobalZoneManager.Manager.Get(zoneID));
        }

        /// <summary>
        /// Writes this zone to an outgoing packet, DOES NOT WRITE PACKET HEADER
        /// </summary>
        /// <param name="msg">The packet to write to</param>
        public void WriteToPacket(NetOutgoingMessage msg)
        {
            msg.Write(_bounds.X);
            msg.Write(_bounds.Y);
            msg.Write(_bounds.Width);
            msg.Write(_bounds.Height);

            msg.Write(_playerID);

            msg.Write(_info.ZoneID);
        }

        /// <summary>
        /// Reads a zone from an incoming packet, DOES NOT READ PACKET HEADER
        /// </summary>
        /// <param name="msg">The packet to parse</param>
        /// <returns>A zone read from the packet</returns>
        public static Zoning ReadFromPacket(NetIncomingMessage msg)
        {
            int x = msg.ReadInt32();
            int y = msg.ReadInt32();
            int width = msg.ReadInt32();
            int height = msg.ReadInt32();

            byte playerID = msg.ReadByte();

            short id = msg.ReadInt16();

            return new Zoning(new Rectangle(x, y, width, height), playerID, GlobalZoneManager.Manager.Get(id));
        }
    }

    /// <summary>
    /// Represents information about a zone
    /// </summary>
    public struct ZoneInfo
    {
        UnitBuff _freindlyBuff;
        UnitBuff _enemyBuff;
        Color _drawColor;
        TechNode _requiredtechNode;
        List<TechNode> _techUnlocks;
        EconomyBuff _economyBuff;
        short _zoneID;
        string _name;

        /// <summary>
        /// Gets the color for this zone
        /// </summary>
        public Color Color
        {
            get { return _drawColor; }
            set { _drawColor = value; }
        }
        /// <summary>
        /// Gets the buff for freindly units in this zone
        /// </summary>
        public UnitBuff FreindlyBuff
        {
            get { return _freindlyBuff; }
            set { _freindlyBuff = value; }
        }
        /// <summary>
        /// Gets the buff for enemy units in this zone
        /// </summary>
        public UnitBuff EnemyBuff
        {
            get { return _enemyBuff; }
            set { _enemyBuff = value; }
        }
        /// <summary>
        /// Gets or sets the tech node required to create this zone
        /// </summary>
        public TechNode RequiredTechNode
        {
            get { return _requiredtechNode; }
            set { _requiredtechNode = value; }
        }
        /// <summary>
        /// Gets a list of tech unlocks that placing this zone grants
        /// </summary>
        public List<TechNode> TechUnlocks
        {
            get { return _techUnlocks; }
            set { _techUnlocks = value; }
        }
        /// <summary>
        /// Gets the per-tile economy buff for this zone
        /// </summary>
        public EconomyBuff EconomyBuff
        {
            get { return _economyBuff; }
            set
            {
                _economyBuff = value;
            }
        }

        /// <summary>
        /// Gets the name of this zone
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets the ID of this zone
        /// </summary>
        public short ZoneID
        {
            get { return _zoneID; }
            set { _zoneID = value; }
        }

        /// <summary>
        /// Creates a new zoning info
        /// </summary>
        /// <param name="color">The color of the zone</param>
        /// <param name="name">The name of the zone</param>
        public ZoneInfo(Color color, string name)
        {
            _name = name;
            _drawColor = color;

            _freindlyBuff = UnitBuff.Empty;
            _enemyBuff = UnitBuff.Empty;
            _requiredtechNode = TechNode.None;
            _techUnlocks = new List<TechNode>();
            _economyBuff = EconomyBuff.Empty;
            _zoneID = 0;
        }
    }
}
