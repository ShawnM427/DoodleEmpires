using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Utilities
{
    /// <summary>
    /// Represents a class for handling named objects
    /// </summary>
    /// <typeparam name="T">The type of objects to handle</typeparam>
    public class ObjectManager<T>
    {
        protected List<T> _zones = new List<T>();
        protected Dictionary<string, short> _zookUp = new Dictionary<string, short>();

        /// <summary>
        /// Gets the object with the given ID
        /// </summary>
        /// <param name="name">The name of object to add</param>
        /// <param name="item">The item to add</param>
        public virtual void Add(string name, T item)
        {
            if (_zones.Count < short.MaxValue)
            {
                _zones.Add(item);
            }
        }

        /// <summary>
        /// Gets the object with the given ID
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <returns>The item with the given ID, or the default object for the type</returns>
        public virtual T Get(string name)
        {
            if (_zookUp.ContainsKey(name))
            {
                return _zones[_zookUp[name]];
            }
            else
                return default(T);
        }

        /// <summary>
        /// Gets the object with the given ID
        /// </summary>
        /// <param name="id">The ID of the object</param>
        /// <returns>The item with the given ID, or the default object for the type</returns>
        public virtual T Get(int id)
        {
            if (id < _zones.Count)
            {
                return _zones[id];
            }
            else
                return default(T);
        }

        /// <summary>
        /// Gets the ID of the object with the given name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The ID, or -1 if an item with that name does not exist</returns>
        public virtual short GetID(string name)
        {
            if (_zookUp.ContainsKey(name))
                return _zookUp[name];
            else
                return -1;
        }
    }
}
