namespace DoodleAnims
{
    partial class SettingsDialog
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
            this.chk_showTree = new System.Windows.Forms.CheckBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.lbl_endpointSize = new System.Windows.Forms.Label();
            this.grp_endpoint = new System.Windows.Forms.GroupBox();
            this.cdd_enpointColor = new DoodleAnims.ColorDropDown();
            this.lbl_endpointColor = new System.Windows.Forms.Label();
            this.nib_endpointSize = new DoodleAnims.NumericInputBox();
            this.cmb_endpointType = new System.Windows.Forms.ComboBox();
            this.lbl_endPointType = new System.Windows.Forms.Label();
            this.grp_orgin = new System.Windows.Forms.GroupBox();
            this.cdd_orginColor = new DoodleAnims.ColorDropDown();
            this.lbl_orginColor = new System.Windows.Forms.Label();
            this.nib_orginSize = new DoodleAnims.NumericInputBox();
            this.cmb_orginType = new System.Windows.Forms.ComboBox();
            this.lbl_orginType = new System.Windows.Forms.Label();
            this.lbl_orginSize = new System.Windows.Forms.Label();
            this.lbl_backgroundColor = new System.Windows.Forms.Label();
            this.cdd_backgroundColor = new DoodleAnims.ColorDropDown();
            this.grp_endpoint.SuspendLayout();
            this.grp_orgin.SuspendLayout();
            this.SuspendLayout();
            // 
            // chk_showTree
            // 
            this.chk_showTree.AutoSize = true;
            this.chk_showTree.Location = new System.Drawing.Point(13, 13);
            this.chk_showTree.Name = "chk_showTree";
            this.chk_showTree.Size = new System.Drawing.Size(104, 17);
            this.chk_showTree.TabIndex = 0;
            this.chk_showTree.Text = "Show Tree View";
            this.chk_showTree.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(143, 298);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 1;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(12, 298);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 2;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // lbl_endpointSize
            // 
            this.lbl_endpointSize.AutoSize = true;
            this.lbl_endpointSize.Location = new System.Drawing.Point(11, 22);
            this.lbl_endpointSize.Name = "lbl_endpointSize";
            this.lbl_endpointSize.Size = new System.Drawing.Size(30, 13);
            this.lbl_endpointSize.TabIndex = 3;
            this.lbl_endpointSize.Text = "Size:";
            // 
            // grp_endpoint
            // 
            this.grp_endpoint.Controls.Add(this.cdd_enpointColor);
            this.grp_endpoint.Controls.Add(this.lbl_endpointColor);
            this.grp_endpoint.Controls.Add(this.nib_endpointSize);
            this.grp_endpoint.Controls.Add(this.cmb_endpointType);
            this.grp_endpoint.Controls.Add(this.lbl_endPointType);
            this.grp_endpoint.Controls.Add(this.lbl_endpointSize);
            this.grp_endpoint.Location = new System.Drawing.Point(13, 37);
            this.grp_endpoint.Name = "grp_endpoint";
            this.grp_endpoint.Size = new System.Drawing.Size(205, 111);
            this.grp_endpoint.TabIndex = 4;
            this.grp_endpoint.TabStop = false;
            this.grp_endpoint.Text = "Endpoint marker";
            // 
            // cdd_enpointColor
            // 
            this.cdd_enpointColor.AllowCustomColor = false;
            this.cdd_enpointColor.AllowTransparent = true;
            this.cdd_enpointColor.BackColor = System.Drawing.Color.White;
            this.cdd_enpointColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cdd_enpointColor.ForeColor = System.Drawing.Color.Black;
            this.cdd_enpointColor.FormattingEnabled = true;
            this.cdd_enpointColor.Location = new System.Drawing.Point(47, 72);
            this.cdd_enpointColor.Name = "cdd_enpointColor";
            this.cdd_enpointColor.SelectedColor = System.Drawing.Color.Transparent;
            this.cdd_enpointColor.Size = new System.Drawing.Size(137, 21);
            this.cdd_enpointColor.TabIndex = 9;
            // 
            // lbl_endpointColor
            // 
            this.lbl_endpointColor.AutoSize = true;
            this.lbl_endpointColor.Location = new System.Drawing.Point(7, 76);
            this.lbl_endpointColor.Name = "lbl_endpointColor";
            this.lbl_endpointColor.Size = new System.Drawing.Size(34, 13);
            this.lbl_endpointColor.TabIndex = 8;
            this.lbl_endpointColor.Text = "Color:";
            // 
            // nib_endpointSize
            // 
            this.nib_endpointSize.Integer = false;
            this.nib_endpointSize.Location = new System.Drawing.Point(47, 19);
            this.nib_endpointSize.Name = "nib_endpointSize";
            this.nib_endpointSize.Size = new System.Drawing.Size(137, 20);
            this.nib_endpointSize.TabIndex = 6;
            this.nib_endpointSize.Text = "0";
            this.nib_endpointSize.Value = 0F;
            // 
            // cmb_endpointType
            // 
            this.cmb_endpointType.FormattingEnabled = true;
            this.cmb_endpointType.Items.AddRange(new object[] {
            "Filled",
            "Outline"});
            this.cmb_endpointType.Location = new System.Drawing.Point(47, 45);
            this.cmb_endpointType.Name = "cmb_endpointType";
            this.cmb_endpointType.Size = new System.Drawing.Size(137, 21);
            this.cmb_endpointType.TabIndex = 5;
            // 
            // lbl_endPointType
            // 
            this.lbl_endPointType.AutoSize = true;
            this.lbl_endPointType.Location = new System.Drawing.Point(7, 48);
            this.lbl_endPointType.Name = "lbl_endPointType";
            this.lbl_endPointType.Size = new System.Drawing.Size(34, 13);
            this.lbl_endPointType.TabIndex = 4;
            this.lbl_endPointType.Text = "Type:";
            // 
            // grp_orgin
            // 
            this.grp_orgin.Controls.Add(this.cdd_orginColor);
            this.grp_orgin.Controls.Add(this.lbl_orginColor);
            this.grp_orgin.Controls.Add(this.nib_orginSize);
            this.grp_orgin.Controls.Add(this.cmb_orginType);
            this.grp_orgin.Controls.Add(this.lbl_orginType);
            this.grp_orgin.Controls.Add(this.lbl_orginSize);
            this.grp_orgin.Location = new System.Drawing.Point(13, 154);
            this.grp_orgin.Name = "grp_orgin";
            this.grp_orgin.Size = new System.Drawing.Size(205, 111);
            this.grp_orgin.TabIndex = 10;
            this.grp_orgin.TabStop = false;
            this.grp_orgin.Text = "Orgin Marker";
            // 
            // cdd_orginColor
            // 
            this.cdd_orginColor.AllowCustomColor = false;
            this.cdd_orginColor.AllowTransparent = true;
            this.cdd_orginColor.BackColor = System.Drawing.Color.White;
            this.cdd_orginColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cdd_orginColor.ForeColor = System.Drawing.Color.Black;
            this.cdd_orginColor.FormattingEnabled = true;
            this.cdd_orginColor.Location = new System.Drawing.Point(47, 72);
            this.cdd_orginColor.Name = "cdd_orginColor";
            this.cdd_orginColor.SelectedColor = System.Drawing.Color.Transparent;
            this.cdd_orginColor.Size = new System.Drawing.Size(137, 21);
            this.cdd_orginColor.TabIndex = 9;
            // 
            // lbl_orginColor
            // 
            this.lbl_orginColor.AutoSize = true;
            this.lbl_orginColor.Location = new System.Drawing.Point(7, 76);
            this.lbl_orginColor.Name = "lbl_orginColor";
            this.lbl_orginColor.Size = new System.Drawing.Size(34, 13);
            this.lbl_orginColor.TabIndex = 8;
            this.lbl_orginColor.Text = "Color:";
            // 
            // nib_orginSize
            // 
            this.nib_orginSize.Integer = false;
            this.nib_orginSize.Location = new System.Drawing.Point(47, 19);
            this.nib_orginSize.Name = "nib_orginSize";
            this.nib_orginSize.Size = new System.Drawing.Size(137, 20);
            this.nib_orginSize.TabIndex = 6;
            this.nib_orginSize.Text = "0";
            this.nib_orginSize.Value = 0F;
            // 
            // cmb_orginType
            // 
            this.cmb_orginType.FormattingEnabled = true;
            this.cmb_orginType.Items.AddRange(new object[] {
            "Filled",
            "Outline"});
            this.cmb_orginType.Location = new System.Drawing.Point(47, 45);
            this.cmb_orginType.Name = "cmb_orginType";
            this.cmb_orginType.Size = new System.Drawing.Size(137, 21);
            this.cmb_orginType.TabIndex = 5;
            // 
            // lbl_orginType
            // 
            this.lbl_orginType.AutoSize = true;
            this.lbl_orginType.Location = new System.Drawing.Point(7, 48);
            this.lbl_orginType.Name = "lbl_orginType";
            this.lbl_orginType.Size = new System.Drawing.Size(34, 13);
            this.lbl_orginType.TabIndex = 4;
            this.lbl_orginType.Text = "Type:";
            // 
            // lbl_orginSize
            // 
            this.lbl_orginSize.AutoSize = true;
            this.lbl_orginSize.Location = new System.Drawing.Point(11, 22);
            this.lbl_orginSize.Name = "lbl_orginSize";
            this.lbl_orginSize.Size = new System.Drawing.Size(30, 13);
            this.lbl_orginSize.TabIndex = 3;
            this.lbl_orginSize.Text = "Size:";
            // 
            // lbl_backgroundColor
            // 
            this.lbl_backgroundColor.AutoSize = true;
            this.lbl_backgroundColor.Location = new System.Drawing.Point(9, 274);
            this.lbl_backgroundColor.Name = "lbl_backgroundColor";
            this.lbl_backgroundColor.Size = new System.Drawing.Size(95, 13);
            this.lbl_backgroundColor.TabIndex = 10;
            this.lbl_backgroundColor.Text = "Background Color:";
            // 
            // cdd_backgroundColor
            // 
            this.cdd_backgroundColor.AllowCustomColor = false;
            this.cdd_backgroundColor.AllowTransparent = false;
            this.cdd_backgroundColor.BackColor = System.Drawing.Color.White;
            this.cdd_backgroundColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cdd_backgroundColor.ForeColor = System.Drawing.Color.Black;
            this.cdd_backgroundColor.FormattingEnabled = true;
            this.cdd_backgroundColor.Location = new System.Drawing.Point(110, 271);
            this.cdd_backgroundColor.Name = "cdd_backgroundColor";
            this.cdd_backgroundColor.SelectedColor = System.Drawing.Color.Transparent;
            this.cdd_backgroundColor.Size = new System.Drawing.Size(108, 21);
            this.cdd_backgroundColor.TabIndex = 11;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 331);
            this.Controls.Add(this.cdd_backgroundColor);
            this.Controls.Add(this.lbl_backgroundColor);
            this.Controls.Add(this.grp_orgin);
            this.Controls.Add(this.grp_endpoint);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.chk_showTree);
            this.Name = "SettingsDialog";
            this.Text = "Settings";
            this.grp_endpoint.ResumeLayout(false);
            this.grp_endpoint.PerformLayout();
            this.grp_orgin.ResumeLayout(false);
            this.grp_orgin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chk_showTree;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Label lbl_endpointSize;
        private System.Windows.Forms.GroupBox grp_endpoint;
        private NumericInputBox nib_endpointSize;
        private System.Windows.Forms.ComboBox cmb_endpointType;
        private System.Windows.Forms.Label lbl_endPointType;
        private System.Windows.Forms.Label lbl_endpointColor;
        private ColorDropDown cdd_enpointColor;
        private System.Windows.Forms.GroupBox grp_orgin;
        private ColorDropDown cdd_orginColor;
        private System.Windows.Forms.Label lbl_orginColor;
        private NumericInputBox nib_orginSize;
        private System.Windows.Forms.ComboBox cmb_orginType;
        private System.Windows.Forms.Label lbl_orginType;
        private System.Windows.Forms.Label lbl_orginSize;
        private ColorDropDown cdd_backgroundColor;
        private System.Windows.Forms.Label lbl_backgroundColor;
    }
}