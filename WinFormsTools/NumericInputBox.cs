using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsTools
{
    /// <summary>
    /// A text box that only accepts numeric input
    /// </summary>
    public class NumericInputBox : TextBox
    {
        /// <summary>
        /// Gets or sets the value for this control
        /// </summary>
        public float Value
        {
            get
            {
                return Text.Replace(".", "").Replace("-", "").Length > 0 ? float.Parse(Text) : 0;
            }
            set
            {
                Text = Integer ? ((int)value).ToString() : value.ToString();

            }
        }
        /// <summary>
        /// Gets or sets whether this control should allow integers only
        /// </summary>
        public bool Integer { get; set; }

        /// <summary>
        /// Overrides the keypress to ignore all non-numeric inputs
        /// </summary>
        /// <param name="e">The key event to handle</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) & 
                !((e.KeyChar == '.' & !Text.Contains('.') ) & !Integer) &
                !(e.KeyChar == '-' & Text.Length == 0)
                & !(char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }

            base.OnKeyPress(e);
        }
    }
}
