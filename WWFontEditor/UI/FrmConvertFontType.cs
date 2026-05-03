using Nyerguds.Util.UI;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WWFontEditor.Domain;
using WWFontEditor.Domain.FontTypes;

namespace WWFontEditor.UI
{
    public partial class FrmConvertFontType : Form
    {
        public FontFile SourceFontFile { get; private set; }
        public FontFile TargetFontFile { get; private set; }
        
        private FrmConvertFontType(Boolean asCreateNew)
        {
            InitializeComponent();
        }

        public FrmConvertFontType(FontFile fontfile, Boolean asCreateNew)
            : this(false)
        {
            this.SourceFontFile = fontfile;
            if (asCreateNew)
            {
                lblNeedsConversion.Visible = false;
                lblNeedsConversionVal.Visible = false;
                lblNote.Visible = false;
                lblTypeInfo.Height = lblNote.Location.Y - lblTypeInfo.Location.Y + lblNote.Height;
                this.Text = "Create new font";
                this.Height = this.MinimumSize.Height;
                btnConvert.Text = "Create";
            }
            FileDialogItem<FontFile>[] fonttypes = FontFile.SupportedTypes.Select(x => new FileDialogItem<FontFile>(x)).ToArray();
            cmbTypes.DataSource = fonttypes;
            if (SourceFontFile != null)
            {
                FileDialogItem<FontFile> fontItem = fonttypes.FirstOrDefault(x => x.ItemType == SourceFontFile.GetType());
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
