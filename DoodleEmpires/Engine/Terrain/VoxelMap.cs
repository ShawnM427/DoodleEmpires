using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// The base class for the voxel maps
    /// </summary>
    public abstract class VoxelMap
    {
        public abstract byte this[int x, int y]
        {
            get;
            set;
        }

        public abstract void SetTile(int x, int y, byte newID);

        public abstract void SetTileSafe(int x, int y, byte newID);

        public abstract void SetMeta(int x, int y, byte newMeta);

        public abstract byte GetMeta(int x, int y);
    }
}
