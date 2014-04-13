using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace DoodleAnims
{
    /// <summary>
    /// Represents a combo box where individual items can be enabled and disabled with check boxes
    /// </summary>
    public class CheckList : ComboBox
    {
        int _prevSelectedIndex = -1;
        bool _deflectClose = false;
        bool _mouseHover = false;
        
        #region Properties
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
        new public List<CheckListItem> Items
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
        /// Gets or sets the text shown in the edit box
        /// </summary>
        public string ShownText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets this check list's text
        /// </summary>
        public override string Text
        {
            get
            {
                return ShownText;
            }
            set
            {
            }
        }
        #endregion

        /// <summary>
        /// Creates a new Color Drop Down
        /// </summary>
        public CheckList()
        {
            Items = new List<CheckListItem>();

            //Select the first item from the list
            SelectedIndex = -1;
            _prevSelectedIndex = 0;

            //Make sure we draw the items
            DrawMode = DrawMode.OwnerDrawVariable;
            //Set the drop down style to fix bugs with other styles
            base.DropDownStyle = ComboBoxStyle.DropDownList;

            SetStyle(ControlStyles.UserPaint, true);
            DoubleBuffered = true;
        }

        /// <summary>
        /// Adds a new items to this combo box
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(CheckListItem item)
        {
            Items.Add(item);
            base.Items.Add(item);
        }

        /// <summary>
        /// Adds a new items to this combo box
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="isChecked">True if the item should start as checked</param>
        /// <param name="enabled">The item to add</param>
        public void Add(object item, bool isChecked = false, bool enabled = true)
        {
            Items.Add(new CheckListItem(item, isChecked, enabled));
            base.Items.Add(new CheckListItem(item, enabled));
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
            if (e.Index == -1)
            {
                e.Graphics.DrawString(
                    ShownText,
                    this.Font,
                    Brushes.Black,
                    new Point(e.Bounds.X, e.Bounds.Y));
                return;
            }                   
            
            // Draw the background 
            e.DrawBackground();
            
            // Gets the color  
            Color color = Items[e.Index].Enabled & Enabled ? ForeColor : DisabledColor;

            if ((e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit)
            {
                e.Graphics.DrawString(
                    ShownText,
                    this.Font,
                    Brushes.Black,
                    new Point(e.Bounds.X, e.Bounds.Y));
                return;
            }
            else
            {
                CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(e.Bounds.X, e.Bounds.Y),
                    Items[e.Index].Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);

                e.Graphics.DrawString(Items[e.Index].ToString(), Font, new SolidBrush(color),
                    e.Bounds.X + e.Bounds.Height, e.Bounds.Y - 2.0F);
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
            if (DroppedDown)
            {
                Items[SelectedIndex].Checked = !Items[SelectedIndex].Checked;
                _deflectClose = true;
            }
            else
            {
                if (!((CheckListItem)base.SelectedItem).Enabled)
                    SelectedIndex = _prevSelectedIndex;
                else
                    _prevSelectedIndex = SelectedIndex;
            }
        }
        
        /// <summary>
        /// Paints this control to the form
        /// </summary>
        /// <param name="paintEvent">The paint event arguments to use</param>
        protected override void OnPaint(PaintEventArgs paintEvent)
        {
            Graphics graphics = paintEvent.Graphics;
            
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color topColor = Enabled ? (Focused || _mouseHover ? Color.CornflowerBlue : Color.Gray) : Color.Gray;
            Color bottomColor = Enabled ? (Focused || _mouseHover ? Color.AliceBlue : Color.LightGray) : Color.LightGray;

            Brush lineBrush = new LinearGradientBrush(ClientRectangle, Color.Gray, Color.LightGray, LinearGradientMode.Vertical);
            Pen linePen = new Pen(lineBrush);

            Color backTop = Color.White;
            Color backBottom = Enabled ? Color.FromArgb(255, 200, 200, 200) : Color.LightGray;

            Color buttonTop = Enabled ? Color.LightGray : Color.FromArgb(255, 230, 230, 230) ;
            Color buttonBottom = Enabled ? Color.Gray : Color.LightGray;

            GraphicsPath mainRect = RoundedRectangle.Create(ClientRectangle, 2);
            Brush brush = new LinearGradientBrush(ClientRectangle, backTop, backBottom, LinearGradientMode.Vertical);
            graphics.FillPath(brush, mainRect);
            graphics.DrawPath(linePen, mainRect);

            graphics.DrawString(ShownText, Font, new SolidBrush(Enabled ? ForeColor : Color.Gray), new PointF(0, Height / 10));

            GraphicsPath buttonRect = RoundedRectangle.Create(new RectangleF(Width - Height * 0.75F, 0, Height * 0.75F, Height),2 , RoundedRectangle.RectangleCorners.TopRight | RoundedRectangle.RectangleCorners.BottomRight);
            Brush buttonBrush = new LinearGradientBrush(ClientRectangle, buttonTop, buttonBottom, LinearGradientMode.Vertical);
            graphics.FillPath(buttonBrush, buttonRect);
            graphics.DrawPath(linePen, buttonRect);
        }

        /// <summary>
        /// Called when this control loses focus
        /// </summary>
        /// <param name="e">A blank event argument</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseHover = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseHover = false;
        }
        
        protected override void  OnDropDownClosed(EventArgs e)
        {
            if (_deflectClose)
            {
                DroppedDown = true;
                _deflectClose = false;
            }
        }
    }

    /// <summary>
    /// Represents an object that can be disbaled in a combobox
    /// </summary>
    public class CheckListItem
    {
        /// <summary>
        /// Called when this item's checked status has changed
        /// </summary>
        public event EventHandler<CheckListItemChanged> CheckedChanged;
        /// <summary>
        /// Called when this item's enabled status has changed
        /// </summary>
        public event EventHandler<CheckListItemChanged> EnabledChanged;
        /// <summary>
        /// Called when this item's item value changed
        /// </summary>
        public event EventHandler<CheckListItemChanged> ItemChanged;

        bool _enabled;
        /// <summary>
        /// Gets or sets whether this object is enabled
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;

                    if (EnabledChanged != null)
                        EnabledChanged.Invoke(this, new CheckListItemChanged(this));
                }
                else
                    _enabled = value;
            }
        }

        bool _checked;
        /// <summary>
        /// Gets or sets whether this item is checked
        /// </summary>
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value != _checked)
                {
                    _checked = value;

                    if (CheckedChanged != null)
                        CheckedChanged.Invoke(this, new CheckListItemChanged(this));
                }
                else
                    _checked = value;
            }
        }

        object _item;
        /// <summary>
        /// Gets or sets the item in this object
        /// </summary>
        public object Item
        {
            get { return _item; }
            set
            {
                if (value != _item)
                {
                    _item = value;

                    if(ItemChanged != null)
                        ItemChanged.Invoke(this, new CheckListItemChanged(this));
                }
                else
                    _item = value;
            }
        }

        /// <summary>
        /// Creates a new disableable item
        /// </summary>
        public CheckListItem()
        {
            _enabled = false;
            _item = null;
        }
        
        /// <summary>
        /// Creates a new disableable item
        /// </summary>
        /// <param name="item">The item in this object</param>
        /// <param name="isChecked">True whether the item starts checked</param>
        /// <param name="enabled">True if this item is enabled</param>
        public CheckListItem(object item, bool isChecked = false, bool enabled = true)
        {
            _item = item;
            _checked = isChecked;
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

    /// <summary>
    /// The event args to use when a parameter for a CheckListItem has changed
    /// </summary>
    public class CheckListItemChanged : EventArgs
    {
        /// <summary>
        /// Gets the item that is being checked
        /// </summary>
        public CheckListItem Item;

        /// <summary>
        /// Creates a new event argument
        /// </summary>
        /// <param name="item">The check list item that raised the event</param>
        public CheckListItemChanged(CheckListItem item)
        {
            this.Item = item;
        }
    }
}
