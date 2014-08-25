using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace WinFormsTools
{
    /// <summary>
    /// Represents a drop-down box for colors
    /// </summary>
    public class ColorDropDown : ComboBox
    {
        /// <summary>
        /// A list of all colors, exluding transparent
        /// </summary>
        static object[] _colors;

        static ColorDropDown()
        {
            List<object> temp = new List<object>();
            
            //Loop through all known colors
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(knownColor); //get the actual color

                if (!color.IsSystemColor) //if this is not a system color, add it
                {
                    temp.Add(color);
                }
            }

            temp.Remove(Color.Transparent);

            _colors = temp.ToArray();
        }

        /// <summary>
        /// Gets the currently selected color
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return SelectedIndex == 0 & _allowCustomColor ? _customColor : 
                    SelectedIndex > -1 ? (Color)SelectedItem : Color.Transparent;
            }
            set
            {
                if (Items.Contains(value))
                    SelectedItem = value;
                else if (_allowCustomColor)
                {
                    this.BeginUpdate();
                    _customColor = value;
                    _colorDialog.Color = value;
                    Items.RemoveAt(0);
                    Items.Insert(0, value);
                    this.EndUpdate();

                    SelectedIndex = 0;
                }
                else
                    SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Gets or sets whether this control allows the user to enter custom colors
        /// </summary>
        public bool AllowCustomColor
        {
            get { return _allowCustomColor; }
            set
            {
                //If this control does not allow tansparent colors, we remove it from the list
                if (!value & _allowCustomColor)
                    Items.RemoveAt(0);
                else if (value & !_allowTransparent)
                    Items.Insert(0, _customColor);

                SelectedIndex = 0;

                _allowCustomColor = value;
            }
        }
        bool _allowCustomColor = false;
        /// <summary>
        /// Gets or sets whether this control allows the user to select transparent
        /// </summary>
        public bool AllowTransparent
        {
            get { return _allowTransparent; }
            set
            {
                //If this control does not allow tansparent colors, we remove it from the list
                if (!value & _allowTransparent)
                    Items.Remove(Color.Transparent);
                else if (value & !_allowTransparent)
                    Items.Insert(_allowCustomColor ? 1 : 0, Color.Transparent);

                SelectedIndex = 0;

                _allowTransparent = value;
            }
        }
        bool _allowTransparent = false;

        /// <summary>
        /// Gets the drop down style for this control
        /// </summary>
        new public ComboBoxStyle DropDownStyle
        {
            get { return ComboBoxStyle.DropDownList; }
            set { }
        }

        Color _customColor = Color.Black;
        ColorDialog _colorDialog;
        
        /// <summary>
        /// Creates a new Color Drop Down
        /// </summary>
        public ColorDropDown()
        {
            //creates the dialog used for picking custom colors
            _colorDialog = new ColorDialog(); 

            //begins updating the item list, so as to improve the loading speed
            BeginUpdate();

            Items.Clear();

            Items.AddRange(_colors);
            
            //Finish updating the item lists
            EndUpdate();

            //Select the first item from the list
            SelectedIndex = 0;
            //Make sure we draw the items
            DrawMode = DrawMode.OwnerDrawVariable;
            //Set the drop down style to fix bugs with other styles
            base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// Overrides the default keypress to disallow user editing
        /// </summary>
        /// <param name="e">The key arguments</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        { 
            //do not accept any input
            e.Handled = true;
        }

        /// <summary>
        /// Overrides the item draw for this control
        /// </summary>
        /// <param name="e">The drawing event arguments</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                // Draw the background 
                e.DrawBackground();

                // Gets the color  
                Color color = (Color)Items[e.Index];

                // Determine text for the tag
                string text = e.Index == 0 & _allowCustomColor ? "<custom>" :
                    color.IsNamedColor ?
                    SplitCamelCase(color.Name) :
                    string.Format("{{0},{1},{2},{3}}", new object[] { color.R, color.G, color.B, color.A });

                // Get the brush color from the selected color  
                Brush brush = new SolidBrush(Enabled ? color : Math2.Lerp(color, Color.White, 0.5F));
                Brush textBrush = new SolidBrush(Enabled ? ForeColor : Math2.Lerp(ForeColor, Color.White, 0.5F));

                // Draws a rectangle, and the text next to it
                e.Graphics.FillRectangle(brush, e.Bounds.X + 0.5F, e.Bounds.Y + 0.5F, e.Bounds.Height - 1, e.Bounds.Height - 1);
                e.Graphics.DrawString(text, Font, textBrush, e.Bounds.X + e.Bounds.Height, e.Bounds.Y);
            }
        }

        /// <summary>
        /// Overrides the measure item
        /// </summary>
        /// <param name="e">The measure item arguments</param>
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            // Implement the base measurement to get things set up
            base.OnMeasureItem(e);

            // Check that the index is not unselected
            if (e.Index > -1)
            {
                //Gets the text size
                Size textSize = e.Graphics.MeasureString(this.Items[e.Index].ToString(), this.Font).ToSize();
                
                // Assign the values to the output
                e.ItemHeight = textSize.Height;
                e.ItemWidth = textSize.Width + textSize.Height;
            }
        }

        /// <summary>
        /// Overrides the selected index changed event
        /// </summary>
        /// <param name="e">A blank event argument</param>
        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            // Perform the base action to set up the parameters
            base.OnSelectionChangeCommitted(e);

            // If this is the selected color
            if (SelectedIndex == 0 & _allowCustomColor)
            {
                // Show the color dialog
                _colorDialog.ShowDialog();

                // Begin updating
                this.BeginUpdate();
                // Gets the custom color
                _customColor = _colorDialog.Color;
                // Replace the first item
                Items.RemoveAt(0);
                Items.Insert(0, _customColor);
                //Finish updating
                this.EndUpdate();

                // Select the first item again 
                SelectedIndex = 0;
            }

            Invalidate();
        }

        /// <summary>
        /// Splits a string by capitals
        /// </summary>
        /// <param name="source">The string to split</param>
        /// <returns></returns>
        string SplitCamelCase(string source)
        {
            return Regex.Replace(source, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
