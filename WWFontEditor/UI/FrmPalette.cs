using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ColorManipulation;
using System.Drawing.Imaging;

namespace WWFontEditor.Ui
{
    public partial class FrmPalette : Form
    {
        protected Int32[] m_Customcolors;
        protected Boolean m_ApplyRemap;
        protected String m_Filename;
        protected Boolean m_Editable;
        
        private FrmPalette()
            : this(null, 256, null, false, ColorSelMode.None, null)
        { }
        
        public FrmPalette(Color[] palette, Int32 maxColors, String filename, Boolean editable, ColorSelMode colorSelectMode, Int32[] selectedIndices)
        {
            InitializeComponent();
            Int32 panelInitialHeight = palettePanel.Height;
            Int32 panelInitialWidth = palettePanel.Width;
            Point BtnCloseInitial = btnClose.Location;
            palettePanel.MaxColors = maxColors;
            palettePanel.TableWidth = 8; //(Int32)Math.Sqrt(maxColors); //Math.Min(16, maxColors);
            palettePanel.Palette = palette;
            palettePanel.ColorSelectMode = colorSelectMode;
            palettePanel.SelectedIndices = selectedIndices;
            this.m_Filename = filename;
            this.m_Editable = editable;
            Int32 heightdiff = panelInitialHeight - this.palettePanel.Height;
            Int32 widthDiff = panelInitialWidth - this.palettePanel.Width;
            Int32 actualWidthDiff = this.Width - Math.Max(208, this.Width - widthDiff);
            this.Height -= heightdiff;
            this.Width -=actualWidthDiff;
            btnClose.Location = new Point(BtnCloseInitial.X - actualWidthDiff, BtnCloseInitial.Y - heightdiff);
        }

        public Int32[] SelectedIndices
        {
            get { return palettePanel.SelectedIndices; }
            set { palettePanel.SelectedIndices = value; }
        }

        protected virtual void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void palettePanel_LabelMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!m_Editable || e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            Int32 colindex = (Int32)sender;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = palettePanel.Palette[colindex];
            cdl.FullOpen = true;
            cdl.CustomColors = m_Customcolors;
            DialogResult res = cdl.ShowDialog();
            m_Customcolors = cdl.CustomColors;
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                palettePanel.Palette[colindex] = cdl.Color;
            }
        }
    }
}
