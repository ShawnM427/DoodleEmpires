using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Represents a node in a tech tree
    /// </summary>
    public class TechNode
    {
        public static TechNode None = new TechNode("None", "", null);

        string _name;
        string _description;
        Texture2D _icon;
        
        /// <summary>
        /// Gets the name of this tech node
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// Gets the description for this node
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
        /// <summary>
        /// Gets the icon of this tech node
        /// </summary>
        public Texture2D Icon
        {
            get { return _icon; }
        }

        /// <summary>
        /// Creates a new tech node
        /// </summary>
        /// <param name="name">The name for this node</param>
        /// <param name="description">The description for this node</param>
        /// <param name="icon">The icon for this node</param>
        /// <param name="requiredNodes">The required nodes for this node</param>
        protected TechNode(string name, string description, Texture2D icon)
        {
            _name = name;
            _icon = icon;
        }
    }
}
