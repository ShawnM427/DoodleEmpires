using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoodleAnims
{
    public class NumericInputBox : TextBox
    {
        public float Value
        {
            get
            {
                return Text.Length > 0 ? float.Parse(Text) : 0;
            }
            set
            {
                Text = Integer ? ((int)value).ToString() : value.ToString();

            }
        }
        public bool Integer { get; set; }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) & !((e.KeyChar == '.' & !Text.Contains('.') ) & !Integer)
                & !(char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }
    }
}
