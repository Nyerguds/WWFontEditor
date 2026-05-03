namespace WWFontEditor.UI
{
    partial class FrmReplaceColor
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
            this.lblExplanation = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.palColorSelector1 = new Nyerguds.Util.UI.PalettePanel();
            this.lblSelected = new System.Windows.Forms.Label();
            this.lblSelectedVal1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSelectedVal2 = new System.Windows.Forms.Label();
            this.lblArrow = new System.Windows.Forms.Label();
            this.palColorSelector2 = new Nyerguds.Util.UI.PalettePanel();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblExplanation
            // 
            this.lblExplanation.Location = new System.Drawing.Point(12, 31);
            this.lblExplanation.Margin = new System.Windows.Forms.Padding(3);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(236, 47);
            this.lblExplanation.TabIndex = 0;
            this.lblExplanation.Text = "Select a color to replace by another color. This is a font-wide process that will" +
    " affect all symbols on the font.";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(391, 163);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(472, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // palColorSelector1
            // 
            this.palColorSelector1.AutoSize = true;
            this.palColorSelector1.ColorTableWidth = 4;
            this.palColorSelector1.LabelSize = new System.Drawing.Size(30, 30);
            this.palColorSelector1.Location = new System.Drawing.Point(254, 12);
            this.palColorSelector1.MaxColors = 16;
            this.palColorSelector1.Name = "palColorSelector1";
            this.palColorSelector1.Padding = new System.Windows.Forms.Padding(0);
            this.palColorSelector1.Palette = null;
            this.palColorSelector1.Remap = null;
            this.palColorSelector1.SelectedIndices = new int[] {
        1};
            this.palColorSelector1.ShowRemappedPalette = true;
            this.palColorSelector1.Size = new System.Drawing.Size(132, 132);
            this.palColorSelector1.TabIndex = 81;
            this.palColorSelector1.TabStop = false;
            this.palColorSelector1.TransItemBackColor = System.Drawing.Color.Empty;
            this.palColorSelector1.ColorSelectionChanged += new System.EventHandler(this.PalColorSelector1_ColorSelectionChanged);
            // 
            // lblSelected
            // 
            this.lblSelected.AutoSize = true;
            this.lblSelected.Location = new System.Drawing.Point(12, 84);
            this.lblSelected.Margin = new System.Windows.Forms.Padding(3);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(52, 13);
            this.lblSelected.TabIndex = 83;
            this.lblSelected.Text = "Selected:";
            // 
            // lblSelectedVal1
            // 
            this.lblSelectedVal1.AutoSize = true;
            this.lblSelectedVal1.Location = new System.Drawing.Point(70, 84);
            this.lblSelectedVal1.Margin = new System.Windows.Forms.Padding(3);
            this.lblSelectedVal1.Name = "lblSelectedVal1";
            this.lblSelectedVal1.Size = new System.Drawing.Size(13, 13);
            this.lblSelectedVal1.TabIndex = 83;
            this.lblSelectedVal1.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 84;
            this.label1.Text = "Global color replace:";
            // 
            // lblSelectedVal2
            // 
            this.lblSelectedVal2.AutoSize = true;
            this.lblSelectedVal2.Location = new System.Drawing.Point(122, 84);
            this.lblSelectedVal2.Margin = new System.Windows.Forms.Padding(3);
            this.lblSelectedVal2.Name = "lblSelectedVal2";
            this.lblSelectedVal2.Size = new System.Drawing.Size(13, 13);
            this.lblSelectedVal2.TabIndex = 83;
            this.lblSelectedVal2.Text = "1";
            // 
            // lblArrow
            // 
            this.lblArrow.AutoSize = true;
            this.lblArrow.Location = new System.Drawing.Point(93, 84);
            this.lblArrow.Margin = new System.Windows.Forms.Padding(3);
            this.lblArrow.Name = "lblArrow";
            this.lblArrow.Size = new System.Drawing.Size(19, 13);
            this.lblArrow.TabIndex = 85;
            this.lblArrow.Text = "⇒";
            // 
            // palColorSelector2
            // 
            this.palColorSelector2.AutoSize = true;
            this.palColorSelector2.ColorTableWidth = 4;
            this.palColorSelector2.LabelSize = new System.Drawing.Size(30, 30);
            this.palColorSelector2.Location = new System.Drawing.Point(417, 12);
            this.palColorSelector2.MaxColors = 16;
            this.palColorSelector2.Name = "palColorSelector2";
            this.palColorSelector2.Padding = new System.Windows.Forms.Padding(0);
            this.palColorSelector2.Palette = null;
            this.palColorSelector2.Remap = null;
            this.palColorSelector2.SelectedIndices = new int[] {
        1};
            this.palColorSelector2.ShowRemappedPalette = true;
            this.palColorSelector2.Size = new System.Drawing.Size(132, 132);
            this.palColorSelector2.TabIndex = 81;
            this.palColorSelector2.TabStop = false;
            this.palColorSelector2.TransItemBackColor = System.Drawing.Color.Empty;
            this.palColorSelector2.ColorSelectionChanged += new System.EventHandler(this.palColorSelector2_ColorSelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(392, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 85;
            this.label2.Text = "⇒";
            // 
            // FrmReplaceColor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(559, 198);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblArrow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSelectedVal2);
            this.Controls.Add(this.lblSelectedVal1);
            this.Controls.Add(this.lblSelected);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.palColorSelector2);
            this.Controls.Add(this.palColorSelector1);
            this.Controls.Add(this.lblExplanation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.Name = "FrmReplaceColor";
            this.Text = "Replace Color";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExplanation;
        private Nyerguds.Util.UI.PalettePanel palColorSelector1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.Label lblSelectedVal1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSelectedVal2;
        private System.Windows.Forms.Label lblArrow;
        private Nyerguds.Util.UI.PalettePanel palColorSelector2;
        private System.Windows.Forms.Label label2;
    }
}