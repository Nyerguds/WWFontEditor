using System;
using System.Drawing;
using System.Windows.Forms;
using WWFontEditor.Domain;

namespace WWFontEditor.UI
{
    public partial class FrmSettings : Form
    {
        public Int32[] CustomColors { get; set; }
        private FontEditSettings m_Settings;

        public FrmSettings()
        {
            InitializeComponent();
        }

        public FrmSettings(Int32[] customColors, FontEditSettings fontEditSettings)
            :this()
        {
            this.CustomColors = customColors;
            this.m_Settings = fontEditSettings;
            LoadSettings();
        }

        private void LoadSettings()
        {
            this.SetLabelColor(lblValEditorBackColor, m_Settings.Background);
            this.SetLabelColor(lblValEditorGridColor, m_Settings.BackgroundGrid);
            this.SetLabelColor(lblValEditorOutlineColor, m_Settings.BackgroundFrame);
            this.SetLabelColor(lblValEditAreaOutlineColor, m_Settings.EditAreaFrame);
            this.SetLabelColor(lblValEditAreaGridColor, m_Settings.EditAreaGrid);
            this.chkUsePaletteBg.Checked = this.m_Settings.UsePaletteBG;
            this.numDefaultZoom.Value = this.m_Settings.Zoom;
            this.numDefaultSelectedSymbol.Value = this.m_Settings.SelectedSymbol;
            this.chkEnableGrid.Checked = this.m_Settings.EnableGrid;
            this.chkEnableEditArea.Checked = this.m_Settings.EnableArea;
            this.chkEnablePixelWrap.Checked = this.m_Settings.EnablePixelWrap;
            this.chkPal1BppBR.Checked = this.m_Settings.Generate1BitBR;
            this.chkPal1BppBW.Checked = this.m_Settings.Generate1BitBW;
            this.chkPal1BppWB.Checked = this.m_Settings.Generate1BitWB;
            this.chkPal4BppRainbow.Checked = this.m_Settings.Generate4BitRainbow;
            this.chkPal4BppWin.Checked = this.m_Settings.Generate4BitWindows;
            this.chkPal4BppBW.Checked = this.m_Settings.Generate4BitBW;
            this.chkPal4BppWB.Checked = this.m_Settings.Generate4BitWB;
            this.chkPal8BppRainbow.Checked = this.m_Settings.Generate8BitRainbow;
            this.chkPal8BppWin.Checked = this.m_Settings.Generate8BitWindows;
            this.chkPal8BppBW.Checked = this.m_Settings.Generate8BitBW;
            this.chkPal8BppWB.Checked = this.m_Settings.Generate8BitWB;
        }

        private void SetLabelColor(Label label, Color color)
        {
            label.BackColor = color;
            label.ForeColor = color;
        }

        private void ChkUsePaletteBg_CheckedChanged(object sender, EventArgs e)
        {
            Boolean useBg = this.chkUsePaletteBg.Checked;
            this.lblEditorBackColor.Enabled = !useBg;
            this.lblValEditorBackColor.Enabled = !useBg;
            if (useBg)
                this.lblValEditorBackColor.BackColor = Color.Gray;
            else
                this.lblValEditorBackColor.BackColor = this.lblValEditorBackColor.ForeColor;
        }

        private void ColorLabel_Click(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = label.BackColor;
            cdl.FullOpen = true;
            cdl.CustomColors = this.CustomColors;
            DialogResult res = cdl.ShowDialog();
            this.CustomColors = cdl.CustomColors;
            if (res == DialogResult.OK)
                SetLabelColor(label, cdl.Color);
        }

        private void ColorLabel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ' || e.KeyChar == '\r' || e.KeyChar == '\n')
                ColorLabel_Click(sender, e);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((!this.chkPal1BppBR.Checked && !this.chkPal1BppBW.Checked && !this.chkPal1BppWB.Checked)
                || (!this.chkPal4BppRainbow.Checked && !this.chkPal4BppWin.Checked && !this.chkPal4BppBW.Checked && !this.chkPal4BppWB.Checked)
                || (!this.chkPal8BppRainbow.Checked && !this.chkPal8BppWin.Checked && !this.chkPal8BppBW.Checked && !this.chkPal8BppWB.Checked))
            {
                MessageBox.Show(this, "Error: at least one default palette must be selected for each image color type!", "Font Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.m_Settings.Background       = lblValEditorBackColor.ForeColor;
            this.m_Settings.BackgroundGrid  = lblValEditorGridColor.ForeColor;
            this.m_Settings.BackgroundFrame = lblValEditorOutlineColor.ForeColor;
            this.m_Settings.EditAreaFrame    = lblValEditAreaOutlineColor.ForeColor;
            this.m_Settings.EditAreaGrid     = lblValEditAreaGridColor.ForeColor;
            this.m_Settings.UsePaletteBG = this.chkUsePaletteBg.Checked;
            this.m_Settings.Zoom = (Int32)this.numDefaultZoom.Value;
            this.m_Settings.SelectedSymbol = (Int32)this.numDefaultSelectedSymbol.Value;
            this.m_Settings.EnableGrid = this.chkEnableGrid.Checked;
            this.m_Settings.EnableArea = this.chkEnableEditArea.Checked;
            this.m_Settings.EnablePixelWrap = this.chkEnablePixelWrap.Checked;

            this.m_Settings.Generate1BitBR = this.chkPal1BppBR.Checked;
            this.m_Settings.Generate1BitBW = this.chkPal1BppBW.Checked;
            this.m_Settings.Generate1BitWB = this.chkPal1BppWB.Checked;
            this.m_Settings.Generate4BitRainbow = this.chkPal4BppRainbow.Checked;
            this.m_Settings.Generate4BitWindows = this.chkPal4BppWin.Checked;
            this.m_Settings.Generate4BitBW = this.chkPal4BppBW.Checked;
            this.m_Settings.Generate4BitWB = this.chkPal4BppWB.Checked;
            this.m_Settings.Generate8BitRainbow = this.chkPal8BppRainbow.Checked;
            this.m_Settings.Generate8BitWindows = this.chkPal8BppWin.Checked;
            this.m_Settings.Generate8BitBW = this.chkPal8BppBW.Checked;
            this.m_Settings.Generate8BitWB = this.chkPal8BppWB.Checked;
            this.m_Settings.SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.SetLabelColor(lblValEditorBackColor, FontEditSettings.DefBackground);
            this.SetLabelColor(lblValEditorGridColor, FontEditSettings.DefBackgroundGrid);
            this.SetLabelColor(lblValEditorOutlineColor, FontEditSettings.DefBackgroundFrame);
            this.SetLabelColor(lblValEditAreaOutlineColor, FontEditSettings.DefEditAreaFrame);
            this.SetLabelColor(lblValEditAreaGridColor, FontEditSettings.DefEditAreaGrid);
            this.chkUsePaletteBg.Checked = FontEditSettings.DefUsePaletteBG;
            this.numDefaultZoom.Value = FontEditSettings.DefZoom;
            this.numDefaultSelectedSymbol.Value = FontEditSettings.DefSelectedSymbol;
            this.chkEnableGrid.Checked = FontEditSettings.DefEnableGrid;
            this.chkEnableEditArea.Checked = FontEditSettings.DefEnableArea;
            this.chkEnablePixelWrap.Checked = FontEditSettings.DefEnablePixelWrap;
            this.chkPal1BppBR.Checked = FontEditSettings.DefGenerate1BitBR;
            this.chkPal1BppBW.Checked = FontEditSettings.DefGenerate1BitBW;
            this.chkPal1BppWB.Checked = FontEditSettings.DefGenerate1BitWB;
            this.chkPal4BppRainbow.Checked = FontEditSettings.DefGenerate4BitRainbow;
            this.chkPal4BppWin.Checked = FontEditSettings.DefGenerate4BitWindows;
            this.chkPal4BppBW.Checked = FontEditSettings.DefGenerate4BitBW;
            this.chkPal4BppWB.Checked = FontEditSettings.DefGenerate4BitWB;
            this.chkPal8BppRainbow.Checked = FontEditSettings.DefGenerate8BitRainbow;
            this.chkPal8BppWin.Checked = FontEditSettings.DefGenerate8BitWindows;
            this.chkPal8BppBW.Checked = FontEditSettings.DefGenerate8BitBW;
            this.chkPal8BppWB.Checked = FontEditSettings.DefGenerate8BitWB;
        }
    }
}
