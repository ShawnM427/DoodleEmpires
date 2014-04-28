namespace DoodleServer
{
    partial class frm_main
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
            this.rtb_output = new System.Windows.Forms.RichTextBox();
            this.btn_launch = new System.Windows.Forms.Button();
            this.lbl_name = new System.Windows.Forms.Label();
            this.txt_serverName = new System.Windows.Forms.TextBox();
            this.lbl_port = new System.Windows.Forms.Label();
            this.nud_port = new System.Windows.Forms.NumericUpDown();
            this.txt_commandIn = new System.Windows.Forms.TextBox();
            this.btn_perform = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).BeginInit();
            this.SuspendLayout();
            // 
            // rtb_output
            // 
            this.rtb_output.Enabled = false;
            this.rtb_output.Location = new System.Drawing.Point(13, 40);
            this.rtb_output.Name = "rtb_output";
            this.rtb_output.Size = new System.Drawing.Size(538, 376);
            this.rtb_output.TabIndex = 0;
            this.rtb_output.Text = "";
            // 
            // btn_launch
            // 
            this.btn_launch.Location = new System.Drawing.Point(476, 12);
            this.btn_launch.Name = "btn_launch";
            this.btn_launch.Size = new System.Drawing.Size(75, 23);
            this.btn_launch.TabIndex = 1;
            this.btn_launch.Text = "Launch";
            this.btn_launch.UseVisualStyleBackColor = true;
            this.btn_launch.Click += new System.EventHandler(this.btn_launch_Click);
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(12, 17);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(72, 13);
            this.lbl_name.TabIndex = 2;
            this.lbl_name.Text = "Server Name:";
            // 
            // txt_serverName
            // 
            this.txt_serverName.Location = new System.Drawing.Point(90, 14);
            this.txt_serverName.Name = "txt_serverName";
            this.txt_serverName.Size = new System.Drawing.Size(100, 20);
            this.txt_serverName.TabIndex = 3;
            // 
            // lbl_port
            // 
            this.lbl_port.AutoSize = true;
            this.lbl_port.Location = new System.Drawing.Point(205, 17);
            this.lbl_port.Name = "lbl_port";
            this.lbl_port.Size = new System.Drawing.Size(29, 13);
            this.lbl_port.TabIndex = 4;
            this.lbl_port.Text = "Port:";
            // 
            // nud_port
            // 
            this.nud_port.Location = new System.Drawing.Point(240, 14);
            this.nud_port.Maximum = new decimal(new int[] {
            14250,
            0,
            0,
            0});
            this.nud_port.Minimum = new decimal(new int[] {
            14240,
            0,
            0,
            0});
            this.nud_port.Name = "nud_port";
            this.nud_port.Size = new System.Drawing.Size(80, 20);
            this.nud_port.TabIndex = 5;
            this.nud_port.Value = new decimal(new int[] {
            14245,
            0,
            0,
            0});
            // 
            // txt_commandIn
            // 
            this.txt_commandIn.Location = new System.Drawing.Point(15, 422);
            this.txt_commandIn.Name = "txt_commandIn";
            this.txt_commandIn.Size = new System.Drawing.Size(455, 20);
            this.txt_commandIn.TabIndex = 6;
            this.txt_commandIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_commandIn_KeyUp);
            // 
            // btn_perform
            // 
            this.btn_perform.Location = new System.Drawing.Point(476, 422);
            this.btn_perform.Name = "btn_perform";
            this.btn_perform.Size = new System.Drawing.Size(75, 23);
            this.btn_perform.TabIndex = 7;
            this.btn_perform.Text = "Perform";
            this.btn_perform.UseVisualStyleBackColor = true;
            this.btn_perform.Click += new System.EventHandler(this.btn_perform_Click);
            // 
            // frm_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 456);
            this.Controls.Add(this.btn_perform);
            this.Controls.Add(this.txt_commandIn);
            this.Controls.Add(this.nud_port);
            this.Controls.Add(this.lbl_port);
            this.Controls.Add(this.txt_serverName);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.btn_launch);
            this.Controls.Add(this.rtb_output);
            this.Name = "frm_main";
            this.Text = "Doodle Server";
            ((System.ComponentModel.ISupportInitialize)(this.nud_port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_launch;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.TextBox txt_serverName;
        private System.Windows.Forms.Label lbl_port;
        private System.Windows.Forms.NumericUpDown nud_port;
        private System.Windows.Forms.TextBox txt_commandIn;
        private System.Windows.Forms.Button btn_perform;
        public System.Windows.Forms.RichTextBox rtb_output;
    }
}

