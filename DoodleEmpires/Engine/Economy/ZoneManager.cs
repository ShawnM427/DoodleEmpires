using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Manages zones and zone types
    /// </summary>
    public class ZoneManager 
        : ObjectManager<ZoneInfo> 
    {
        public override void Add(string name, ZoneInfo item)
        {
            item.ZoneID = (short)_items.Count;
            base.Add(name, item);
        }
    }

    public static class GlobalZoneManager
    {
        static ZoneManager _manager;

        public static ZoneManager Manager
        {
            get { return _manager; }
        }

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
