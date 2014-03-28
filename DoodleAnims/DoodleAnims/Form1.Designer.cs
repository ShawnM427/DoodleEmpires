namespace DoodleAnims
{
    partial class Form1
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
            this.mainWindow = new System.Windows.Forms.SplitContainer();
            this.dbpnl_renderScreen = new DoodleAnims.DoubleBufferedPanel();
            this.grp_properties = new System.Windows.Forms.GroupBox();
            this.nib_length = new DoodleAnims.NumericInputBox();
            this.lbl_length = new System.Windows.Forms.Label();
            this.nib_scale = new DoodleAnims.NumericInputBox();
            this.txt_prop_scale = new System.Windows.Forms.Label();
            this.nib_rotation = new DoodleAnims.NumericInputBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.lbl_rotation = new System.Windows.Forms.Label();
            this.lbl_name = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).BeginInit();
            this.mainWindow.Panel1.SuspendLayout();
            this.mainWindow.Panel2.SuspendLayout();
            this.mainWindow.SuspendLayout();
            this.grp_properties.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainWindow
            // 
            this.mainWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainWindow.Location = new System.Drawing.Point(0, 0);
            this.mainWindow.Name = "mainWindow";
            // 
            // mainWindow.Panel1
            // 
            this.mainWindow.Panel1.BackColor = System.Drawing.Color.White;
            this.mainWindow.Panel1.Controls.Add(this.dbpnl_renderScreen);
            // 
            // mainWindow.Panel2
            // 
            this.mainWindow.Panel2.Controls.Add(this.grp_properties);
            this.mainWindow.Size = new System.Drawing.Size(720, 411);
            this.mainWindow.SplitterDistance = 533;
            this.mainWindow.TabIndex = 0;
            // 
            // dbpnl_renderScreen
            // 
            this.dbpnl_renderScreen.Location = new System.Drawing.Point(0, 0);
            this.dbpnl_renderScreen.Name = "dbpnl_renderScreen";
            this.dbpnl_renderScreen.Size = new System.Drawing.Size(534, 411);
            this.dbpnl_renderScreen.TabIndex = 0;
            this.dbpnl_renderScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.dbpnl_renderScreen_Paint);
            this.dbpnl_renderScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseDown);
            this.dbpnl_renderScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseMove);
            this.dbpnl_renderScreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseUp);
            // 
            // grp_properties
            // 
            this.grp_properties.Controls.Add(this.nib_length);
            this.grp_properties.Controls.Add(this.lbl_length);
            this.grp_properties.Controls.Add(this.nib_scale);
            this.grp_properties.Controls.Add(this.txt_prop_scale);
            this.grp_properties.Controls.Add(this.nib_rotation);
            this.grp_properties.Controls.Add(this.txt_name);
            this.grp_properties.Controls.Add(this.lbl_rotation);
            this.grp_properties.Controls.Add(this.lbl_name);
            this.grp_properties.Location = new System.Drawing.Point(3, 3);
            this.grp_properties.Name = "grp_properties";
            this.grp_properties.Size = new System.Drawing.Size(177, 405);
            this.grp_properties.TabIndex = 0;
            this.grp_properties.TabStop = false;
            this.grp_properties.Text = "Properties";
            // 
            // nib_length
            // 
            this.nib_length.Integer = false;
            this.nib_length.Location = new System.Drawing.Point(67, 95);
            this.nib_length.Name = "nib_length";
            this.nib_length.Size = new System.Drawing.Size(104, 20);
            this.nib_length.TabIndex = 8;
            this.nib_length.Text = "0";
            this.nib_length.Value = 0F;
            this.nib_length.TextChanged += new System.EventHandler(this.nib_length_TextChanged);
            // 
            // lbl_length
            // 
            this.lbl_length.AutoSize = true;
            this.lbl_length.Location = new System.Drawing.Point(14, 98);
            this.lbl_length.Name = "lbl_length";
            this.lbl_length.Size = new System.Drawing.Size(43, 13);
            this.lbl_length.TabIndex = 7;
            this.lbl_length.Text = "Length:";
            // 
            // nib_scale
            // 
            this.nib_scale.Integer = false;
            this.nib_scale.Location = new System.Drawing.Point(67, 69);
            this.nib_scale.Name = "nib_scale";
            this.nib_scale.Size = new System.Drawing.Size(104, 20);
            this.nib_scale.TabIndex = 6;
            this.nib_scale.Text = "0";
            this.nib_scale.Value = 0F;
            this.nib_scale.TextChanged += new System.EventHandler(this.nib_scale_TextChanged);
            // 
            // txt_prop_scale
            // 
            this.txt_prop_scale.AutoSize = true;
            this.txt_prop_scale.Location = new System.Drawing.Point(20, 72);
            this.txt_prop_scale.Name = "txt_prop_scale";
            this.txt_prop_scale.Size = new System.Drawing.Size(37, 13);
            this.txt_prop_scale.TabIndex = 5;
            this.txt_prop_scale.Text = "Scale:";
            // 
            // nib_rotation
            // 
            this.nib_rotation.Integer = true;
            this.nib_rotation.Location = new System.Drawing.Point(67, 43);
            this.nib_rotation.Name = "nib_rotation";
            this.nib_rotation.Size = new System.Drawing.Size(104, 20);
            this.nib_rotation.TabIndex = 4;
            this.nib_rotation.Text = "0";
            this.nib_rotation.Value = 0F;
            this.nib_rotation.TextChanged += new System.EventHandler(this.nib_rotation_ValueChanged);
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(67, 17);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(104, 20);
            this.txt_name.TabIndex = 3;
            this.txt_name.TextChanged += new System.EventHandler(this.txt_prop_name_TextChanged);
            // 
            // lbl_rotation
            // 
            this.lbl_rotation.AutoSize = true;
            this.lbl_rotation.Location = new System.Drawing.Point(7, 46);
            this.lbl_rotation.Name = "lbl_rotation";
            this.lbl_rotation.Size = new System.Drawing.Size(50, 13);
            this.lbl_rotation.TabIndex = 1;
            this.lbl_rotation.Text = "Rotation:";
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(19, 20);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(38, 13);
            this.lbl_name.TabIndex = 0;
            this.lbl_name.Text = "Name:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 411);
            this.Controls.Add(this.mainWindow);
            this.Name = "Form1";
            this.Text = "Form1";
            this.mainWindow.Panel1.ResumeLayout(false);
            this.mainWindow.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).EndInit();
            this.mainWindow.ResumeLayout(false);
            this.grp_properties.ResumeLayout(false);
            this.grp_properties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainWindow;
        private System.Windows.Forms.GroupBox grp_properties;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Label lbl_rotation;
        private System.Windows.Forms.Label lbl_name;
        private NumericInputBox nib_rotation;
        private NumericInputBox nib_scale;
        private System.Windows.Forms.Label txt_prop_scale;
        private System.Windows.Forms.Label lbl_length;
        private NumericInputBox nib_length;
        private DoubleBufferedPanel dbpnl_renderScreen;
    }
}

