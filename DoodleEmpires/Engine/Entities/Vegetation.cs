using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Economy;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities
{
    public abstract class Vegetation
    {
        Rectangle _bounds;
        public abstract Resources Harvest { get; }
    }
}
