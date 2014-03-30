using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DoodleAnims
{
    /// <summary>
    /// Represents a drop-down box for colors
    /// </summary>
    class ColorDropDown : ComboBox
    {
        /// <summary>
        /// Gets the currently selected color
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return SelectedIndex == 0 & _allowCustomColor ? _customColor : (Color)SelectedItem;
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

                    _selectNoise = true;
                    SelectedIndex = 0;
                    _selectNoise = false;
                }
                else
                {
                    throw new ArgumentException("The color must exist in the list when using AllowCustomColor = false");
                }
            }
        }
        /// <summary>
        /// Gets or sets whether this control allows the user to enter custom colors
        /// </summary>
        public bool AllowCustomColor
        {
            get { return _allowCustomColor; }
            set { _allowCustomColor = value; }
        }
        bool _allowCustomColor = false;
        /// <summary>
        /// Gets or sets whether this control allows the user to select transparent
        /// </summary>
        public bool AllowTransparent
        {
            get { return _allowTransparent; }
            set { _allowTransparent = value; }
        }
        bool _allowTransparent = true;

        Color _customColor = Color.Transparent;
        ColorDialog _colorDialog;
        bool _selectNoise = false;
        
        /// <summary>
        /// Creates a new Color Drop Down
        /// </summary>
        public ColorDropDown()
        {
            _colorDialog = new ColorDialog();

            if (_allowCustomColor)
                Items.Add(Color.FromArgb(0, 0, 0, 0));

            Array _elements = Enum.GetValues(typeof(KnownColor));
            Array.Sort(_elements);

            foreach (KnownColor knownColor in _elements)
            {
                Color color = Color.FromKnownColor(knownColor);
                if (!color.IsSystemColor)
                {
                    Items.Add(color);
                }
            }

            if (!AllowTransparent)
                Items.Remove(Color.Transparent);

            SelectedIndex = _allowCustomColor ? 1 : 0;
            DrawMode = DrawMode.OwnerDrawVariable;
            base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// Overrides the default keypress to disallow user editing
        /// </summary>
        /// <param name="e">The key arguments</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        { 
            e.Handled = true;
        }

        /// <summary>
        /// Overrides the item draw for this control
        /// </summary>
        /// <param name="e">The drawing event arguments</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Draw the background 
            e.DrawBackground();

            // Get the item text    
            Color color = (Color)Items[e.Index];

            string text = e.Index == 0  & _allowCustomColor ? "<custom>" :
                color.IsNamedColor ?
                SplitCamelCase(color.Name) :
                string.Format("{{0},{1},{2},{3}}", new object[] { color.R, color.G, color.B, color.A });

            // Determine the forecolor based on whether or not the item is selected    
            Brush brush = new SolidBrush(color);

            // Draw the text 
            e.Graphics.FillRectangle(brush, e.Bounds.X + 0.5F, e.Bounds.Y + 0.5F, e.Bounds.Height - 1, e.Bounds.Height - 1);
            e.Graphics.DrawString(text, Font, Brushes.Black, e.Bounds.X + e.Bounds.Height, e.Bounds.Y);
        }

        /// <summary>
        /// Overrides the measure item
        /// </summary>
        /// <param name="e">The measure item arguments</param>
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (e.Index > -1)
            {
                Size textSize = e.Graphics.MeasureString(this.Items[e.Index].ToString(), this.Font).ToSize();
                e.ItemHeight = textSize.Height;
                e.ItemWidth = textSize.Width + textSize.Height;
            }
        }

        /// <summary>
        /// Overrides the selected index changed event
        /// </summary>
        /// <param name="e">A blank event argument</param>
        protected override void  OnSelectionChangeCommitted(EventArgs e)
        {
            base.OnSelectionChangeCommitted(e);

            if (!_selectNoise)
            {
                if (SelectedIndex == 0 & _allowCustomColor)
                {
                    _colorDialog.ShowDialog();

                    this.BeginUpdate();
                    _customColor = _colorDialog.Color;
                    Items.RemoveAt(0);
                    Items.Insert(0, _customColor);
                    this.EndUpdate();

                    _selectNoise = true;
                    SelectedIndex = 0;
                    _selectNoise = false;
                }

                ForeColor = SelectedColor != Color.Transparent | SelectedColor == Color.White ?
                SelectedColor : Color.Black;
                Invalidate();
            }
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
