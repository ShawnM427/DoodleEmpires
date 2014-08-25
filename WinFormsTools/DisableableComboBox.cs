using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;

namespace WinFormsTools
{
    /// <summary>
    /// Represents a combo box where individual items can be enabled and disabled
    /// </summary>
    public class DisableableComboBox : ComboBox
    {
        int _prevSelectedIndex = -1;

        Color _disabledColor = Color.LightGray;
        /// <summary>
        /// Gets or sets the color for disabled items
        /// </summary>
        public Color DisabledColor
        {
            get { return _disabledColor; }
            set { _disabledColor = value; }
        }

        /// <summary>
        /// Gets the items in this combo box
        /// </summary>
        new public List<DisableableComboBoxItem> Items
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the currently selected item
        /// </summary>
        new public object SelectedItem
        {
            get { return SelectedIndex != -1 ? ((DisableableComboBoxItem)base.Items[SelectedIndex]).Item : null; }
        }

        /// <summary>
        /// Gets the drop down style for this control
        /// </summary>
        new public ComboBoxStyle DropDownStyle
        {
            get { return ComboBoxStyle.DropDownList; }
            set { }
        }
        
        /// <summary>
        /// Creates a new Color Drop Down
        /// </summary>
        public DisableableComboBox()
        {
            Items = new List<DisableableComboBoxItem>();

            //Select the first item from the list
            SelectedIndex = Items.Count > 0 ? 0 : -1;
            _prevSelectedIndex = 0;

            //Make sure we draw the items
            DrawMode = DrawMode.OwnerDrawVariable;
            //Set the drop down style to fix bugs with other styles
            base.DropDownStyle = ComboBoxStyle.DropDownList;

            base.Items.AddRange(Items.ToArray());
        }

        /// <summary>
        /// Adds a new items to this combo box
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(DisableableComboBoxItem item)
        {
            Items.Add(item);
            base.Items.Add(item);
        }

        /// <summary>
        /// Adds a new items to this combo box
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="enabled">The item to add</param>
        public void Add(object item, bool enabled = true)
        {
            Items.Add(new DisableableComboBoxItem(item, enabled));
            base.Items.Add(new DisableableComboBoxItem(item, enabled));
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
                Color color = Items[e.Index].Enabled & Enabled ? ForeColor : DisabledColor;

                e.Graphics.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(color), e.Bounds.X, e.Bounds.Y - 2.0F);
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
        /// Handles when items are selected
        /// </summary>
        /// <param name="e">A blank event arguments</param>
        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            if (!((DisableableComboBoxItem)base.SelectedItem).Enabled)
                SelectedIndex = _prevSelectedIndex;
            else
                _prevSelectedIndex = SelectedIndex;
        }
    }

    /// <summary>
    /// Represents an object that can be disbaled in a combobox
    /// </summary>
    public class DisableableComboBoxItem
    {
        bool _enabled;
        /// <summary>
        /// Gets or sets whether this object is enabled
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        object _item;
        /// <summary>
        /// Gets or sets the item in this object
        /// </summary>
        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Creates a new disableable item
        /// </summary>
        public DisableableComboBoxItem()
        {
            _enabled = false;
            _item = null;
        }
        
        /// <summary>
        /// Creates a new disableable item
        /// </summary>
        /// <param name="item">The item in this object</param>
        /// <param name="enabled">True if this item is enabled</param>
        public DisableableComboBoxItem(object item, bool enabled = true)
        {
            _item = item;
            _enabled = enabled;
        }

        /// <summary>
        /// Converts this item to a string representation
        /// </summary>
        /// <returns>The item's name</returns>
        public override string ToString()
        {
            return _item.ToString();
        }
    }
}
