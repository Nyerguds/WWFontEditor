using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nyerguds.Util.UI;

namespace WWFontEditor.UI
{
    public partial class FrmReplaceColor : Form
    {
        private const Int32 PALETTE_MAX_DIM = 134;

        public Int32 SelectedIndexSource { get { return palColorSelector1.SelectedIndices[0]; } }
        public Int32 SelectedIndexTarget { get { return palColorSelector2.SelectedIndices[0]; } }

        public FrmReplaceColor()
            :this(4, null)
        { }

        public FrmReplaceColor(Int32 bppValue, Color[] palette)
        {
            InitializeComponent();
            if (palette != null)
            {
                FrmFontEditor.InitPaletteControl(bppValue, this.palColorSelector1, palette, PALETTE_MAX_DIM);
                FrmFontEditor.InitPaletteControl(bppValue, this.palColorSelector2, palette, PALETTE_MAX_DIM);
            }
        }

        private void PalColorSelector1_ColorSelectionChanged(object sender, EventArgs e)
        {
            lblSelectedVal1.Text = SelectedIndexSource.ToString();
        }

        private void palColorSelector2_ColorSelectionChanged(object sender, EventArgs e)
        {
            lblSelectedVal2.Text = SelectedIndexTarget.ToString();
        }
    }
}
