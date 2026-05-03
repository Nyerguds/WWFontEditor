namespace WWFontEditor.UI
{
    partial class FrmSettings
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkEnablePixelWrap = new System.Windows.Forms.CheckBox();
            this.chkEnableEditArea = new System.Windows.Forms.CheckBox();
            this.chkEnableGrid = new System.Windows.Forms.CheckBox();
            this.lblDefaultSelectedSymbol = new System.Windows.Forms.Label();
            this.numDefaultSelectedSymbol = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.lblDefaultZoom = new System.Windows.Forms.Label();
            this.numDefaultZoom = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.chkUsePaletteBg = new System.Windows.Forms.CheckBox();
            this.lblEditorBackColor = new System.Windows.Forms.Label();
            this.lblValEditorBackColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.lblValEditorGridColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.lblValEditAreaOutlineColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.lblValEditAreaGridColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.lblValEditorOutlineColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.lblEditorOutlineColor = new System.Windows.Forms.Label();
            this.lblEditorGridColor = new System.Windows.Forms.Label();
            this.lblEditAreaOutlineColor = new System.Windows.Forms.Label();
            this.lblEditAreaGridColor = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkPal8BppRainbow = new System.Windows.Forms.CheckBox();
            this.chkPal4BppWB = new System.Windows.Forms.CheckBox();
            this.chkPal8BppWB = new System.Windows.Forms.CheckBox();
            this.chkPal4BppBW = new System.Windows.Forms.CheckBox();
            this.chkPal1BppWB = new System.Windows.Forms.CheckBox();
            this.chkPal8BppBW = new System.Windows.Forms.CheckBox();
            this.chkPal4BppWin = new System.Windows.Forms.CheckBox();
            this.chkPal1BppBW = new System.Windows.Forms.CheckBox();
            this.chkPal8BppWin = new System.Windows.Forms.CheckBox();
            this.chkPal4BppRainbow = new System.Windows.Forms.CheckBox();
            this.chkPal1BppBR = new System.Windows.Forms.CheckBox();
            this.lblPalEightBit = new System.Windows.Forms.Label();
            this.lblPalFourBit = new System.Windows.Forms.Label();
            this.lblPalOneBit = new System.Windows.Forms.Label();
            this.lblGenerateDefaultPalettes = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.chkLimit8Bit = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultSelectedSymbol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultZoom)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(402, 290);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkEnablePixelWrap);
            this.tabPage1.Controls.Add(this.chkEnableEditArea);
            this.tabPage1.Controls.Add(this.chkEnableGrid);
            this.tabPage1.Controls.Add(this.lblDefaultSelectedSymbol);
            this.tabPage1.Controls.Add(this.numDefaultSelectedSymbol);
            this.tabPage1.Controls.Add(this.lblDefaultZoom);
            this.tabPage1.Controls.Add(this.numDefaultZoom);
            this.tabPage1.Controls.Add(this.chkUsePaletteBg);
            this.tabPage1.Controls.Add(this.lblEditorBackColor);
            this.tabPage1.Controls.Add(this.lblValEditorBackColor);
            this.tabPage1.Controls.Add(this.lblValEditorGridColor);
            this.tabPage1.Controls.Add(this.lblValEditAreaOutlineColor);
            this.tabPage1.Controls.Add(this.lblValEditAreaGridColor);
            this.tabPage1.Controls.Add(this.lblValEditorOutlineColor);
            this.tabPage1.Controls.Add(this.lblEditorOutlineColor);
            this.tabPage1.Controls.Add(this.lblEditorGridColor);
            this.tabPage1.Controls.Add(this.lblEditAreaOutlineColor);
            this.tabPage1.Controls.Add(this.lblEditAreaGridColor);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(394, 264);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Editor";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkEnablePixelWrap
            // 
            this.chkEnablePixelWrap.AutoSize = true;
            this.chkEnablePixelWrap.Location = new System.Drawing.Point(11, 235);
            this.chkEnablePixelWrap.Name = "chkEnablePixelWrap";
            this.chkEnablePixelWrap.Size = new System.Drawing.Size(215, 17);
            this.chkEnablePixelWrap.TabIndex = 27;
            this.chkEnablePixelWrap.Text = "Enable wrapping on pixel shift by default";
            this.chkEnablePixelWrap.UseVisualStyleBackColor = true;
            // 
            // chkEnableEditArea
            // 
            this.chkEnableEditArea.AutoSize = true;
            this.chkEnableEditArea.Location = new System.Drawing.Point(11, 212);
            this.chkEnableEditArea.Name = "chkEnableEditArea";
            this.chkEnableEditArea.Size = new System.Drawing.Size(172, 17);
            this.chkEnableEditArea.TabIndex = 26;
            this.chkEnableEditArea.Text = "Enable editable area by default";
            this.chkEnableEditArea.UseVisualStyleBackColor = true;
            // 
            // chkEnableGrid
            // 
            this.chkEnableGrid.AutoSize = true;
            this.chkEnableGrid.Location = new System.Drawing.Point(11, 189);
            this.chkEnableGrid.Name = "chkEnableGrid";
            this.chkEnableGrid.Size = new System.Drawing.Size(128, 17);
            this.chkEnableGrid.TabIndex = 25;
            this.chkEnableGrid.Text = "Enable grid by default";
            this.chkEnableGrid.UseVisualStyleBackColor = true;
            // 
            // lblDefaultSelectedSymbol
            // 
            this.lblDefaultSelectedSymbol.AutoSize = true;
            this.lblDefaultSelectedSymbol.Location = new System.Drawing.Point(8, 166);
            this.lblDefaultSelectedSymbol.Name = "lblDefaultSelectedSymbol";
            this.lblDefaultSelectedSymbol.Size = new System.Drawing.Size(119, 13);
            this.lblDefaultSelectedSymbol.TabIndex = 23;
            this.lblDefaultSelectedSymbol.Text = "Default selected symbol";
            // 
            // numDefaultSelectedSymbol
            // 
            this.numDefaultSelectedSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDefaultSelectedSymbol.EnteredValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDefaultSelectedSymbol.Location = new System.Drawing.Point(331, 164);
            this.numDefaultSelectedSymbol.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numDefaultSelectedSymbol.Name = "numDefaultSelectedSymbol";
            this.numDefaultSelectedSymbol.Size = new System.Drawing.Size(53, 20);
            this.numDefaultSelectedSymbol.TabIndex = 24;
            this.numDefaultSelectedSymbol.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // lblDefaultZoom
            // 
            this.lblDefaultZoom.AutoSize = true;
            this.lblDefaultZoom.Location = new System.Drawing.Point(8, 143);
            this.lblDefaultZoom.Name = "lblDefaultZoom";
            this.lblDefaultZoom.Size = new System.Drawing.Size(69, 13);
            this.lblDefaultZoom.TabIndex = 21;
            this.lblDefaultZoom.Text = "Default zoom";
            // 
            // numDefaultZoom
            // 
            this.numDefaultZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDefaultZoom.EnteredValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDefaultZoom.Location = new System.Drawing.Point(331, 141);
            this.numDefaultZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDefaultZoom.Name = "numDefaultZoom";
            this.numDefaultZoom.Size = new System.Drawing.Size(53, 20);
            this.numDefaultZoom.TabIndex = 22;
            this.numDefaultZoom.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // chkUsePaletteBg
            // 
            this.chkUsePaletteBg.AutoSize = true;
            this.chkUsePaletteBg.Location = new System.Drawing.Point(34, 29);
            this.chkUsePaletteBg.Name = "chkUsePaletteBg";
            this.chkUsePaletteBg.Size = new System.Drawing.Size(162, 17);
            this.chkUsePaletteBg.TabIndex = 12;
            this.chkUsePaletteBg.Text = "Use first palette color instead";
            this.chkUsePaletteBg.UseVisualStyleBackColor = true;
            this.chkUsePaletteBg.CheckedChanged += new System.EventHandler(this.ChkUsePaletteBg_CheckedChanged);
            // 
            // lblEditorBackColor
            // 
            this.lblEditorBackColor.AutoSize = true;
            this.lblEditorBackColor.Location = new System.Drawing.Point(8, 10);
            this.lblEditorBackColor.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lblEditorBackColor.Name = "lblEditorBackColor";
            this.lblEditorBackColor.Size = new System.Drawing.Size(123, 13);
            this.lblEditorBackColor.TabIndex = 10;
            this.lblEditorBackColor.Text = "Editor background color:";
            // 
            // lblValEditorBackColor
            // 
            this.lblValEditorBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValEditorBackColor.BackColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorBackColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValEditorBackColor.Checked = true;
            this.lblValEditorBackColor.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorBackColor.Location = new System.Drawing.Point(365, 9);
            this.lblValEditorBackColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValEditorBackColor.Name = "lblValEditorBackColor";
            this.lblValEditorBackColor.Size = new System.Drawing.Size(19, 17);
            this.lblValEditorBackColor.TabIndex = 11;
            this.lblValEditorBackColor.Toggle = false;
            this.lblValEditorBackColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValEditorBackColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // lblValEditorGridColor
            // 
            this.lblValEditorGridColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValEditorGridColor.BackColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorGridColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValEditorGridColor.Checked = true;
            this.lblValEditorGridColor.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorGridColor.Location = new System.Drawing.Point(365, 73);
            this.lblValEditorGridColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValEditorGridColor.Name = "lblValEditorGridColor";
            this.lblValEditorGridColor.Size = new System.Drawing.Size(19, 17);
            this.lblValEditorGridColor.TabIndex = 16;
            this.lblValEditorGridColor.Toggle = false;
            this.lblValEditorGridColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValEditorGridColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // lblValEditAreaOutlineColor
            // 
            this.lblValEditAreaOutlineColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValEditAreaOutlineColor.BackColor = System.Drawing.Color.Fuchsia;
            this.lblValEditAreaOutlineColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValEditAreaOutlineColor.Checked = true;
            this.lblValEditAreaOutlineColor.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblValEditAreaOutlineColor.Location = new System.Drawing.Point(365, 96);
            this.lblValEditAreaOutlineColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValEditAreaOutlineColor.Name = "lblValEditAreaOutlineColor";
            this.lblValEditAreaOutlineColor.Size = new System.Drawing.Size(19, 17);
            this.lblValEditAreaOutlineColor.TabIndex = 18;
            this.lblValEditAreaOutlineColor.Toggle = false;
            this.lblValEditAreaOutlineColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValEditAreaOutlineColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // lblValEditAreaGridColor
            // 
            this.lblValEditAreaGridColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValEditAreaGridColor.BackColor = System.Drawing.Color.Fuchsia;
            this.lblValEditAreaGridColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValEditAreaGridColor.Checked = true;
            this.lblValEditAreaGridColor.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblValEditAreaGridColor.Location = new System.Drawing.Point(365, 119);
            this.lblValEditAreaGridColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValEditAreaGridColor.Name = "lblValEditAreaGridColor";
            this.lblValEditAreaGridColor.Size = new System.Drawing.Size(19, 17);
            this.lblValEditAreaGridColor.TabIndex = 20;
            this.lblValEditAreaGridColor.Toggle = false;
            this.lblValEditAreaGridColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValEditAreaGridColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // lblValEditorOutlineColor
            // 
            this.lblValEditorOutlineColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValEditorOutlineColor.BackColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorOutlineColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValEditorOutlineColor.Checked = true;
            this.lblValEditorOutlineColor.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblValEditorOutlineColor.Location = new System.Drawing.Point(365, 50);
            this.lblValEditorOutlineColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValEditorOutlineColor.Name = "lblValEditorOutlineColor";
            this.lblValEditorOutlineColor.Size = new System.Drawing.Size(19, 17);
            this.lblValEditorOutlineColor.TabIndex = 14;
            this.lblValEditorOutlineColor.Toggle = false;
            this.lblValEditorOutlineColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValEditorOutlineColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // lblEditorOutlineColor
            // 
            this.lblEditorOutlineColor.AutoSize = true;
            this.lblEditorOutlineColor.Location = new System.Drawing.Point(8, 51);
            this.lblEditorOutlineColor.Name = "lblEditorOutlineColor";
            this.lblEditorOutlineColor.Size = new System.Drawing.Size(97, 13);
            this.lblEditorOutlineColor.TabIndex = 13;
            this.lblEditorOutlineColor.Text = "Editor outline color:";
            // 
            // lblEditorGridColor
            // 
            this.lblEditorGridColor.AutoSize = true;
            this.lblEditorGridColor.Location = new System.Drawing.Point(8, 74);
            this.lblEditorGridColor.Name = "lblEditorGridColor";
            this.lblEditorGridColor.Size = new System.Drawing.Size(83, 13);
            this.lblEditorGridColor.TabIndex = 15;
            this.lblEditorGridColor.Text = "Editor grid color:";
            // 
            // lblEditAreaOutlineColor
            // 
            this.lblEditAreaOutlineColor.AutoSize = true;
            this.lblEditAreaOutlineColor.Location = new System.Drawing.Point(8, 97);
            this.lblEditAreaOutlineColor.Name = "lblEditAreaOutlineColor";
            this.lblEditAreaOutlineColor.Size = new System.Drawing.Size(132, 13);
            this.lblEditAreaOutlineColor.TabIndex = 17;
            this.lblEditAreaOutlineColor.Text = "Editable area outline color:";
            // 
            // lblEditAreaGridColor
            // 
            this.lblEditAreaGridColor.AutoSize = true;
            this.lblEditAreaGridColor.Location = new System.Drawing.Point(8, 120);
            this.lblEditAreaGridColor.Name = "lblEditAreaGridColor";
            this.lblEditAreaGridColor.Size = new System.Drawing.Size(118, 13);
            this.lblEditAreaGridColor.TabIndex = 19;
            this.lblEditAreaGridColor.Text = "Editable area grid color:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkPal8BppRainbow);
            this.tabPage2.Controls.Add(this.chkPal4BppWB);
            this.tabPage2.Controls.Add(this.chkLimit8Bit);
            this.tabPage2.Controls.Add(this.chkPal8BppWB);
            this.tabPage2.Controls.Add(this.chkPal4BppBW);
            this.tabPage2.Controls.Add(this.chkPal1BppWB);
            this.tabPage2.Controls.Add(this.chkPal8BppBW);
            this.tabPage2.Controls.Add(this.chkPal4BppWin);
            this.tabPage2.Controls.Add(this.chkPal1BppBW);
            this.tabPage2.Controls.Add(this.chkPal8BppWin);
            this.tabPage2.Controls.Add(this.chkPal4BppRainbow);
            this.tabPage2.Controls.Add(this.chkPal1BppBR);
            this.tabPage2.Controls.Add(this.lblPalEightBit);
            this.tabPage2.Controls.Add(this.lblPalFourBit);
            this.tabPage2.Controls.Add(this.lblPalOneBit);
            this.tabPage2.Controls.Add(this.lblGenerateDefaultPalettes);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(394, 264);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Palettes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkPal8BppRainbow
            // 
            this.chkPal8BppRainbow.AutoSize = true;
            this.chkPal8BppRainbow.Location = new System.Drawing.Point(209, 48);
            this.chkPal8BppRainbow.Name = "chkPal8BppRainbow";
            this.chkPal8BppRainbow.Size = new System.Drawing.Size(158, 17);
            this.chkPal8BppRainbow.TabIndex = 121;
            this.chkPal8BppRainbow.Text = "16-color rainbow + Rainbow";
            this.chkPal8BppRainbow.UseVisualStyleBackColor = true;
            // 
            // chkPal4BppWB
            // 
            this.chkPal4BppWB.AutoSize = true;
            this.chkPal4BppWB.Location = new System.Drawing.Point(21, 205);
            this.chkPal4BppWB.Name = "chkPal4BppWB";
            this.chkPal4BppWB.Size = new System.Drawing.Size(123, 17);
            this.chkPal4BppWB.TabIndex = 114;
            this.chkPal4BppWB.Text = "White-to-Black Fade";
            this.chkPal4BppWB.UseVisualStyleBackColor = true;
            // 
            // chkPal8BppWB
            // 
            this.chkPal8BppWB.AutoSize = true;
            this.chkPal8BppWB.Location = new System.Drawing.Point(209, 117);
            this.chkPal8BppWB.Name = "chkPal8BppWB";
            this.chkPal8BppWB.Size = new System.Drawing.Size(123, 17);
            this.chkPal8BppWB.TabIndex = 124;
            this.chkPal8BppWB.Text = "White-to-Black Fade";
            this.chkPal8BppWB.UseVisualStyleBackColor = true;
            // 
            // chkPal4BppBW
            // 
            this.chkPal4BppBW.AutoSize = true;
            this.chkPal4BppBW.Location = new System.Drawing.Point(21, 182);
            this.chkPal4BppBW.Name = "chkPal4BppBW";
            this.chkPal4BppBW.Size = new System.Drawing.Size(120, 17);
            this.chkPal4BppBW.TabIndex = 113;
            this.chkPal4BppBW.Text = "Black-to-White fade";
            this.chkPal4BppBW.UseVisualStyleBackColor = true;
            // 
            // chkPal1BppWB
            // 
            this.chkPal1BppWB.AutoSize = true;
            this.chkPal1BppWB.Location = new System.Drawing.Point(21, 94);
            this.chkPal1BppWB.Name = "chkPal1BppWB";
            this.chkPal1BppWB.Size = new System.Drawing.Size(93, 17);
            this.chkPal1BppWB.TabIndex = 103;
            this.chkPal1BppWB.Text = "White && Black";
            this.chkPal1BppWB.UseVisualStyleBackColor = true;
            // 
            // chkPal8BppBW
            // 
            this.chkPal8BppBW.AutoSize = true;
            this.chkPal8BppBW.Location = new System.Drawing.Point(209, 94);
            this.chkPal8BppBW.Name = "chkPal8BppBW";
            this.chkPal8BppBW.Size = new System.Drawing.Size(120, 17);
            this.chkPal8BppBW.TabIndex = 123;
            this.chkPal8BppBW.Text = "Black-to-White fade";
            this.chkPal8BppBW.UseVisualStyleBackColor = true;
            // 
            // chkPal4BppWin
            // 
            this.chkPal4BppWin.AutoSize = true;
            this.chkPal4BppWin.Location = new System.Drawing.Point(21, 159);
            this.chkPal4BppWin.Name = "chkPal4BppWin";
            this.chkPal4BppWin.Size = new System.Drawing.Size(111, 17);
            this.chkPal4BppWin.TabIndex = 112;
            this.chkPal4BppWin.Text = "Windows 16-color";
            this.chkPal4BppWin.UseVisualStyleBackColor = true;
            // 
            // chkPal1BppBW
            // 
            this.chkPal1BppBW.AutoSize = true;
            this.chkPal1BppBW.Location = new System.Drawing.Point(21, 71);
            this.chkPal1BppBW.Name = "chkPal1BppBW";
            this.chkPal1BppBW.Size = new System.Drawing.Size(93, 17);
            this.chkPal1BppBW.TabIndex = 102;
            this.chkPal1BppBW.Text = "Black && White";
            this.chkPal1BppBW.UseVisualStyleBackColor = true;
            // 
            // chkPal8BppWin
            // 
            this.chkPal8BppWin.AutoSize = true;
            this.chkPal8BppWin.Location = new System.Drawing.Point(209, 71);
            this.chkPal8BppWin.Name = "chkPal8BppWin";
            this.chkPal8BppWin.Size = new System.Drawing.Size(165, 17);
            this.chkPal8BppWin.TabIndex = 122;
            this.chkPal8BppWin.Text = "Windows 16-color + Rainbow";
            this.chkPal8BppWin.UseVisualStyleBackColor = true;
            // 
            // chkPal4BppRainbow
            // 
            this.chkPal4BppRainbow.AutoSize = true;
            this.chkPal4BppRainbow.Location = new System.Drawing.Point(21, 136);
            this.chkPal4BppRainbow.Name = "chkPal4BppRainbow";
            this.chkPal4BppRainbow.Size = new System.Drawing.Size(68, 17);
            this.chkPal4BppRainbow.TabIndex = 111;
            this.chkPal4BppRainbow.Text = "Rainbow";
            this.chkPal4BppRainbow.UseVisualStyleBackColor = true;
            // 
            // chkPal1BppBR
            // 
            this.chkPal1BppBR.AutoSize = true;
            this.chkPal1BppBR.Location = new System.Drawing.Point(21, 48);
            this.chkPal1BppBR.Name = "chkPal1BppBR";
            this.chkPal1BppBR.Size = new System.Drawing.Size(85, 17);
            this.chkPal1BppBR.TabIndex = 101;
            this.chkPal1BppBR.Text = "Black && Red";
            this.chkPal1BppBR.UseVisualStyleBackColor = true;
            // 
            // lblPalEightBit
            // 
            this.lblPalEightBit.AutoSize = true;
            this.lblPalEightBit.Location = new System.Drawing.Point(196, 29);
            this.lblPalEightBit.Margin = new System.Windows.Forms.Padding(3);
            this.lblPalEightBit.Name = "lblPalEightBit";
            this.lblPalEightBit.Size = new System.Drawing.Size(48, 13);
            this.lblPalEightBit.TabIndex = 120;
            this.lblPalEightBit.Text = "Eight-bit:";
            // 
            // lblPalFourBit
            // 
            this.lblPalFourBit.AutoSize = true;
            this.lblPalFourBit.Location = new System.Drawing.Point(8, 117);
            this.lblPalFourBit.Margin = new System.Windows.Forms.Padding(3);
            this.lblPalFourBit.Name = "lblPalFourBit";
            this.lblPalFourBit.Size = new System.Drawing.Size(45, 13);
            this.lblPalFourBit.TabIndex = 110;
            this.lblPalFourBit.Text = "Four-bit:";
            // 
            // lblPalOneBit
            // 
            this.lblPalOneBit.AutoSize = true;
            this.lblPalOneBit.Location = new System.Drawing.Point(8, 29);
            this.lblPalOneBit.Margin = new System.Windows.Forms.Padding(3);
            this.lblPalOneBit.Name = "lblPalOneBit";
            this.lblPalOneBit.Size = new System.Drawing.Size(44, 13);
            this.lblPalOneBit.TabIndex = 100;
            this.lblPalOneBit.Text = "One-bit:";
            // 
            // lblGenerateDefaultPalettes
            // 
            this.lblGenerateDefaultPalettes.AutoSize = true;
            this.lblGenerateDefaultPalettes.Location = new System.Drawing.Point(8, 10);
            this.lblGenerateDefaultPalettes.Margin = new System.Windows.Forms.Padding(3);
            this.lblGenerateDefaultPalettes.Name = "lblGenerateDefaultPalettes";
            this.lblGenerateDefaultPalettes.Size = new System.Drawing.Size(104, 13);
            this.lblGenerateDefaultPalettes.TabIndex = 50;
            this.lblGenerateDefaultPalettes.Text = "Add default palettes:";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 296);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(104, 23);
            this.btnReset.TabIndex = 200;
            this.btnReset.Text = "Reset to defaults";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(315, 296);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 202;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(234, 296);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 201;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // chkLimit8Bit
            // 
            this.chkLimit8Bit.AutoSize = true;
            this.chkLimit8Bit.Location = new System.Drawing.Point(209, 150);
            this.chkLimit8Bit.Name = "chkLimit8Bit";
            this.chkLimit8Bit.Size = new System.Drawing.Size(160, 17);
            this.chkLimit8Bit.TabIndex = 124;
            this.chkLimit8Bit.Text = "Limit 8-bit fonts to 16 colours";
            this.chkLimit8Bit.UseVisualStyleBackColor = true;
            this.chkLimit8Bit.CheckedChanged += new System.EventHandler(this.chkLimit8Bit_CheckedChanged);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(402, 325);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.Name = "FrmSettings";
            this.Text = "Font Editor Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultSelectedSymbol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDefaultZoom)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblEditAreaGridColor;
        private System.Windows.Forms.Label lblEditorOutlineColor;
        private System.Windows.Forms.Label lblEditorGridColor;
        private System.Windows.Forms.Label lblEditAreaOutlineColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValEditorBackColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValEditorGridColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValEditAreaOutlineColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValEditAreaGridColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValEditorOutlineColor;
        private System.Windows.Forms.CheckBox chkUsePaletteBg;
        private System.Windows.Forms.Label lblEditorBackColor;
        private Nyerguds.Util.UI.EnhNumericUpDown numDefaultZoom;
        private System.Windows.Forms.Label lblDefaultZoom;
        private System.Windows.Forms.CheckBox chkEnablePixelWrap;
        private System.Windows.Forms.CheckBox chkEnableEditArea;
        private System.Windows.Forms.CheckBox chkEnableGrid;
        private System.Windows.Forms.Label lblDefaultSelectedSymbol;
        private Nyerguds.Util.UI.EnhNumericUpDown numDefaultSelectedSymbol;
        private System.Windows.Forms.Label lblPalEightBit;
        private System.Windows.Forms.Label lblPalFourBit;
        private System.Windows.Forms.Label lblPalOneBit;
        private System.Windows.Forms.Label lblGenerateDefaultPalettes;
        private System.Windows.Forms.CheckBox chkPal8BppRainbow;
        private System.Windows.Forms.CheckBox chkPal4BppWB;
        private System.Windows.Forms.CheckBox chkPal8BppWB;
        private System.Windows.Forms.CheckBox chkPal4BppBW;
        private System.Windows.Forms.CheckBox chkPal1BppWB;
        private System.Windows.Forms.CheckBox chkPal8BppBW;
        private System.Windows.Forms.CheckBox chkPal4BppWin;
        private System.Windows.Forms.CheckBox chkPal1BppBW;
        private System.Windows.Forms.CheckBox chkPal8BppWin;
        private System.Windows.Forms.CheckBox chkPal4BppRainbow;
        private System.Windows.Forms.CheckBox chkPal1BppBR;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox chkLimit8Bit;
    }
}