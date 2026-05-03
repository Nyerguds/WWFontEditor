using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WWFontEditor.Domain;

namespace WWFontEditor.UI
{
    public partial class FrmConvertFontType : Form
    {
        public FontFile SourceFontFile { get; private set; }
        public FontFile TargetFontFile { get; private set; }
        
        public FrmConvertFontType()
        {
            InitializeComponent();
        }

        public FrmConvertFontType(FontFile fontfile)
            : this()
        {
            this.SourceFontFile = fontfile;
            FontFileDialogItem[] fonttypes = FontFile.AutoDetectTypes.Select(x => new FontFileDialogItem(x)).ToArray();
            cmbTypes.DataSource = fonttypes;
            if (SourceFontFile != null)
            {
                FontFileDialogItem fontItem = fonttypes.First(x => x.FontType == SourceFontFile.GetType());
                if (fontItem != null)
                    cmbTypes.SelectedItem = fontItem;
            }
        }

        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            FontFileDialogItem selectedItem = cmbTypes.SelectedItem as FontFileDialogItem;
            if (selectedItem == null)
                return;
            TargetFontFile = selectedItem.FontTypeObject;
            btnConvert.Enabled = selectedItem.FontType != SourceFontFile.GetType();
            lblTypeInfo.Text = TargetFontFile.LongTypeDescription;
            lblGamesList.Text = "Games list:\n- " + String.Join("\n- ", TargetFontFile.GamesListForType).Replace("&", "&&");
            Boolean tooHigh = SourceFontFile.BitsPerPixel > TargetFontFile.BitsPerPixel;
            Boolean tooHighCol = tooHigh;
            if (tooHigh)
            {
                tooHigh = false;
                Int32 colValLimit = (Int32)Math.Pow(2, TargetFontFile.BitsPerPixel);
                foreach (FontFileSymbol ffs in SourceFontFile.GetAllSymbols())
                {
                    if (ffs.ByteData.Any(x => x >= colValLimit))
                    {
                        tooHigh = true;
                        break;
                    }
                }
            }
            lblNeedsConversionVal.Text = (tooHigh ? "Yes" : "No") + (tooHighCol && !tooHigh ? " (no actual color overflow found)" : String.Empty);
            lblNote.Visible = tooHigh;
        }
    }
}
