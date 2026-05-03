namespace WWFontEditor.UI
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
            this.lblFontMaxX = new System.Windows.Forms.Label();
            this.lblSymbolWidth = new System.Windows.Forms.Label();
            this.lblSymbolHeight = new System.Windows.Forms.Label();
            this.lblSymbolYOffset = new System.Windows.Forms.Label();
            this.lblPaintColor1 = new System.Windows.Forms.Label();
            this.lblPaintColor2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnShiftLeft = new System.Windows.Forms.Button();
            this.btnShiftDown = new System.Windows.Forms.Button();
            this.btnShiftRight = new System.Windows.Forms.Button();
            this.btnShiftUp = new System.Windows.Forms.Button();
            this.btnRemap = new System.Windows.Forms.Button();
            this.chkPicker = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkPaint = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkOutline = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.chkShiftWrap = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.numYOffset = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.chkGrid = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numWidth = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numHeight = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNewFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveFontAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRevertFont = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopySymbol = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasteSymbol = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasteSymbolTrans = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRevertSymbol = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOptimizeWidths = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiManagePalettes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSavePalette = new System.Windows.Forms.Button();
            this.btnResetPalette = new System.Windows.Forms.Button();
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.btnValType = new System.Windows.Forms.Button();
            this.chkWrapPreview = new System.Windows.Forms.CheckBox();
            this.lblZoomPreview = new System.Windows.Forms.Label();
            this.pnlImagePreview = new Nyerguds.Util.UI.SelectablePanel();
            this.pxbPreview = new Nyerguds.Util.UI.PixelBox();
            this.cmbPalettes = new Nyerguds.Util.UI.ComboBoxSmartWidth();
            this.numFontHeight = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numFontWidth = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numSymbols = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.cmbEncodings = new Nyerguds.Util.UI.ComboBoxSmartWidth();
            this.dgrvSymbolsList = new Nyerguds.Util.UI.DataGridViewScrollSupport();
            this.palColorSelector = new Nyerguds.Util.UI.PalettePanel();
            this.numZoomPreview = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numZoom = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.pnlImageScroll = new Nyerguds.Util.UI.SelectablePanel();
            this.pxbEditGridFront = new Nyerguds.Util.UI.PixelBox();
            this.pxbImage = new Nyerguds.Util.UI.PixelBox();
            this.pxbEditGridBehind = new Nyerguds.Util.UI.PixelBox();
            this.pxbFullSize = new Nyerguds.Util.UI.PixelBox();
            ((System.ComponentModel.ISupportInitialize)(this.numYOffset)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.pnlImagePreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pxbPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSymbols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvSymbolsList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoomPreview)).BeginInit();
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
            this.lblZoom.TabIndex = 41;
            this.lblZoom.Text = "Zoom:";
            this.lblZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFontMax
            // 
            this.lblFontMax.Location = new System.Drawing.Point(12, 87);
            this.lblFontMax.Name = "lblFontMax";
            this.lblFontMax.Size = new System.Drawing.Size(62, 20);
            this.lblFontMax.TabIndex = 4;
            this.lblFontMax.Text = "Max size:";
            this.lblFontMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSymbols
            // 
            this.lblSymbols.Location = new System.Drawing.Point(12, 61);
            this.lblSymbols.Name = "lblSymbols";
            this.lblSymbols.Size = new System.Drawing.Size(62, 20);
            this.lblSymbols.TabIndex = 2;
            this.lblSymbols.Text = "Symbols:";
            this.lblSymbols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblType
            // 
            this.lblType.Location = new System.Drawing.Point(12, 35);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(62, 20);
            this.lblType.TabIndex = 0;
            this.lblType.Text = "Type:";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFontMaxX
            // 
            this.lblFontMaxX.AutoSize = true;
            this.lblFontMaxX.Location = new System.Drawing.Point(123, 91);
            this.lblFontMaxX.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblFontMaxX.Name = "lblFontMaxX";
            this.lblFontMaxX.Size = new System.Drawing.Size(12, 13);
            this.lblFontMaxX.TabIndex = 6;
            this.lblFontMaxX.Text = "x";
            // 
            // lblSymbolWidth
            // 
            this.lblSymbolWidth.Location = new System.Drawing.Point(6, 16);
            this.lblSymbolWidth.Name = "lblSymbolWidth";
            this.lblSymbolWidth.Size = new System.Drawing.Size(50, 20);
            this.lblSymbolWidth.TabIndex = 61;
            this.lblSymbolWidth.Text = "Width:";
            this.lblSymbolWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSymbolHeight
            // 
            this.lblSymbolHeight.Location = new System.Drawing.Point(6, 42);
            this.lblSymbolHeight.Name = "lblSymbolHeight";
            this.lblSymbolHeight.Size = new System.Drawing.Size(50, 20);
            this.lblSymbolHeight.TabIndex = 63;
            this.lblSymbolHeight.Text = "Height:";
            this.lblSymbolHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSymbolYOffset
            // 
            this.lblSymbolYOffset.Location = new System.Drawing.Point(6, 68);
            this.lblSymbolYOffset.Name = "lblSymbolYOffset";
            this.lblSymbolYOffset.Size = new System.Drawing.Size(50, 20);
            this.lblSymbolYOffset.TabIndex = 65;
            this.lblSymbolYOffset.Text = "Y-offset:";
            this.lblSymbolYOffset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaintColor1
            // 
            this.lblPaintColor1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPaintColor1.BackColor = System.Drawing.Color.Black;
            this.lblPaintColor1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPaintColor1.Location = new System.Drawing.Point(620, 407);
            this.lblPaintColor1.Name = "lblPaintColor1";
            this.lblPaintColor1.Size = new System.Drawing.Size(20, 20);
            this.lblPaintColor1.TabIndex = 100;
            this.lblPaintColor1.DoubleClick += new System.EventHandler(this.LblPaintColor1_DoubleClick);
            this.lblPaintColor1.MouseEnter += new System.EventHandler(this.LblPaintColor1_MouseEnter);
            this.lblPaintColor1.MouseLeave += new System.EventHandler(this.LblPaintColor_MouseLeave);
            // 
            // lblPaintColor2
            // 
            this.lblPaintColor2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPaintColor2.BackColor = System.Drawing.Color.Black;
            this.lblPaintColor2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPaintColor2.Location = new System.Drawing.Point(630, 418);
            this.lblPaintColor2.Name = "lblPaintColor2";
            this.lblPaintColor2.Size = new System.Drawing.Size(20, 20);
            this.lblPaintColor2.TabIndex = 101;
            this.lblPaintColor2.DoubleClick += new System.EventHandler(this.LblPaintColor2_DoubleClick);
            this.lblPaintColor2.MouseEnter += new System.EventHandler(this.LblPaintColor2_MouseEnter);
            this.lblPaintColor2.MouseLeave += new System.EventHandler(this.LblPaintColor_MouseLeave);
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
            this.btnPaste.Location = new System.Drawing.Point(130, 96);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(26, 26);
            this.btnPaste.TabIndex = 81;
            this.toolTip1.SetToolTip(this.btnPaste, "Paste symbol from clipboard");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.TsmiPasteSymbol_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Image = global::WWFontEditor.Properties.Resources.icon_copy;
            this.btnCopy.Location = new System.Drawing.Point(98, 96);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(26, 26);
            this.btnCopy.TabIndex = 80;
            this.toolTip1.SetToolTip(this.btnCopy, "Copy symbol to clipboard");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.TsmiCopySymbol_Click);
            // 
            // btnShiftLeft
            // 
            this.btnShiftLeft.Enabled = false;
            this.btnShiftLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftLeft.Location = new System.Drawing.Point(7, 121);
            this.btnShiftLeft.Name = "btnShiftLeft";
            this.btnShiftLeft.Size = new System.Drawing.Size(26, 26);
            this.btnShiftLeft.TabIndex = 70;
            this.btnShiftLeft.Text = "⇐";
            this.toolTip1.SetToolTip(this.btnShiftLeft, "Shift left (Ctrl+Left)\r\nHold [Shift] to apply to all images");
            this.btnShiftLeft.UseVisualStyleBackColor = true;
            this.btnShiftLeft.Click += new System.EventHandler(this.BtnShiftLeft_Click);
            // 
            // btnShiftDown
            // 
            this.btnShiftDown.Enabled = false;
            this.btnShiftDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftDown.Location = new System.Drawing.Point(32, 146);
            this.btnShiftDown.Name = "btnShiftDown";
            this.btnShiftDown.Size = new System.Drawing.Size(26, 26);
            this.btnShiftDown.TabIndex = 73;
            this.btnShiftDown.Text = "⇓";
            this.toolTip1.SetToolTip(this.btnShiftDown, "Shift down (Ctrl+Down)\r\nHold [Shift] to apply to all images\r\nHold [Alt] to expand" +
        " image with the shift");
            this.btnShiftDown.UseVisualStyleBackColor = true;
            this.btnShiftDown.Click += new System.EventHandler(this.BtnShiftDown_Click);
            // 
            // btnShiftRight
            // 
            this.btnShiftRight.Enabled = false;
            this.btnShiftRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftRight.Location = new System.Drawing.Point(57, 121);
            this.btnShiftRight.Name = "btnShiftRight";
            this.btnShiftRight.Size = new System.Drawing.Size(26, 26);
            this.btnShiftRight.TabIndex = 72;
            this.btnShiftRight.Text = "⇒";
            this.toolTip1.SetToolTip(this.btnShiftRight, "Shift right (Ctrl+Right)\r\nHold [Shift] to apply to all images\r\nHold [Alt] to expa" +
        "nd image with the shift");
            this.btnShiftRight.UseVisualStyleBackColor = true;
            this.btnShiftRight.Click += new System.EventHandler(this.BtnShiftRight_Click);
            // 
            // btnShiftUp
            // 
            this.btnShiftUp.Enabled = false;
            this.btnShiftUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShiftUp.Location = new System.Drawing.Point(32, 96);
            this.btnShiftUp.Name = "btnShiftUp";
            this.btnShiftUp.Size = new System.Drawing.Size(26, 26);
            this.btnShiftUp.TabIndex = 71;
            this.btnShiftUp.Text = "⇑";
            this.toolTip1.SetToolTip(this.btnShiftUp, "Shift up (Ctrl+Up)\r\nHold [Shift] to apply to all images\r\nHold [Alt] to expand ima" +
        "ge with the shift\r\n");
            this.btnShiftUp.UseVisualStyleBackColor = true;
            this.btnShiftUp.Click += new System.EventHandler(this.BtnShiftUp_Click);
            // 
            // btnRemap
            // 
            this.btnRemap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemap.Enabled = false;
            this.btnRemap.Location = new System.Drawing.Point(671, 436);
            this.btnRemap.Name = "btnRemap";
            this.btnRemap.Size = new System.Drawing.Size(104, 23);
            this.btnRemap.TabIndex = 112;
            this.btnRemap.Text = "Color replace...";
            this.toolTip1.SetToolTip(this.btnRemap, "Global color replace");
            this.btnRemap.UseVisualStyleBackColor = true;
            this.btnRemap.Click += new System.EventHandler(this.BtnRemap_Click);
            // 
            // chkPicker
            // 
            this.chkPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPicker.Image = global::WWFontEditor.Properties.Resources.icon_colpicker;
            this.chkPicker.Location = new System.Drawing.Point(592, 440);
            this.chkPicker.Name = "chkPicker";
            this.chkPicker.Size = new System.Drawing.Size(21, 21);
            this.chkPicker.TabIndex = 46;
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
            this.chkPaint.TabIndex = 45;
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
            this.chkOutline.TabIndex = 44;
            this.toolTip1.SetToolTip(this.chkOutline, "Toggle editable area");
            this.chkOutline.CheckStateChanged += new System.EventHandler(this.CheckboxGridOptionChanged);
            // 
            // chkShiftWrap
            // 
            this.chkShiftWrap.Image = global::WWFontEditor.Properties.Resources.icon_wraparound;
            this.chkShiftWrap.Location = new System.Drawing.Point(34, 122);
            this.chkShiftWrap.Name = "chkShiftWrap";
            this.chkShiftWrap.Size = new System.Drawing.Size(24, 24);
            this.chkShiftWrap.TabIndex = 74;
            this.toolTip1.SetToolTip(this.chkShiftWrap, "Wrap around when shifting");
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
            this.numYOffset.SelectedText = "";
            this.numYOffset.SelectionLength = 0;
            this.numYOffset.SelectionStart = 0;
            this.numYOffset.Size = new System.Drawing.Size(94, 20);
            this.numYOffset.TabIndex = 66;
            this.toolTip1.SetToolTip(this.numYOffset, "Change Y-offset (Ctrl+PgUp/PgDown)\r\nHold [Shift] to apply to all images");
            this.numYOffset.ValueChanged += new System.EventHandler(this.NumYOffset_ValueChanged);
            // 
            // chkGrid
            // 
            this.chkGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkGrid.Checked = true;
            this.chkGrid.Image = global::WWFontEditor.Properties.Resources.icon_grid;
            this.chkGrid.Location = new System.Drawing.Point(513, 441);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(21, 21);
            this.chkGrid.TabIndex = 43;
            this.toolTip1.SetToolTip(this.chkGrid, "Toggle grid");
            this.chkGrid.CheckStateChanged += new System.EventHandler(this.CheckboxGridOptionChanged);
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
            this.groupBox1.Controls.Add(this.lblSymbolWidth);
            this.groupBox1.Controls.Add(this.btnShiftUp);
            this.groupBox1.Controls.Add(this.numYOffset);
            this.groupBox1.Controls.Add(this.lblSymbolHeight);
            this.groupBox1.Controls.Add(this.lblSymbolYOffset);
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
            this.numWidth.SelectedText = "";
            this.numWidth.SelectionLength = 0;
            this.numWidth.SelectionStart = 0;
            this.numWidth.Size = new System.Drawing.Size(94, 20);
            this.numWidth.TabIndex = 62;
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
            this.numHeight.SelectedText = "";
            this.numHeight.SelectionLength = 0;
            this.numHeight.SelectionStart = 0;
            this.numHeight.Size = new System.Drawing.Size(94, 20);
            this.numHeight.TabIndex = 64;
            this.numHeight.ValueChanged += new System.EventHandler(this.NumHeight_ValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiInfo});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 306;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNewFont,
            this.tsmiOpenFont,
            this.tsmiSaveFont,
            this.tsmiSaveFontAs,
            this.tsmiRevertFont,
            this.tsmiExit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "File";
            // 
            // tsmiNewFont
            // 
            this.tsmiNewFont.Name = "tsmiNewFont";
            this.tsmiNewFont.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNewFont.Size = new System.Drawing.Size(222, 22);
            this.tsmiNewFont.Text = "&New";
            this.tsmiNewFont.Click += new System.EventHandler(this.TsmiNewFont_Click);
            // 
            // tsmiOpenFont
            // 
            this.tsmiOpenFont.Name = "tsmiOpenFont";
            this.tsmiOpenFont.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpenFont.Size = new System.Drawing.Size(222, 22);
            this.tsmiOpenFont.Text = "&Open Font";
            this.tsmiOpenFont.Click += new System.EventHandler(this.TsmiOpenFont_Click);
            // 
            // tsmiSaveFont
            // 
            this.tsmiSaveFont.Enabled = false;
            this.tsmiSaveFont.Name = "tsmiSaveFont";
            this.tsmiSaveFont.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSaveFont.Size = new System.Drawing.Size(222, 22);
            this.tsmiSaveFont.Text = "&Save Font";
            this.tsmiSaveFont.Click += new System.EventHandler(this.TsmiSaveFont_Click);
            // 
            // tsmiSaveFontAs
            // 
            this.tsmiSaveFontAs.Enabled = false;
            this.tsmiSaveFontAs.Name = "tsmiSaveFontAs";
            this.tsmiSaveFontAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsmiSaveFontAs.Size = new System.Drawing.Size(222, 22);
            this.tsmiSaveFontAs.Text = "Save Font &As...";
            this.tsmiSaveFontAs.Click += new System.EventHandler(this.TsmiSaveFontAs_Click);
            // 
            // tsmiRevertFont
            // 
            this.tsmiRevertFont.Enabled = false;
            this.tsmiRevertFont.Name = "tsmiRevertFont";
            this.tsmiRevertFont.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsmiRevertFont.Size = new System.Drawing.Size(222, 22);
            this.tsmiRevertFont.Text = "&Revert Font";
            this.tsmiRevertFont.Click += new System.EventHandler(this.TsmiRevertFont_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.tsmiExit.Size = new System.Drawing.Size(222, 22);
            this.tsmiExit.Text = "E&xit";
            this.tsmiExit.Click += new System.EventHandler(this.TsmiExit_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopySymbol,
            this.tsmiPasteSymbol,
            this.tsmiPasteSymbolTrans,
            this.tsmiRevertSymbol,
            this.tsmiOptimizeWidths,
            this.tsmiManagePalettes,
            this.tsmiEditorSettings});
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiCopySymbol
            // 
            this.tsmiCopySymbol.Enabled = false;
            this.tsmiCopySymbol.Name = "tsmiCopySymbol";
            this.tsmiCopySymbol.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopySymbol.Size = new System.Drawing.Size(269, 22);
            this.tsmiCopySymbol.Text = "&Copy symbol";
            this.tsmiCopySymbol.Click += new System.EventHandler(this.TsmiCopySymbol_Click);
            // 
            // tsmiPasteSymbol
            // 
            this.tsmiPasteSymbol.Enabled = false;
            this.tsmiPasteSymbol.Name = "tsmiPasteSymbol";
            this.tsmiPasteSymbol.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.tsmiPasteSymbol.Size = new System.Drawing.Size(269, 22);
            this.tsmiPasteSymbol.Text = "&Paste as symbol";
            this.tsmiPasteSymbol.Click += new System.EventHandler(this.TsmiPasteSymbol_Click);
            // 
            // tsmiPasteSymbolTrans
            // 
            this.tsmiPasteSymbolTrans.Enabled = false;
            this.tsmiPasteSymbolTrans.Name = "tsmiPasteSymbolTrans";
            this.tsmiPasteSymbolTrans.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.tsmiPasteSymbolTrans.Size = new System.Drawing.Size(269, 22);
            this.tsmiPasteSymbolTrans.Text = "Paste &on symbol";
            this.tsmiPasteSymbolTrans.Click += new System.EventHandler(this.TsmiPasteSymbolTrans_Click);
            // 
            // tsmiRevertSymbol
            // 
            this.tsmiRevertSymbol.Enabled = false;
            this.tsmiRevertSymbol.Name = "tsmiRevertSymbol";
            this.tsmiRevertSymbol.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.tsmiRevertSymbol.Size = new System.Drawing.Size(269, 22);
            this.tsmiRevertSymbol.Text = "&Revert symbol";
            this.tsmiRevertSymbol.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // tsmiOptimizeWidths
            // 
            this.tsmiOptimizeWidths.Enabled = false;
            this.tsmiOptimizeWidths.Name = "tsmiOptimizeWidths";
            this.tsmiOptimizeWidths.Size = new System.Drawing.Size(269, 22);
            this.tsmiOptimizeWidths.Text = "Optimize widths of all symbols";
            this.tsmiOptimizeWidths.Click += new System.EventHandler(this.tsmiOptimizeWidths_Click);
            // 
            // tsmiManagePalettes
            // 
            this.tsmiManagePalettes.Name = "tsmiManagePalettes";
            this.tsmiManagePalettes.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.tsmiManagePalettes.Size = new System.Drawing.Size(269, 22);
            this.tsmiManagePalettes.Text = "&Manage 16-colour palettes...";
            this.tsmiManagePalettes.Click += new System.EventHandler(this.TsmiManagePalettes_Click);
            // 
            // tsmiEditorSettings
            // 
            this.tsmiEditorSettings.Name = "tsmiEditorSettings";
            this.tsmiEditorSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tsmiEditorSettings.Size = new System.Drawing.Size(269, 22);
            this.tsmiEditorSettings.Text = "Editor &settings...";
            this.tsmiEditorSettings.Click += new System.EventHandler(this.TsmiEditorSettings_Click);
            // 
            // tsmiInfo
            // 
            this.tsmiInfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAbout});
            this.tsmiInfo.Name = "tsmiInfo";
            this.tsmiInfo.Size = new System.Drawing.Size(40, 20);
            this.tsmiInfo.Text = "Info";
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.tsmiAbout.Size = new System.Drawing.Size(153, 22);
            this.tsmiAbout.Text = "&About...";
            this.tsmiAbout.Click += new System.EventHandler(this.TsmiAbout_Click);
            // 
            // btnSavePalette
            // 
            this.btnSavePalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePalette.Location = new System.Drawing.Point(726, 407);
            this.btnSavePalette.Name = "btnSavePalette";
            this.btnSavePalette.Size = new System.Drawing.Size(49, 23);
            this.btnSavePalette.TabIndex = 111;
            this.btnSavePalette.Text = "Save...";
            this.btnSavePalette.UseVisualStyleBackColor = true;
            this.btnSavePalette.Click += new System.EventHandler(this.BtnSavePalette_Click);
            // 
            // btnResetPalette
            // 
            this.btnResetPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetPalette.Enabled = false;
            this.btnResetPalette.Location = new System.Drawing.Point(671, 407);
            this.btnResetPalette.Name = "btnResetPalette";
            this.btnResetPalette.Size = new System.Drawing.Size(49, 23);
            this.btnResetPalette.TabIndex = 110;
            this.btnResetPalette.Text = "Revert";
            this.btnResetPalette.UseVisualStyleBackColor = true;
            this.btnResetPalette.Click += new System.EventHandler(this.BtnResetPalette_Click);
            // 
            // txtPreview
            // 
            this.txtPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPreview.Location = new System.Drawing.Point(12, 467);
            this.txtPreview.Multiline = true;
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPreview.Size = new System.Drawing.Size(381, 106);
            this.txtPreview.TabIndex = 120;
            this.txtPreview.Text = "Hello, world!";
            this.txtPreview.TextChanged += new System.EventHandler(this.txtPreview_TextChanged);
            this.txtPreview.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSelectAll);
            // 
            // btnValType
            // 
            this.btnValType.Enabled = false;
            this.btnValType.Location = new System.Drawing.Point(80, 34);
            this.btnValType.Name = "btnValType";
            this.btnValType.Size = new System.Drawing.Size(128, 23);
            this.btnValType.TabIndex = 1;
            this.btnValType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnValType.UseVisualStyleBackColor = true;
            this.btnValType.Click += new System.EventHandler(this.BtnValType_Click);
            // 
            // chkWrapPreview
            // 
            this.chkWrapPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkWrapPreview.AutoSize = true;
            this.chkWrapPreview.Location = new System.Drawing.Point(15, 580);
            this.chkWrapPreview.Name = "chkWrapPreview";
            this.chkWrapPreview.Size = new System.Drawing.Size(172, 17);
            this.chkWrapPreview.TabIndex = 121;
            this.chkWrapPreview.Text = "Wrap preview text to box width";
            this.chkWrapPreview.UseVisualStyleBackColor = true;
            this.chkWrapPreview.CheckedChanged += new System.EventHandler(this.chkWrapPreview_CheckedChanged);
            // 
            // lblZoomPreview
            // 
            this.lblZoomPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblZoomPreview.AutoSize = true;
            this.lblZoomPreview.Location = new System.Drawing.Point(260, 581);
            this.lblZoomPreview.Name = "lblZoomPreview";
            this.lblZoomPreview.Size = new System.Drawing.Size(76, 13);
            this.lblZoomPreview.TabIndex = 122;
            this.lblZoomPreview.Text = "Preview zoom:";
            this.lblZoomPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlImagePreview
            // 
            this.pnlImagePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImagePreview.AutoScroll = true;
            this.pnlImagePreview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlImagePreview.BackColor = System.Drawing.Color.Silver;
            this.pnlImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlImagePreview.Controls.Add(this.pxbPreview);
            this.pnlImagePreview.Enabled = false;
            this.pnlImagePreview.Location = new System.Drawing.Point(399, 467);
            this.pnlImagePreview.Margin = new System.Windows.Forms.Padding(0);
            this.pnlImagePreview.Name = "pnlImagePreview";
            this.pnlImagePreview.Padding = new System.Windows.Forms.Padding(1);
            this.pnlImagePreview.Size = new System.Drawing.Size(373, 132);
            this.pnlImagePreview.TabIndex = 124;
            this.pnlImagePreview.TabStop = true;
            this.pnlImagePreview.MouseScroll += new System.Windows.Forms.MouseEventHandler(this.PnlImagePreview_MouseScroll);
            // 
            // pxbPreview
            // 
            this.pxbPreview.BackColor = System.Drawing.Color.Silver;
            this.pxbPreview.Location = new System.Drawing.Point(0, 0);
            this.pxbPreview.Name = "pxbPreview";
            this.pxbPreview.Size = new System.Drawing.Size(181, 53);
            this.pxbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pxbPreview.TabIndex = 315;
            this.pxbPreview.TabStop = false;
            this.pxbPreview.Click += new System.EventHandler(this.PreviewImageBox_Click);
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
            // numFontHeight
            // 
            this.numFontHeight.Enabled = false;
            this.numFontHeight.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numFontHeight.Location = new System.Drawing.Point(141, 89);
            this.numFontHeight.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numFontHeight.MouseWheelIncrement = 0;
            this.numFontHeight.Name = "numFontHeight";
            this.numFontHeight.SelectedText = "";
            this.numFontHeight.SelectionLength = 0;
            this.numFontHeight.SelectionStart = 0;
            this.numFontHeight.Size = new System.Drawing.Size(40, 20);
            this.numFontHeight.TabIndex = 7;
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
            this.numFontWidth.Location = new System.Drawing.Point(80, 89);
            this.numFontWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numFontWidth.MouseWheelIncrement = 0;
            this.numFontWidth.Name = "numFontWidth";
            this.numFontWidth.SelectedText = "";
            this.numFontWidth.SelectionLength = 0;
            this.numFontWidth.SelectionStart = 0;
            this.numFontWidth.Size = new System.Drawing.Size(40, 20);
            this.numFontWidth.TabIndex = 5;
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
            this.numSymbols.Location = new System.Drawing.Point(80, 63);
            this.numSymbols.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numSymbols.MouseWheelIncrement = 0;
            this.numSymbols.Name = "numSymbols";
            this.numSymbols.SelectedText = "";
            this.numSymbols.SelectionLength = 0;
            this.numSymbols.SelectionStart = 0;
            this.numSymbols.Size = new System.Drawing.Size(128, 20);
            this.numSymbols.TabIndex = 3;
            this.numSymbols.ValueChanged += new System.EventHandler(this.NumSymbols_ValueChanged);
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
            this.dgrvSymbolsList.Location = new System.Drawing.Point(13, 115);
            this.dgrvSymbolsList.MultiSelect = false;
            this.dgrvSymbolsList.Name = "dgrvSymbolsList";
            this.dgrvSymbolsList.ReadOnly = true;
            this.dgrvSymbolsList.RowHeadersVisible = false;
            this.dgrvSymbolsList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgrvSymbolsList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrvSymbolsList.Size = new System.Drawing.Size(195, 319);
            this.dgrvSymbolsList.StandardTab = true;
            this.dgrvSymbolsList.TabIndex = 20;
            this.dgrvSymbolsList.VerticalScrollbarOffset = 0;
            this.dgrvSymbolsList.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dgrvSymbolsList_CellContextMenuStripNeeded);
            this.dgrvSymbolsList.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgrvSymbolsList_CellMouseDown);
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
            // numZoomPreview
            // 
            this.numZoomPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numZoomPreview.EnteredValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoomPreview.Location = new System.Drawing.Point(342, 579);
            this.numZoomPreview.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numZoomPreview.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoomPreview.Name = "numZoomPreview";
            this.numZoomPreview.SelectedText = "";
            this.numZoomPreview.SelectionLength = 0;
            this.numZoomPreview.SelectionStart = 0;
            this.numZoomPreview.Size = new System.Drawing.Size(51, 20);
            this.numZoomPreview.TabIndex = 123;
            this.numZoomPreview.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoomPreview.ValueChanged += new System.EventHandler(this.numZoomPreview_ValueChanged);
            // 
            // numZoom
            // 
            this.numZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numZoom.EnteredValue = new decimal(new int[] {
            20,
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
            this.numZoom.SelectedText = "";
            this.numZoom.SelectionLength = 0;
            this.numZoom.SelectionStart = 0;
            this.numZoom.Size = new System.Drawing.Size(51, 20);
            this.numZoom.TabIndex = 42;
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
            this.pnlImageScroll.MouseScroll += new System.Windows.Forms.MouseEventHandler(this.PnlImageScroll_MouseScroll);
            // 
            // pxbEditGridFront
            // 
            this.pxbEditGridFront.BackColor = System.Drawing.Color.Transparent;
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
            this.pxbFullSize.Location = new System.Drawing.Point(3, 3);
            this.pxbFullSize.Margin = new System.Windows.Forms.Padding(0);
            this.pxbFullSize.Name = "pxbFullSize";
            this.pxbFullSize.Size = new System.Drawing.Size(200, 200);
            this.pxbFullSize.TabIndex = 1;
            this.pxbFullSize.TabStop = false;
            this.pxbFullSize.Visible = false;
            this.pxbFullSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageBox_Click);
            // 
            // FrmFontEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 612);
            this.Controls.Add(this.chkWrapPreview);
            this.Controls.Add(this.pnlImagePreview);
            this.Controls.Add(this.btnValType);
            this.Controls.Add(this.btnRemap);
            this.Controls.Add(this.txtPreview);
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
            this.Controls.Add(this.lblZoomPreview);
            this.Controls.Add(this.lblZoom);
            this.Controls.Add(this.numZoomPreview);
            this.Controls.Add(this.numZoom);
            this.Controls.Add(this.pnlImageScroll);
            this.Controls.Add(this.menuStrip1);
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 650);
            this.Name = "FrmFontEditor";
            this.Text = "Westwood Font Editor v#.#.# - Created by Nyerguds";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFontEditor_FormClosing);
            this.Shown += new System.EventHandler(this.FrmFontEditor_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Frm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Frm_DragEnter);
            this.Resize += new System.EventHandler(this.FrmFontEditor_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numYOffset)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlImagePreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pxbPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFontWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSymbols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvSymbolsList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoomPreview)).EndInit();
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
        private System.Windows.Forms.Label lblFontMaxX;
        private System.Windows.Forms.Label lblSymbolWidth;
        private System.Windows.Forms.Label lblSymbolHeight;
        private System.Windows.Forms.Label lblSymbolYOffset;
        private Nyerguds.Util.UI.PixelBox pxbFullSize;
        private Nyerguds.Util.UI.PixelBox pxbEditGridBehind;
        private Nyerguds.Util.UI.PixelBox pxbImage;
        private Nyerguds.Util.UI.PixelBox pxbEditGridFront;
        private System.Windows.Forms.Label lblPaintColor1;
        private Nyerguds.Util.UI.PalettePanel palColorSelector;
        private System.Windows.Forms.Label lblPaintColor2;
        private System.Windows.Forms.ToolTip toolTip1;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkGrid;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkOutline;
        private Nyerguds.Util.UI.EnhNumericUpDown numYOffset;
        private Nyerguds.Util.UI.DataGridViewScrollSupport dgrvSymbolsList;
        private Nyerguds.Util.UI.ComboBoxSmartWidth cmbEncodings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFont;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFont;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFontAs;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.Button btnShiftDown;
        private System.Windows.Forms.Button btnShiftRight;
        private System.Windows.Forms.Button btnShiftUp;
        private System.Windows.Forms.Button btnShiftLeft;
        private Nyerguds.Util.UI.EnhNumericUpDown numWidth;
        private Nyerguds.Util.UI.EnhNumericUpDown numHeight;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiRevertSymbol;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopySymbol;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasteSymbol;
        private Nyerguds.Util.UI.EnhNumericUpDown numSymbols;
        private Nyerguds.Util.UI.EnhNumericUpDown numFontWidth;
        private Nyerguds.Util.UI.EnhNumericUpDown numFontHeight;
        private System.Windows.Forms.ToolStripMenuItem tsmiRevertFont;
        private System.Windows.Forms.ToolStripMenuItem tsmiInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkPaint;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkPicker;
        private Nyerguds.Util.UI.ImageButtonCheckBox chkShiftWrap;
        private Nyerguds.Util.UI.ComboBoxSmartWidth cmbPalettes;
        private System.Windows.Forms.Button btnSavePalette;
        private System.Windows.Forms.Button btnResetPalette;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditorSettings;
        private System.Windows.Forms.TextBox txtPreview;
        private Nyerguds.Util.UI.PixelBox pxbPreview;
        private System.Windows.Forms.Button btnRemap;
        private System.Windows.Forms.Button btnValType;
        private System.Windows.Forms.ToolStripMenuItem tsmiManagePalettes;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasteSymbolTrans;
        private Nyerguds.Util.UI.SelectablePanel pnlImagePreview;
        private System.Windows.Forms.CheckBox chkWrapPreview;
        private Nyerguds.Util.UI.EnhNumericUpDown numZoomPreview;
        private System.Windows.Forms.Label lblZoomPreview;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewFont;
        private System.Windows.Forms.ToolStripMenuItem tsmiOptimizeWidths;
    }
}

