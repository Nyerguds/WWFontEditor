namespace WWFontEditor.UI
{
    partial class FrmConvertToLowerBpp
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
            this.palColorSelector = new Nyerguds.Util.UI.PalettePanel();
            this.lblSelected = new System.Windows.Forms.Label();
            this.lblSelectedVal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblExplanation
            // 
            this.lblExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExplanation.Location = new System.Drawing.Point(12, 9);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(184, 137);
            this.lblExplanation.TabIndex = 0;
            this.lblExplanation.Text = "-";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(180, 163);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 82;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(261, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 82;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // palColorSelector
            // 
            this.palColorSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.palColorSelector.AutoSize = true;
            this.palColorSelector.ColorTableWidth = 4;
            this.palColorSelector.LabelSize = new System.Drawing.Size(30, 30);
            this.palColorSelector.Location = new System.Drawing.Point(202, 12);
            this.palColorSelector.MaxColors = 16;
            this.palColorSelector.Name = "palColorSelector";
            this.palColorSelector.Padding = new System.Windows.Forms.Padding(0);
            this.palColorSelector.Palette = null;
            this.palColorSelector.Remap = null;
            this.palColorSelector.SelectedIndices = new int[] {
        1};
            this.palColorSelector.ShowRemappedPalette = true;
            this.palColorSelector.Size = new System.Drawing.Size(132, 132);
            this.palColorSelector.TabIndex = 81;
            this.palColorSelector.TransItemBackColor = System.Drawing.Color.Empty;
            this.palColorSelector.ColorSelectionChanged += new System.EventHandler(this.palColorSelector_ColorSelectionChanged);
            // 
            // lblSelected
            // 
            this.lblSelected.AutoSize = true;
            this.lblSelected.Location = new System.Drawing.Point(15, 163);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(52, 13);
            this.lblSelected.TabIndex = 83;
            this.lblSelected.Text = "Selected:";
            // 
            // lblSelectedVal
            // 
            this.lblSelectedVal.AutoSize = true;
            this.lblSelectedVal.Location = new System.Drawing.Point(73, 163);
            this.lblSelectedVal.Name = "lblSelectedVal";
            this.lblSelectedVal.Size = new System.Drawing.Size(13, 13);
            this.lblSelectedVal.TabIndex = 83;
            this.lblSelectedVal.Text = "1";
            // 
            // FrmConvertToLowerBpp
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(348, 198);
            this.Controls.Add(this.lblSelectedVal);
            this.Controls.Add(this.lblSelected);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.palColorSelector);
            this.Controls.Add(this.lblExplanation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.Name = "FrmConvertToLowerBpp";
            this.Text = "Convert to lower bitrate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExplanation;
        private Nyerguds.Util.UI.PalettePanel palColorSelector;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.Label lblSelectedVal;
    }
}