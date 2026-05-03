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
    public partial class FrmConvertToLowerBpp : Form
    {
        protected const String TEXT_SINGLESYM = "The symbol you are trying to insert has a higher bitrate than the target font";
        protected const String TEXT_WHOLEFONT = "The font you are trying to convert has a higher bitrate than the target font type";
        protected const String TEXT_PART2 = ", and contains values that are too high for it.\n\nPlease select a default value for any bytes that are too high.";
        private const Int32 PALETTE_MAX_DIM = 134;

        public Int32 SelectedIndex { get { return palColorSelector.SelectedIndices[0]; } }

        public FrmConvertToLowerBpp()
            :this(true, 4, null)
        { }

        public FrmConvertToLowerBpp(Boolean singleSymbol, Int32 bppValue, Color[] palette)
        {
            InitializeComponent();
            this.lblExplanation.Text = (singleSymbol? TEXT_SINGLESYM : TEXT_WHOLEFONT) + TEXT_PART2;
            if (palette != null)
            {
                PalettePanel.InitPaletteControl(bppValue, this.palColorSelector, palette, PALETTE_MAX_DIM);
            }
        }

        private void palColorSelector_ColorSelectionChanged(object sender, EventArgs e)
        {
            lblSelectedVal.Text = SelectedIndex.ToString();
        }
    }
}
