using Nyerguds.Util.UI;
using System;
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
            FileDialogItem<FontFile>[] fonttypes = FontFile.SupportedTypes.Select(x => new FileDialogItem<FontFile>(x)).ToArray();
            cmbTypes.DataSource = fonttypes;
            if (SourceFontFile != null)
            {
                FileDialogItem<FontFile> fontItem = fonttypes.First(x => x.ItemType == SourceFontFile.GetType());
                if (fontItem != null)
                    cmbTypes.SelectedItem = fontItem;
            }
        }

        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileDialogItem<FontFile> selectedItem = cmbTypes.SelectedItem as FileDialogItem<FontFile>;
            if (selectedItem == null)
                return;
            TargetFontFile = selectedItem.ItemObject;
            btnConvert.Enabled = selectedItem.ItemType != SourceFontFile.GetType();
            lblTypeInfo.Text = TargetFontFile.LongTypeDescription;
            String games = String.Join(Environment.NewLine + "- ", TargetFontFile.GamesListForType);
            if (!String.IsNullOrEmpty(games))
                games = "- " + games;
            rtbGamesList.Text = games;
            Boolean tooHighCol = SourceFontFile.BitsPerPixel > TargetFontFile.BitsPerPixel;
            Boolean tooHigh = tooHighCol;
            if (tooHighCol)
                tooHigh = SourceFontFile.HasTooHighDataFor(TargetFontFile.BitsPerPixel);
            lblNeedsConversionVal.Text = (tooHigh ? "Yes" : "No") + (tooHighCol && !tooHigh ? " (no actual color overflow found)" : String.Empty);
            lblNote.Visible = tooHigh;
        }
    }
}
