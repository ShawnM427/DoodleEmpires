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
        /// <summary>
        /// Gets or sets the tile ID at the given [x, y] position
        /// </summary>
        /// <param name="x">The x coord of the tile</param>
        /// <param name="y">The y coord of the tile</param>
        /// <returns>Gets the tile ID at the given [x, y] position</returns>
        public abstract byte this[int x, int y]
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the tile ID at the given [x, y] position
        /// </summary>
        /// <param name="x">The x coord of the tile</param>
        /// <param name="y">The y coord of the tile</param>
        /// <param name="newID">The ID of the tile to set</param>
        public abstract void SetTile(int x, int y, byte newID);

        /// <summary>
        /// Safely sets the tile ID at the given [x, y] position
        /// </summary>
        /// <param name="x">The x coord of the tile</param>
        /// <param name="y">The y coord of the tile</param>
        /// <param name="newID">The ID of the tile to set</param>
        public abstract void SetTileSafe(int x, int y, byte newID);

        /// <summary>
        /// Sets the meta at the given [x, y] position
        /// </summary>
        /// <param name="x">The x coord of the tile</param>
        /// <param name="y">The y coord of the tile</param>
        /// <param name="newMeta">The meta of the tile to set</param>
        public abstract void SetMeta(int x, int y, byte newMeta);

        /// <summary>
        /// Gets the meta at the given [x, y] position
        /// </summary>
        /// <param name="x">The x coord of the tile</param>
        /// <param name="y">The y coord of the tile</param>
        /// <returns>The meta of the tile</returns>
        public abstract byte GetMeta(int x, int y);
    }
}
