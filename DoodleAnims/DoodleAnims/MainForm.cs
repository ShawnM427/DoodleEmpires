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

namespace DoodleAnims
{
    public partial class MainForm : Form
    {
        Limb _orginLimb;
        Limb _selectedLimb;

        Timer _clock = new Timer();

        bool _mouseDown = false;

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
                    pnl_colorStrip.BackColor = _selectedLimb.Color;
                    nib_offsetX.Value = _selectedLimb.OffsetX;
                    nib_offsetY.Value = _selectedLimb.OffsetY;

                    nib_imageAngle.Value = _selectedLimb.ImageAngle;
                    chk_xFlip.Checked = _selectedLimb.XFlip;
                    chk_yFlip.Checked = _selectedLimb.YFlip;

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
                    pnl_colorStrip.BackColor = Color.Black;
                    nib_offsetX.Value = 0;
                    nib_offsetY.Value = 0;
                    nib_imageAngle.Value = 0;
                    chk_xFlip.Checked = false;
                    chk_yFlip.Checked = false;
                    img_texture.Image = null;
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();

            DoubleBuffered = true;

            _orginLimb = new Limb(new PointF(dbpnl_renderScreen.Size.Width / 2, dbpnl_renderScreen.Size.Height / 2), Color.Black, 0);
            SelectedLimb = new Limb(_orginLimb, Color.Black, 16);

            _clock.Interval = 16;

            cmb_type.Items.Add(LimbType.Line);
            cmb_type.Items.Add(LimbType.Circle);
            cmb_type.Items.Add(LimbType.Textured);

            cmb_type.SelectedItem = LimbType.Line;
        }
        
        /// <summary>
        /// Called when the main screen needs to render
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The drawing arguments to use</param>
        private void dbpnl_renderScreen_Paint(object sender, PaintEventArgs e)
        {
            _orginLimb.Paint(e.Graphics);

            if (_selectedLimb != null)
            {
                e.Graphics.FillEllipse(Brushes.Red, _selectedLimb.EndPoint.X - 4, _selectedLimb.EndPoint.Y - 4, 8, 8);
                if (_selectedLimb.Parent != null)
                    e.Graphics.FillEllipse(Brushes.Green, _selectedLimb.Parent.EndPoint.X - 2, _selectedLimb.Parent.EndPoint.Y - 2, 4, 4);
            }
        }

        /// <summary>
        /// Called when the mouse is pressed in the main render screen
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The mouse arguments</param>
        private void dbpnl_renderScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                SelectedLimb = _orginLimb.Selected(e);
            }

            _mouseDown = true;
            //Capture = true;
        }

        /// <summary>
        /// Called when the mouse is released in the main render screen
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The mouse arguments</param>
        private void dbpnl_renderScreen_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            //Capture = false;
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
        /// Called when the color strip is double clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void pnl_colorStrip_DoubleClick(object sender, EventArgs e)
        {
            cdl_color.ShowDialog();
            pnl_colorStrip.BackColor = cdl_color.Color;

            if (SelectedLimb != null)
            {
                SelectedLimb.Color = cdl_color.Color;
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
                SelectedLimb.XFlip = chk_xFlip.Checked;
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
                SelectedLimb.YFlip = chk_yFlip.Checked;
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
            if (SelectedLimb != null)
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
                    else if (_selectedLimb == _orginLimb)
                    {
                        _orginLimb = null;
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
            else if (_orginLimb == null)
            {
                _orginLimb = new Limb(new PointF(dbpnl_renderScreen.Size.Width / 2, dbpnl_renderScreen.Size.Height / 2), Color.Black, 0);
                SelectedLimb = new Limb(_orginLimb, Color.Black, 16);
            }
        }

        /// <summary>
        /// Called when the new tool strip item is clicked
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The blank event args</param>
        private void tsi_new_Click(object sender, EventArgs e)
        {
            _orginLimb = new Limb(new PointF(dbpnl_renderScreen.Size.Width / 2, dbpnl_renderScreen.Size.Height / 2), Color.Black, 0);
            SelectedLimb = new Limb(_orginLimb, Color.Black, 16);
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

                _orginLimb.Save(w);

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

                _orginLimb = Limb.Load(null, rd);

                SelectedLimb = _orginLimb;

                rd.Close();
                rd.Dispose();
                s.Dispose();
            }
        }
    }
}
