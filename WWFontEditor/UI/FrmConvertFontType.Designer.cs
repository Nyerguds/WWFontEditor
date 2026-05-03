namespace WWFontEditor.UI
{
    partial class FrmConvertFontType
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
            this.cmbTypes = new System.Windows.Forms.ComboBox();
            this.lblNewType = new System.Windows.Forms.Label();
            this.lblTypeInfo = new System.Windows.Forms.Label();
            this.lblNeedsConversionVal = new System.Windows.Forms.Label();
            this.lblNeedsConversion = new System.Windows.Forms.Label();
            this.lblGamesList = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            this.rtbGamesList = new Nyerguds.Util.UI.EnhRichTextBox();
            this.SuspendLayout();
            // 
            // cmbTypes
            // 
            this.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypes.FormattingEnabled = true;
            this.cmbTypes.Location = new System.Drawing.Point(115, 10);
            this.cmbTypes.Name = "cmbTypes";
            this.cmbTypes.Size = new System.Drawing.Size(318, 21);
            this.cmbTypes.TabIndex = 0;
            this.cmbTypes.SelectedIndexChanged += new System.EventHandler(this.cmbTypes_SelectedIndexChanged);
            // 
            // lblNewType
            // 
            this.lblNewType.AutoSize = true;
            this.lblNewType.Location = new System.Drawing.Point(13, 13);
            this.lblNewType.Name = "lblNewType";
            this.lblNewType.Size = new System.Drawing.Size(55, 13);
            this.lblNewType.TabIndex = 1;
            this.lblNewType.Text = "New type:";
            // 
            // lblTypeInfo
            // 
            this.lblTypeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTypeInfo.Location = new System.Drawing.Point(112, 37);
            this.lblTypeInfo.Margin = new System.Windows.Forms.Padding(3);
            this.lblTypeInfo.Name = "lblTypeInfo";
            this.lblTypeInfo.Size = new System.Drawing.Size(321, 70);
            this.lblTypeInfo.TabIndex = 2;
            this.lblTypeInfo.Text = "info\r\ninfo\r\ninfo\r\ninfo\r\ninfo";
            // 
            // lblNeedsConversionVal
            // 
            this.lblNeedsConversionVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNeedsConversionVal.AutoSize = true;
            this.lblNeedsConversionVal.Location = new System.Drawing.Point(112, 113);
            this.lblNeedsConversionVal.Margin = new System.Windows.Forms.Padding(3);
            this.lblNeedsConversionVal.Name = "lblNeedsConversionVal";
            this.lblNeedsConversionVal.Size = new System.Drawing.Size(44, 13);
            this.lblNeedsConversionVal.TabIndex = 3;
            this.lblNeedsConversionVal.Text = "Yes/No";
            // 
            // lblNeedsConversion
            // 
            this.lblNeedsConversion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNeedsConversion.AutoSize = true;
            this.lblNeedsConversion.Location = new System.Drawing.Point(13, 113);
            this.lblNeedsConversion.Name = "lblNeedsConversion";
            this.lblNeedsConversion.Size = new System.Drawing.Size(89, 13);
            this.lblNeedsConversion.TabIndex = 3;
            this.lblNeedsConversion.Text = "Color conversion:";
            // 
            // lblGamesList
            // 
            this.lblGamesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGamesList.AutoSize = true;
            this.lblGamesList.Location = new System.Drawing.Point(439, 12);
            this.lblGamesList.Margin = new System.Windows.Forms.Padding(3);
            this.lblGamesList.Name = "lblGamesList";
            this.lblGamesList.Size = new System.Drawing.Size(58, 13);
            this.lblGamesList.TabIndex = 2;
            this.lblGamesList.Text = "Games list:";
            // 
            // btnConvert
            // 
            this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConvert.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConvert.Location = new System.Drawing.Point(12, 187);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 4;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(93, 187);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblNote
            // 
            this.lblNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNote.Location = new System.Drawing.Point(112, 132);
            this.lblNote.Margin = new System.Windows.Forms.Padding(3);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(321, 49);
            this.lblNote.TabIndex = 6;
            this.lblNote.Text = "Note: you can use the \"Color replace\" button on the main editor to quickly adapt " +
    "the colours of the entire font to match a lower color depth before doing this co" +
    "nversion.";
            this.lblNote.Visible = false;
            // 
            // rtbGamesList
            // 
            this.rtbGamesList.AllowKeyZoom = false;
            this.rtbGamesList.AllowScrollWheelZoom = false;
            this.rtbGamesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbGamesList.BackColor = System.Drawing.SystemColors.Control;
            this.rtbGamesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbGamesList.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbGamesList.Location = new System.Drawing.Point(442, 31);
            this.rtbGamesList.Name = "rtbGamesList";
            this.rtbGamesList.ReadOnly = true;
            this.rtbGamesList.Selectable = false;
            this.rtbGamesList.Size = new System.Drawing.Size(333, 182);
            this.rtbGamesList.TabIndex = 8;
            this.rtbGamesList.Text = "";
            this.rtbGamesList.WordWrap = false;
            // 
            // FrmConvertFontType
            // 
            this.AcceptButton = this.btnConvert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 222);
            this.Controls.Add(this.rtbGamesList);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.lblNeedsConversion);
            this.Controls.Add(this.lblNeedsConversionVal);
            this.Controls.Add(this.lblGamesList);
            this.Controls.Add(this.lblTypeInfo);
            this.Controls.Add(this.lblNewType);
            this.Controls.Add(this.cmbTypes);
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.MinimumSize = new System.Drawing.Size(550, 204);
            this.Name = "FrmConvertFontType";
            this.Text = "Change font type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTypes;
        private System.Windows.Forms.Label lblNewType;
        private System.Windows.Forms.Label lblTypeInfo;
        private System.Windows.Forms.Label lblNeedsConversionVal;
        private System.Windows.Forms.Label lblNeedsConversion;
        private System.Windows.Forms.Label lblGamesList;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblNote;
        private Nyerguds.Util.UI.EnhRichTextBox rtbGamesList;
    }
}