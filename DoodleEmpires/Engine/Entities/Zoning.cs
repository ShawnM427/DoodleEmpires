using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Economy;
using DoodleEmpires.Engine.Terrain;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// Represents a zone that applies unit buffs and allows new tech options
    /// </summary>
    public class Zoning
    {
        Rectangle _bounds;
        UnitBuff _freindlyBuff = UnitBuff.Empty;
        UnitBuff _enemyBuff = UnitBuff.Empty;
        Color _drawColor = Color.Red;
        TechNode _requiredtechNode;
        List<TechNode> _techUnlocks;
        EconomyBuff _economyBuff = EconomyBuff.Empty;
        EconomyBuff _totalBuff = EconomyBuff.Empty;

        /// <summary>
        /// Gets the color for this zone
        /// </summary>
        public Color Color
        {
            get { return _drawColor; }
            protected set { _drawColor = value; }
        }
        /// <summary>
        /// Gets the buff for freindly units in this zone
        /// </summary>
        public UnitBuff FreindlyBuff
        {
            get { return _freindlyBuff; }
            protected set { _freindlyBuff = value; }
        }
        /// <summary>
        /// Gets the buff for enemy units in this zone
        /// </summary>
        public UnitBuff EnemyBuff
        {
            get { return _enemyBuff; }
            protected set { _enemyBuff = value; }
        }
        /// <summary>
        /// Gets or sets the bounds for this zone
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                _totalBuff = _economyBuff *
                ((_bounds.Width / VoxelMap.TILE_WIDTH) * (_bounds.Height / VoxelMap.TILE_HEIGHT));
            }
        }
        /// <summary>
        /// Gets or sets the tech node required to create this zone
        /// </summary>
        public TechNode RequiredTechNode
        {
            get { return _requiredtechNode; }
            protected set { _requiredtechNode = value; }
        }
        /// <summary>
        /// Gets a list of tech unlocks that placing this zone grants
        /// </summary>
        public List<TechNode> TechUnlocks
        {
            get { return _techUnlocks; }
            protected set { _techUnlocks = value; }
        }
        /// <summary>
        /// Gets the per-tile economy buff for this zone
        /// </summary>
        public EconomyBuff EconomyBuff
        {
            get { return _economyBuff; }
            protected set
            {
                _economyBuff = value;
                _totalBuff = _economyBuff *
                ((_bounds.Width / VoxelMap.TILE_WIDTH) * (_bounds.Height / VoxelMap.TILE_HEIGHT));
            }
        }

        /// <summary>
        /// Gets the total economy buff for this zone
        /// </summary>
        public EconomyBuff TotalEconomyBuff
        {
            get { return _totalBuff; }
        }
    }

    /// <summary>
    /// Represents a stockpile zone
    /// </summary>
    public class StockPileZone : Zoning
    {
        public StockPileZone()
            : base()
        {
            Color = Color.FromNonPremultiplied(128,128,128,255);
            EconomyBuff = new EconomyBuff()
            {
                StockpileRelative = 5.0f
            };
        }
    }
}
