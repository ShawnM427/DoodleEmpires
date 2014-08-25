using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DoodleAnims.Lib.Anim;
using System.IO;
using WinFormsTools;

namespace DoodleAnims
{
    public partial class MainForm : Form
    {
        Limb _selectedLimb;

        Skeleton _skeleton;

        Timer _clock = new Timer();

        float _eMS; // Enpoint Marker size
        float _oMS; // Orgin Marker Size
        byte _eMT; // Endpoint Marker Type
        byte _oMT; // Orgin Marker Type

        Brush _eBrush; // Endpoint Brush
        Pen _ePen; // Endpoint Pen
        Brush _oBrush; // Endpoint Brush
        Pen _oPen; // Endpoint Brush

        bool _mouseDown = false;

        FormState _state = FormState.Rigging;

        SettingsDialog _settings = new SettingsDialog();
        DisableableComboBox _disableBox;

        /// <summary>
        /// Gets or sets the currently selected limb
        /// </summary>
        Limb SelectedLimb
        {
            get { return _selectedLimb; }
            set
            {
                _selectedLimb = value;
                dbpnl_renderScreen.Invalidate();

                if (_selectedLimb != null)
                {
                    nib_rotation.Value = _selectedLimb.Rotation;
                    nib_scale.Value = _selectedLimb.Scale;
                    nib_length.Value = _selectedLimb.Length;
                    txt_name.Text = _selectedLimb.Name;
                    cmb_type.SelectedItem = _selectedLimb.LimbType;
                    cdl_color.Color = _selectedLimb.Color;
                    cdd_limbColor.SelectedColor = _selectedLimb.Color;
                    nib_offsetX.Value = _selectedLimb.OffsetX;
                    nib_offsetY.Value = _selectedLimb.OffsetY;

                    nib_imageAngle.Value = _selectedLimb.ImageAngle;
                    chk_xFlip.Checked = _selectedLimb.XScale == -1;
                    chk_yFlip.Checked = _selectedLimb.YScale == -1;

                    if (_selectedLimb.LimbType == LimbType.Textured)
                    {
                        img_texture.Image = (Image)_selectedLimb.Tag;
                    }
                    else
                        img_texture.Image = null;
                }
                else
                {
                    nib_rotation.Value = 0;
                    nib_scale.Value = 0;
                    nib_length.Value = 0;
                    txt_name.Text = "";
                    cmb_type.SelectedItem = LimbType.Line;
                    cdl_color.Color = Color.Black;
                    cdd_limbColor.SelectedColor = Color.Black;
                    nib_offsetX.Value = 0;
                    nib_offsetY.Value = 0;
                    nib_imageAngle.Value = 0;
                    chk_xFlip.Checked = false;
                    chk_yFlip.Checked = false;
                    img_texture.Image = null;
                }
            }
        }

        /// <summary>
        /// Creates a new main DE Anims form
        /// </summary>
        public MainForm()
        {
            #if DEBUG && WIPE_SETTINGS
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            #endif

            InitializeComponent();

            _disableBox = new DisableableComboBox();
            _disableBox.Add(new DisableableComboBoxItem(FormState.Rigging, true));
            _disableBox.Add(new DisableableComboBoxItem(FormState.Animation, true));
            _disableBox.SelectedIndex = 0;
            _disableBox.DropDownHeight = 100;
            _disableBox.SelectedValueChanged += new EventHandler(disableBox_SelectedValueChanged);
            ToolStripControlHost host = new ToolStripControlHost(_disableBox);
            
            if (_state == FormState.Animation)
            {

                dbpnl_renderScreen.Height = Height - _disableBox.Height;
                amc_animControl.Visible = true;
                grp_properties.Enabled = false;
            }
            else
            {
                dbpnl_renderScreen.Height = Height;
                amc_animControl.Visible = false;
                grp_properties.Enabled = true;
            }

            ts_main.Items.Add(host);

            dbpnl_renderScreen.BackColor = Properties.Settings.Default.BackgroundColor;

            DoubleBuffered = true;

            _skeleton = new Skeleton(dbpnl_renderScreen, "", new PointF(dbpnl_renderScreen.Size.Width / 2, dbpnl_renderScreen.Size.Height / 2));

            SelectedLimb = _skeleton.Root;

            trv_limbBrowser.Nodes.Clear();
            trv_limbBrowser.Nodes.Add(_skeleton.RootNode);

            _clock.Interval = 16;

            cmb_type.Items.Add(LimbType.Line);
            cmb_type.Items.Add(LimbType.Circle);
            cmb_type.Items.Add(LimbType.Textured);

            cmb_type.SelectedItem = LimbType.Line;

            BuildSettingsRelated();
        }

        /// <summary>
        /// Rebuilds anything that changes based on the settings
        /// </summary>
        private void BuildSettingsRelated()
        {
            dbpnl_renderScreen.BackColor = Properties.Settings.Default.BackgroundColor;

            _eMS = Properties.Settings.Default.EndPointMarkerSize;
            _oMS = Properties.Settings.Default.OrginMarkerSize;

            _eMT = Properties.Settings.Default.EndPointMarkerType;
            _oMT = Properties.Settings.Default.EndPointMarkerType;

            _eBrush = new SolidBrush(Properties.Settings.Default.EndPointColor);
            _ePen = new Pen(Properties.Settings.Default.EndPointColor);

            _oBrush = new SolidBrush(Properties.Settings.Default.OrginColor);
            _oPen = new Pen(Properties.Settings.Default.OrginColor);

            if (!Properties.Settings.Default.ShowTree)
            {
                int pnlWidth = mainWindow.Panel1.Width;
                Width = mainWindow.Panel1.Width + grp_properties.Right + 24;
                mainWindow.SplitterDistance = pnlWidth;
                trv_limbBrowser.Visible = false;
            }
            else
            {
                int pnlWidth = mainWindow.Panel1.Width;
                Width = mainWindow.Panel1.Width + trv_limbBrowser.Right + 20;
                mainWindow.SplitterDistance = pnlWidth;
                trv_limbBrowser.Visible = true;
            }

            dbpnl_renderScreen.Invalidate();
        }
        
        /// <summary>
        /// Called when the main screen needs to render
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The drawing arguments to use</param>
        private void dbpnl_renderScreen_Paint(object sender, PaintEventArgs e)
        {
            _skeleton.Paint(e.Graphics);

            if (_selectedLimb != null)
            {
                if (_eMT == 0)
                {
                    e.Graphics.FillEllipse(_eBrush,
                        _selectedLimb.EndPoint.X - _eMS / 2, _selectedLimb.EndPoint.Y - _eMS / 2, _eMS, _eMS);
                }
                else if (_eMT == 1)
                {
                    e.Graphics.DrawEllipse(_ePen,
                        _selectedLimb.EndPoint.X - _eMS / 2, _selectedLimb.EndPoint.Y - _eMS / 2, _eMS, _eMS);
                }

                if (_oMT == 0)
                {
                    e.Graphics.FillEllipse(_oBrush,
                        _selectedLimb.Orgin.X - _oMS / 2, _selectedLimb.Orgin.Y - _oMS / 2, _oMS, _oMS);
                }
                else if (_oMT == 1)
                {
                    e.Graphics.DrawEllipse(_oPen,
                        _selectedLimb.Orgin.X - _oMS / 2, _selectedLimb.Orgin.Y - _oMS / 2, _oMS, _oMS);
                }
            }
        }

        /// <summary>
        /// Called when the mouse is pressed in the main render screen
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The mouse arguments</param>
        private void dbpnl_renderScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!_mouseDown)
                {
                    SelectedLimb = _skeleton.Root.Selected(e);
                }

                _mouseDown = true;
            }
        }

        /// <summary>
        /// Called when the mouse is released in the main render screen
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The mouse arguments</param>
        private void dbpnl_renderScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = false;
            }
        }

        /// <summary>
        /// Called when the mouse is moved over the main render screen
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The mouse arguments</param>
        private void dbpnl_renderScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                if (SelectedLimb != null)
                {
                    SelectedLimb.PointAt(e.Location);
                    nib_rotation.Value = SelectedLimb.Rotation;
                    dbpnl_renderScreen.Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Called when a user tries to input text into the type picker
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The key event args</param>
        private void cmb_type_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Called when the value for the rotation field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event argument</param>
        private void nib_rotation_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Rotation = nib_rotation.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the scale field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void nib_scale_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Scale = nib_scale.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value in the name field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void txt_name_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Name = txt_name.Text;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the length field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void nib_length_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Length = nib_length.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when a new limb type is selected
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void cmb_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.LimbType = (LimbType)cmb_type.SelectedItem;

                if (SelectedLimb.LimbType == LimbType.Textured & SelectedLimb.Tag == null)
                {
                    fdl_imageImport.ShowDialog();

                    if (File.Exists(fdl_imageImport.FileName))
                    {
                        SelectedLimb.Tag = Image.FromFile(fdl_imageImport.FileName);
                        img_texture.Image = (Image)SelectedLimb.Tag;
                    }
                }

                dbpnl_renderScreen.Invalidate();
            }
        }
        
        /// <summary>
        /// Called when the image icon is double clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void img_texture_Click(object sender, EventArgs e)
        {
            if (img_texture != null && _selectedLimb != null &&
                SelectedLimb.LimbType == LimbType.Textured)
            {
                fdl_imageImport.ShowDialog();

                if (File.Exists(fdl_imageImport.FileName))
                {
                    SelectedLimb.Tag = Image.FromFile(fdl_imageImport.FileName);
                    img_texture.Image = (Image)SelectedLimb.Tag;
                }

                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the x offset field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void nib_offsetX_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.OffsetX = nib_offsetX.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the y offset field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void nib_offsetY_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.OffsetY = nib_offsetY.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the x flip field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void chk_xFlip_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.XScale = chk_xFlip.Checked ? -1 : 1;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value for the y flip field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void chk_yFlip_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.YScale = chk_yFlip.Checked ? -1 : 1;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the value int the image anlge field has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void nib_imageAngle_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.ImageAngle = nib_imageAngle.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when the remove button is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void btn_remove_Click(object sender, EventArgs e)
        {
            if (SelectedLimb != null & SelectedLimb != _skeleton.Root)
            {
                BoolDialog d = new BoolDialog("Are you sure you want to delete this limb segment?");
                DialogResult result = d.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    if (_selectedLimb.Parent != null)
                    {
                        _selectedLimb.Parent.RemoveChild(_selectedLimb);
                        _selectedLimb = null;
                    }

                    dbpnl_renderScreen.Invalidate();
                }
            }
        }

        /// <summary>
        /// Called when the add button is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void btn_add_Click(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                Limb l = new Limb(_selectedLimb, Color.Black);
                SelectedLimb = l;
            }
        }

        /// <summary>
        /// Called when the new tool strip item is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void tsi_new_Click(object sender, EventArgs e)
        {
            _skeleton = new Skeleton(dbpnl_renderScreen, "", new PointF(dbpnl_renderScreen.Size.Width / 2, dbpnl_renderScreen.Size.Height / 2));
            SelectedLimb = _skeleton.Root;
            trv_limbBrowser.Nodes.Clear();
            trv_limbBrowser.Nodes.Add(_skeleton.RootNode);
        }

        /// <summary>
        /// Called when the save skeleton tool strip item is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void tsi_save_Click(object sender, EventArgs e)
        {
            DialogResult r = fdl_saveSkeleton.ShowDialog();

            if (r == DialogResult.OK)
            {
                Stream s = File.OpenWrite(fdl_saveSkeleton.FileName);
                BinaryWriter w = new BinaryWriter(s);

                _skeleton.Save(w);

                w.Close();
                w.Dispose();
                s.Dispose();
            }
        }

        /// <summary>
        /// Called when the load skeleton tool strip item is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void tsi_loadSkele_Click(object sender, EventArgs e)
        {
            DialogResult r = fdl_loadSkeleton.ShowDialog();

            if (r == DialogResult.OK && File.Exists(fdl_loadSkeleton.FileName))
            {
                Stream s = File.OpenRead(fdl_loadSkeleton.FileName);
                BinaryReader rd = new BinaryReader(s);

                _skeleton = Skeleton.Load(dbpnl_renderScreen, rd);

                SelectedLimb = _skeleton.Root;

                trv_limbBrowser.Nodes.Clear();
                trv_limbBrowser.Nodes.Add(_skeleton.RootNode);

                rd.Close();
                rd.Dispose();
                s.Dispose();


                Animation anim = new Animation(_skeleton);
                AnimKeyFrame frame1 = _skeleton.GetKeyFrame();
                AnimKeyFrame frame2 = _skeleton.GetKeyFrame(1000);

                Random rand = new Random();
                foreach (AnimState state in frame2.KeyStates)
                {
                    state.Rotation = rand.Next(361);
                }

                anim.AddKeyFrame(frame1);
                anim.AddKeyFrame(frame2);

                amc_animControl.Animation = anim;
            }
        }

        /// <summary>
        /// Called when the settings tool strip item is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void tsi_settings_Click(object sender, EventArgs e)
        {
            DialogResult result = _settings.ShowDialog();

            if (result == DialogResult.OK)
            {
                BuildSettingsRelated();
            }
        }

        /// <summary>
        /// Overrides the keypress for the mode dropdown
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The key event arguments</param>
        private void tsc_mode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Called when the color selector's value has changed
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void cdd_limbColor_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Color = cdd_limbColor.SelectedColor;
                dbpnl_renderScreen.Invalidate();
            }
        }

        /// <summary>
        /// Called when a new limb is selected in the limb browser
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The related event arguments</param>
        private void trv_limbBrowser_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            SelectedLimb = (Limb)e.Node.Tag;
        }

        /// <summary>
        /// Called when the value in the disable box has changed
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">A blank event args</param>
        void disableBox_SelectedValueChanged(object sender, EventArgs e)
        {
            _state = (FormState)_disableBox.SelectedItem;

            if (_state == FormState.Animation)
            {

                dbpnl_renderScreen.Height = Height - _disableBox.Height;
                amc_animControl.Visible = true;
                grp_properties.Enabled = false;
            }
            else
            {
                dbpnl_renderScreen.Height = Height;
                amc_animControl.Visible = false;
                grp_properties.Enabled = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            chklst_test.Add("foo");
            chklst_test.Add("bar");
        }
    }

    /// <summary>
    /// Represents the state of the main form
    /// </summary>
    public enum FormState
    {
        /// <summary>
        /// The form is in rigging mode
        /// </summary>
        Rigging,
        /// <summary>
        /// The form is in animation mode
        /// </summary>
        Animation
    }
}
