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
            this.chk_upnp = new System.Windows.Forms.CheckBox();
            this.btn_begin = new System.Windows.Forms.Button();
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
            this.rtb_info.Location = new System.Drawing.Point(188, 50);
            this.rtb_info.Name = "rtb_info";
            this.rtb_info.ReadOnly = true;
            this.rtb_info.Size = new System.Drawing.Size(420, 241);
            this.rtb_info.TabIndex = 2;
            this.rtb_info.Text = "";
            // 
            // chk_upnp
            // 
            this.chk_upnp.AutoSize = true;
            this.chk_upnp.Checked = true;
            this.chk_upnp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_upnp.Location = new System.Drawing.Point(188, 27);
            this.chk_upnp.Name = "chk_upnp";
            this.chk_upnp.Size = new System.Drawing.Size(96, 17);
            this.chk_upnp.TabIndex = 3;
            this.chk_upnp.Text = "UPnP Enabled";
            this.chk_upnp.UseVisualStyleBackColor = true;
            // 
            // btn_begin
            // 
            this.btn_begin.Location = new System.Drawing.Point(290, 23);
            this.btn_begin.Name = "btn_begin";
            this.btn_begin.Size = new System.Drawing.Size(75, 23);
            this.btn_begin.TabIndex = 4;
            this.btn_begin.Text = "Begin";
            this.btn_begin.UseVisualStyleBackColor = true;
            this.btn_begin.Click += new System.EventHandler(this.btn_begin_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 514);
            this.Controls.Add(this.btn_begin);
            this.Controls.Add(this.chk_upnp);
            this.Controls.Add(this.rtb_info);
            this.Controls.Add(this.lbl_knownServers);
            this.Controls.Add(this.lst_knownServers);
            this.Name = "MainWindow";
            this.Text = "Doodle Empires Master Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lst_knownServers;
        private System.Windows.Forms.Label lbl_knownServers;
        private System.Windows.Forms.RichTextBox rtb_info;
        private System.Windows.Forms.CheckBox chk_upnp;
        private System.Windows.Forms.Button btn_begin;
    }
}

