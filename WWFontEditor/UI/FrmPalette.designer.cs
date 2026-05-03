namespace WWFontEditor.Ui
{
    partial class FrmPalette
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
            this.btnClose = new System.Windows.Forms.Button();
            this.palettePanel = new WWFontEditor.Ui.PalettePanel();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(216, 369);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close window";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // palettePanel
            // 
            this.palettePanel.AutoSize = true;
            this.palettePanel.Border = new System.Windows.Forms.Padding(20);
            this.palettePanel.EmptyIndicatorBackColor = System.Drawing.Color.Black;
            this.palettePanel.EmptyIndicatorChar = 'X';
            this.palettePanel.EmptyIndicatorCharColor = System.Drawing.Color.Red;
            this.palettePanel.LabelSize = new System.Drawing.Size(16, 16);
            this.palettePanel.Location = new System.Drawing.Point(0, 0);
            this.palettePanel.MaxColors = 256;
            this.palettePanel.Name = "palettePanel";
            this.palettePanel.PadBetween = new System.Drawing.Point(4, 4);
            this.palettePanel.Palette = null;
            this.palettePanel.Remap = null;
            this.palettePanel.ColorSelectMode = ColorSelMode.Single;
            this.palettePanel.SelectedIndices = new int[0];
            this.palettePanel.ShowColorToolTips = true;
            this.palettePanel.ShowRemappedPalette = true;
            this.palettePanel.Size = new System.Drawing.Size(356, 356);
            this.palettePanel.TabIndex = 0;
            this.palettePanel.TableWidth = 16;
            this.palettePanel.TransparencyIndicatorBackColor = System.Drawing.Color.Empty;
            this.palettePanel.TransparencyIndicatorChar = 'T';
            this.palettePanel.TransparencyIndicatorCharColor = System.Drawing.Color.Blue;
            this.palettePanel.LabelMouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.palettePanel_LabelMouseDoubleClick);
            // 
            // FrmPalette
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(356, 407);
            this.Controls.Add(this.palettePanel);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmPalette";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Palette";
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Button btnClose;
        protected Ui.PalettePanel palettePanel;

    }
}