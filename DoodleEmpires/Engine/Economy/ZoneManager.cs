using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Manages zone types
    /// </summary>
    public class ZoneManager 
        : ObjectManager<ZoneInfo> 
    {
        /// <summary>
        /// Adds a new zone type to this manager
        /// </summary>
        /// <param name="name">The name of the zone to add</param>
        /// <param name="item">The zone info</param>
        public override void Add(string name, ZoneInfo item)
        {
            item.ZoneID = (short)_items.Count;
            base.Add(name, item);
        }
    }

    /// <summary>
    /// A static class for handling zonings
    /// </summary>
    public static class GlobalZoneManager
    {
        static ZoneManager _manager;

        /// <summary>
        /// Gets the internal zone manager
        /// </summary>
        public static ZoneManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Initializes the static zone manager
        /// </summary>
        static GlobalZoneManager()
        {
            _manager = new ZoneManager();

            _manager.Add("Stockpile", new ZoneInfo(Color.LightGray, "Stockpile") 
            { 
                EconomyBuff = new EconomyBuff() 
                {  
                    StockpileRelative = 10 
                }
            });

            _manager.Add("Bunker", new ZoneInfo(Color.Red, "Bunker")
            {
                FreindlyBuff = new Entities.UnitBuff()
                {
                    AmmoRegenMultiplier = 0.1f,
                    RegenMultiplier = 0.5f,
                    ArmorMultiplier = 0.2f
                }
            });
        }
    }
}
