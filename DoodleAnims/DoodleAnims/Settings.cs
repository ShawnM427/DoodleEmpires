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
    public partial class Settings : Form
    {
        /// <summary>
        /// Creates a new settings form
        /// </summary>
        public Settings()
        {
            InitializeComponent();

            chk_showTree.Checked = Properties.Settings.Default.ShowTree;
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
