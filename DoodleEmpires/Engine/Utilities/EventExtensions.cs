using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.GUI;

namespace DoodleEmpires.Engine.Utilities
{
    public static class EventExtensions
    {
        public static void Raise<T>(this EventHandler<T> handler, object sender,  T args) where T : EventArgs
        {
            if (handler != null) 
                handler(sender, args);
        }
    }
}
