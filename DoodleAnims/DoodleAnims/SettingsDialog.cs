using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoodleAnims
{
    /// <summary>
    /// A form where the user can edit settings
    /// </summary>
    public partial class SettingsDialog : Form
    {
        /// <summary>
        /// Creates a new settings form
        /// </summary>
        public SettingsDialog()
        {
            InitializeComponent();

            chk_showTree.Checked = Properties.Settings.Default.ShowTree;
            cmb_endpointType.SelectedIndex = Properties.Settings.Default.EndPointMarkerType == 0 ? 0 : 1;
            nib_endpointSize.Value = Properties.Settings.Default.EndPointMarkerSize;
            cdd_enpointColor.SelectedColor = Properties.Settings.Default.EndPointColor;

            cmb_orginType.SelectedIndex = Properties.Settings.Default.OrginMarkerType == 0 ? 0 : 1;
            nib_orginSize.Value = Properties.Settings.Default.OrginMarkerSize;
            cdd_orginColor.SelectedColor = Properties.Settings.Default.OrginColor;

            cdd_backgroundColor.SelectedColor = Properties.Settings.Default.BackgroundColor;
        }

        /// <summary>
        /// Called when the user clicks ok
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">A blank event args</param>
        private void btn_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Properties.Settings.Default.ShowTree = chk_showTree.Checked;
            Properties.Settings.Default.EndPointMarkerType = (byte)cmb_endpointType.SelectedIndex;
            Properties.Settings.Default.EndPointMarkerSize = nib_endpointSize.Value;
            Properties.Settings.Default.EndPointColor = cdd_enpointColor.SelectedColor;

            Properties.Settings.Default.OrginMarkerType = (byte)cmb_orginType.SelectedIndex;
            Properties.Settings.Default.OrginMarkerSize = nib_orginSize.Value;
            Properties.Settings.Default.OrginColor = cdd_orginColor.SelectedColor;

            Properties.Settings.Default.BackgroundColor = cdd_backgroundColor.SelectedColor;

            Properties.Settings.Default.Save();

            Close();
        }

        /// <summary>
        /// Called when the user clicks cancel
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">A blank event args</param>
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
