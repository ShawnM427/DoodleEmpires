using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DoodleAnims.Lib.Anim;

namespace DoodleAnims
{
    public partial class Form1 : Form
    {
        Limb _parentLimb, _childLimb;
        Limb _selectedLimb;

        Timer _clock = new Timer();

        bool _mouseDown = false;

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
                }
            }
        }

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            _parentLimb = new Limb(new PointF(100, 100), Color.Black, 16);
            _parentLimb.Scale = 3.0F;

            _childLimb = new Limb(_parentLimb, Color.Black, 16);
            _childLimb.Scale = 1.0F;

            _clock.Interval = 16;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void dbpnl_renderScreen_Paint(object sender, PaintEventArgs e)
        {
            _parentLimb.Paint(e.Graphics);

            if (_selectedLimb != null)
                e.Graphics.DrawEllipse(Pens.Yellow, _selectedLimb.EndPoint.X - 4, _selectedLimb.EndPoint.Y - 4, 8, 8);
        }

        private void dbpnl_renderScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                SelectedLimb = _parentLimb.Selected(e);
            }

            _mouseDown = true;
            //Capture = true;
        }

        private void dbpnl_renderScreen_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            //Capture = false;
        }

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

        private void nib_rotation_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Rotation = nib_rotation.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        private void nib_scale_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Scale = nib_scale.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }

        private void txt_prop_name_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Name = txt_name.Text;
                dbpnl_renderScreen.Invalidate();
            }
        }

        private void nib_length_TextChanged(object sender, EventArgs e)
        {
            if (SelectedLimb != null)
            {
                SelectedLimb.Length = nib_length.Value;
                dbpnl_renderScreen.Invalidate();
            }
        }
    }
}
