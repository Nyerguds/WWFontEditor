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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblZoom = new System.Windows.Forms.Label();
            this.lblIndex = new System.Windows.Forms.Label();
            this.lblValWidth = new System.Windows.Forms.Label();
            this.lblFontWidth = new System.Windows.Forms.Label();
            this.lblFontHeight = new System.Windows.Forms.Label();
            this.lblValHeight = new System.Windows.Forms.Label();
            this.lblColors = new System.Windows.Forms.Label();
            this.lblValColors = new System.Windows.Forms.Label();
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblValFilename = new System.Windows.Forms.Label();
            this.lblValFontWidth = new System.Windows.Forms.Label();
            this.lblValFontHeight = new System.Windows.Forms.Label();
            this.lblValYOffset = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblYOffset = new System.Windows.Forms.Label();
            this.numIndex = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.numZoom = new Nyerguds.Util.UI.EnhNumericUpDown();
            this.pnlImageScroll = new Nyerguds.Util.UI.SelectablePanel();
            this.pxbEditGridFront = new RedCell.UI.Controls.PixelBox();
            this.pxbImage = new RedCell.UI.Controls.PixelBox();
            this.pxbEditGridBehind = new RedCell.UI.Controls.PixelBox();
            this.pxbFullSize = new RedCell.UI.Controls.PixelBox();
            this.chkGrid = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZoom)).BeginInit();
            this.pnlImageScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridFront)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbEditGridBehind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pxbFullSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(591, 327);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(81, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save image";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(510, 327);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open file";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // lblZoom
            // 
            this.lblZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZoom.Location = new System.Drawing.Point(171, 330);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(72, 20);
            this.lblZoom.TabIndex = 23;
            this.lblZoom.Text = "Zoom factor:";
            this.lblZoom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIndex
            // 
            this.lblIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIndex.Location = new System.Drawing.Point(375, 13);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(94, 20);
            this.lblIndex.TabIndex = 25;
            this.lblIndex.Text = "Index:";
            this.lblIndex.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValWidth
            // 
            this.lblValWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValWidth.AutoSize = true;
            this.lblValWidth.Location = new System.Drawing.Point(475, 148);
            this.lblValWidth.Name = "lblValWidth";
            this.lblValWidth.Size = new System.Drawing.Size(10, 13);
            this.lblValWidth.TabIndex = 26;
            this.lblValWidth.Text = "-";
            // 
            // lblFontWidth
            // 
            this.lblFontWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFontWidth.Location = new System.Drawing.Point(375, 84);
            this.lblFontWidth.Name = "lblFontWidth";
            this.lblFontWidth.Size = new System.Drawing.Size(94, 20);
            this.lblFontWidth.TabIndex = 27;
            this.lblFontWidth.Text = "Width:";
            this.lblFontWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFontHeight
            // 
            this.lblFontHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFontHeight.Location = new System.Drawing.Point(375, 104);
            this.lblFontHeight.Name = "lblFontHeight";
            this.lblFontHeight.Size = new System.Drawing.Size(94, 20);
            this.lblFontHeight.TabIndex = 27;
            this.lblFontHeight.Text = "Height:";
            this.lblFontHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValHeight
            // 
            this.lblValHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValHeight.AutoSize = true;
            this.lblValHeight.Location = new System.Drawing.Point(475, 168);
            this.lblValHeight.Name = "lblValHeight";
            this.lblValHeight.Size = new System.Drawing.Size(10, 13);
            this.lblValHeight.TabIndex = 26;
            this.lblValHeight.Text = "-";
            // 
            // lblColors
            // 
            this.lblColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblColors.Location = new System.Drawing.Point(375, 64);
            this.lblColors.Name = "lblColors";
            this.lblColors.Size = new System.Drawing.Size(94, 20);
            this.lblColors.TabIndex = 27;
            this.lblColors.Text = "Colors:";
            this.lblColors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValColors
            // 
            this.lblValColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValColors.AutoSize = true;
            this.lblValColors.Location = new System.Drawing.Point(475, 68);
            this.lblValColors.Name = "lblValColors";
            this.lblValColors.Size = new System.Drawing.Size(10, 13);
            this.lblValColors.TabIndex = 28;
            this.lblValColors.Text = "-";
            // 
            // lblFilename
            // 
            this.lblFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilename.Location = new System.Drawing.Point(375, 44);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(94, 20);
            this.lblFilename.TabIndex = 29;
            this.lblFilename.Text = "Filename:";
            this.lblFilename.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblValFilename
            // 
            this.lblValFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValFilename.AutoSize = true;
            this.lblValFilename.Location = new System.Drawing.Point(475, 48);
            this.lblValFilename.Name = "lblValFilename";
            this.lblValFilename.Size = new System.Drawing.Size(10, 13);
            this.lblValFilename.TabIndex = 30;
            this.lblValFilename.Text = "-";
            // 
            // lblValFontWidth
            // 
            this.lblValFontWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValFontWidth.AutoSize = true;
            this.lblValFontWidth.Location = new System.Drawing.Point(475, 88);
            this.lblValFontWidth.Name = "lblValFontWidth";
            this.lblValFontWidth.Size = new System.Drawing.Size(10, 13);
            this.lblValFontWidth.TabIndex = 26;
            this.lblValFontWidth.Text = "-";
            // 
            // lblValFontHeight
            // 
            this.lblValFontHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValFontHeight.AutoSize = true;
            this.lblValFontHeight.Location = new System.Drawing.Point(475, 108);
            this.lblValFontHeight.Name = "lblValFontHeight";
            this.lblValFontHeight.Size = new System.Drawing.Size(10, 13);
            this.lblValFontHeight.TabIndex = 26;
            this.lblValFontHeight.Text = "-";
            // 
            // lblValYOffset
            // 
            this.lblValYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValYOffset.AutoSize = true;
            this.lblValYOffset.Location = new System.Drawing.Point(475, 188);
            this.lblValYOffset.Name = "lblValYOffset";
            this.lblValYOffset.Size = new System.Drawing.Size(10, 13);
            this.lblValYOffset.TabIndex = 26;
            this.lblValYOffset.Text = "-";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(375, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 20);
            this.label4.TabIndex = 27;
            this.label4.Text = "Character width:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(375, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 20);
            this.label5.TabIndex = 27;
            this.label5.Text = "Character height:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblYOffset
            // 
            this.lblYOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblYOffset.Location = new System.Drawing.Point(375, 184);
            this.lblYOffset.Name = "lblYOffset";
            this.lblYOffset.Size = new System.Drawing.Size(94, 20);
            this.lblYOffset.TabIndex = 27;
            this.lblYOffset.Text = "Y-offset:";
            this.lblYOffset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numIndex
            // 
            this.numIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numIndex.EnteredValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numIndex.Location = new System.Drawing.Point(475, 15);
            this.numIndex.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numIndex.Name = "numIndex";
            this.numIndex.Size = new System.Drawing.Size(53, 20);
            this.numIndex.TabIndex = 24;
            this.numIndex.ValueChanged += new System.EventHandler(this.numIndex_ValueChanged);
            // 
            // numZoom
            // 
            this.numZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numZoom.EnteredValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoom.Location = new System.Drawing.Point(252, 330);
            this.numZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numZoom.Name = "numZoom";
            this.numZoom.Size = new System.Drawing.Size(120, 20);
            this.numZoom.TabIndex = 4;
            this.numZoom.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numZoom.ValueChanged += new System.EventHandler(this.numZoom_ValueChanged);
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
            this.pnlImageScroll.Location = new System.Drawing.Point(12, 12);
            this.pnlImageScroll.Margin = new System.Windows.Forms.Padding(0);
            this.pnlImageScroll.Name = "pnlImageScroll";
            this.pnlImageScroll.Size = new System.Drawing.Size(360, 307);
            this.pnlImageScroll.TabIndex = 3;
            this.pnlImageScroll.TabStop = true;
            // 
            // pxbEditGridFront
            // 
            this.pxbEditGridFront.BackColor = System.Drawing.Color.Coral;
            this.pxbEditGridFront.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pxbEditGridFront.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbEditGridFront.Location = new System.Drawing.Point(0, 0);
            this.pxbEditGridFront.Margin = new System.Windows.Forms.Padding(0);
            this.pxbEditGridFront.Name = "pxbEditGridFront";
            this.pxbEditGridFront.Size = new System.Drawing.Size(50, 50);
            this.pxbEditGridFront.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pxbEditGridFront.TabIndex = 2;
            this.pxbEditGridFront.TabStop = false;
            this.pxbEditGridFront.Visible = false;
            // 
            // pxbImage
            // 
            this.pxbImage.BackColor = System.Drawing.Color.Transparent;
            this.pxbImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pxbImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbImage.Location = new System.Drawing.Point(0, 0);
            this.pxbImage.Margin = new System.Windows.Forms.Padding(0);
            this.pxbImage.Name = "pxbImage";
            this.pxbImage.Size = new System.Drawing.Size(100, 100);
            this.pxbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pxbImage.TabIndex = 0;
            this.pxbImage.TabStop = false;
            this.pxbImage.Visible = false;
            this.pxbImage.Click += new System.EventHandler(this.picImage_Click);
            // 
            // pxbEditGridBehind
            // 
            this.pxbEditGridBehind.BackColor = System.Drawing.Color.Coral;
            this.pxbEditGridBehind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pxbEditGridBehind.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbEditGridBehind.Location = new System.Drawing.Point(0, 0);
            this.pxbEditGridBehind.Margin = new System.Windows.Forms.Padding(0);
            this.pxbEditGridBehind.Name = "pxbEditGridBehind";
            this.pxbEditGridBehind.Size = new System.Drawing.Size(150, 150);
            this.pxbEditGridBehind.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pxbEditGridBehind.TabIndex = 2;
            this.pxbEditGridBehind.TabStop = false;
            this.pxbEditGridBehind.Visible = false;
            // 
            // pxbFullSize
            // 
            this.pxbFullSize.BackColor = System.Drawing.Color.Maroon;
            this.pxbFullSize.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pxbFullSize.Location = new System.Drawing.Point(0, 0);
            this.pxbFullSize.Margin = new System.Windows.Forms.Padding(0);
            this.pxbFullSize.Name = "pxbFullSize";
            this.pxbFullSize.Size = new System.Drawing.Size(200, 200);
            this.pxbFullSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pxbFullSize.TabIndex = 1;
            this.pxbFullSize.TabStop = false;
            this.pxbFullSize.Visible = false;
            // 
            // chkGrid
            // 
            this.chkGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkGrid.AutoSize = true;
            this.chkGrid.Checked = true;
            this.chkGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGrid.Location = new System.Drawing.Point(13, 331);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(73, 17);
            this.chkGrid.TabIndex = 31;
            this.chkGrid.Text = "Show grid";
            this.chkGrid.UseVisualStyleBackColor = true;
            this.chkGrid.CheckedChanged += new System.EventHandler(this.chkTrans_CheckedChanged);
            // 
            // FrmFontEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 362);
            this.Controls.Add(this.chkGrid);
            this.Controls.Add(this.lblValFilename);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.lblValColors);
            this.Controls.Add(this.lblYOffset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblFontHeight);
            this.Controls.Add(this.lblColors);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblValYOffset);
            this.Controls.Add(this.lblFontWidth);
            this.Controls.Add(this.lblValFontHeight);
            this.Controls.Add(this.lblValFontWidth);
            this.Controls.Add(this.lblValHeight);
            this.Controls.Add(this.lblValWidth);
            this.Controls.Add(this.lblIndex);
            this.Controls.Add(this.numIndex);
            this.Controls.Add(this.lblZoom);
            this.Controls.Add(this.numZoom);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnlImageScroll);
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.MinimumSize = new System.Drawing.Size(700, 300);
            this.Name = "FrmFontEditor";
            this.Text = "N64 IMG Viewer - Created by Nyerguds";
            this.Shown += new System.EventHandler(this.FrmCnC64ImgViewer_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Frm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Frm_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).EndInit();
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

        private RedCell.UI.Controls.PixelBox pxbImage;
        private Nyerguds.Util.UI.SelectablePanel pnlImageScroll;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpen;
        private Nyerguds.Util.UI.EnhNumericUpDown numZoom;
        private System.Windows.Forms.Label lblZoom;
        private Nyerguds.Util.UI.EnhNumericUpDown numIndex;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Label lblValWidth;
        private System.Windows.Forms.Label lblFontWidth;
        private System.Windows.Forms.Label lblFontHeight;
        private System.Windows.Forms.Label lblValHeight;
        private System.Windows.Forms.Label lblColors;
        private System.Windows.Forms.Label lblValColors;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label lblValFilename;
        private System.Windows.Forms.Label lblValFontWidth;
        private System.Windows.Forms.Label lblValFontHeight;
        private System.Windows.Forms.Label lblValYOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblYOffset;
        private RedCell.UI.Controls.PixelBox pxbFullSize;
        private RedCell.UI.Controls.PixelBox pxbEditGridFront;
        private RedCell.UI.Controls.PixelBox pxbEditGridBehind;
        private System.Windows.Forms.CheckBox chkGrid;
    }
}

