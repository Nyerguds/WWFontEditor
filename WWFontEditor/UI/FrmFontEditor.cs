using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;
using Nyerguds.Util.UI;
using Nyerguds.Util.Ui.SaveOptions;
using WWFontEditor.Domain;
using WWFontEditor.Domain.FontTypes;
using Nyerguds.Util.UI.Wrappers;

namespace WWFontEditor.UI
{
    public partial class FrmFontEditor : Form
    {
        private const Int32 PALETTE_MAX_DIM = 162;
        private const String PROG_NAME = "Westwood Font Editor";
        private const String PROG_AUTHOR = "Created by Nyerguds";
        private const String ASCII_CONVERT = " ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼";
        private const String QUESTION_RESETFONT = "This will remove all changes you have made to the font since it was loaded!\n\nAre you sure you want to continue?";
        private const String QUESTION_REVERTSYMBOL = "This will revert the current edits on this\nsymbol image to their original state!\n\nAre you sure you want to continue?";
        private const String QUESTION_SAVEFILE_OPENNEW = "The font has unsaved changes!\n\nDo you want to save the changes to the current font?";
        private const String QUESTION_SAVEFILE_CLOSE = "The font has unsaved changes!\n\nDo you want to save the changes to the font?";
        private const String ABOUTTEXT = "Program icon created by Tomsons26\n\nFont format research by Nyerguds, assisted by Omniblade, CCHyper and Tomsons26\n\nPalette manager design assisted by Moon Flower";
        private const String NEWFONTNAME = "newfont.";
        private const String UNHANDLED_EXCEPTION_MESSAGE = "Unhandled exception. Please copy this message to the clipboard using Ctrl+C and send it to the development team.\n\n";

        private Boolean m_Loading;
        private Boolean m_Clicking;
        private Int32 m_GridRowTemplateHeight;
        private Boolean m_TempColActive;
        private Boolean m_TempColPickerSelected;
        private String m_FileName;
        private FontFile m_LoadedFont;
        private FontFile m_LoadedFontBackup;
        private Int32 m_CurHeight;
        private Int32 m_CurWidth;
        private Int32 m_CurYOffset;
        private Int32 m_LastHoverPixelX = -1;
        private Int32 m_LastHoverPixelY = -1;
        private ContextMenuStrip m_tsmiCopyGridChar;

        private Byte m_CurrentPaintColor1 = 1;
        private Byte m_CurrentPaintColor2 = 0;
        private List<PaletteDropDownInfo> m_DefaultPalettes;
        private List<PaletteDropDownInfo> m_ReadPalettes;
        private Color[] m_CurrentPalette;

        private Int32[] m_CustomColors;

        private FontEditSettings m_Settings;

        public FrmFontEditor(String[] args)
            : this()
        {
            if (args.Length > 0 && File.Exists(args[0]))
                this.m_FileName = args[0];
        }

        public FrmFontEditor()
        {
            this.m_Loading = true;
            this.InitializeComponent();
            this.m_GridRowTemplateHeight = this.dgrvSymbolsList.RowTemplate.Height;
            // Load settings
            this.m_Settings = new FontEditSettings();
            this.numZoom.EnteredValue = this.m_Settings.Zoom;
            this.chkGrid.Checked = this.m_Settings.EnableGrid;
            this.chkOutline.Checked = this.m_Settings.EnableArea;
            this.chkShiftWrap.Checked = this.m_Settings.EnablePixelWrap;
            this.chkWrapPreview.Checked = this.m_Settings.EnablePreviewWrap;

            // encodings init
            List<EncodingDropDownInfo> encodings = Encoding.GetEncodings() // Get all known .Net encodings
                .Select(e => e.GetEncoding()) // From EncodingInfo to Encoding
                .Where(e => e.IsSingleByte && TextUtils.IsAsciiCompatible(e)) // Filter out single byte ASCII-compatible ones
                .Select(e => new EncodingDropDownInfo(e)) // Put in wrapper class to add a ToString() for the dropdown
                .OrderBy(n => n.ToString()) // Order by name as returned by wrapper class (with extra info first)
                .ToList();
            // Add standard Dune 2000 text encoding
            encodings.Add(new EncodingDropDownInfo(new D2KEncoding()));
            // Add custom added Dune 2000 text encodings
            List<D2KEncoding> d2kEncodings = this.ScanForD2KEncodings();
            encodings.AddRange(d2kEncodings.Select(e => new EncodingDropDownInfo(e)));

            this.cmbEncodings.DataSource = encodings;
            // Select DOS-437 encoding, the one all original C&C fonts are based on.
            this.cmbEncodings.SelectedItem = encodings.Find(e => e.Encoding.CodePage == 437);

            // Colors init.
            this.m_DefaultPalettes = this.LoadDefaultPalettes();
            // Default to show on UI at startup: 4bpp palettes
            List<PaletteDropDownInfo> allPalettesForBpp = this.GetPalettes(4, true);
            if (allPalettesForBpp.Count == 0)
                allPalettesForBpp.Add(new PaletteDropDownInfo("Rainbow", 4, GetDummyPalette(), null, -1, false, false));
            this.cmbPalettes.DataSource = allPalettesForBpp;

            // PixelBox hierarchy init
            this.pxbEditGridBehind.Parent = this.pxbFullSize;
            this.pxbEditGridBehind.BackColor = Color.Transparent;
            this.pxbEditGridBehind.Location = new Point(0, 0);
            this.pxbImage.Parent = this.pxbFullSize;
            this.pxbImage.BackColor = Color.Transparent;
            this.pxbImage.Location = new Point(0, 0);
            this.pxbImage.BringToFront();
            this.pxbEditGridFront.Parent = this.pxbImage;
            this.pxbEditGridFront.BackColor = Color.Transparent;
            this.pxbEditGridFront.Location = new Point(0, 0);

            // Set paint colors
            this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor1]);
            this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor2]);

            // Add right click menu to preview panel
            ContextMenu cmCopyPreview = new ContextMenu();
            MenuItem mniCopy = new MenuItem("Copy");
            mniCopy.Click += this.CopyPreview;
            cmCopyPreview.MenuItems.Add(mniCopy);
            // doesn't work; clipboard itself doesn't support transparency.
            MenuItem mniCopyTrans = new MenuItem("Copy (transparent background)");
            mniCopyTrans.Click += this.CopyPreviewTrans;
            cmCopyPreview.MenuItems.Add(mniCopyTrans);
            //cmCopyPreview.MenuItems.Add(mniCopyTrans);
            this.pnlImagePreview.ContextMenu = cmCopyPreview;

            // Create right-click menu for toolstrip items
            this.m_tsmiCopyGridChar = new ContextMenuStrip();
            ToolStripMenuItem mniCopyChar = new ToolStripMenuItem("Copy", null, this.TsmiCopySymbol_Click);
            this.m_tsmiCopyGridChar.Items.Add(mniCopyChar);

            // Set title
            this.Text = GetTitle(true);
            this.m_Loading = false;
        }

        public static String GetTitle(Boolean withAuthor)
        {
            String title = PROG_NAME + " " + GeneralUtils.ProgramVersion();
            if (withAuthor)
                title += " - " + PROG_AUTHOR;
            return title;
        }

        private List<D2KEncoding> ScanForD2KEncodings()
        {
            Regex codePageRegex = new Regex("^FONT_?(\\d+).*?\\.BIN$");
            String appFolder = Path.GetDirectoryName(Application.ExecutablePath);
            FileInfo[] files = new DirectoryInfo(appFolder).GetFiles("FONT*.BIN");
            List<D2KEncoding> d2kEncodings = new List<D2KEncoding>();
            foreach (FileInfo file in files)
            {
                try
                {
                    if (file.Length != 0x100)
                        continue;
                    Encoding enc = null;
                    Match m = codePageRegex.Match(file.Name);
                    if (m.Success)
                    {
                        Int32 codepage = Int32.Parse(m.Groups[1].Value);
                        try { enc = Encoding.GetEncoding(codepage); }
                        catch { /* ignore */ }
                        if (enc != null && (!enc.IsSingleByte || !TextUtils.IsAsciiCompatible(enc)))
                            enc = null;
                    }
                    Byte[] remapTable = File.ReadAllBytes(file.FullName);
                    d2kEncodings.Add(new D2KEncoding(remapTable, file.Name + " (D2K Encoding)", enc));
                }
                catch (Exception e)
                {
                    // Should normally never happen: all necessary checks are done in advance.
                    MessageBox.Show(this, String.Format("Loading of file \"{0}\" as Dune 2000 text encoding failed:\n\n{1}", file.Name, e.Message), GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return d2kEncodings;
        }

        private void Frm_DragEnter(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Frm_DragDrop(Object sender, DragEventArgs e)
        {
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length != 1)
                return;
            String path = files[0];
            //String ext = Path.GetExtension(path).TrimStart('.');
            //List<String> supportedExtensions = FontFile.GetSupportedExtensions();
            //if (!supportedExtensions.Any(x => x.Equals(ext, StringComparison.InvariantCultureIgnoreCase))) return;
            if (this.AbortForChangesAskSave(QUESTION_SAVEFILE_OPENNEW))
                return;
            this.LoadFontFile(path, null);
        }

        private void LoadFontFile(String path, FontFile fontFile)
        {
            this.m_Loading = true;
            try
            {
                String error = null;
                Byte[] data = null;
                try
                {
                    data = File.ReadAllBytes(path);
                }
                catch (IOException ex)
                {
                    error = ex.Message;
                }
                catch (Exception ex)
                {
                    error = UNHANDLED_EXCEPTION_MESSAGE + ex.GetType() + ": " + ex.Message + "\n\n" + ex.StackTrace;
                }
                if (error == null && data != null)
                {
                    if (fontFile != null)
                    {
                        try { fontFile.LoadFont(data); }
                        catch (FileTypeLoadException ftle) { error = ftle.Message; }
                        catch (Exception ex) { error = UNHANDLED_EXCEPTION_MESSAGE + ex.GetType() + ": " + ex.Message + "\n\n" + ex.StackTrace; }
                        if (error != null)
                            error = "Could not load font file as " + fontFile.ShortTypeDescription + ":\n\n" + error;
                    }
                    else
                    {
                        try
                        {
                            List<FileTypeLoadException> loadErrors;
                            fontFile = FontFile.LoadFontFile(path, data, out loadErrors);
                            if (fontFile == null)
                                error = "Font type could not be identified. Errors returned by all attempts:\n\n" + String.Join("\n", loadErrors.Select(er => er.AttemptedLoadedType + ": " + er.Message).ToArray());
                        }
                        catch (Exception ex)
                        {
                            error = UNHANDLED_EXCEPTION_MESSAGE + ex.GetType() + ": " + ex.Message + "\n\n" + ex.StackTrace;
                        }
                    }
                }
                if (error != null)
                {
                    MessageBox.Show(this, "Font loading failed: " + error, GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.m_LoadedFont = fontFile;
                this.m_FileName = path;
                this.FinishLoading(false);
            }
            finally
            {
                this.m_Loading = false;
            }
        }

        private void FinishLoading(Boolean isNew)
        {
            this.m_LoadedFontBackup = this.m_LoadedFont == null || isNew ? null : this.m_LoadedFont.Clone();
            if (this.m_LoadedFont != null && this.m_LoadedFont.BitsPerPixel > this.GetEditBpp(this.m_LoadedFont))
                this.AdjustFontSymbolsBpp(this.m_LoadedFont);
            Boolean loadOk = this.ReloadUi(true);
            if (!loadOk)
                MessageBox.Show(this, "Font loading failed!", GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private Boolean ReloadUi(Boolean newFontLoaded)
        {
            Boolean wasloading = this.m_Loading;
            this.m_Loading = true;
            Boolean loadOk = this.m_LoadedFont != null;
            this.btnValType.Enabled = loadOk;
            this.numSymbols.Enabled = loadOk && Math.Max(this.m_LoadedFont.SymbolsTypeMin, this.m_LoadedFont.SymbolsTypeFirst) < this.m_LoadedFont.SymbolsTypeMax;
            this.numFontWidth.Enabled = loadOk && this.m_LoadedFont.FontWidthTypeMin < this.m_LoadedFont.FontWidthTypeMax;
            this.numFontHeight.Enabled = loadOk && this.m_LoadedFont.FontHeightTypeMin < this.m_LoadedFont.FontHeightTypeMax;
            this.numWidth.Enabled = loadOk && this.m_LoadedFont.CustomSymbolWidthsForType;
            this.numHeight.Enabled = loadOk && this.m_LoadedFont.CustomSymbolHeightsForType;
            this.numYOffset.Enabled = loadOk && this.m_LoadedFont.YOffsetTypeMax > 0;
            this.btnShiftUp.Enabled = loadOk;
            this.btnShiftLeft.Enabled = loadOk;
            this.btnShiftRight.Enabled = loadOk;
            this.btnShiftDown.Enabled = loadOk;
            this.btnCopy.Enabled = loadOk;
            this.tsmiCopySymbol.Enabled = loadOk;
            this.btnPaste.Enabled = loadOk;
            this.btnRemap.Enabled = loadOk;
            this.tsmiPasteSymbol.Enabled = loadOk;
            this.tsmiPasteSymbolTrans.Enabled = loadOk;
            this.tsmiSaveFont.Enabled = loadOk;
            this.tsmiSaveFontAs.Enabled = loadOk;
            this.tsmiRevertFont.Enabled = loadOk;
            this.pxbFullSize.Visible = loadOk;
            if (loadOk)
            {
                this.RefreshPalettes(newFontLoaded, false);
                String filename = this.m_FileName == null ? NEWFONTNAME + (this.m_LoadedFont.FileExtensions.FirstOrDefault() ?? "fnt") : Path.GetFileName(this.m_FileName);
                this.Text = String.Format("{0} - \"{1}\" ({2})", GetTitle(true), filename, this.m_LoadedFont.ShortTypeName);
                this.btnValType.Text = this.m_LoadedFont.ShortTypeName.Replace("&", "&&");
                this.toolTip1.SetToolTip(this.btnValType, this.m_LoadedFont.ShortTypeDescription);
                this.numSymbols.Minimum = Math.Max(this.m_LoadedFont.SymbolsTypeMin, this.m_LoadedFont.SymbolsTypeFirst);
                this.numSymbols.Maximum = this.m_LoadedFont.SymbolsTypeMax;
                this.numSymbols.Value = this.m_LoadedFont.Length;
                this.numFontHeight.Minimum = this.m_LoadedFont.FontHeightTypeMin;
                this.numFontHeight.Maximum = this.m_LoadedFont.FontHeightTypeMax;
                this.numFontHeight.Value = this.m_LoadedFont.FontHeight;
                this.numFontWidth.Minimum = this.m_LoadedFont.FontWidthTypeMin;
                this.numFontWidth.Maximum = this.m_LoadedFont.FontWidthTypeMax;
                this.numFontWidth.Value = this.m_LoadedFont.FontWidth;
                this.numYOffset.Maximum = this.m_LoadedFont.YOffsetTypeMax;
                this.numWidth.Maximum = this.m_LoadedFont.FontWidthTypeMax;
                this.numHeight.Maximum = this.m_LoadedFont.FontHeightTypeMax;
                this.tsmiOptimizeWidths.Enabled = this.m_LoadedFont.CustomSymbolWidthsForType;
            }
            else
            {
                this.m_FileName = null;
                this.Text = GetTitle(true);
                this.btnValType.Text = "-";
                this.toolTip1.SetToolTip(this.btnValType, null);
                this.numSymbols.Maximum = 0;
                this.numSymbols.Value = 0;
                this.numFontHeight.Maximum = 0;
                this.numFontHeight.Value = 0;
                this.numFontWidth.Maximum = 0;
                this.numFontWidth.Value = 0;
                this.numWidth.Maximum = 0;
                this.numWidth.Value = 0;
                this.numHeight.Maximum = 0;
                this.numHeight.Value = 0;
                this.numYOffset.Maximum = 0;
                this.numYOffset.Value = 0;
                this.tsmiOptimizeWidths.Enabled = false;
            }
            this.ReloadImageInfo(true);
            this.ReloadDataGrid(newFontLoaded);
            if (loadOk)
            {
                // to allow index changed events on the following piece
                this.m_Loading = false;
                if (newFontLoaded)
                {
                    Int32 firstSelected = Math.Max(this.m_LoadedFont.SymbolsTypeFirst, this.m_Settings.SelectedSymbol);
                    if (this.m_LoadedFont.Length <= firstSelected)
                        firstSelected = Math.Max(this.m_LoadedFont.SymbolsTypeFirst, 0);
                    if (this.m_LoadedFont.Length > firstSelected)
                    {
                        Int32 newIndex = firstSelected - this.m_LoadedFont.SymbolsTypeFirst;
                        this.dgrvSymbolsList.FirstDisplayedCell = this.dgrvSymbolsList.Rows[newIndex].Cells[0];
                        this.dgrvSymbolsList.FirstDisplayedCell.Selected = true;
                        this.dgrvSymbolsList.Focus();
                    }
                    this.ReloadImageInfo(true);
                }
            }
            this.m_Loading = wasloading;
            return loadOk;
        }

        private void RefreshPalettes(Boolean forced, Boolean reloadFiles)
        {
            Int32 oldBpp = -1;
            PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if (currentPal != null)
                oldBpp = currentPal.BitsPerPixel;
            Int32 bpp = this.GetEditBpp(this.m_LoadedFont);
            if (oldBpp == -1 || oldBpp != bpp || forced)
            {
                Int32 index = -1;
                this.m_CurrentPaintColor1 = 1;
                this.m_CurrentPaintColor2 = 0;
                List<PaletteDropDownInfo> bppPalettes = this.GetPalettes(bpp, reloadFiles);
                if (forced && oldBpp != -1 && oldBpp == bpp && currentPal != null)
                    index = bppPalettes.FindIndex(x => x.Name == currentPal.Name);
                if (bppPalettes.Count == 0)
                    bppPalettes.Add(new PaletteDropDownInfo("Rainbow", bpp, GetDummyPalette(), null, -1, false, false));
                this.cmbPalettes.DataSource = bppPalettes;
                if (index >= 0)
                    this.cmbPalettes.SelectedIndex = index;
            }
        }

        private Int32 GetEditBpp(FontFile font)
        {
            if (font == null)
                return 4;
            Int32 bpp = font.BitsPerPixel;
            if (bpp != 8 || !this.m_Settings.Limit8BitPalettes)
                return bpp;
            return 4;
        }

        private void AdjustFontSymbolsBpp(FontFile fontFile)
        {
            if (fontFile == null)
                return;
            FontFileSymbol[] symbols = fontFile.GetAllSymbols();
            foreach (FontFileSymbol symbol in symbols)
                symbol.ConvertToBpp(0, this.GetEditBpp(fontFile));
        }

        public static Color[] GetDummyPalette()
        {
            return PaletteUtils.GenerateRainbowPalette(4, 0, null, false);
        }

        public List<PaletteDropDownInfo> LoadDefaultPalettes()
        {
            List<PaletteDropDownInfo> palettes = new List<PaletteDropDownInfo>();
            // 1-bit:
            // Not gonna make those customizable. These three ought to do. People can always change the palette to view them in different colours.
            if (this.m_Settings.Generate1BitBR)
                palettes.Add(new PaletteDropDownInfo("Black-Red", 1, new Color[] { Color.FromArgb(0x00, Color.Black), Color.Red }, null, -1, false, false));
            if (this.m_Settings.Generate1BitBW)
                palettes.Add(new PaletteDropDownInfo("Black-White", 1, new Color[] { Color.FromArgb(0x00, Color.Black), Color.White }, null, -1, false, false));
            if (this.m_Settings.Generate1BitWB)
                palettes.Add(new PaletteDropDownInfo("White-Black", 1, new Color[] { Color.FromArgb(0x00, Color.White), Color.Black }, null, -1, false, false));
            // 4-bit and 8-bit
            if (this.m_Settings.Generate4BitRainbow)
                //palettes.Add(new PaletteDropDownInfo("Rainbow", 4, PaletteRainbow, null, -1));
                palettes.Add(new PaletteDropDownInfo("Rainbow", 4, PaletteUtils.GenerateRainbowPalette(4, 0, null, false), null, -1, false, false));
            if (this.m_Settings.Generate4BitWindows)
                palettes.Add(new PaletteDropDownInfo("Windows palette", 4, PaletteUtils.GenerateDefWindowsPalette(4, null, false), null, -1, false, false));
            if (this.m_Settings.Generate4BitBW)
                palettes.Add(new PaletteDropDownInfo("Grayscale B->W", 4, PaletteUtils.GenerateGrayPalette(4, null, false), null, -1, false, false));
            if (this.m_Settings.Generate4BitWB)
                palettes.Add(new PaletteDropDownInfo("Grayscale W->B", 4, PaletteUtils.GenerateGrayPalette(4, null, true), null, -1, false, false));
            if (this.m_Settings.Generate8BitRainbow)
                palettes.Add(new PaletteDropDownInfo("Rainbow", 8, PaletteUtils.GenerateDoubleRainbow(0, null, false), null, -1, false, false));
            if (this.m_Settings.Generate8BitWindows)
                palettes.Add(new PaletteDropDownInfo("Windows palette", 8, PaletteUtils.GenerateDefWindowsPalette(8, null, false), null, -1, false, false));
            if (this.m_Settings.Generate8BitBW)
                palettes.Add(new PaletteDropDownInfo("Grayscale B->W", 8, PaletteUtils.GenerateGrayPalette(8, null, false), null, -1, false, false));
            if (this.m_Settings.Generate8BitWB)
                palettes.Add(new PaletteDropDownInfo("Grayscale W->B", 8, PaletteUtils.GenerateGrayPalette(8, null, true), null, -1, false, false));
            return palettes;
        }

        public List<PaletteDropDownInfo> LoadExtraPalettes()
        {
            List<PaletteDropDownInfo> palettes = new List<PaletteDropDownInfo>();
            String appFolder = Path.GetDirectoryName(Application.ExecutablePath);
            FileInfo[] files = new DirectoryInfo(appFolder).GetFiles("*.pal");
            Array.Sort(files, (x,y) => String.Compare(x.Name,y.Name, StringComparison.InvariantCultureIgnoreCase));
            foreach (FileInfo file in files)
                palettes.AddRange(PaletteDropDownInfo.LoadSubPalettesInfoFromPalette(file, false, false, true));
            return palettes;
        }

        public List<PaletteDropDownInfo> GetPalettes(Int32 bpp, Boolean reloadFiles)
        {
            List<PaletteDropDownInfo> allPalettes = this.m_DefaultPalettes.Where(p => p.BitsPerPixel == bpp).ToList();
            if (reloadFiles)
                this.m_ReadPalettes = this.LoadExtraPalettes();
            allPalettes.AddRange(this.m_ReadPalettes.Where(p => p.BitsPerPixel == bpp));
            return allPalettes;
        }
        
        private void ReloadDataGrid(Boolean ignoreScroll)
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            DataTable oldSymbolsTable = this.dgrvSymbolsList.DataSource as DataTable;
            try
            {
                if (this.m_LoadedFont == null)
                {
                    this.dgrvSymbolsList.DataSource = null;
                    return;
                }
                // add as param later
                Encoding enc = ((EncodingDropDownInfo) this.cmbEncodings.SelectedItem).Encoding;
                Color[] palette = this.m_CurrentPalette.ToArray();
                palette[this.m_LoadedFont.TransparencyColor] = Color.FromArgb(0xFF, palette[this.m_LoadedFont.TransparencyColor]);
                Int32 selectedIndex = -1;
                Int32 scrollOffset = -1;
                if (this.dgrvSymbolsList.Rows.Count > 0 && this.dgrvSymbolsList.CurrentCell != null)
                {
                    selectedIndex = this.dgrvSymbolsList.CurrentCell.RowIndex;
                    if (!ignoreScroll)
                        scrollOffset = this.dgrvSymbolsList.VerticalScrollbarOffset;
                }
                DataTable symbolsTable = new DataTable("Symbols");
                symbolsTable.Columns.Add(new DataColumn("Hex", typeof(String)));
                symbolsTable.Columns.Add(new DataColumn("Dec", typeof(Int32)));
                symbolsTable.Columns.Add(new DataColumn("Char", typeof(String)));
                symbolsTable.Columns.Add(new DataColumn("Pic", typeof(Bitmap)));
                //NullValue 

                FontFileSymbol[] allSymbols = this.m_LoadedFont.GetAllSymbols();
                for (Int32 i = this.m_LoadedFont.SymbolsTypeFirst; i < allSymbols.Length; i++)
                {
                    FontFileSymbol symbol = allSymbols[i];
                    DataRow row = symbolsTable.NewRow();
                    row[0] = "0x" + i.ToString("X2");
                    row[1] = i;
                    row[2] = i < 0x20 ? ASCII_CONVERT[i].ToString() : enc.GetString(new Byte[] { (Byte)i });
                    Bitmap bm = symbol.GetBitmapFullSize(palette, this.m_LoadedFont, false);
                    row[3] = bm;
                    symbolsTable.Rows.Add(row);
                }
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_LoadedFont.TransparencyColor]);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                style.NullValue = null; // ensures empty images will simply be shown empty
                this.dgrvSymbolsList.RowTemplate.Height = Math.Max(this.m_GridRowTemplateHeight, this.m_LoadedFont.FontHeight);
                this.dgrvSymbolsList.DataSource = symbolsTable;
                this.dgrvSymbolsList.Columns[3].DefaultCellStyle = style;
                if (selectedIndex < symbolsTable.Rows.Count && selectedIndex >= 0)
                {
                    this.dgrvSymbolsList.Rows[selectedIndex].Cells[0].Selected = true;
                    if (scrollOffset >= 0)
                        this.dgrvSymbolsList.VerticalScrollbarOffset = Math.Max(0, scrollOffset);
                }
            }
            finally
            {
                // Cleanup
                if (oldSymbolsTable != null && oldSymbolsTable.Columns.Count >= 4)
                {
                    foreach (DataRow row in oldSymbolsTable.Rows)
                    {
                        Image row3 = row[3] as Image;
                        if (row3 == null)
                            continue;
                        row[3] = null;
                        try { row3.Dispose(); }
                        catch { /* ignore */ }
                    }
                }
                this.m_Loading = wasLoading;
            }
        }

        private void RefreshCurrentGridImage()
        {
            if (!(this.dgrvSymbolsList.DataSource is DataTable))
                return;
            if (this.dgrvSymbolsList.SelectedRows.Count == 0)
                return;
            DataGridViewRow selRow = this.dgrvSymbolsList.SelectedRows[0];

            Int32 index = this.GetSelectedIndex();
            FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(index);
            Color[] palette = this.m_CurrentPalette.ToArray();
            palette[this.m_LoadedFont.TransparencyColor] = Color.FromArgb(0xFF, palette[this.m_LoadedFont.TransparencyColor]);
            Image bmOld = selRow.Cells[3].Value as Image;
            Bitmap bm = symbol.GetBitmapFullSize(palette, this.m_LoadedFont, false);
            selRow.Cells[3].Value = bm;
            if (bmOld != null)
            {
                try { bmOld.Dispose(); }
                catch { /* ignore */ }
            }
        }

        private void SaveFontFile()
        {
            if (!this.m_LoadedFont.CanSave || this.m_FileName == null)
                this.SaveFontAs();
            else
                this.SaveFontFile(this.m_FileName);
        }

        private void SaveFontFile(String fileName)
        {
            if (this.m_LoadedFont == null)
                return;
            try
            {
                SaveOption[] saveOptions = this.m_LoadedFont.GetSaveOptions(fileName);
                if (saveOptions != null && saveOptions.Length > 0)
                {
                    SaveOptionInfo soi = new SaveOptionInfo();
                    soi.Name = "Extra save options for " + this.m_LoadedFont.ShortTypeDescription;
                    soi.Properties = saveOptions;
                    FrmExtraOptions extraopts = new FrmExtraOptions(GetTitle(false));
                    extraopts.Init(soi);
                    if (extraopts.ShowDialog(this) != DialogResult.OK)
                        return;
                    saveOptions = extraopts.GetSaveOptions();
                }
                Byte[] filedata = this.m_LoadedFont.SaveFont(saveOptions);
                File.WriteAllBytes(fileName, filedata);
                this.m_LoadedFontBackup = this.m_LoadedFont.Clone();
                this.m_FileName = fileName;
                this.Text = GetTitle(true) + " - \"" + Path.GetFileName(this.m_FileName) + "\" (" + this.m_LoadedFont.ShortTypeName + ")";
                this.tsmiRevertSymbol.Enabled = false;
                this.ReloadUIWithSelection(false);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Error occurred when saving:\n\n" + e.Message, GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FrmFontEditor_Shown(Object sender, EventArgs e)
        {
            if (this.m_FileName != null)
                this.LoadFontFile(this.m_FileName, null);
        }

        private void ReloadImageInfo(Boolean refreshEditor)
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            Image oldImg = this.pxbImage.Image;
            try
            {
                Int32 curIndex = this.GetSelectedIndex();
                if (this.m_LoadedFont == null)
                {
                    this.pxbImage.Image = null;
                    this.m_CurHeight = 0;
                    this.m_CurWidth = 0;
                    this.m_CurYOffset = 0;
                    this.numHeight.Maximum = 0;
                    this.numHeight.Value = 0;
                    this.numWidth.Maximum = 0;
                    this.numWidth.Value = 0;
                    this.numYOffset.Value = 0;
                    this.RepaintPreview();
                    return;
                }
                this.pxbImage.Image = this.m_LoadedFont.GetBitmap(curIndex, this.m_CurrentPalette, true);
                this.m_CurHeight = this.m_LoadedFont.GetSymbolHeight(curIndex);
                this.m_CurWidth = this.m_LoadedFont.GetSymbolWidth(curIndex);
                this.m_CurYOffset = this.m_LoadedFont.GetSymbolYOffset(curIndex);
                this.numHeight.Maximum = this.m_LoadedFont.FontHeight;
                this.numHeight.Value = this.m_CurHeight;
                this.numWidth.Maximum = this.m_LoadedFont.FontWidth;
                this.numWidth.Value = this.m_CurWidth;
                this.numYOffset.Value = this.m_CurYOffset;
                this.AdjustRevertButton();
                this.RepaintPreview();
                if (refreshEditor)
                    this.RefreshEditor();
            }
            finally
            {
                //Cleanup
                if (oldImg != null && !ReferenceEquals(this.pxbImage.Image, oldImg))
                {
                    try { oldImg.Dispose(); }
                    catch { /*ignore*/ }
                }
                this.m_Loading = wasLoading;
            }
        }

        private Int32 GetSelectedIndex()
        {
            Int32 selectedIndex = 0;
            if (this.dgrvSymbolsList.SelectedRows.Count > 0)
                selectedIndex = (Int32)this.dgrvSymbolsList.SelectedRows[0].Cells[1].Value;
            return selectedIndex;
        }
        
        private void PnlImageScroll_MouseScroll(Object sender, MouseEventArgs e)
        {
            Keys k = ModifierKeys;
            if ((k & Keys.Control) != 0)
            {
                this.numZoom.EnteredValue = this.numZoom.LimitRange(this.numZoom.EnteredValue + (e.Delta / 120));
                HandledMouseEventArgs args = e as HandledMouseEventArgs;
                if (args != null)
                    args.Handled = true;
            }
        }

        private void PnlImagePreview_MouseScroll(Object sender, MouseEventArgs e)
        {
            Keys k = ModifierKeys;
            if ((k & Keys.Control) != 0)
            {
                this.numZoomPreview.EnteredValue = this.numZoomPreview.LimitRange(this.numZoomPreview.EnteredValue + (e.Delta / 120));
                HandledMouseEventArgs args = e as HandledMouseEventArgs;
                if (args != null)
                    args.Handled = true;
            }
        }

        private void NumZoom_ValueChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.RefreshEditor();
        }

        private void RefreshEditor()
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            Image imEdGrBeh = this.pxbEditGridBehind.Image;
            Image imEdGrFrBg = this.pxbEditGridFront.BackgroundImage;
            Image imEdGrFr = this.pxbEditGridFront.Image;
            Image imPxFull = this.pxbFullSize.Image;
            try
            {
                // Beware! Heavy grid logic abound!
                // False if no actual image data loaded.
                Boolean imgLoadOk = this.pxbImage.Image != null && this.m_CurWidth != 0 && this.m_CurHeight != 0;
                Boolean fntLoadOk = this.m_LoadedFont != null;
                Int32 zoom = (Int32) this.numZoom.Value;
                Boolean drawGrid = this.chkGrid.Checked;
                Boolean drawOutline = this.chkOutline.Checked;
                // AddGrid means some kind of grid overlay needs to be drawn; either the grid itself or the outline.
                Boolean addGrid = zoom > 4 && (drawGrid || drawOutline);
                this.pxbImage.Visible = imgLoadOk | addGrid;
                this.pxbImage.Location = new Point(0, this.m_CurYOffset * zoom);
                this.pxbImage.Width = Math.Max(this.m_CurWidth * zoom, 1);
                this.pxbImage.Height = Math.Max(this.m_CurHeight * zoom, 1);
                Bitmap gridImageSmall = null;
                if (fntLoadOk && addGrid)
                {
                    //Draw normal grid, with or without special outline
                    Color[] palette = new Color[] { Color.Transparent, Color.Black, drawOutline ? this.m_Settings.EditAreaGrid : this.m_Settings.BackgroundGrid, this.m_Settings.EditAreaFrame };
                    gridImageSmall = ImageUtils.GenerateGridImage(this.m_CurWidth, this.m_CurHeight, zoom, palette, 0, drawGrid ? (Byte)2 : (Byte)0, drawOutline ? (Byte)3 : (Byte)2);
                    if (!drawOutline)
                    {
                        // If outline is disabled, restore any edges touching the full size edges to the grid colour of the outside grid.
                        ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, 0, this.m_CurHeight * zoom, 1, true); // left line
                        if (this.m_CurYOffset == 0)
                            ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, this.m_CurWidth * zoom, 0, 1, true); // top line
                        if (this.m_CurHeight + this.m_CurYOffset == this.m_LoadedFont.FontHeight)
                            ImageUtils.DrawRect8Bit(gridImageSmall, 0, this.m_CurHeight * zoom, this.m_CurWidth * zoom, this.m_CurHeight * zoom, 1, true); // bottom line
                        if (this.m_CurWidth == this.m_LoadedFont.FontWidth)
                            ImageUtils.DrawRect8Bit(gridImageSmall, this.m_CurWidth * zoom, 0, this.m_CurWidth * zoom, this.m_CurHeight * zoom, 1, true); // right line
                    }
                }
                this.pxbEditGridBehind.Visible = fntLoadOk && addGrid;
                this.pxbEditGridBehind.Location = new Point(0, this.m_CurYOffset * zoom);
                this.pxbEditGridBehind.Width = Math.Max(this.m_CurWidth * zoom + 1, 1);
                this.pxbEditGridBehind.Height = Math.Max(this.m_CurHeight * zoom + 1, 1);
                this.pxbEditGridBehind.Image = gridImageSmall;
                this.pxbEditGridFront.Visible = true;
                // Parent of pxbImage; no change needed.
                //pxbEditGridFront.Location = new Point(0, curYOffset * zoom);
                this.pxbEditGridFront.BackColor = Color.Transparent;
                this.pxbEditGridFront.BackgroundImage = addGrid ? gridImageSmall : null;
                this.pxbEditGridFront.Width = Math.Max(this.m_CurWidth * zoom, 1);
                this.pxbEditGridFront.Height = Math.Max(this.m_CurHeight * zoom, 1);

                //pxbEditGridFront.Image is the overlay image on which the currently hovered pixel is drawn. Make it null if one of the dimensions is 0.
                if (imgLoadOk)
                {
                    this.WipeEditGridFront();
                    this.WipeColorPickInfo();
                    this.CheckMouseForced();
                }
                else
                    this.pxbEditGridFront.Image = null;
                this.pxbFullSize.Visible = fntLoadOk;
                if (fntLoadOk)
                {
                    Int32 bgWidth = this.m_LoadedFont.FontWidth * zoom;
                    Int32 bgHeight = this.m_LoadedFont.FontHeight * zoom;
                    Int32 addedHeight = this.m_CurHeight + this.m_CurYOffset - this.m_LoadedFont.FontHeight;
                    Color bgColor = this.m_Settings.UsePaletteBG ? Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_LoadedFont.TransparencyColor]) : this.m_Settings.Background;
                    if (addGrid && drawGrid)
                    {
                        this.pxbFullSize.Image = ImageUtils.GenerateGridImage(this.m_LoadedFont.FontWidth, this.m_LoadedFont.FontHeight, zoom,
                            new Color[] { bgColor, this.m_Settings.BackgroundGrid, this.m_Settings.BackgroundFrame }, 0, 1, 2);
                        // an extra one-pixel border has been added at the bottom and right edges.
                        bgWidth++;
                        bgHeight++;
                    }
                    else
                    {
                        // No extra border since it'll deform the image
                        // ... except if the outline is drawn
                        if (drawOutline && addGrid && this.m_CurWidth == this.m_LoadedFont.FontWidth)
                            bgWidth++;
                        if (drawOutline && addGrid && (this.m_CurHeight + this.m_CurYOffset == this.m_LoadedFont.FontHeight || addedHeight > 0))
                            bgHeight++;
                        this.pxbFullSize.Image = ImageUtils.GenerateBlankImage(bgWidth, bgHeight, new Color[] { bgColor }, 0);
                        this.pxbFullSize.BackColor = bgColor;
                    }
                    this.pxbFullSize.Width = bgWidth;
                    this.pxbFullSize.Height = addedHeight > 0 ? bgHeight + (addedHeight * zoom) : bgHeight;
                }
            }
            finally
            {
                // Cleanup. All of these should have been replaced.
                if (imEdGrBeh != null && !ReferenceEquals(imEdGrBeh, this.pxbEditGridBehind.Image))
                {
                    try { imEdGrBeh.Dispose(); }
                    catch { /*ignore*/ }
                }
                if (imEdGrFrBg != null && !ReferenceEquals(imEdGrFrBg, this.pxbEditGridFront.BackgroundImage))
                {
                    try { imEdGrFrBg.Dispose(); }
                    catch { /*ignore*/ }
                }
                if (imEdGrFr != null && !ReferenceEquals(imEdGrFr, this.pxbEditGridFront.Image))
                {
                    try { imEdGrFr.Dispose(); }
                    catch { /*ignore*/ }
                }
                if (imPxFull != null && !ReferenceEquals(imPxFull, this.pxbFullSize.Image))
                {
                    try { imPxFull.Dispose(); }
                    catch { /*ignore*/ }
                }
                this.m_Loading = wasLoading;
            }
        }

        private void ImageBox_Click(Object sender, MouseEventArgs e)
        {
            this.pnlImageScroll.Focus();
        }

        private void CheckboxGridOptionChanged(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            this.RefreshEditor();
        }

        private void pxbEditGridFront_MouseMove(Object sender, MouseEventArgs e)
        {
            this.CheckMouse(e.X, e.Y, e.Button, this.chkPaint.Checked, false);
        }

        private void pxbEditGridFront_MouseDown(Object sender, MouseEventArgs e)
        {
            this.pnlImageScroll.Focus();
            // prevents problem where the closing click of a dialog is seen as valid mouse-up event on the edit grid
            this.m_Clicking = (e.Button & MouseButtons.Left) != 0 || (e.Button & MouseButtons.Right) != 0;
            this.CheckMouse(e.X, e.Y, e.Button, false, false);
        }

        private void pxbEditGridFront_MouseUp(Object sender, MouseEventArgs e)
        {
            this.m_Clicking = false;
            if ((e.Button & MouseButtons.Left) != 0 || (e.Button & MouseButtons.Right) != 0)
            {
                //ReloadDataGrid(false);
                this.RefreshCurrentGridImage();
                this.RepaintPreview();
                this.AdjustRevertButton();
            }
        }

        private void pxbEditGridFront_MouseLeave(Object sender, EventArgs e)
        {
            this.m_Clicking = false;
            this.WipeEditGridFront();
            this.WipeColorPickInfo();
            this.m_LastHoverPixelX = -1;
            this.m_LastHoverPixelY = -1;
        }

        private void CheckMouseForced()
        {
            Point mousePos = this.pxbEditGridFront.PointToClient(Cursor.Position);
            this.CheckMouse(mousePos.X, mousePos.Y, MouseButtons.None, this.chkPaint.Checked, true);
        }

        private void CheckMouse(Int32 mouseX, Int32 mouseY, MouseButtons pressedbuttons, Boolean drawPreviewPixel, Boolean force)
        {
            Bitmap gridFront = this.pxbEditGridFront.Image as Bitmap;
            if (gridFront == null || this.m_LoadedFont == null)
                return;
            Int32 picX = mouseX / (Int32)this.numZoom.Value;
            Int32 picY = mouseY / (Int32)this.numZoom.Value;
            // Optimize by aborting immediately if location is unchanged
            Boolean inBounds = mouseX >= 0 && picX < gridFront.Width && mouseY >= 0 && picY < gridFront.Height;
            Boolean hasntMoved = this.m_LastHoverPixelX == picX && this.m_LastHoverPixelY == picY;
            Boolean isLeftClick = (pressedbuttons & MouseButtons.Left) != 0;
            Boolean isRightClick = (pressedbuttons & MouseButtons.Right) != 0;
            if (hasntMoved && !isLeftClick && !isRightClick && !force)
                return;
            if ((drawPreviewPixel && !hasntMoved) || force)
            {
                // Clear previous pixel
                if (this.m_LastHoverPixelX != -1 && this.m_LastHoverPixelY != -1)
                    ImageUtils.DrawRect8Bit(gridFront, this.m_LastHoverPixelX, this.m_LastHoverPixelY, this.m_LastHoverPixelX, this.m_LastHoverPixelY, 0, true);
                // set color, just in case it changed.
                if (this.m_CurrentPalette.Length > this.m_CurrentPaintColor1)
                    gridFront.Palette.Entries[1] = this.m_CurrentPalette[this.m_CurrentPaintColor1];
                // Draw new pixel
                if (inBounds && drawPreviewPixel)
                    ImageUtils.DrawRect8Bit(gridFront, picX, picY, picX, picY, 1, true);
                this.pxbEditGridFront.Invalidate();
            }
            this.m_LastHoverPixelX = picX;
            this.m_LastHoverPixelY = picY;
            if (!inBounds)
                return;
            Int32 curIndex = this.GetSelectedIndex();
            if (this.chkPaint.Checked)
            {
                this.toolTip1.SetToolTip(this.pxbEditGridFront, null);
                if ((isLeftClick || isRightClick) && this.m_Clicking)
                {
                    try
                    {
                        if (isLeftClick)
                            this.m_LoadedFont.PaintPixel(curIndex, picX, picY, this.m_CurrentPaintColor1);
                        else
                            this.m_LoadedFont.PaintPixel(curIndex, picX, picY, this.m_CurrentPaintColor2);
                        this.pxbImage.Image = this.m_LoadedFont.GetBitmap(curIndex, this.m_CurrentPalette, true);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        // Trying to draw a >15 color index on a 4-bit image. Shouldn't happen in the final version.
                        MessageBox.Show(this, ex.Message, GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (this.chkPicker.Checked)
            {
                Byte val = this.m_LoadedFont.GetSymbol(curIndex).GetPixelValue(picX, picY);
                this.SetColorPickHighlight(val);
                
                Color c = this.GetPaletteColor(val);
                String toolTip = String.Format("#{0} ({1},{2},{3})", val, c.R, c.G, c.B);
                this.toolTip1.SetToolTip(this.pxbEditGridFront, toolTip);
                if (this.m_Clicking)
                {
                    if (isLeftClick)
                    {
                        this.m_CurrentPaintColor1 = val;
                        this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, c);
                        // Since the grid only shows edit color 1, it's only needed for Left button.
                        this.WipeEditGridFront();
                    }
                    else if (isRightClick)
                    {
                        this.m_CurrentPaintColor2 = val;
                        this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, c);
                    }
                }
            }

        }

        private Boolean CheckIsEqual()
        {
            return this.CheckIsEqual(this.GetSelectedIndex());
        }

        private Boolean CheckIsEqual(Int32 index)
        {
            if (this.m_LoadedFont == null || this.m_LoadedFontBackup == null)
                return false;
            FontFileSymbol rawData1 = this.m_LoadedFont.GetSymbol(index);
            FontFileSymbol rawData2 = this.m_LoadedFontBackup.GetSymbol(index);
            if (rawData1 == null && rawData2 == null)
                return true;
            if (rawData1 == null || rawData2 == null)
                return false;
            return rawData1.Equals(rawData2);
        }

        private Boolean CheckCanRevert()
        {
            if (this.m_LoadedFont == null || this.m_LoadedFontBackup == null)
                return false;
            Int32 index = this.GetSelectedIndex();
            FontFileSymbol rawData1 = this.m_LoadedFont.GetSymbol(index);
            FontFileSymbol rawData2 = this.m_LoadedFontBackup.GetSymbol(index);
            if (rawData1 == null || rawData2 == null)
                return false;
            // they're the same; can't revert.
            if (this.CheckIsEqual(index))
                return false;
            // different dimensions; can't revert. Would never be equal to original.
            if ((!this.m_LoadedFont.CustomSymbolWidthsForType && this.m_LoadedFont.FontWidth != this.m_LoadedFontBackup.FontWidth) || (!this.m_LoadedFont.CustomSymbolHeightsForType && this.m_LoadedFont.FontHeight != this.m_LoadedFontBackup.FontHeight))
                return false;
            if (this.m_LoadedFont.FontWidth < rawData2.Width || this.m_LoadedFont.FontHeight < rawData2.Height)
                return false;
            return true;
        }

        private Boolean AdjustRevertButton()
        {
            Boolean enable = this.CheckCanRevert();
            this.tsmiRevertSymbol.Enabled = enable;
            return enable;
        }

        /// <summary>
        /// Regenerates the preview pixel image drawn on top of the front edit grid
        /// to get a blank slate with the correct preview pixel color set.
        /// </summary>
        private void WipeEditGridFront()
        {
            Color col = this.GetPaletteColor(this.m_CurrentPaintColor1);
            Color paintColor = Color.FromArgb(0xFF, col);
            Image oldImg = this.pxbEditGridFront.Image;
            this.pxbEditGridFront.Image = ImageUtils.GenerateBlankImage(this.m_CurWidth, this.m_CurHeight, new Color[] { Color.Transparent, paintColor }, 0);
            if (oldImg != null && !ReferenceEquals(oldImg, this.pxbEditGridFront.Image))
            {
                try { oldImg.Dispose(); }
                catch { /*ignore*/ }
            }
        }

        private void WipeColorPickInfo()
        {
            this.toolTip1.SetToolTip(this.pxbEditGridFront, null);
            this.WipeColorPickHighlight();
        }

        private void WipeColorPickHighlight()
        {
            this.palColorSelector.TransItemCharColor = Color.Blue;
            this.palColorSelector.ColorSelectMode = ColorSelMode.None;
        }

        private void SetColorPickHighlight(Int32 index)
        {
            Color c = this.GetPaletteColor(index);
            if (c.A != 0xFF && this.palColorSelector.LabelSize.Width < 10)
                this.palColorSelector.TransItemCharColor = Color.Empty;
            else
                this.palColorSelector.TransItemCharColor = Color.Blue;
            this.palColorSelector.ColorSelectMode = ColorSelMode.Single;
            this.palColorSelector.SelectedIndices = new Int32[] { index };
        }

        private Color GetPaletteColor(Int32 index)
        {
            return index < this.m_CurrentPalette.Length ? this.m_CurrentPalette[index] : Color.Black;
        }

        private void palColorSelector_ColorLabelMouseClick(Object sender, PaletteClickEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                this.m_CurrentPaintColor1 = (Byte)(e.Index & 0xFF);
                this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, e.Color);
                // Since the grid only shows edit color 1, it's only needed for Left button.
                this.WipeEditGridFront();
            }
            if ((e.Button & MouseButtons.Right) != 0)
            {
                this.m_CurrentPaintColor2 = (Byte)(e.Index & 0xFF);
                this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, e.Color);
            }
        }

        private void btnRevert_Click(Object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(QUESTION_REVERTSYMBOL, GetTitle(false), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes)
                return;
            this.m_LoadedFont.RestorePicFromBackup(this.GetSelectedIndex(), this.m_LoadedFontBackup, this.GetEditBpp(this.m_LoadedFont));
            this.ReloadImageInfo(true);
            //this.ReloadDataGrid(false);
            this.RefreshCurrentGridImage();
            this.pnlImageScroll.Focus();
        }

        private void NumYOffset_ValueChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.m_LoadedFont.GetSymbol(this.GetSelectedIndex()).YOffset = (Byte)this.numYOffset.Value;
            this.ReloadImageInfo(true);
            this.RefreshCurrentGridImage();
        }

        private void CmbEncodings_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.ReloadDataGrid(false);
            this.RepaintPreview();
        }

        private void DgrvSymbolsList_SelectionChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.ReloadImageInfo(true);
        }

        private void PalColorSelector_ColorLabelMouseDoubleClick(Object sender, PaletteClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            PalettePanel palpanel = (PalettePanel)sender;
            Int32 colindex = e.Index;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = e.Color;
            this.OpenColorEditDialog(colindex, palpanel);
        }

        private void LblPaintColor1_DoubleClick(Object sender, EventArgs e)
        {
            this.OpenColorEditDialog(this.m_CurrentPaintColor1, this.palColorSelector);
        }

        private void LblPaintColor2_DoubleClick(Object sender, EventArgs e)
        {
            this.OpenColorEditDialog(this.m_CurrentPaintColor2, this.palColorSelector);
        }

        private void OpenColorEditDialog(Int32 colindex, PalettePanel palpanel)
        {
            Byte transCol = this.m_LoadedFont != null ? this.m_LoadedFont.TransparencyColor : (Byte)0;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = this.GetPaletteColor(colindex);
            cdl.FullOpen = true;
            cdl.CustomColors = this.m_CustomColors;
            DialogResult res = cdl.ShowDialog(this);
            this.m_CustomColors = cdl.CustomColors;
            if (res == DialogResult.OK)
            {
                Color paletteColor = Color.FromArgb(colindex == transCol ? 0x00 : 0xFF, cdl.Color);
                this.m_CurrentPalette[colindex] = paletteColor;
                if (palpanel != null)
                {
                    palpanel.Palette[colindex] = paletteColor;
                    palpanel.Invalidate();
                }
                if (colindex == this.m_CurrentPaintColor1)
                    this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor1]);
                if (colindex == this.m_CurrentPaintColor2)
                    this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor2]);
                this.ReloadDataGrid(false);
                this.ReloadImageInfo(true);
                PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
                this.btnResetPalette.Enabled = currentPal != null && currentPal.IsChanged();
            }
        }

        private void TsmiNewFont_Click(Object sender, EventArgs e)
        {
            if (this.AbortForChangesAskSave(QUESTION_SAVEFILE_OPENNEW))
                return;
            FontFile sourceFontFile = new FontDummy();
            FrmConvertFontType fontConvertDialog = new FrmConvertFontType(sourceFontFile, true);
            fontConvertDialog.StartPosition = FormStartPosition.CenterParent;
            if (fontConvertDialog.ShowDialog(this) != DialogResult.OK)
                return;
            FontFile targetFontFile = fontConvertDialog.TargetFontFile;
            sourceFontFile.CloneInto(targetFontFile, 0, this.GetEditBpp(targetFontFile));
            if (targetFontFile.TransparencyColor != sourceFontFile.TransparencyColor)
                foreach (FontFileSymbol ffs in targetFontFile.GetAllSymbols())
                    ffs.ReplaceColor(sourceFontFile.TransparencyColor, targetFontFile.TransparencyColor);
            this.OptimizeFontWidths(targetFontFile, false);
            FontFileSymbol space = targetFontFile.GetSymbol(0x20);
            if (space != null)
            {
                if (targetFontFile.CustomSymbolWidthsForType)
                    space.ChangeWidth(5,targetFontFile.TransparencyColor);
                if (targetFontFile.YOffsetTypeMax >= targetFontFile.FontHeight)
                    space.YOffset = targetFontFile.FontHeight;
            }
            this.m_LoadedFont = targetFontFile;
            this.m_FileName = null;
            this.FinishLoading(true);
        }

        private void TsmiOpenFont_Click(Object sender, EventArgs e)
        {
            if (this.AbortForChangesAskSave(QUESTION_SAVEFILE_OPENNEW))
                return;
            FontFile selectedItem;
            String filename = FileDialogGenerator.ShowOpenFileFialog(this, FontFile.SupportedTypes, this.m_FileName, "fonts", "fnt", out selectedItem);
            if (filename == null)
                return;
            this.LoadFontFile(filename, selectedItem);
        }
                
        private void TsmiSaveFont_Click(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            // no backup: new font file.
            if (this.m_LoadedFontBackup == null || this.m_LoadedFontBackup.GetType() != this.m_LoadedFont.GetType())
                this.SaveFontAs();
            else
                this.SaveFontFile();
        }
        
        private void TsmiSaveFontAs_Click(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            this.SaveFontAs();
        }

        private void SaveFontAs()
        {
            if (this.m_LoadedFont == null)
                return;
            FontFile selectedItem;
            String suggestedfilename = this.m_FileName ?? NEWFONTNAME + (this.m_LoadedFont.FileExtensions.FirstOrDefault() ?? "fnt");
            String filename = FileDialogGenerator.ShowSaveFileFialog(this, this.m_LoadedFont.GetType(), FontFile.SupportedTypes, typeof(FontFileWsV3), true, suggestedfilename, out selectedItem);
            if (filename == null || selectedItem == null)
                return;
            if (this.m_LoadedFont.GetType() != selectedItem.GetType() && !this.ChangeFontType(selectedItem))
                return;
            this.SaveFontFile(filename);
        }

        private void TsmiExit_Click(Object sender, EventArgs e)
        {
            // Add changes check?
            Application.Exit();
        }

        private void YShiftCurrentImage(ShiftDirection shiftDirection, Boolean all)
        {
            Int32 shift = 0;
            if (this.m_LoadedFont == null)
                return;
            if (this.m_LoadedFont.YOffsetTypeMax == 0)
                return;
            if (shiftDirection == ShiftDirection.Up)
                shift--;
            else if (shiftDirection == ShiftDirection.Down)
                shift++;
            if (all)
            {
                foreach (FontFileSymbol symbol in this.m_LoadedFont.GetAllSymbols())
                    symbol.YOffset = Math.Min(this.m_LoadedFont.YOffsetTypeMax, Math.Max(0, symbol.YOffset + shift));
            }
            else
            {
                FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(this.GetSelectedIndex());
                if (symbol == null)
                    return;
                symbol.YOffset = Math.Min(this.m_LoadedFont.YOffsetTypeMax, Math.Max(0, symbol.YOffset + shift));
            }
            this.ReloadImageInfo(true);
            if (!all)
                this.RefreshCurrentGridImage();
            else
                this.ReloadDataGrid(false);
        }
        
        private void ShiftCurrentImage(ShiftDirection shiftDirection, Boolean all, Boolean expand)
        {
            if (this.m_LoadedFont == null)
                return;
            if (expand && shiftDirection == ShiftDirection.Left)
                return;
            Boolean cont = true;
            if (all)
            {
                foreach (FontFileSymbol symbol in this.m_LoadedFont.GetAllSymbols())
                {
                    cont = true;
                    if (expand)
                        cont = symbol.TryExpandImage(shiftDirection, this.m_LoadedFont);
                    if (cont)
                        symbol.ShiftImageData(shiftDirection, this.chkShiftWrap.Checked, this.m_LoadedFont.TransparencyColor);
                }
            }
            else
            {
                Int32 curIndex = this.GetSelectedIndex();
                FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(curIndex);
                if (symbol == null)
                    return;
                if (expand)
                    cont = symbol.TryExpandImage(shiftDirection, this.m_LoadedFont);
                if (cont)
                    symbol.ShiftImageData(shiftDirection, this.chkShiftWrap.Checked, this.m_LoadedFont.TransparencyColor);
            }
            this.ReloadImageInfo(true);
            if (!all)
                this.RefreshCurrentGridImage();
            else
                this.ReloadDataGrid(false);
        }

        private void ToggleTempColorSelect(Boolean enabled)
        {
            if (this.m_Loading)
                return;
            if (enabled && this.m_TempColActive)
                return;
            if (!enabled && !this.m_TempColActive)
                return;
            this.m_Loading = true;

            if (enabled)
            {
                this.m_TempColPickerSelected = this.chkPicker.Checked;
                this.chkPaint.Checked = false;
                this.chkPicker.Checked = true;
                this.WipeEditGridFront();
                this.pxbEditGridFront.Cursor = Cursors.Hand;
            }
            else
            {
                this.chkPaint.Checked = !this.m_TempColPickerSelected;
                this.chkPicker.Checked = this.m_TempColPickerSelected;
                this.WipeColorPickInfo();
                this.pxbEditGridFront.Cursor = Cursors.Default;
            }
            this.m_TempColActive = enabled;
            this.m_Loading = false;
            this.CheckMouseForced();
        }

        private void BtnShiftUp_Click(Object sender, EventArgs e)
        {
            this.ShiftCurrentImage(ShiftDirection.Up, (ModifierKeys & Keys.Shift) != 0, (ModifierKeys & Keys.Alt) != 0);
        }
        private void BtnShiftRight_Click(Object sender, EventArgs e)
        {
            this.ShiftCurrentImage(ShiftDirection.Right, (ModifierKeys & Keys.Shift) != 0, (ModifierKeys & Keys.Alt) != 0);
        }

        private void BtnShiftDown_Click(Object sender, EventArgs e)
        {
            this.ShiftCurrentImage(ShiftDirection.Down, (ModifierKeys & Keys.Shift) != 0, (ModifierKeys & Keys.Alt) != 0);
        }

        private void BtnShiftLeft_Click(Object sender, EventArgs e)
        {
            this.ShiftCurrentImage(ShiftDirection.Left, (ModifierKeys & Keys.Shift) != 0, (ModifierKeys & Keys.Alt) != 0);
        }

        private void ChangeCurrentImageDimension(Int32 newDimension, Boolean isHeight)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = this.GetSelectedIndex();
            Int32 oldHeight = this.m_LoadedFont.FontHeight;
            FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(curIndex);
            if (symbol == null)
                return;
            if (isHeight)
                symbol.ChangeHeight(newDimension, this.m_LoadedFont.TransparencyColor);
            else
                symbol.ChangeWidth(newDimension, this.m_LoadedFont.TransparencyColor);
            this.ReloadImageInfo(true);

            if (isHeight && (newDimension > this.m_GridRowTemplateHeight || oldHeight > this.m_GridRowTemplateHeight))
                this.ReloadDataGrid(true);
            else
                this.RefreshCurrentGridImage();
        }

        private void NumWidth_ValueChanged(Object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Int32)this.numWidth.Value, false);
        }

        private void NumHeight_ValueChanged(Object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Int32)this.numHeight.Value, true);
        }

        protected override Boolean IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control:
                    return true;
                case Keys.Control | Keys.Right:
                case Keys.Control | Keys.Left:
                case Keys.Control | Keys.Up:
                case Keys.Control | Keys.Down:
                    return true;
                case Keys.Control | Keys.Alt | Keys.Right:
                case Keys.Control | Keys.Alt | Keys.Left:
                case Keys.Control | Keys.Alt | Keys.Up:
                case Keys.Control | Keys.Alt | Keys.Down:
                    return true;
                case Keys.Control | Keys.Shift | Keys.Right:
                case Keys.Control | Keys.Shift | Keys.Left:
                case Keys.Control | Keys.Shift | Keys.Up:
                case Keys.Control | Keys.Shift | Keys.Down:
                    return true;
                case Keys.Control | Keys.Alt | Keys.Shift | Keys.Right:
                case Keys.Control | Keys.Alt | Keys.Shift | Keys.Left:
                case Keys.Control | Keys.Alt | Keys.Shift | Keys.Up:
                case Keys.Control | Keys.Alt | Keys.Shift | Keys.Down:
                    return true;
                case Keys.Control | Keys.PageUp:
                case Keys.Control | Keys.PageDown:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            base.OnKeyDown(e);
            if (!e.Control)
                return;
            this.ToggleTempColorSelect(true);
            // Abort if inside a text field or the datagridview
            Control control = this;
            while (control is IContainerControl)
            {
                control = ((IContainerControl)control).ActiveControl;
                if (control is TextBox || control is DataGridView || control is NumericUpDown)
                {
                    return;
                }
            }
            ShiftDirection sd;
            Boolean yShift = false;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    sd = ShiftDirection.Left;
                    break;
                case Keys.Right:
                    sd = ShiftDirection.Right;
                    break;
                case Keys.Up:
                    sd = ShiftDirection.Up;
                    break;
                case Keys.Down:
                    sd = ShiftDirection.Down;
                    break;
                case Keys.PageUp:
                    sd = ShiftDirection.Up;
                    yShift = true;
                    break;
                case Keys.PageDown:
                    sd = ShiftDirection.Down;
                    yShift = true;
                    break;
                default:
                    return;
            }
            Boolean processAll = e.Shift;
            if (yShift)
            {
                this.YShiftCurrentImage(sd, processAll);
            }
            else
            {
                this.ShiftCurrentImage(sd, processAll, e.Alt);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == 0)
                this.ToggleTempColorSelect(false);
        }

        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // override of menu shortcuts to allow copying and pasting text in the preview text field and numeric up/down controls.
            Boolean isCtrlC = keyData == (Keys.Control | Keys.C);
            Boolean isCtrlV = keyData == (Keys.Control | Keys.V);
            if (!isCtrlC && !isCtrlV)
                return base.ProcessCmdKey(ref msg, keyData);
            TextBox tb = this.ActiveControl as TextBox;
            EnhNumericUpDown num = this.ActiveControl as EnhNumericUpDown;
            if (tb == null && num == null)
                return base.ProcessCmdKey(ref msg, keyData);
            if (tb == null)
            {
                if (isCtrlC)
                {
                    if (!String.IsNullOrEmpty(num.SelectedText))
                        Clipboard.SetText(num.SelectedText);
                }
                else
                    num.SelectedText = Clipboard.GetText();
            }
            else
            {
                if (isCtrlC)
                {
                    if (!String.IsNullOrEmpty(tb.SelectedText))
                        Clipboard.SetText(tb.SelectedText);
                }
                else
                    tb.SelectedText = Clipboard.GetText();
            }
            return true;
        }

        private void TsmiCopySymbol_Click(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = this.GetSelectedIndex();
            FontFileSymbol ffs = this.m_LoadedFont.GetSymbol(curIndex);
            if (ffs == null)
                return;
            Color[] noTransPal = this.m_CurrentPalette.ToArray();
            if (noTransPal.Length > this.m_LoadedFont.TransparencyColor)
                noTransPal[this.m_LoadedFont.TransparencyColor] = Color.FromArgb(255, noTransPal[this.m_LoadedFont.TransparencyColor]);
            DataObject data = new DataObject();
            using (Bitmap imageNoTr = ffs.GetBitmapFullSize(noTransPal, this.m_LoadedFont, true))
            using (Bitmap image = ffs.GetBitmapFullSize(this.m_CurrentPalette, this.m_LoadedFont, true))
            {
                // As text character
                Int32 index = (Int32)this.dgrvSymbolsList.Rows[curIndex - this.m_LoadedFont.SymbolsTypeFirst].Cells[1].Value;
                // Reconvert from encoding to compensate for 0x00-0x20 ASCII substitution.
                Encoding enc = ((EncodingDropDownInfo)this.cmbEncodings.SelectedItem).Encoding;
                String str = enc.GetString(new Byte[] {(Byte) index});
                data.SetData(DataFormats.Text, str);
                //data.SetData(DataFormats.Text, (String)this.dgrvSymbolsList.Rows[curIndex - this.m_LoadedFont.SymbolsTypeFirst].Cells[2].Value);
                // As Font Editor object
                data.SetData(typeof(FontFileSymbol), ffs.Clone());
                // if one of the symbol dimensions is 0, the image will be null. In that case, don't copy it to the clipboard.
                if (image != null)
                    ClipboardImage.SetClipboardImage(image, imageNoTr, data);
                else
                    Clipboard.SetDataObject(data, true);
            }
        }

        private void TsmiPasteSymbol_Click(Object sender, EventArgs e)
        {
            this.Paste(false);
        }

        private void TsmiPasteSymbolTrans_Click(Object sender, EventArgs e)
        {
            this.Paste(true);
        }
        
        private void Paste(Boolean pasteCombined)
        {
            if (this.m_LoadedFont == null)
                return;
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();
            FontFileSymbol clipboard = null;
            if (retrievedData != null)
                clipboard = this.GetClipboardData(retrievedData);
            if (clipboard == null)
            {
                MessageBox.Show("No image data found on the clipboard.", GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Int32 curIndex = this.GetSelectedIndex();
            FontFileSymbol fc = this.m_LoadedFont.GetSymbol(curIndex);
            if (fc == null)
                return;
            Boolean canrevert = this.AdjustRevertButton();
            // if there are unsaved changes, or the image is new and not empty, ask specifically
            if (!pasteCombined && !this.CheckIsEqual() && !canrevert && this.m_LoadedFontBackup != null || (this.m_LoadedFontBackup != null && this.m_LoadedFontBackup.Length <= curIndex && fc.Width > 0 && fc.Height > 0))
            {
                DialogResult dr = MessageBox.Show("This will completely overwrite the current symbol.\n\nAre you sure you want to continue?", GetTitle(false), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }
            try
            {
                if (pasteCombined)
                {
                    Color[] pal = this.m_CurrentPalette.ToArray();
                    pal[this.m_LoadedFont.TransparencyColor] = Color.FromArgb(0, pal[this.m_LoadedFont.TransparencyColor]);
                    clipboard = FontFileSymbol.Combine(fc, clipboard, this.m_LoadedFont, pal);
                }
                fc = clipboard.CloneFor(this.m_LoadedFont, this.GetEditBpp(this.m_LoadedFont));
            }
            catch (InvalidOperationException)
            {
                FrmConvertToLowerBpp convertPopup = new FrmConvertToLowerBpp(true, this.m_LoadedFont.BitsPerPixel, this.m_CurrentPalette);
                convertPopup.StartPosition = FormStartPosition.CenterParent;
                if (convertPopup.ShowDialog() == DialogResult.OK)
                {
                    fc = clipboard.CloneFor(this.m_LoadedFont, (Byte)convertPopup.SelectedIndex, this.GetEditBpp(this.m_LoadedFont));
                }
            }
            try
            {
                this.m_LoadedFont.RestorePicFromBackup(curIndex, fc, this.GetEditBpp(this.m_LoadedFont));
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.ReloadImageInfo(true);
            this.ReloadDataGrid(false);
        }

        private FontFileSymbol GetClipboardData(DataObject retrievedData)
        {
            if (retrievedData.GetDataPresent(typeof(FontFileSymbol)))
                return retrievedData.GetData(typeof(FontFileSymbol)) as FontFileSymbol;
            Bitmap clipboardimage = ClipboardImage.GetClipboardImage(retrievedData);
            if (clipboardimage == null)
                return null;
            FontFileSymbol clipboardSymbol = new FontFileSymbol(clipboardimage, this.m_CurrentPalette, this.m_LoadedFont);
            clipboardimage.Dispose();
            return clipboardSymbol;
        }

        private void NumSymbols_ValueChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            this.m_Loading = true;
            try
            {
                Int32 newLen = (Int32)this.numSymbols.Value;
                this.m_LoadedFont.Length = newLen;
                newLen = this.m_LoadedFont.Length;
                this.numSymbols.Value = newLen;
                this.ReloadDataGrid(false);
                if (newLen > 0)
                {
                    Int32 newIndex = newLen - 1 - this.m_LoadedFont.SymbolsTypeFirst;
                    if (newIndex > 0)
                    {
                        this.dgrvSymbolsList.Rows[newIndex].Cells[0].Selected = true;
                        this.dgrvSymbolsList.FirstDisplayedCell = this.dgrvSymbolsList.Rows[newIndex].Cells[0];
                    }
                }
                this.ReloadImageInfo(true);
            }
            finally
            {
                this.m_Loading = false;
            }
        }

        private void NumFontWidth_ValueChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontWidth.Value, 0xFF);
            this.m_LoadedFont.FontWidth = newVal;
            this.ReloadDataGrid(false);
            this.ReloadImageInfo(true);
        }

        private void NumFontHeight_ValueChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontHeight.Value, 0xFF);
            this.m_LoadedFont.FontHeight = newVal;
            this.ReloadDataGrid(false);
            this.ReloadImageInfo(true);
        }

        private void TsmiRevertFont_Click(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            if (!this.ConfirmOnUnsavedChanges(QUESTION_RESETFONT))
                return;
            this.m_LoadedFont = this.m_LoadedFontBackup.Clone();
            this.ReloadUIWithSelection(true);
        }

        private void TsmiManagePalettes_Click(Object sender, EventArgs e)
        {
            FrmManagePalettes palSave = new FrmManagePalettes(4);
            palSave.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = palSave.ShowDialog(this);
            // Get source position, reload all, then loop through to check which one to reselect.
            if (dr == DialogResult.OK)
                this.RefreshPalettes(true, true);
        }

        private void ReloadUIWithSelection(Boolean newFontLoaded)
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            try
            {
                Int32 selectedIndex = this.m_LoadedFont == null ? this.m_Settings.SelectedSymbol : this.GetSelectedIndex();
                Int32 scrollOffset = 0;
                if (this.dgrvSymbolsList.SelectedRows.Count > 0)
                    scrollOffset = this.dgrvSymbolsList.VerticalScrollbarOffset;

                this.m_Loading = false;
                this.ReloadUi(newFontLoaded);
                if (this.m_LoadedFont != null)
                {
                    // Adjust to font limitations
                    if (this.m_LoadedFont.SymbolsTypeFirst > selectedIndex)
                        selectedIndex = 0;
                    else
                        selectedIndex -= this.m_LoadedFont.SymbolsTypeFirst;
                }
                if ((this.dgrvSymbolsList.DataSource as DataTable) != null && selectedIndex < ((DataTable)(this.dgrvSymbolsList.DataSource)).Rows.Count && selectedIndex > 0)
                {
                    this.dgrvSymbolsList.VerticalScrollbarOffset = Math.Min(Math.Max(0, scrollOffset), this.dgrvSymbolsList.ClientSize.Height);
                    this.dgrvSymbolsList.Rows[selectedIndex].Cells[0].Selected = true;
                }
            }
            finally
            {
                this.m_Loading = wasLoading;
            }
        }

        private void tsmiOptimizeWidths_Click(Object sender, EventArgs e)
        {
            this.OptimizeFontWidths(this.m_LoadedFont, true);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid(false);

        }
        private void OptimizeFontWidths(FontFile font, Boolean alsoTrimLeft)
        {
            if (font == null || !font.CustomSymbolWidthsForType)
                return;
            FontFileSymbol[] symbols = font.GetAllSymbols();
            //Saves the number of symbols encountered for each trim amount.
            Dictionary<Int32, Int32> trimValueAmounts = new Dictionary<Int32, Int32>();
            Int32 totalTrimmed = 0;
            FontFileSymbol space = null;
            for (Int32 i = 0; i < symbols.Length; i++)
            {
                FontFileSymbol symbol = symbols[i];
                // Skip space.
                if (i == 0x20)
                {
                    space = symbol;
                    continue;
                }
                Int32 initialWidth = symbol.Width;
                if (alsoTrimLeft)
                    symbol.OptimizeXWidth(true);
                else
                    symbol.CropRightSide();
                if (symbol.Width > 0 && symbol.Width < font.FontWidth && font.FontTypePaddingRight == 0)
                    symbol.ChangeWidth(symbol.Width + 1, font.TransparencyColor);
                // only count 'normal' characters, which contain data.
                if (initialWidth > 0 && i > 0x20)
                {
                    Int32 diff = initialWidth - symbol.Width;
                    if (trimValueAmounts.ContainsKey(diff))
                        trimValueAmounts[diff] = trimValueAmounts[diff] + 1;
                    else trimValueAmounts.Add(diff, 1);
                    totalTrimmed++;
                }
            }
            if (trimValueAmounts.Keys.Count > 0)
            {
                Int32 maxTrimmed = trimValueAmounts.Values.Max();
                Double percentage = (Double)maxTrimmed / (Double)totalTrimmed;
                Int32 trimmed = trimValueAmounts.FirstOrDefault(x => x.Value == maxTrimmed).Key;
                // only adjust space if at least 90% of the trimmed normal-range characters had the same amount trimmed off.
                if (percentage > 0.90d && space.Width > trimmed)
                    space.ChangeWidth(space.Width - trimmed, 0);
            }
        }

        private void TsmiAbout_Click(Object sender, EventArgs e)
        {
            MessageBox.Show(this, GetTitle(true) + "\n\n" + ABOUTTEXT, GetTitle(false), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChkPaint_CheckStateChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.m_Loading = true;
            this.toolTip1.SetToolTip(this.pxbEditGridFront, null);
            this.palColorSelector.TransItemCharColor = Color.Blue;
            this.palColorSelector.ColorSelectMode = ColorSelMode.None;
            this.chkPicker.Checked = false;
            this.WipeEditGridFront();
            this.pxbEditGridFront.Cursor = Cursors.Default;
            this.CheckMouseForced();
            this.m_Loading = false;
        }

        private void ChkPick_CheckStateChanged(Object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.m_Loading = true;
            this.chkPaint.Checked = false;
            this.WipeColorPickInfo();
            this.pxbEditGridFront.Cursor = Cursors.Hand;
            this.CheckMouseForced();
            this.m_Loading = false;
        }

        private void CmbPalettes_SelectedIndexChanged(Object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
            Int32 bpp;
            if (currentPal == null)
            {
                if (!this.btnSavePalette.Enabled)
                    this.btnSavePalette.Enabled = true;
                this.m_CurrentPalette = GetDummyPalette();
                bpp = 4;
            }
            else
            {
                this.m_CurrentPalette = currentPal.Colors;
                bpp = currentPal.BitsPerPixel;
                if (this.btnSavePalette.Enabled && bpp == 1)
                    this.btnSavePalette.Enabled = false;
                else if (!this.btnSavePalette.Enabled && bpp != 1)
                    this.btnSavePalette.Enabled = true;
                this.btnResetPalette.Enabled = currentPal.IsChanged();
            }
            this.ReloadColors(bpp);
        }

        private void BtnResetPalette_Click(Object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if (currentPal == null)
                return;
            if (currentPal.SourceFile != null && currentPal.Entry >= 0)
            {
                DialogResult dr = MessageBox.Show("This will remove all changes you have made to the palette since it was loaded!\n\nAre you sure you want to continue?", GetTitle(false), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }
            currentPal.Revert();
            this.btnResetPalette.Enabled = currentPal.IsChanged();
            this.ReloadColors(currentPal.BitsPerPixel);
        }

        private void ReloadColors(Int32 bpp)
        {
            Byte transparent = this.m_LoadedFont == null ? (Byte)0 : this.m_LoadedFont.TransparencyColor;
            for (Int32 i = 0; i < this.m_CurrentPalette.Length; i++)
                this.m_CurrentPalette[i] = Color.FromArgb(i == transparent ? 0 : 0xFF, this.m_CurrentPalette[i]);
            PalettePanel.InitPaletteControl(bpp, this.palColorSelector, this.m_CurrentPalette, PALETTE_MAX_DIM);
            if (this.m_CurrentPaintColor1 >= this.m_CurrentPalette.Length)
                this.m_CurrentPaintColor1 = (Byte)(transparent == 0 ? 1 : 0);
            // Transparent SHOULD be inside palette bounds, but, better safe...
            if (this.m_CurrentPaintColor2 >= this.m_CurrentPalette.Length)
                this.m_CurrentPaintColor2 = (Byte)Math.Min(this.m_CurrentPalette.Length-1, transparent);
            this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor1]);
            this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_CurrentPaintColor2]);
            if (!this.m_Loading)
            {
                this.ReloadImageInfo(true);
                this.ReloadDataGrid(false);
            }
        }

        private void BtnSavePalette_Click(Object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if (currentPal == null)
                return;
            FrmManagePalettes palSave = new FrmManagePalettes(currentPal.BitsPerPixel);
            palSave.PaletteToSave = currentPal;
            palSave.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = palSave.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                // If null, it was a simple immediate overwrite, without the management box ever popping up, so
                // just consider the current entry "saved".
                if (palSave.PaletteToSave == null)
                    currentPal.ClearRevert();
                else
                {
                    // Get source position, reload all, then loop through to check which one to reselect.
                    this.RefreshPalettes(true, true);
                    String source = palSave.PaletteToSave.SourceFile;
                    Int32 index = palSave.PaletteToSave.Entry;
                    foreach (PaletteDropDownInfo pdd in this.cmbPalettes.Items)
                    {
                        if (pdd.SourceFile != source || pdd.Entry != index)
                            continue;
                        this.cmbPalettes.SelectedItem = pdd;
                        break;
                    }
                }
                currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
                if (currentPal == null)
                    return;
                this.btnResetPalette.Enabled = currentPal.IsChanged();
            }
        }

        private void BtnRemap_Click(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            PaletteDropDownInfo currentPal = this.cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if(currentPal == null)
                return;
            FrmReplaceColor convertPopup = new FrmReplaceColor(currentPal.BitsPerPixel, currentPal.Colors);
            convertPopup.StartPosition = FormStartPosition.CenterParent;
            if (convertPopup.ShowDialog(this) == DialogResult.OK && convertPopup.SelectedIndexSource != convertPopup.SelectedIndexTarget)
            {
                foreach (FontFileSymbol ffs in this.m_LoadedFont.GetAllSymbols())
                {
                    ffs.ReplaceColor((Byte)convertPopup.SelectedIndexSource, (Byte)convertPopup.SelectedIndexTarget);
                }
                this.ReloadImageInfo(true);
                this.ReloadDataGrid(false);
            }
        }

        private void RepaintPreview()
        {
            Image oldImg = this.pxbPreview.Image;
            try
            {
                if (this.m_LoadedFont == null)
                {
                    this.pxbPreview.Image = null;
                    this.pxbPreview.BackColor = Color.Silver;
                    this.pnlImagePreview.Enabled = false;
                    this.pnlImagePreview.BackColor = Color.Silver;
                    return;
                }
                Int32 zoom = (Int32) this.numZoomPreview.Value;
                this.pnlImagePreview.Enabled = true;
                this.pnlImagePreview.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_LoadedFont.TransparencyColor]);
                this.pxbPreview.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[this.m_LoadedFont.TransparencyColor]);
                Image image1 = null;
                Image image2 = null;
                if (this.chkWrapPreview.Checked)
                {
                    // Done three times to prevent scrollbar problems.
                    if (this.pnlImagePreview.VerticalScroll.Visible)
                    {
                        image1 = this.GeneratePreview(String.Empty, 0, true);
                        this.pxbPreview.Image = image1;
                        this.pxbPreview.Size = new Size(image1.Width * zoom, image1.Height * zoom);
                    }
                    image2 = this.GeneratePreview(0, true);
                    this.pxbPreview.Image = image2;
                    this.pxbPreview.Size = new Size(image2.Width * zoom, image2.Height * zoom);
                }
                Image image3 = this.GeneratePreview(this.chkWrapPreview.Checked ? 0 : -1, true);
                this.pxbPreview.Image = image3;
                this.pxbPreview.Size = new Size(image3.Width * zoom, image3.Height * zoom);
                try { if (image1 != null && !ReferenceEquals(image1, this.pxbPreview.Image)) image1.Dispose(); }
                catch { /*ignore*/ }
                try { if (image2 != null && !ReferenceEquals(image2, this.pxbPreview.Image)) image2.Dispose(); }
                catch { /*ignore*/ }
            }
            finally
            {
                if (oldImg != null && !ReferenceEquals(oldImg, this.pxbPreview.Image))
                {
                    try { oldImg.Dispose(); }
                    catch { /*ignore*/ }
                }
            }
        }

        private Bitmap GeneratePreview(Int32 width, Boolean transparentBg)
        {
            return this.GeneratePreview(this.txtPreview.Text, width, transparentBg);
        }

        private Bitmap GeneratePreview(String text, Int32 width, Boolean transparentBg)
        {
            if (this.m_LoadedFont == null)
                return null;
            if (width == 0)
                width = (this.pnlImagePreview.ClientRectangle.Width - this.pnlImagePreview.Padding.Left - this.pnlImagePreview.Padding.Right) / (Int32) this.numZoomPreview.Value;
            Encoding enc = ((EncodingDropDownInfo) this.cmbEncodings.SelectedItem).Encoding;
            return this.m_LoadedFont.PrintText(text, this.m_CurrentPalette, transparentBg, enc, width);
        }

        private void TsmiEditorSettings_Click(Object sender, EventArgs e)
        {
            Int32 oldEditBpp = this.GetEditBpp(this.m_LoadedFont);
            FrmSettings settingsFrm = new FrmSettings(this.m_CustomColors, this.m_Settings);
            settingsFrm.StartPosition = FormStartPosition.CenterParent;
            settingsFrm.ShowDialog(this);
            Boolean refreshSymbols = this.m_LoadedFont != null && oldEditBpp > this.GetEditBpp(this.m_LoadedFont);
            if (refreshSymbols)
            {
                this.AdjustFontSymbolsBpp(this.m_LoadedFont);
            }
            this.m_CustomColors = settingsFrm.CustomColors;
            this.m_DefaultPalettes = this.LoadDefaultPalettes();
            this.RefreshPalettes(true, false);
            if (refreshSymbols)
            {
                this.ReloadUIWithSelection(false);
            }
        }

        private void PreviewImageBox_Click(Object sender, EventArgs e)
        {
            this.pnlImagePreview.Focus();
        }

        private void numZoomPreview_ValueChanged(Object sender, EventArgs e)
        {
            this.RepaintPreview();
        }

        private void chkWrapPreview_CheckedChanged(Object sender, EventArgs e)
        {
            this.RepaintPreview();
        }

        private void txtPreview_TextChanged(Object sender, EventArgs e)
        {
            this.RepaintPreview();
        }

        private void FrmFontEditor_Resize(Object sender, EventArgs e)
        {
            this.RepaintPreview();
        }

        private void CopyPreview(Object sender, EventArgs e)
        {
            this.CopyPreview(false);
        }

        private void CopyPreviewTrans(Object sender, EventArgs e)
        {
            this.CopyPreview(true);
        }

        private void CopyPreview(Boolean asTransparent)
        {
            if (this.m_LoadedFont == null)
                return;
            Clipboard.Clear();
            Color[] noTransPal = this.m_CurrentPalette.ToArray();
            if (noTransPal.Length > this.m_LoadedFont.TransparencyColor)
                noTransPal[this.m_LoadedFont.TransparencyColor] = Color.FromArgb(255, noTransPal[this.m_LoadedFont.TransparencyColor]);
            using (Bitmap prevNoTrans = this.GeneratePreview(this.chkWrapPreview.Checked ? 0 : -1, false))
            using (Bitmap prevTrans = this.GeneratePreview(this.chkWrapPreview.Checked ? 0 : -1, asTransparent))
                ClipboardImage.SetClipboardImage(prevTrans, prevNoTrans, null);
        }

        private void CopyCharacter(Object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            if (this.dgrvSymbolsList.SelectedRows.Count == 0)
                return;
            String selectedIndexChar = (String)this.dgrvSymbolsList.SelectedRows[0].Cells[2].Value;
            Clipboard.Clear();
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, selectedIndexChar);
            Clipboard.SetDataObject(data);
        }

        private void BtnValType_Click(Object sender, EventArgs e)
        {
            this.ChangeFontType(null);
        }

        /// <summary>
        /// Returns true if the conversion succeeded.
        /// </summary>
        /// <param name="targetFontFile"></param>
        /// <returns></returns>
        private Boolean ChangeFontType(FontFile targetFontFile)
        {
            if (this.m_LoadedFont == null)
                return false;
            FontFile sourceFontFile = this.m_LoadedFont;
            if (targetFontFile == null)
            {
                FrmConvertFontType fontConvertDialog = new FrmConvertFontType(this.m_LoadedFont, false);
                fontConvertDialog.StartPosition = FormStartPosition.CenterParent;
                if (fontConvertDialog.ShowDialog(this) != DialogResult.OK)
                    return false;
                targetFontFile = fontConvertDialog.TargetFontFile;
            }
            Byte replaceIndex = 0;
            if (sourceFontFile.HasTooHighDataFor(targetFontFile.BitsPerPixel))
            {
                FrmConvertToLowerBpp convertPopup = new FrmConvertToLowerBpp(false, targetFontFile.BitsPerPixel, this.m_CurrentPalette);
                convertPopup.StartPosition = FormStartPosition.CenterParent;
                if (convertPopup.ShowDialog() != DialogResult.OK)
                    return false;
                replaceIndex = (Byte)convertPopup.SelectedIndex;
            }
            this.m_LoadedFont.CloneInto(targetFontFile, replaceIndex, this.GetEditBpp(targetFontFile));
            this.m_LoadedFont = targetFontFile;
            this.m_LoadedFontBackup = targetFontFile.Clone();
            this.ReloadUIWithSelection(true);
            return true;
        }

        private void TextBoxSelectAll(Object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A)) return;
            if (!(sender is TextBox)) return;
            ((TextBox)sender).SelectAll();
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private void dgrvSymbolsList_CellMouseDown(Object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgrvSender = sender as DataGridView;
            if (e.ColumnIndex == -1 || e.RowIndex == -1 || e.Button != MouseButtons.Right || dgrvSender == null)
                return;
            DataGridViewCell c = dgrvSender[e.ColumnIndex, e.RowIndex];
            if (c.Selected)
                return;
            c.DataGridView.ClearSelection();
            c.DataGridView.CurrentCell = c;
            c.Selected = true;
        }

        private void dgrvSymbolsList_CellContextMenuStripNeeded(Object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            e.ContextMenuStrip = this.m_tsmiCopyGridChar;
        }

        private void FrmFontEditor_FormClosing(Object sender, FormClosingEventArgs e)
        {
            e.Cancel = this.AbortForChangesAskSave(QUESTION_SAVEFILE_CLOSE);
            if(e.Cancel)
                return;
            this.m_LoadedFont = null;
            this.ReloadUi(true);
        }

        private Boolean AbortForChangesAskSave(String question)
        {
            Boolean? saveFile = this.ConfirmOnUnsavedChanges(question, true);
            // abort
            if (!saveFile.HasValue)
                return true;
            // Save
            if (saveFile.Value)
                this.SaveFontFile();
            // Not aborted; either saved or user doesn't care about lost changes.
            return false;
        }

        /// <summary>
        /// Checks if there are unsaved changes, and returns whether the current action should be aborted because of that.
        /// </summary>
        /// <param name="question">Message to give as question in case there are unsaved changes.</param>
        /// <returns>True if the action should be aborted.</returns>
        private Boolean ConfirmOnUnsavedChanges(String question)
        {
            return this.ConfirmOnUnsavedChanges(question, false).GetValueOrDefault(false);
        }

        /// <summary>
        /// Checks if there are unsaved changes, and returns whether the current action should be aborted because of that.
        /// </summary>
        /// <param name="question">Message to give as question in case there are unsaved changes.</param>
        /// <param name="withCancel">Include Cancel in the choices. Will return as Null.</param>
        /// <returns>True if the action should be aborted.</returns>
        private Boolean? ConfirmOnUnsavedChanges(String question, Boolean withCancel)
        {
            if (this.m_LoadedFont == null)
                return false;
            if (this.m_LoadedFontBackup != null && this.m_LoadedFont.Equals(this.m_LoadedFontBackup))
                return false;
            MessageBoxButtons mbb = withCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo;
            DialogResult res = MessageBox.Show(this, question, GetTitle(false), mbb, MessageBoxIcon.Warning);
            if (withCancel && res == DialogResult.Cancel)
                return null;
            return res == DialogResult.Yes;
        }

        private void LblPaintColor1_MouseEnter(Object sender, EventArgs e)
        {
            this.SetColorPickHighlight(this.m_CurrentPaintColor1);
        }

        private void LblPaintColor2_MouseEnter(Object sender, EventArgs e)
        {
            this.SetColorPickHighlight(this.m_CurrentPaintColor2);
        }

        private void LblPaintColor_MouseLeave(Object sender, EventArgs e)
        {
            this.WipeColorPickHighlight();
        }
    }
}
