using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class frm_launcher : Form
    {
        public frm_launcher()
        {
            InitializeComponent();
        }

        private void btn_launch_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "DoodleEmpires.exe";
                startInfo.Arguments = string.Format("\"{0}\"", txt_username.Text);
                Process.Start(startInfo);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
            }
            this.Close();
        }
    }
}
