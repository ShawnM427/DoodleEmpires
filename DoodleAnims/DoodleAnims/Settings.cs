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
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            chk_showTree.Checked = Properties.Settings.Default.ShowTree;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Properties.Settings.Default.ShowTree = chk_showTree.Checked;

            Properties.Settings.Default.Save();

            Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
