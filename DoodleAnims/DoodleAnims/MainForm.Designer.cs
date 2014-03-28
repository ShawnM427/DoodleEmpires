namespace DoodleAnims
{
    partial class MainForm
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
            this.grp_properties = new System.Windows.Forms.GroupBox();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_add = new System.Windows.Forms.Button();
            this.img_texture = new System.Windows.Forms.PictureBox();
            this.lbl_image = new System.Windows.Forms.Label();
            this.pnl_colorStrip = new System.Windows.Forms.Panel();
            this.txt_color = new System.Windows.Forms.Label();
            this.cmb_type = new System.Windows.Forms.ComboBox();
            this.lbl_type = new System.Windows.Forms.Label();
            this.lbl_length = new System.Windows.Forms.Label();
            this.txt_prop_scale = new System.Windows.Forms.Label();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.lbl_rotation = new System.Windows.Forms.Label();
            this.lbl_name = new System.Windows.Forms.Label();
            this.cdl_color = new System.Windows.Forms.ColorDialog();
            this.fdl_imageImport = new System.Windows.Forms.OpenFileDialog();
            this.fdl_saveSkeleton = new System.Windows.Forms.SaveFileDialog();
            this.fdl_loadSkeleton = new System.Windows.Forms.OpenFileDialog();
            this.dbpnl_renderScreen = new DoodleAnims.DoubleBufferedPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_new = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_save = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_saveSkele = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_saveAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_load = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_loadSkele = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_loadAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_export = new System.Windows.Forms.ToolStripMenuItem();
            this.nib_length = new DoodleAnims.NumericInputBox();
            this.nib_scale = new DoodleAnims.NumericInputBox();
            this.nib_rotation = new DoodleAnims.NumericInputBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).BeginInit();
            this.mainWindow.Panel1.SuspendLayout();
            this.mainWindow.Panel2.SuspendLayout();
            this.mainWindow.SuspendLayout();
            this.grp_properties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_texture)).BeginInit();
            this.dbpnl_renderScreen.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            // grp_properties
            // 
            this.grp_properties.Controls.Add(this.btn_remove);
            this.grp_properties.Controls.Add(this.btn_add);
            this.grp_properties.Controls.Add(this.img_texture);
            this.grp_properties.Controls.Add(this.lbl_image);
            this.grp_properties.Controls.Add(this.pnl_colorStrip);
            this.grp_properties.Controls.Add(this.txt_color);
            this.grp_properties.Controls.Add(this.cmb_type);
            this.grp_properties.Controls.Add(this.lbl_type);
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
            // btn_remove
            // 
            this.btn_remove.BackColor = System.Drawing.Color.Transparent;
            this.btn_remove.BackgroundImage = global::DoodleAnims.Properties.Resources.minus;
            this.btn_remove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_remove.FlatAppearance.BorderSize = 0;
            this.btn_remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_remove.Location = new System.Drawing.Point(47, 364);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(35, 35);
            this.btn_remove.TabIndex = 16;
            this.btn_remove.UseVisualStyleBackColor = false;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // btn_add
            // 
            this.btn_add.BackColor = System.Drawing.Color.Transparent;
            this.btn_add.BackgroundImage = global::DoodleAnims.Properties.Resources.plus;
            this.btn_add.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_add.FlatAppearance.BorderSize = 0;
            this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add.Location = new System.Drawing.Point(6, 364);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(35, 35);
            this.btn_add.TabIndex = 15;
            this.btn_add.UseVisualStyleBackColor = false;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // img_texture
            // 
            this.img_texture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.img_texture.Location = new System.Drawing.Point(67, 177);
            this.img_texture.Name = "img_texture";
            this.img_texture.Size = new System.Drawing.Size(101, 66);
            this.img_texture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.img_texture.TabIndex = 14;
            this.img_texture.TabStop = false;
            this.img_texture.Click += new System.EventHandler(this.img_texture_Click);
            // 
            // lbl_image
            // 
            this.lbl_image.AutoSize = true;
            this.lbl_image.Location = new System.Drawing.Point(22, 180);
            this.lbl_image.Name = "lbl_image";
            this.lbl_image.Size = new System.Drawing.Size(39, 13);
            this.lbl_image.TabIndex = 13;
            this.lbl_image.Text = "Image:";
            // 
            // pnl_colorStrip
            // 
            this.pnl_colorStrip.BackColor = System.Drawing.Color.Black;
            this.pnl_colorStrip.Location = new System.Drawing.Point(67, 148);
            this.pnl_colorStrip.Name = "pnl_colorStrip";
            this.pnl_colorStrip.Size = new System.Drawing.Size(101, 22);
            this.pnl_colorStrip.TabIndex = 12;
            this.pnl_colorStrip.DoubleClick += new System.EventHandler(this.pnl_colorStrip_DoubleClick);
            // 
            // txt_color
            // 
            this.txt_color.AutoSize = true;
            this.txt_color.Location = new System.Drawing.Point(27, 153);
            this.txt_color.Name = "txt_color";
            this.txt_color.Size = new System.Drawing.Size(34, 13);
            this.txt_color.TabIndex = 11;
            this.txt_color.Text = "Color:";
            // 
            // cmb_type
            // 
            this.cmb_type.FormattingEnabled = true;
            this.cmb_type.Location = new System.Drawing.Point(67, 121);
            this.cmb_type.Name = "cmb_type";
            this.cmb_type.Size = new System.Drawing.Size(104, 21);
            this.cmb_type.TabIndex = 10;
            this.cmb_type.SelectedIndexChanged += new System.EventHandler(this.cmb_type_SelectedIndexChanged);
            this.cmb_type.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_type_KeyPress);
            // 
            // lbl_type
            // 
            this.lbl_type.AutoSize = true;
            this.lbl_type.Location = new System.Drawing.Point(27, 124);
            this.lbl_type.Name = "lbl_type";
            this.lbl_type.Size = new System.Drawing.Size(34, 13);
            this.lbl_type.TabIndex = 9;
            this.lbl_type.Text = "Type:";
            // 
            // lbl_length
            // 
            this.lbl_length.AutoSize = true;
            this.lbl_length.Location = new System.Drawing.Point(18, 98);
            this.lbl_length.Name = "lbl_length";
            this.lbl_length.Size = new System.Drawing.Size(43, 13);
            this.lbl_length.TabIndex = 7;
            this.lbl_length.Text = "Length:";
            // 
            // txt_prop_scale
            // 
            this.txt_prop_scale.AutoSize = true;
            this.txt_prop_scale.Location = new System.Drawing.Point(24, 72);
            this.txt_prop_scale.Name = "txt_prop_scale";
            this.txt_prop_scale.Size = new System.Drawing.Size(37, 13);
            this.txt_prop_scale.TabIndex = 5;
            this.txt_prop_scale.Text = "Scale:";
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(67, 17);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(104, 20);
            this.txt_name.TabIndex = 3;
            this.txt_name.TextChanged += new System.EventHandler(this.txt_name_TextChanged);
            // 
            // lbl_rotation
            // 
            this.lbl_rotation.AutoSize = true;
            this.lbl_rotation.Location = new System.Drawing.Point(11, 46);
            this.lbl_rotation.Name = "lbl_rotation";
            this.lbl_rotation.Size = new System.Drawing.Size(50, 13);
            this.lbl_rotation.TabIndex = 1;
            this.lbl_rotation.Text = "Rotation:";
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(23, 20);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(38, 13);
            this.lbl_name.TabIndex = 0;
            this.lbl_name.Text = "Name:";
            // 
            // fdl_imageImport
            // 
            this.fdl_imageImport.DefaultExt = "\".png\"";
            this.fdl_imageImport.Filter = "\"PNG|*.png|JPEG|*.jpg\"";
            this.fdl_imageImport.Title = "Import Image";
            // 
            // fdl_saveSkeleton
            // 
            this.fdl_saveSkeleton.DefaultExt = "des";
            this.fdl_saveSkeleton.Filter = "Doodle Empire Skeleton|*.des";
            // 
            // fdl_loadSkeleton
            // 
            this.fdl_loadSkeleton.DefaultExt = "des";
            this.fdl_loadSkeleton.Filter = "Doodle Empires Skeleton|*.des";
            // 
            // dbpnl_renderScreen
            // 
            this.dbpnl_renderScreen.Controls.Add(this.menuStrip1);
            this.dbpnl_renderScreen.Location = new System.Drawing.Point(0, 0);
            this.dbpnl_renderScreen.Name = "dbpnl_renderScreen";
            this.dbpnl_renderScreen.Size = new System.Drawing.Size(534, 411);
            this.dbpnl_renderScreen.TabIndex = 0;
            this.dbpnl_renderScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.dbpnl_renderScreen_Paint);
            this.dbpnl_renderScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseDown);
            this.dbpnl_renderScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseMove);
            this.dbpnl_renderScreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_new,
            this.tsi_save,
            this.tsi_load,
            this.tsi_export});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tsi_new
            // 
            this.tsi_new.Name = "tsi_new";
            this.tsi_new.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsi_new.Size = new System.Drawing.Size(150, 22);
            this.tsi_new.Text = "New";
            this.tsi_new.Click += new System.EventHandler(this.tsi_new_Click);
            // 
            // tsi_save
            // 
            this.tsi_save.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_saveSkele,
            this.tsi_saveAnim});
            this.tsi_save.Name = "tsi_save";
            this.tsi_save.Size = new System.Drawing.Size(150, 22);
            this.tsi_save.Text = "Save";
            // 
            // tsi_saveSkele
            // 
            this.tsi_saveSkele.Name = "tsi_saveSkele";
            this.tsi_saveSkele.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsi_saveSkele.Size = new System.Drawing.Size(191, 22);
            this.tsi_saveSkele.Text = "Skeleton";
            this.tsi_saveSkele.Click += new System.EventHandler(this.tsi_save_Click);
            // 
            // tsi_saveAnim
            // 
            this.tsi_saveAnim.Name = "tsi_saveAnim";
            this.tsi_saveAnim.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsi_saveAnim.Size = new System.Drawing.Size(191, 22);
            this.tsi_saveAnim.Text = "Animation";
            // 
            // tsi_load
            // 
            this.tsi_load.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_loadSkele,
            this.tsi_loadAnim});
            this.tsi_load.Name = "tsi_load";
            this.tsi_load.Size = new System.Drawing.Size(150, 22);
            this.tsi_load.Text = "Load";
            // 
            // tsi_loadSkele
            // 
            this.tsi_loadSkele.Name = "tsi_loadSkele";
            this.tsi_loadSkele.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.tsi_loadSkele.Size = new System.Drawing.Size(191, 22);
            this.tsi_loadSkele.Text = "Skeleton";
            this.tsi_loadSkele.Click += new System.EventHandler(this.tsi_loadSkele_Click);
            // 
            // tsi_loadAnim
            // 
            this.tsi_loadAnim.Name = "tsi_loadAnim";
            this.tsi_loadAnim.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.tsi_loadAnim.Size = new System.Drawing.Size(191, 22);
            this.tsi_loadAnim.Text = "Animation";
            // 
            // tsi_export
            // 
            this.tsi_export.Name = "tsi_export";
            this.tsi_export.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsi_export.Size = new System.Drawing.Size(150, 22);
            this.tsi_export.Text = "Export";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 411);
            this.Controls.Add(this.mainWindow);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Doodle Anims";
            this.mainWindow.Panel1.ResumeLayout(false);
            this.mainWindow.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).EndInit();
            this.mainWindow.ResumeLayout(false);
            this.grp_properties.ResumeLayout(false);
            this.grp_properties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_texture)).EndInit();
            this.dbpnl_renderScreen.ResumeLayout(false);
            this.dbpnl_renderScreen.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.ComboBox cmb_type;
        private System.Windows.Forms.Label lbl_type;
        private System.Windows.Forms.Panel pnl_colorStrip;
        private System.Windows.Forms.Label txt_color;
        private System.Windows.Forms.ColorDialog cdl_color;
        private System.Windows.Forms.OpenFileDialog fdl_imageImport;
        private System.Windows.Forms.PictureBox img_texture;
        private System.Windows.Forms.Label lbl_image;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsi_new;
        private System.Windows.Forms.ToolStripMenuItem tsi_save;
        private System.Windows.Forms.ToolStripMenuItem tsi_load;
        private System.Windows.Forms.ToolStripMenuItem tsi_export;
        private System.Windows.Forms.ToolStripMenuItem tsi_saveSkele;
        private System.Windows.Forms.ToolStripMenuItem tsi_saveAnim;
        private System.Windows.Forms.SaveFileDialog fdl_saveSkeleton;
        private System.Windows.Forms.ToolStripMenuItem tsi_loadSkele;
        private System.Windows.Forms.ToolStripMenuItem tsi_loadAnim;
        private System.Windows.Forms.OpenFileDialog fdl_loadSkeleton;
    }
}

