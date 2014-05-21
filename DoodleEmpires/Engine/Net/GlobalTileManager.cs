using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Terrain;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Net
{
    class GlobalTileManager
    {
        static TileManager _tileManager;

        /// <summary>
        /// Gets the global tile manager
        /// </summary>
        public static TileManager TileManager
        {
            get { return _tileManager; }
        }

        static GlobalTileManager()
        {
            _tileManager = new TileManager();
            _tileManager.RegisterTile("Grass", 0, Color.Green, RenderType.Land, true);
            _tileManager.RegisterTile("Stone", 0, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Concrete", 0, Color.LightGray, RenderType.Land, true);
            _tileManager.RegisterTile("Wood", 20, Color.Brown, RenderType.Land, false);
            _tileManager.RegisterTile("Leaves", new Leaves(0));
            _tileManager.RegisterTile("Cobble", 60, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Wooden Spikes", new WoodSpike(0));
            _tileManager.RegisterTile("Ladder", new Ladder(0));
            _tileManager.RegisterTile("Door", new Door(0));

            _tileManager.RegisterConnect("Grass", "Stone");
            _tileManager.RegisterConnect("Grass", "Concrete");
            _tileManager.RegisterConnect("Stone", "Concrete");
            _tileManager.RegisterConnect("Wood", "Grass");
            _tileManager.RegisterConnect("Wood", "Leaves");

            _tileManager.RegisterOneWayConnect("Ladder", "Door");
            _tileManager.RegisterOneWayConnect("Ladder", "Wood");
            _tileManager.RegisterOneWayConnect("Ladder", "Concrete");
            _tileManager.RegisterOneWayConnect("Ladder", "Stone");
            _tileManager.RegisterOneWayConnect("Ladder", "Cobble");
        }
    }
}
