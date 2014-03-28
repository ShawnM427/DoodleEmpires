using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoodleAnims
{
    class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
            : base()
        {
            this.DoubleBuffered = true;
        }
    }
}
