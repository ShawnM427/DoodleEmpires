namespace MasterServer
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lst_knownServers = new System.Windows.Forms.ListView();
            this.lbl_knownServers = new System.Windows.Forms.Label();
            this.rtb_info = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lst_knownServers
            // 
            this.lst_knownServers.Location = new System.Drawing.Point(13, 27);
            this.lst_knownServers.Name = "lst_knownServers";
            this.lst_knownServers.Size = new System.Drawing.Size(169, 475);
            this.lst_knownServers.TabIndex = 0;
            this.lst_knownServers.UseCompatibleStateImageBehavior = false;
            // 
            // lbl_knownServers
            // 
            this.lbl_knownServers.AutoSize = true;
            this.lbl_knownServers.Location = new System.Drawing.Point(13, 8);
            this.lbl_knownServers.Name = "lbl_knownServers";
            this.lbl_knownServers.Size = new System.Drawing.Size(82, 13);
            this.lbl_knownServers.TabIndex = 1;
            this.lbl_knownServers.Text = "Known Servers:";
            // 
            // rtb_info
            // 
            this.rtb_info.Location = new System.Drawing.Point(189, 27);
            this.rtb_info.Name = "rtb_info";
            this.rtb_info.ReadOnly = true;
            this.rtb_info.Size = new System.Drawing.Size(419, 243);
            this.rtb_info.TabIndex = 2;
            this.rtb_info.Text = "";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 514);
            this.Controls.Add(this.rtb_info);
            this.Controls.Add(this.lbl_knownServers);
            this.Controls.Add(this.lst_knownServers);
            this.Name = "MainWindow";
            this.Text = "Doodle Empires Master Server";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lst_knownServers;
        private System.Windows.Forms.Label lbl_knownServers;
        private System.Windows.Forms.RichTextBox rtb_info;
    }
}

