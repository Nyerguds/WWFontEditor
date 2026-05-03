namespace WWFontEditor
{
    partial class FrmFontEditor
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
            this.components = new System.ComponentModel.Container();
            this.lblZoom = new System.Windows.Forms.Label();
            this.lblFontMax = new System.Windows.Forms.Label();
            this.lblSymbols = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblValType = new System.Windows.Forms.Label();
            this.lblFontMaxX = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblYOffset = new System.Windows.Forms.Label();
            this.lblPaintColor1 = new System.Windows.Forms.Label();
            this.lblPaintColor2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnShiftLeft = new System.Windows.Forms.Button();
            this.btnShiftDown = new System.Windows.Forms.Button();
            this.btnShiftRight = new System.Windows.Forms.Button();
            this.btnShiftUp = new System.Windows.Forms.Button();
            this.chkPicker = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkPaint = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkOutline = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkShiftWrap = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkGrid = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.cmbEncodings = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numWidth = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numHeight = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numYOffset = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFontAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbPalettes = new System.Windows.Forms.ComboBox();
            this.btnSavePalette = new System.Windows.Forms.Button();
            this.numFontHeight = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numFontWidth = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numSymbols = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.dgrvSymbolsList = new WWFontEditor.UI.Tools.DataGridViewScrollSupport();
            this.palColorSelector = new Nyerguds.Util.UI.PalettePanel();
            this.numZoom = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.pnlImageScroll = new Nyerguds.Util.UI.SelectablePanel();
            this.pxbEditGridFront = new RedCell.UI.Controls.PixelBox();
            this.pxbImage = new RedCell.UI.Controls.PixelBox();
            this.pxbEditGridBehind = new RedCell.UI.Controls.PixelBox();
            this.pxbFullSize = new RedCell.UI.Controls.PixelBox();
            this.btnResetPalette = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYOffset)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSymbols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvSymbolsList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoom)).BeginInit();
            this.pnlImageScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridFront)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridBehind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbFullSize)).BeginInit();
            this.SuspendLayout();
            // 
            // lblZoom
            // 
            this.lblZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZoom.AutoSize = true;
            this.lblZoom.Location = new System.Drawing.Point(413, 444);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(37, 13);
            this.lblZoom.TabIndex = 23;
            this.lblZoom.Text = "Zoom:";
            this.lblZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFontMax
            // 
            this.lblFontMax.Location = new System.Drawing.Point(12, 77);
            this.lblFontMax.Name = "lblFontMax";
            this.lblFontMax.Size = new System.Drawing.Size(62, 20);
            this.lblFontMax.TabIndex = 27;
            this.lblFontMax.Text = "Max size:";
            this.lblFontMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSymbols
            // 
            this.lblSymbols.Location = new System.Drawing.Point(12, 51);
            this.lblSymbols.Name = "lblSymbols";
            this.lblSymbols.Size = new System.Drawing.Size(62, 20);
            this.lblSymbols.TabIndex = 27;
            this.lblSymbols.Text = "Symbols:";
            this.lblSymbols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblType
            // 
            this.lblType.Location = new System.Drawing.Point(12, 31);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(62, 20);
            this.lblType.TabIndex = 29;
            this.lblType.Text = "Type:";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValType
            // 
            this.lblValType.AutoSize = true;
            this.lblValType.Location = new System.Drawing.Point(80, 35);
            this.lblValType.Name = "lblValType";
            this.lblValType.Size = new System.Drawing.Size(10, 13);
            this.lblValType.TabIndex = 30;
            this.lblValType.Text = "-";
            // 
            // lblFontMaxX
            // 
            this.lblFontMaxX.AutoSize = true;
            this.lblFontMaxX.Location = new System.Drawing.Point(123, 81);
            this.lblFontMaxX.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblFontMaxX.Name = "lblFontMaxX";
            this.lblFontMaxX.Size = new System.Drawing.Size(12, 13);
            this.lblFontMaxX.TabIndex = 26;
            this.lblFontMaxX.Text = "x";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = "Width:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 20);
            this.label5.TabIndex = 27;
            this.label5.Text = "Height:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblYOffset
            // 
            this.lblYOffset.Location = new System.Drawing.Point(6, 68);
            this.lblYOffset.Name = "lblYOffset";
            this.lblYOffset.Size = new System.Drawing.Size(50, 20);
            this.lblYOffset.TabIndex = 27;
            this.lblYOffset.Text = "Y-offset:";
            this.lblYOffset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaintColor1
            // 
            this.lblPaintColor1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPaintColor1.BackColor = System.Drawing.Color.Black;
            this.lblPaintColor1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPaintColor1.Location = new System.Drawing.Point(620, 407);
            this.lblPaintColor1.Name = "lblPaintColor1";
            this.lblPaintColor1.Size = new System.Drawing.Size(20, 20);
            this.lblPaintColor1.TabIndex = 119;
            // 
            // lblPaintColor2
            // 
            this.lblPaintColor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPaintColor2.BackColor = System.Drawing.Color.Black;
            this.lblPaintColor2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPaintColor2.Location = new System.Drawing.Point(630, 418);
            this.lblPaintColor2.Name = "lblPaintColor2";
            this.lblPaintColor2.Size = new System.Drawing.Size(20, 20);
            this.lblPaintColor2.TabIndex = 122;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 32000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // btnPaste
            // 
            this.btnPaste.Enabled = false;
            this.btnPaste.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaste.Image = global::WWFontEditor.Properties.Resources.icon_paste;
            this.btnPaste.Location = new System.Drawing.Point(130, 137);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(26, 26);
            this.btnPaste.TabIndex = 81;
            this.toolTip1.SetToolTip(this.btnPaste, "Paste symbol from clipboard");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.BtnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Image = global::WWFontEditor.Properties.Resources.icon_copy;
            this.btnCopy.Location = new System.Drawing.Point(130, 105);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(26, 26);
            this.btnCopy.TabIndex = 80;
            this.toolTip1.SetToolTip(this.btnCopy, "Copy symbol to clipboard");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // btnShiftLeft
            // 
            this.btnShiftLeft.Enabled = false;
            this.btnShiftLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftLeft.Location = new System.Drawing.Point(24, 121);
            this.btnShiftLeft.Name = "btnShiftLeft";
            this.btnShiftLeft.Size = new System.Drawing.Size(26, 26);
            this.btnShiftLeft.TabIndex = 71;
            this.btnShiftLeft.Text = "⇐";
            this.toolTip1.SetToolTip(this.btnShiftLeft, "Shift left");
            this.btnShiftLeft.UseVisualStyleBackColor = true;
            this.btnShiftLeft.Click += new System.EventHandler(this.BtnShiftLeft_Click);
            // 
            // btnShiftDown
            // 
            this.btnShiftDown.Enabled = false;
            this.btnShiftDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftDown.Location = new System.Drawing.Point(49, 146);
            this.btnShiftDown.Name = "btnShiftDown";
            this.btnShiftDown.Size = new System.Drawing.Size(26, 26);
            this.btnShiftDown.TabIndex = 72;
            this.btnShiftDown.Text = "⇓";
            this.toolTip1.SetToolTip(this.btnShiftDown, "Shift down");
            this.btnShiftDown.UseVisualStyleBackColor = true;
            this.btnShiftDown.Click += new System.EventHandler(this.BtnShiftDown_Click);
            // 
            // btnShiftRight
            // 
            this.btnShiftRight.Enabled = false;
            this.btnShiftRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftRight.Location = new System.Drawing.Point(74, 121);
            this.btnShiftRight.Name = "btnShiftRight";
            this.btnShiftRight.Size = new System.Drawing.Size(26, 26);
            this.btnShiftRight.TabIndex = 73;
            this.btnShiftRight.Text = "⇒";
            this.toolTip1.SetToolTip(this.btnShiftRight, "Shift right");
            this.btnShiftRight.UseVisualStyleBackColor = true;
            this.btnShiftRight.Click += new System.EventHandler(this.BtnShiftRight_Click);
            // 
            // btnShiftUp
            // 
            this.btnShiftUp.Enabled = false;
            this.btnShiftUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftUp.Location = new System.Drawing.Point(49, 96);
            this.btnShiftUp.Name = "btnShiftUp";
            this.btnShiftUp.Size = new System.Drawing.Size(26, 26);
            this.btnShiftUp.TabIndex = 70;
            this.btnShiftUp.Text = "⇑";
            this.toolTip1.SetToolTip(this.btnShiftUp, "Shift up");
            this.btnShiftUp.UseVisualStyleBackColor = true;
            this.btnShiftUp.Click += new System.EventHandler(this.BtnShiftUp_Click);
            // 
            // chkPicker
            // 
            this.chkPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPicker.Image = global::WWFontEditor.Properties.Resources.icon_colpicker;
            this.chkPicker.Location = new System.Drawing.Point(592, 440);
            this.chkPicker.Name = "chkPicker";
            this.chkPicker.Size = new System.Drawing.Size(21, 21);
            this.chkPicker.TabIndex = 45;
            this.chkPicker.Toggle = false;
            this.toolTip1.SetToolTip(this.chkPicker, "Color picker");
            this.chkPicker.CheckStateChanged += new System.EventHandler(this.ChkPick_CheckStateChanged);
            // 
            // chkPaint
            // 
            this.chkPaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPaint.Checked = true;
            this.chkPaint.Image = global::WWFontEditor.Properties.Resources.icon_pencil;
            this.chkPaint.Location = new System.Drawing.Point(567, 441);
            this.chkPaint.Name = "chkPaint";
            this.chkPaint.Size = new System.Drawing.Size(21, 21);
            this.chkPaint.TabIndex = 44;
            this.chkPaint.Toggle = false;
            this.toolTip1.SetToolTip(this.chkPaint, "Pencil");
            this.chkPaint.CheckStateChanged += new System.EventHandler(this.ChkPaint_CheckStateChanged);
            // 
            // chkOutline
            // 
            this.chkOutline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOutline.Checked = true;
            this.chkOutline.Image = global::WWFontEditor.Properties.Resources.icon_editarea;
            this.chkOutline.Location = new System.Drawing.Point(539, 441);
            this.chkOutline.Margin = new System.Windows.Forms.Padding(2);
            this.chkOutline.Name = "chkOutline";
            this.chkOutline.Size = new System.Drawing.Size(21, 21);
            this.chkOutline.TabIndex = 43;
            this.toolTip1.SetToolTip(this.chkOutline, "Toggle editable area");
            this.chkOutline.CheckStateChanged += new System.EventHandler(this.CheckboxGridOptionChanged);
            // 
            // chkShiftWrap
            // 
            this.chkShiftWrap.Image = global::WWFontEditor.Properties.Resources.icon_wraparound;
            this.chkShiftWrap.Location = new System.Drawing.Point(51, 122);
            this.chkShiftWrap.Name = "chkShiftWrap";
            this.chkShiftWrap.Size = new System.Drawing.Size(24, 24);
            this.chkShiftWrap.TabIndex = 74;
            this.toolTip1.SetToolTip(this.chkShiftWrap, "Wrap around when shifting");
            // 
            // chkGrid
            // 
            this.chkGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkGrid.Checked = true;
            this.chkGrid.Image = global::WWFontEditor.Properties.Resources.icon_grid;
            this.chkGrid.Location = new System.Drawing.Point(513, 441);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(21, 21);
            this.chkGrid.TabIndex = 42;
            this.toolTip1.SetToolTip(this.chkGrid, "Toggle grid");
            this.chkGrid.CheckStateChanged += new System.EventHandler(this.CheckboxGridOptionChanged);
            // 
            // cmbEncodings
            // 
            this.cmbEncodings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncodings.FormattingEnabled = true;
            this.cmbEncodings.Location = new System.Drawing.Point(12, 440);
            this.cmbEncodings.Name = "cmbEncodings";
            this.cmbEncodings.Size = new System.Drawing.Size(196, 21);
            this.cmbEncodings.TabIndex = 21;
            this.cmbEncodings.SelectedIndexChanged += new System.EventHandler(this.CmbEncodings_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkShiftWrap);
            this.groupBox1.Controls.Add(this.btnPaste);
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.Controls.Add(this.numWidth);
            this.groupBox1.Controls.Add(this.numHeight);
            this.groupBox1.Controls.Add(this.btnShiftLeft);
            this.groupBox1.Controls.Add(this.btnShiftDown);
            this.groupBox1.Controls.Add(this.btnShiftRight);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnShiftUp);
            this.groupBox1.Controls.Add(this.numYOffset);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblYOffset);
            this.groupBox1.Location = new System.Drawing.Point(616, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 176);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Symbol info";
            // 
            // numWidth
            // 
            this.numWidth.Enabled = false;
            this.numWidth.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numWidth.Location = new System.Drawing.Point(62, 18);
            this.numWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(94, 20);
            this.numWidth.TabIndex = 61;
            this.numWidth.ValueChanged += new System.EventHandler(this.NumWidth_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Enabled = false;
            this.numHeight.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numHeight.Location = new System.Drawing.Point(62, 44);
            this.numHeight.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(94, 20);
            this.numHeight.TabIndex = 62;
            this.numHeight.ValueChanged += new System.EventHandler(this.NumHeight_ValueChanged);
            // 
            // numYOffset
            // 
            this.numYOffset.Enabled = false;
            this.numYOffset.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numYOffset.Location = new System.Drawing.Point(62, 70);
            this.numYOffset.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numYOffset.Name = "numYOffset";
            this.numYOffset.Size = new System.Drawing.Size(94, 20);
            this.numYOffset.TabIndex = 63;
            this.numYOffset.ValueChanged += new System.EventHandler(this.NumYOffset_ValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 306;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFontToolStripMenuItem,
            this.saveFontToolStripMenuItem,
            this.saveFontAsToolStripMenuItem,
            this.revertFontToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFontToolStripMenuItem
            // 
            this.openFontToolStripMenuItem.Name = "openFontToolStripMenuItem";
            this.openFontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFontToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.openFontToolStripMenuItem.Text = "Open Font";
            this.openFontToolStripMenuItem.Click += new System.EventHandler(this.OpenFontToolStripMenuItem_Click);
            // 
            // saveFontToolStripMenuItem
            // 
            this.saveFontToolStripMenuItem.Name = "saveFontToolStripMenuItem";
            this.saveFontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveFontToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.saveFontToolStripMenuItem.Text = "Save Font";
            this.saveFontToolStripMenuItem.Click += new System.EventHandler(this.SaveFontToolStripMenuItem_Click);
            // 
            // saveFontAsToolStripMenuItem
            // 
            this.saveFontAsToolStripMenuItem.Name = "saveFontAsToolStripMenuItem";
            this.saveFontAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveFontAsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.saveFontAsToolStripMenuItem.Text = "Save Font As...";
            this.saveFontAsToolStripMenuItem.Click += new System.EventHandler(this.SaveFontAsToolStripMenuItem_Click);
            // 
            // revertFontToolStripMenuItem
            // 
            this.revertFontToolStripMenuItem.Name = "revertFontToolStripMenuItem";
            this.revertFontToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.revertFontToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.revertFontToolStripMenuItem.Text = "Revert Font";
            this.revertFontToolStripMenuItem.Click += new System.EventHandler(this.RevertFontToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.revertToolStripMenuItem,
            this.editorSettingsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyToolStripMenuItem.Text = "Copy symbol";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.BtnCopy_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.pasteToolStripMenuItem.Text = "Paste symbol";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.BtnPaste_Click);
            // 
            // revertToolStripMenuItem
            // 
            this.revertToolStripMenuItem.Enabled = false;
            this.revertToolStripMenuItem.Name = "revertToolStripMenuItem";
            this.revertToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.revertToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.revertToolStripMenuItem.Text = "Revert symbol";
            this.revertToolStripMenuItem.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // editorSettingsToolStripMenuItem
            // 
            this.editorSettingsToolStripMenuItem.Name = "editorSettingsToolStripMenuItem";
            this.editorSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.editorSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.editorSettingsToolStripMenuItem.Text = "Editor settings...";
            this.editorSettingsToolStripMenuItem.Click += new System.EventHandler(this.editorSettingsToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // cmbPalettes
            // 
            this.cmbPalettes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPalettes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPalettes.FormattingEnabled = true;
            this.cmbPalettes.Location = new System.Drawing.Point(616, 212);
            this.cmbPalettes.Name = "cmbPalettes";
            this.cmbPalettes.Size = new System.Drawing.Size(162, 21);
            this.cmbPalettes.TabIndex = 90;
            this.cmbPalettes.SelectedIndexChanged += new System.EventHandler(this.CmbPalettes_SelectedIndexChanged);
            // 
            // btnSavePalette
            // 
            this.btnSavePalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePalette.Location = new System.Drawing.Point(726, 411);
            this.btnSavePalette.Name = "btnSavePalette";
            this.btnSavePalette.Size = new System.Drawing.Size(49, 23);
            this.btnSavePalette.TabIndex = 312;
            this.btnSavePalette.Text = "Save";
            this.btnSavePalette.UseVisualStyleBackColor = true;
            this.btnSavePalette.Click += new System.EventHandler(this.BtnSavePalette_Click);
            // 
            // numFontHeight
            // 
            this.numFontHeight.Enabled = false;
            this.numFontHeight.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numFontHeight.Location = new System.Drawing.Point(141, 79);
            this.numFontHeight.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numFontHeight.MouseWheelIncrement = 0;
            this.numFontHeight.Name = "numFontHeight";
            this.numFontHeight.Size = new System.Drawing.Size(40, 20);
            this.numFontHeight.TabIndex = 12;
            this.numFontHeight.ValueChanged += new System.EventHandler(this.NumFontHeight_ValueChanged);
            // 
            // numFontWidth
            // 
            this.numFontWidth.Enabled = false;
            this.numFontWidth.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numFontWidth.Location = new System.Drawing.Point(80, 79);
            this.numFontWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numFontWidth.MouseWheelIncrement = 0;
            this.numFontWidth.Name = "numFontWidth";
            this.numFontWidth.Size = new System.Drawing.Size(40, 20);
            this.numFontWidth.TabIndex = 11;
            this.numFontWidth.ValueChanged += new System.EventHandler(this.NumFontWidth_ValueChanged);
            // 
            // numSymbols
            // 
            this.numSymbols.Enabled = false;
            this.numSymbols.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numSymbols.Location = new System.Drawing.Point(80, 53);
            this.numSymbols.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numSymbols.MouseWheelIncrement = 0;
            this.numSymbols.Name = "numSymbols";
            this.numSymbols.Size = new System.Drawing.Size(128, 20);
            this.numSymbols.TabIndex = 10;
            this.numSymbols.ValueChanged += new System.EventHandler(this.NumSymbols_ValueChanged);
            // 
            // dgrvSymbolsList
            // 
            this.dgrvSymbolsList.AllowUserToAddRows = false;
            this.dgrvSymbolsList.AllowUserToDeleteRows = false;
            this.dgrvSymbolsList.AllowUserToResizeColumns = false;
            this.dgrvSymbolsList.AllowUserToResizeRows = false;
            this.dgrvSymbolsList.AlwaysShowVerticalScrollbar = true;
            this.dgrvSymbolsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgrvSymbolsList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgrvSymbolsList.BackgroundColor = System.Drawing.Color.White;
            this.dgrvSymbolsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrvSymbolsList.Location = new System.Drawing.Point(13, 103);
            this.dgrvSymbolsList.MultiSelect = false;
            this.dgrvSymbolsList.Name = "dgrvSymbolsList";
            this.dgrvSymbolsList.ReadOnly = true;
            this.dgrvSymbolsList.RowHeadersVisible = false;
            this.dgrvSymbolsList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgrvSymbolsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrvSymbolsList.Size = new System.Drawing.Size(195, 331);
            this.dgrvSymbolsList.StandardTab = true;
            this.dgrvSymbolsList.TabIndex = 20;
            this.dgrvSymbolsList.VerticalScrollbarOffset = 0;
            this.dgrvSymbolsList.SelectionChanged += new System.EventHandler(this.DgrvSymbolsList_SelectionChanged);
            // 
            // palColorSelector
            // 
            this.palColorSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.palColorSelector.AutoSize = true;
            this.palColorSelector.ColorSelectMode = Nyerguds.Util.UI.ColorSelMode.None;
            this.palColorSelector.ColorTableWidth = 4;
            this.palColorSelector.LabelSize = new System.Drawing.Size(36, 36);
            this.palColorSelector.Location = new System.Drawing.Point(616, 240);
            this.palColorSelector.MaxColors = 16;
            this.palColorSelector.Name = "palColorSelector";
            this.palColorSelector.PadBetween = new System.Drawing.Point(5, 5);
            this.palColorSelector.Padding = new System.Windows.Forms.Padding(0);
            this.palColorSelector.Palette = null;
            this.palColorSelector.Remap = null;
            this.palColorSelector.SelectedIndices = new int[0];
            this.palColorSelector.ShowRemappedPalette = true;
            this.palColorSelector.Size = new System.Drawing.Size(159, 159);
            this.palColorSelector.TabIndex = 91;
            this.palColorSelector.TabStop = false;
            this.palColorSelector.TransItemBackColor = System.Drawing.Color.Empty;
            this.palColorSelector.ColorLabelMouseDoubleClick += new Nyerguds.Util.UI.PaletteClickEventHandler(this.PalColorSelector_ColorLabelMouseDoubleClick);
            this.palColorSelector.ColorLabelMouseClick += new Nyerguds.Util.UI.PaletteClickEventHandler(this.palColorSelector_ColorLabelMouseClick);
            // 
            // numZoom
            // 
            this.numZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numZoom.EnteredValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoom.Location = new System.Drawing.Point(456, 441);
            this.numZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoom.Name = "numZoom";
            this.numZoom.Size = new System.Drawing.Size(51, 20);
            this.numZoom.TabIndex = 41;
            this.numZoom.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numZoom.ValueChanged += new System.EventHandler(this.NumZoom_ValueChanged);
            // 
            // pnlImageScroll
            // 
            this.pnlImageScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImageScroll.AutoScroll = true;
            this.pnlImageScroll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlImageScroll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlImageScroll.Controls.Add(this.pxbEditGridFront);
            this.pnlImageScroll.Controls.Add(this.pxbImage);
            this.pnlImageScroll.Controls.Add(this.pxbEditGridBehind);
            this.pnlImageScroll.Controls.Add(this.pxbFullSize);
            this.pnlImageScroll.Location = new System.Drawing.Point(213, 34);
            this.pnlImageScroll.Margin = new System.Windows.Forms.Padding(0);
            this.pnlImageScroll.Name = "pnlImageScroll";
            this.pnlImageScroll.Padding = new System.Windows.Forms.Padding(3);
            this.pnlImageScroll.Size = new System.Drawing.Size(400, 400);
            this.pnlImageScroll.TabIndex = 40;
            this.pnlImageScroll.TabStop = true;
            // 
            // pxbEditGridFront
            // 
            this.pxbEditGridFront.BackColor = System.Drawing.Color.Transparent;
            this.pxbEditGridFront.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbEditGridFront.Location = new System.Drawing.Point(3, 3);
            this.pxbEditGridFront.Margin = new System.Windows.Forms.Padding(0);
            this.pxbEditGridFront.Name = "pxbEditGridFront";
            this.pxbEditGridFront.Size = new System.Drawing.Size(50, 50);
            this.pxbEditGridFront.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pxbEditGridFront.TabIndex = 2;
            this.pxbEditGridFront.TabStop = false;
            this.pxbEditGridFront.Visible = false;
            this.pxbEditGridFront.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pxbEditGridFront_MouseDown);
            this.pxbEditGridFront.MouseLeave += new System.EventHandler(this.pxbEditGridFront_MouseLeave);
            this.pxbEditGridFront.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pxbEditGridFront_MouseMove);
            this.pxbEditGridFront.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pxbEditGridFront_MouseUp);
            // 
            // pxbImage
            // 
            this.pxbImage.BackColor = System.Drawing.Color.Transparent;
            this.pxbImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbImage.Location = new System.Drawing.Point(3, 3);
            this.pxbImage.Margin = new System.Windows.Forms.Padding(0);
            this.pxbImage.Name = "pxbImage";
            this.pxbImage.Size = new System.Drawing.Size(100, 100);
            this.pxbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pxbImage.TabIndex = 0;
            this.pxbImage.TabStop = false;
            this.pxbImage.Visible = false;
            this.pxbImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox_Click);
            // 
            // pxbEditGridBehind
            // 
            this.pxbEditGridBehind.BackColor = System.Drawing.Color.Transparent;
            this.pxbEditGridBehind.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbEditGridBehind.Location = new System.Drawing.Point(3, 3);
            this.pxbEditGridBehind.Margin = new System.Windows.Forms.Padding(0);
            this.pxbEditGridBehind.Name = "pxbEditGridBehind";
            this.pxbEditGridBehind.Size = new System.Drawing.Size(150, 150);
            this.pxbEditGridBehind.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pxbEditGridBehind.TabIndex = 2;
            this.pxbEditGridBehind.TabStop = false;
            this.pxbEditGridBehind.Visible = false;
            this.pxbEditGridBehind.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox_Click);
            // 
            // pxbFullSize
            // 
            this.pxbFullSize.BackColor = System.Drawing.Color.Transparent;
            this.pxbFullSize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbFullSize.Location = new System.Drawing.Point(3, 3);
            this.pxbFullSize.Margin = new System.Windows.Forms.Padding(0);
            this.pxbFullSize.Name = "pxbFullSize";
            this.pxbFullSize.Size = new System.Drawing.Size(200, 200);
            this.pxbFullSize.TabIndex = 1;
            this.pxbFullSize.TabStop = false;
            this.pxbFullSize.Visible = false;
            this.pxbFullSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox_Click);
            // 
            // btnResetPalette
            // 
            this.btnResetPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetPalette.Enabled = false;
            this.btnResetPalette.Location = new System.Drawing.Point(671, 411);
            this.btnResetPalette.Name = "btnResetPalette";
            this.btnResetPalette.Size = new System.Drawing.Size(49, 23);
            this.btnResetPalette.TabIndex = 312;
            this.btnResetPalette.Text = "Revert";
            this.btnResetPalette.UseVisualStyleBackColor = true;
            this.btnResetPalette.Click += new System.EventHandler(this.BtnResetPalette_Click);
            // 
            // FrmFontEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 469);
            this.Controls.Add(this.btnResetPalette);
            this.Controls.Add(this.btnSavePalette);
            this.Controls.Add(this.cmbPalettes);
            this.Controls.Add(this.chkPicker);
            this.Controls.Add(this.chkPaint);
            this.Controls.Add(this.numFontHeight);
            this.Controls.Add(this.numFontWidth);
            this.Controls.Add(this.numSymbols);
            this.Controls.Add(this.chkOutline);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkGrid);
            this.Controls.Add(this.cmbEncodings);
            this.Controls.Add(this.dgrvSymbolsList);
            this.Controls.Add(this.lblPaintColor1);
            this.Controls.Add(this.lblPaintColor2);
            this.Controls.Add(this.palColorSelector);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblSymbols);
            this.Controls.Add(this.lblFontMax);
            this.Controls.Add(this.lblFontMaxX);
            this.Controls.Add(this.lblZoom);
            this.Controls.Add(this.numZoom);
            this.Controls.Add(this.pnlImageScroll);
            this.Controls.Add(this.lblValType);
            this.Controls.Add(this.menuStrip1);
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(725, 457);
            this.Name = "FrmFontEditor";
            this.Text = "Westwood Font Editor v#.#.# - Created by Nyerguds";
            this.Shown += new System.EventHandler(this.FrmFontEditor_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Frm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Frm_DragEnter);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYOffset)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSymbols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvSymbolsList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoom)).EndInit();
            this.pnlImageScroll.ResumeLayout(false);
            this.pnlImageScroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridFront)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridBehind)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbFullSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Nyerguds.Util.UI.SelectablePanel pnlImageScroll;
        private Nyerguds.Util.UI.EnhNumericUpDown numZoom;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.Label lblFontMax;
        private System.Windows.Forms.Label lblSymbols;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblValType;
        private System.Windows.Forms.Label lblFontMaxX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblYOffset;
        private RedCell.UI.Controls.PixelBox pxbFullSize;
        private RedCell.UI.Controls.PixelBox pxbEditGridBehind;
        private RedCell.UI.Controls.PixelBox pxbImage;
        private RedCell.UI.Controls.PixelBox pxbEditGridFront;
        private System.Windows.Forms.Label lblPaintColor1;
        private Nyerguds.Util.UI.PalettePanel palColorSelector;
        private System.Windows.Forms.Label lblPaintColor2;
        private System.Windows.Forms.ToolTip toolTip1;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkGrid;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkOutline;
        private Nyerguds.Util.UI.EnhNumericUpDown numYOffset;
        private WWFontEditor.UI.Tools.DataGridViewScrollSupport dgrvSymbolsList;
        private System.Windows.Forms.ComboBox cmbEncodings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFontAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnShiftDown;
        private System.Windows.Forms.Button btnShiftRight;
        private System.Windows.Forms.Button btnShiftUp;
        private System.Windows.Forms.Button btnShiftLeft;
        private Nyerguds.Util.UI.EnhNumericUpDown numWidth;
        private Nyerguds.Util.UI.EnhNumericUpDown numHeight;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private Nyerguds.Util.UI.EnhNumericUpDown numSymbols;
        private Nyerguds.Util.UI.EnhNumericUpDown numFontWidth;
        private Nyerguds.Util.UI.EnhNumericUpDown numFontHeight;
        private System.Windows.Forms.ToolStripMenuItem revertFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkPaint;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkPicker;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkShiftWrap;
        private System.Windows.Forms.ComboBox cmbPalettes;
        private System.Windows.Forms.Button btnSavePalette;
        private System.Windows.Forms.Button btnResetPalette;
        private System.Windows.Forms.ToolStripMenuItem editorSettingsToolStripMenuItem;
    }
}

