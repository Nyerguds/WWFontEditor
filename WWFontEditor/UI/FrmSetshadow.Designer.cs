namespace WWFontEditor.UI
{
    partial class FrmSetshadow
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtCoords = new System.Windows.Forms.TextBox();
            this.lblCoords = new System.Windows.Forms.Label();
            this.lblColor = new System.Windows.Forms.Label();
            this.lblValShadowColor = new Nyerguds.Util.UI.ImageButtonCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(216, 227);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtCoords
            // 
            this.txtCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoords.Location = new System.Drawing.Point(11, 48);
            this.txtCoords.Multiline = true;
            this.txtCoords.Name = "txtCoords";
            this.txtCoords.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCoords.Size = new System.Drawing.Size(361, 167);
            this.txtCoords.TabIndex = 10;
            this.txtCoords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSelectAll);
            // 
            // lblCoords
            // 
            this.lblCoords.AutoSize = true;
            this.lblCoords.Location = new System.Drawing.Point(12, 32);
            this.lblCoords.Name = "lblCoords";
            this.lblCoords.Size = new System.Drawing.Size(156, 13);
            this.lblCoords.TabIndex = 1;
            this.lblCoords.Text = "Give shadow copy coordinates:";
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(12, 9);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(106, 13);
            this.lblColor.TabIndex = 0;
            this.lblColor.Text = "Select shadow color:";
            // 
            // lblValShadowColor
            // 
            this.lblValShadowColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblValShadowColor.BackColor = System.Drawing.Color.Black;
            this.lblValShadowColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblValShadowColor.Checked = true;
            this.lblValShadowColor.ForeColor = System.Drawing.Color.Black;
            this.lblValShadowColor.Location = new System.Drawing.Point(353, 9);
            this.lblValShadowColor.Margin = new System.Windows.Forms.Padding(3);
            this.lblValShadowColor.Name = "lblValShadowColor";
            this.lblValShadowColor.Size = new System.Drawing.Size(19, 17);
            this.lblValShadowColor.TabIndex = 20;
            this.lblValShadowColor.Toggle = false;
            this.lblValShadowColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ColorLabel_KeyPress);
            this.lblValShadowColor.Click += new System.EventHandler(this.ColorLabel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Format: [1,0] [1,1] [0,1]";
            // 
            // FrmSetshadow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblValShadowColor);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.lblCoords);
            this.Controls.Add(this.txtCoords);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = global::WWFontEditor.Properties.Resources.wwfont;
            this.MinimumSize = new System.Drawing.Size(200, 175);
            this.Name = "FrmSetshadow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shadow drop settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtCoords;
        private System.Windows.Forms.Label lblCoords;
        private System.Windows.Forms.Label lblColor;
        private Nyerguds.Util.UI.ImageButtonCheckBox lblValShadowColor;
        private System.Windows.Forms.Label label1;
    }
}