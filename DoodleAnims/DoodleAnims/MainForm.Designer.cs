namespace DoodleAnims
{
    /// <summary>
    /// The main editor form for DE Anims
    /// </summary>
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
            this.dbpnl_renderScreen = new DoodleAnims.DoubleBufferedPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsi_file = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_new = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_save = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_saveSkele = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_saveAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_load = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_loadSkele = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_loadAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_export = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_settings = new System.Windows.Forms.ToolStripMenuItem();
            this.tss_1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsl_mode = new System.Windows.Forms.ToolStripLabel();
            this.tsc_mode = new System.Windows.Forms.ToolStripComboBox();
            this.trv_limbBrowser = new System.Windows.Forms.TreeView();
            this.grp_properties = new System.Windows.Forms.GroupBox();
            this.cdd_limbColor = new DoodleAnims.ColorDropDown();
            this.chk_yFlip = new System.Windows.Forms.CheckBox();
            this.chk_xFlip = new System.Windows.Forms.CheckBox();
            this.nib_imageAngle = new DoodleAnims.NumericInputBox();
            this.lbl_imageAngle = new System.Windows.Forms.Label();
            this.nib_offsetY = new DoodleAnims.NumericInputBox();
            this.nib_offsetX = new DoodleAnims.NumericInputBox();
            this.lbl_offsetY = new System.Windows.Forms.Label();
            this.lbl_offsetX = new System.Windows.Forms.Label();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_add = new System.Windows.Forms.Button();
            this.img_texture = new System.Windows.Forms.PictureBox();
            this.lbl_image = new System.Windows.Forms.Label();
            this.txt_color = new System.Windows.Forms.Label();
            this.cmb_type = new System.Windows.Forms.ComboBox();
            this.lbl_type = new System.Windows.Forms.Label();
            this.nib_length = new DoodleAnims.NumericInputBox();
            this.lbl_length = new System.Windows.Forms.Label();
            this.nib_scale = new DoodleAnims.NumericInputBox();
            this.txt_prop_scale = new System.Windows.Forms.Label();
            this.nib_rotation = new DoodleAnims.NumericInputBox();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.lbl_rotation = new System.Windows.Forms.Label();
            this.lbl_name = new System.Windows.Forms.Label();
            this.cdl_color = new System.Windows.Forms.ColorDialog();
            this.fdl_imageImport = new System.Windows.Forms.OpenFileDialog();
            this.fdl_saveSkeleton = new System.Windows.Forms.SaveFileDialog();
            this.fdl_loadSkeleton = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).BeginInit();
            this.mainWindow.Panel1.SuspendLayout();
            this.mainWindow.Panel2.SuspendLayout();
            this.mainWindow.SuspendLayout();
            this.dbpnl_renderScreen.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.grp_properties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_texture)).BeginInit();
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
            this.mainWindow.Panel2.Controls.Add(this.trv_limbBrowser);
            this.mainWindow.Panel2.Controls.Add(this.grp_properties);
            this.mainWindow.Size = new System.Drawing.Size(897, 425);
            this.mainWindow.SplitterDistance = 537;
            this.mainWindow.TabIndex = 0;
            this.mainWindow.TabStop = false;
            // 
            // dbpnl_renderScreen
            // 
            this.dbpnl_renderScreen.AutoSize = true;
            this.dbpnl_renderScreen.Controls.Add(this.toolStrip1);
            this.dbpnl_renderScreen.Location = new System.Drawing.Point(0, 0);
            this.dbpnl_renderScreen.Name = "dbpnl_renderScreen";
            this.dbpnl_renderScreen.Size = new System.Drawing.Size(537, 425);
            this.dbpnl_renderScreen.TabIndex = 0;
            this.dbpnl_renderScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.dbpnl_renderScreen_Paint);
            this.dbpnl_renderScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseDown);
            this.dbpnl_renderScreen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseMove);
            this.dbpnl_renderScreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dbpnl_renderScreen_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_file,
            this.tsi_settings,
            this.tss_1,
            this.tsl_mode,
            this.tsc_mode});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(537, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsi_file
            // 
            this.tsi_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_new,
            this.tsi_save,
            this.tsi_load,
            this.tsi_export});
            this.tsi_file.Name = "tsi_file";
            this.tsi_file.Size = new System.Drawing.Size(37, 25);
            this.tsi_file.Text = "File";
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
            // tsi_settings
            // 
            this.tsi_settings.Name = "tsi_settings";
            this.tsi_settings.Size = new System.Drawing.Size(61, 25);
            this.tsi_settings.Text = "Settings";
            this.tsi_settings.Click += new System.EventHandler(this.tsi_settings_Click);
            // 
            // tss_1
            // 
            this.tss_1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.tss_1.Name = "tss_1";
            this.tss_1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsl_mode
            // 
            this.tsl_mode.Name = "tsl_mode";
            this.tsl_mode.Size = new System.Drawing.Size(41, 22);
            this.tsl_mode.Text = "Mode:";
            // 
            // tsc_mode
            // 
            this.tsc_mode.Items.AddRange(new object[] {
            "Rigging",
            "Animation"});
            this.tsc_mode.Name = "tsc_mode";
            this.tsc_mode.Size = new System.Drawing.Size(80, 25);
            this.tsc_mode.Text = "Rigging";
            this.tsc_mode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tsc_mode_KeyPress);
            // 
            // trv_limbBrowser
            // 
            this.trv_limbBrowser.Indent = 6;
            this.trv_limbBrowser.Location = new System.Drawing.Point(186, 3);
            this.trv_limbBrowser.Name = "trv_limbBrowser";
            this.trv_limbBrowser.Size = new System.Drawing.Size(167, 418);
            this.trv_limbBrowser.TabIndex = 1;
            this.trv_limbBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trv_limbBrowser_AfterSelect);
            // 
            // grp_properties
            // 
            this.grp_properties.AutoSize = true;
            this.grp_properties.Controls.Add(this.cdd_limbColor);
            this.grp_properties.Controls.Add(this.chk_yFlip);
            this.grp_properties.Controls.Add(this.chk_xFlip);
            this.grp_properties.Controls.Add(this.nib_imageAngle);
            this.grp_properties.Controls.Add(this.lbl_imageAngle);
            this.grp_properties.Controls.Add(this.nib_offsetY);
            this.grp_properties.Controls.Add(this.nib_offsetX);
            this.grp_properties.Controls.Add(this.lbl_offsetY);
            this.grp_properties.Controls.Add(this.lbl_offsetX);
            this.grp_properties.Controls.Add(this.btn_remove);
            this.grp_properties.Controls.Add(this.btn_add);
            this.grp_properties.Controls.Add(this.img_texture);
            this.grp_properties.Controls.Add(this.lbl_image);
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
            this.grp_properties.Size = new System.Drawing.Size(177, 418);
            this.grp_properties.TabIndex = 0;
            this.grp_properties.TabStop = false;
            this.grp_properties.Text = "Properties";
            // 
            // cdd_limbColor
            // 
            this.cdd_limbColor.AllowCustomColor = true;
            this.cdd_limbColor.AllowTransparent = true;
            this.cdd_limbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cdd_limbColor.ForeColor = System.Drawing.Color.Black;
            this.cdd_limbColor.FormattingEnabled = true;
            this.cdd_limbColor.Location = new System.Drawing.Point(67, 148);
            this.cdd_limbColor.Name = "cdd_limbColor";
            this.cdd_limbColor.Size = new System.Drawing.Size(104, 21);
            this.cdd_limbColor.TabIndex = 22;
            this.cdd_limbColor.SelectedValueChanged += new System.EventHandler(this.cdd_limbColor_SelectedValueChanged);
            // 
            // chk_yFlip
            // 
            this.chk_yFlip.AutoSize = true;
            this.chk_yFlip.Location = new System.Drawing.Point(119, 275);
            this.chk_yFlip.Name = "chk_yFlip";
            this.chk_yFlip.Size = new System.Drawing.Size(52, 17);
            this.chk_yFlip.TabIndex = 8;
            this.chk_yFlip.Text = "Y Flip";
            this.chk_yFlip.UseVisualStyleBackColor = true;
            this.chk_yFlip.CheckedChanged += new System.EventHandler(this.chk_yFlip_CheckedChanged);
            // 
            // chk_xFlip
            // 
            this.chk_xFlip.AutoSize = true;
            this.chk_xFlip.Location = new System.Drawing.Point(58, 275);
            this.chk_xFlip.Name = "chk_xFlip";
            this.chk_xFlip.Size = new System.Drawing.Size(52, 17);
            this.chk_xFlip.TabIndex = 7;
            this.chk_xFlip.Text = "X Flip";
            this.chk_xFlip.UseVisualStyleBackColor = true;
            this.chk_xFlip.CheckedChanged += new System.EventHandler(this.chk_xFlip_CheckedChanged);
            // 
            // nib_imageAngle
            // 
            this.nib_imageAngle.Integer = true;
            this.nib_imageAngle.Location = new System.Drawing.Point(67, 249);
            this.nib_imageAngle.Name = "nib_imageAngle";
            this.nib_imageAngle.Size = new System.Drawing.Size(104, 20);
            this.nib_imageAngle.TabIndex = 6;
            this.nib_imageAngle.Text = "0";
            this.nib_imageAngle.Value = 0F;
            this.nib_imageAngle.TextChanged += new System.EventHandler(this.nib_imageAngle_TextChanged);
            // 
            // lbl_imageAngle
            // 
            this.lbl_imageAngle.AutoSize = true;
            this.lbl_imageAngle.Location = new System.Drawing.Point(24, 252);
            this.lbl_imageAngle.Name = "lbl_imageAngle";
            this.lbl_imageAngle.Size = new System.Drawing.Size(37, 13);
            this.lbl_imageAngle.TabIndex = 21;
            this.lbl_imageAngle.Text = "Angle:";
            // 
            // nib_offsetY
            // 
            this.nib_offsetY.Integer = false;
            this.nib_offsetY.Location = new System.Drawing.Point(64, 328);
            this.nib_offsetY.Name = "nib_offsetY";
            this.nib_offsetY.Size = new System.Drawing.Size(104, 20);
            this.nib_offsetY.TabIndex = 10;
            this.nib_offsetY.Text = "0";
            this.nib_offsetY.Value = 0F;
            this.nib_offsetY.TextChanged += new System.EventHandler(this.nib_offsetY_TextChanged);
            // 
            // nib_offsetX
            // 
            this.nib_offsetX.Integer = false;
            this.nib_offsetX.Location = new System.Drawing.Point(64, 302);
            this.nib_offsetX.Name = "nib_offsetX";
            this.nib_offsetX.Size = new System.Drawing.Size(104, 20);
            this.nib_offsetX.TabIndex = 9;
            this.nib_offsetX.Text = "0";
            this.nib_offsetX.Value = 0F;
            this.nib_offsetX.TextChanged += new System.EventHandler(this.nib_offsetX_TextChanged);
            // 
            // lbl_offsetY
            // 
            this.lbl_offsetY.AutoSize = true;
            this.lbl_offsetY.Location = new System.Drawing.Point(10, 331);
            this.lbl_offsetY.Name = "lbl_offsetY";
            this.lbl_offsetY.Size = new System.Drawing.Size(48, 13);
            this.lbl_offsetY.TabIndex = 18;
            this.lbl_offsetY.Text = "Offset Y:";
            // 
            // lbl_offsetX
            // 
            this.lbl_offsetX.AutoSize = true;
            this.lbl_offsetX.Location = new System.Drawing.Point(10, 305);
            this.lbl_offsetX.Name = "lbl_offsetX";
            this.lbl_offsetX.Size = new System.Drawing.Size(48, 13);
            this.lbl_offsetX.TabIndex = 17;
            this.lbl_offsetX.Text = "Offset X:";
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
            this.btn_remove.TabIndex = 12;
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
            this.btn_add.TabIndex = 11;
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
            this.cmb_type.TabIndex = 4;
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
            // nib_length
            // 
            this.nib_length.Integer = false;
            this.nib_length.Location = new System.Drawing.Point(67, 95);
            this.nib_length.Name = "nib_length";
            this.nib_length.Size = new System.Drawing.Size(104, 20);
            this.nib_length.TabIndex = 3;
            this.nib_length.Text = "0";
            this.nib_length.Value = 0F;
            this.nib_length.TextChanged += new System.EventHandler(this.nib_length_TextChanged);
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
            // nib_scale
            // 
            this.nib_scale.Integer = false;
            this.nib_scale.Location = new System.Drawing.Point(67, 69);
            this.nib_scale.Name = "nib_scale";
            this.nib_scale.Size = new System.Drawing.Size(104, 20);
            this.nib_scale.TabIndex = 2;
            this.nib_scale.Text = "0";
            this.nib_scale.Value = 0F;
            this.nib_scale.TextChanged += new System.EventHandler(this.nib_scale_TextChanged);
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
            // nib_rotation
            // 
            this.nib_rotation.Integer = true;
            this.nib_rotation.Location = new System.Drawing.Point(67, 43);
            this.nib_rotation.Name = "nib_rotation";
            this.nib_rotation.Size = new System.Drawing.Size(104, 20);
            this.nib_rotation.TabIndex = 1;
            this.nib_rotation.Text = "0";
            this.nib_rotation.Value = 0F;
            this.nib_rotation.TextChanged += new System.EventHandler(this.nib_rotation_ValueChanged);
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(67, 17);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(104, 20);
            this.txt_name.TabIndex = 0;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 425);
            this.Controls.Add(this.mainWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Doodle Anims";
            this.mainWindow.Panel1.ResumeLayout(false);
            this.mainWindow.Panel1.PerformLayout();
            this.mainWindow.Panel2.ResumeLayout(false);
            this.mainWindow.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainWindow)).EndInit();
            this.mainWindow.ResumeLayout(false);
            this.dbpnl_renderScreen.ResumeLayout(false);
            this.dbpnl_renderScreen.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grp_properties.ResumeLayout(false);
            this.grp_properties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_texture)).EndInit();
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
        private System.Windows.Forms.Label txt_color;
        private System.Windows.Forms.ColorDialog cdl_color;
        private System.Windows.Forms.OpenFileDialog fdl_imageImport;
        private System.Windows.Forms.PictureBox img_texture;
        private System.Windows.Forms.Label lbl_image;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.SaveFileDialog fdl_saveSkeleton;
        private System.Windows.Forms.OpenFileDialog fdl_loadSkeleton;
        private NumericInputBox nib_offsetY;
        private NumericInputBox nib_offsetX;
        private System.Windows.Forms.Label lbl_offsetY;
        private System.Windows.Forms.Label lbl_offsetX;
        private System.Windows.Forms.CheckBox chk_yFlip;
        private System.Windows.Forms.CheckBox chk_xFlip;
        private NumericInputBox nib_imageAngle;
        private System.Windows.Forms.Label lbl_imageAngle;
        private System.Windows.Forms.TreeView trv_limbBrowser;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsi_file;
        private System.Windows.Forms.ToolStripMenuItem tsi_new;
        private System.Windows.Forms.ToolStripMenuItem tsi_save;
        private System.Windows.Forms.ToolStripMenuItem tsi_saveSkele;
        private System.Windows.Forms.ToolStripMenuItem tsi_saveAnim;
        private System.Windows.Forms.ToolStripMenuItem tsi_load;
        private System.Windows.Forms.ToolStripMenuItem tsi_loadSkele;
        private System.Windows.Forms.ToolStripMenuItem tsi_loadAnim;
        private System.Windows.Forms.ToolStripMenuItem tsi_export;
        private System.Windows.Forms.ToolStripMenuItem tsi_settings;
        private System.Windows.Forms.ToolStripSeparator tss_1;
        private System.Windows.Forms.ToolStripLabel tsl_mode;
        private System.Windows.Forms.ToolStripComboBox tsc_mode;
        private ColorDropDown cdd_limbColor;
    }
}

