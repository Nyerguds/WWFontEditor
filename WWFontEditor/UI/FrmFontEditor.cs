using ColorManipulation;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WWFontEditor.Domain;
using Nyerguds.Util.UI;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WWFontEditor.UI;
using Nyerguds.Ini;

namespace WWFontEditor
{
    public partial class FrmFontEditor : Form
    {
        private const Int32 PALETTE_MAX_DIM = 162;//134;
        private const String INI_SECTION = "Palette";
        protected const String QUESTION_OPENNEWFONT = "The font has unsaved changes!\n\nAre you sure you want to close it?";
        protected const String QUESTION_RESETFONT = "This will remove all changes you have made to the font since it was loaded!\n\nAre you sure you want to continue?";
        protected const String QUESTION_EXITPROGRAM = "The font has unsaved changes!\n\nAre you sure you want to exit?";


        private Boolean m_Loading;
        private Boolean m_Clicking;
        private String m_TitleText;
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
                m_FileName = args[0];
        }

        public FrmFontEditor()
        {
            this.m_Loading = true;
            InitializeComponent();
            // Load settings
            m_Settings = new FontEditSettings();
            this.numZoom.Value = this.m_Settings.Zoom;
            this.chkGrid.Checked = this.m_Settings.EnableGrid;
            this.chkOutline.Checked = this.m_Settings.EnableArea;
            this.chkShiftWrap.Checked = this.m_Settings.EnablePixelWrap;

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
            List<D2KEncoding> d2kEncodings = ScanForD2KEncodings();
            encodings.AddRange(d2kEncodings.Select(e => new EncodingDropDownInfo(e)));

            this.cmbEncodings.DataSource = encodings;
            // Select DOS-437 encoding, the one all original C&C fonts are based on.
            this.cmbEncodings.SelectedItem = encodings.Find(e => e.Encoding.CodePage == 437);

            // Colors init.
            this.m_DefaultPalettes = LoadDefaultPalettes();
            this.m_ReadPalettes = LoadExtraPalettes();
            // Default to show on UI at startup: 4bpp palettes
            List<PaletteDropDownInfo> allPalettesForBpp = GetPalettes(4);
            if (allPalettesForBpp.Count == 0)
                allPalettesForBpp.Add(new PaletteDropDownInfo("Rainbow", 4, GetDummyPalette(4), null, -1));
            this.cmbPalettes.DataSource = allPalettesForBpp;

            // PixelBox hierarchy init            
            this.pxbEditGridBehind.Parent = pxbFullSize;
            this.pxbEditGridBehind.BackColor = Color.Transparent;
            this.pxbEditGridBehind.Location = new Point(0, 0);
            this.pxbImage.Parent = pxbFullSize;
            this.pxbImage.BackColor = Color.Transparent;
            this.pxbImage.Location = new Point(0, 0);
            this.pxbImage.BringToFront();
            this.pxbEditGridFront.Parent = pxbImage;
            this.pxbEditGridFront.BackColor = Color.Transparent;
            this.pxbEditGridFront.Location = new Point(0, 0);

            // Set paint colors
            this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor1]);
            this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor2]);

            // Add right click menu to preview pixelbox
            ContextMenu cmCopyPreview = new ContextMenu();
            MenuItem mniCopy = new MenuItem("Copy");
            mniCopy.Click += new EventHandler(CopyPreview);
            cmCopyPreview.MenuItems.Add(mniCopy);
            pxbPreview.ContextMenu = cmCopyPreview;

            // Create right-click menu for toolstrip items
            m_tsmiCopyGridChar = new ContextMenuStrip();
            ToolStripMenuItem mniCopyChar = new ToolStripMenuItem("Copy character", null, new EventHandler(CopyCharacter));
            m_tsmiCopyGridChar.Items.Add(mniCopyChar);

            // Set title
            m_TitleText = "Westwood Font Editor " + GeneralUtils.ProgramVersion() + " - Created by Nyerguds";
            this.Text = m_TitleText;
            this.m_Loading = false;
        }

        private List<D2KEncoding> ScanForD2KEncodings()
        {
            Regex codePageRegex = new Regex("^FONT(\\d+)\\.BIN$");
            String appFolder = Path.GetDirectoryName(Application.ExecutablePath);
            FileInfo[] files = new DirectoryInfo(appFolder).GetFiles("FONT*.BIN");
            List<D2KEncoding> d2kEncodings = new List<D2KEncoding>();
            foreach (FileInfo file in files)
            {
                try
                {
                    if (file.Length == 0x100)
                    {
                        Encoding enc = null;
                        Match m = codePageRegex.Match(file.Name);
                        if (m.Success)
                        {
                            Int32 codepage = Int32.Parse(m.Groups[1].Value);
                            try { enc = Encoding.GetEncoding(codepage); }
                            catch { /* ignore */ }
                            if (!enc.IsSingleByte || !TextUtils.IsAsciiCompatible(enc))
                                enc = null;
                        }
                        Byte[] remapTable = File.ReadAllBytes(file.FullName);
                        d2kEncodings.Add(new D2KEncoding(remapTable, file.Name + " (D2K Encoding)", enc));
                    }
                }
                catch (Exception e)
                {
                    // Should normally never happen: all necessary checks are done in advance.
                    MessageBox.Show(this, string.Format("Loading of file \"{0}\" as Dune 2000 text encoding failed:\n\n{1}", file.Name, e.Message), m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return d2kEncodings;
        }

        private void Frm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Frm_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                String path = files[0];
                String ext = Path.GetExtension(path);
                if (".fnt".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!testUnsavedConfirm(QUESTION_OPENNEWFONT))
                        return;
                    LoadFontFile(path, null);
                }
            }
        }

        private void LoadFontFile(String path, FontFile fontFile)
        {
            this.m_Loading = true;
            try
            {
                this.m_FileName = path;
                String error = null;
                try
                {
                    this.m_LoadedFont = null;
                    Byte[] data = File.ReadAllBytes(path);
                    if (fontFile != null)
                    {
                        try
                        {
                            fontFile.LoadFont(data, false);
                            m_LoadedFont = fontFile;
                        }
                        catch (LoadFailedException e)
                        {
                            m_LoadedFont = null;
                            error = "Could not load font file as " + fontFile.ShortTypeDescription + ":\n\n" + e.Message;
                        }
                    }
                    else
                    {
                        List<LoadFailedException> loadErrors;
                        this.m_LoadedFont = FontFile.LoadFontFile(data, out loadErrors);
                        if (this.m_LoadedFont == null)
                        {
                            String errors = String.Join("\n", loadErrors.Select(er => er.AttemptedLoadedType + ": " + er.Message).ToArray());
                            MessageBox.Show(this, "Font type could not be identified. Errors returned by all attempts:\n\n" + errors, m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    this.m_LoadedFont = null;
                }
                this.m_LoadedFontBackup = this.m_LoadedFont != null ? this.m_LoadedFont.Clone() : null;
                Boolean loadOk = ReloadUi();
                if (!loadOk)
                    MessageBox.Show(this, "Font loading failed" + (error == null ? "." : ": " + error), m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.m_Loading = false;
            }
        }

        private Boolean ReloadUi()
        {
            Boolean wasloading = this.m_Loading;
            this.m_Loading = true;
            Boolean loadOk = this.m_LoadedFont != null;
            this.btnValType.Enabled = loadOk;
            this.numSymbols.Enabled = loadOk && this.m_LoadedFont.SymbolsTypeMin < this.m_LoadedFont.SymbolsTypeMax;
            this.numFontWidth.Enabled = loadOk && this.m_LoadedFont.FontWidthTypeMin < this.m_LoadedFont.FontWidthTypeMax;
            this.numFontHeight.Enabled = loadOk && this.m_LoadedFont.FontHeightTypeMin < this.m_LoadedFont.FontHeightTypeMax;
            this.numWidth.Enabled = loadOk && this.m_LoadedFont.CustomSymbSizesForType && this.m_LoadedFont.FontWidthTypeMin < this.m_LoadedFont.FontWidthTypeMax;
            this.numHeight.Enabled = loadOk && this.m_LoadedFont.CustomSymbSizesForType && this.m_LoadedFont.FontHeightTypeMin < this.m_LoadedFont.FontHeightTypeMax;
            this.numYOffset.Enabled = loadOk && this.m_LoadedFont.YOffsetTypeMax > 0;
            this.btnShiftUp.Enabled = loadOk;
            this.btnShiftLeft.Enabled = loadOk;
            this.btnShiftRight.Enabled = loadOk;
            this.btnShiftDown.Enabled = loadOk;
            this.btnCopy.Enabled = loadOk;
            this.copySymbolToolStripMenuItem.Enabled = loadOk;
            this.btnPaste.Enabled = loadOk;
            this.btnRemap.Enabled = loadOk;
            this.pasteSymbolToolStripMenuItem.Enabled = loadOk;
            this.saveFontToolStripMenuItem.Enabled = loadOk;
            this.saveFontAsToolStripMenuItem.Enabled = loadOk;
            this.revertFontToolStripMenuItem.Enabled = loadOk;
            this.pxbFullSize.Visible = loadOk;
            if (loadOk)
            {
                Int32 oldBpp = -1;
                PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
                if (currentPal != null)
                    oldBpp = currentPal.BitsPerPixel;
                Int32 bpp = m_LoadedFont.BitsPerPixel;
                // Don't reload if it was the same :)
                if (oldBpp == -1 || oldBpp != bpp)
                {
                    this.m_CurrentPaintColor1 = 1;
                    this.m_CurrentPaintColor2 = 0;
                    List<PaletteDropDownInfo> bppPalettes = GetPalettes(bpp);
                    if (bppPalettes.Count == 0)
                        bppPalettes.Add(new PaletteDropDownInfo("Rainbow", bpp, GetDummyPalette(4), null, -1));
                    this.cmbPalettes.DataSource = bppPalettes;
                }
                this.Text = m_TitleText + " - \"" + Path.GetFileName(this.m_FileName) + "\" (" + m_LoadedFont.ShortTypeName + ")";
                this.btnValType.Text = m_LoadedFont.ShortTypeName.Replace("&", "&&");
                this.toolTip1.SetToolTip(this.btnValType, m_LoadedFont.ShortTypeDescription);
                this.numSymbols.Minimum = this.m_LoadedFont.SymbolsTypeMin;
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
            }
            else
            {
                this.m_FileName = null;
                this.Text = m_TitleText;
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
            }
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
            if (loadOk)
            {
                // to allow index changed events on the following piece
                this.m_Loading = false;
                Int32 firstSelected = this.m_Settings.SelectedSymbol;
                if (this.m_LoadedFont.Length <= firstSelected)
                    firstSelected = 0;
                if (loadOk && this.m_LoadedFont.Length > firstSelected)
                {
                    this.dgrvSymbolsList.FirstDisplayedCell = this.dgrvSymbolsList.Rows[firstSelected].Cells[0];
                    this.dgrvSymbolsList.Rows[firstSelected].Cells[0].Selected = true;
                    this.dgrvSymbolsList.Focus();
                }
            }
            this.m_Loading = wasloading;
            return loadOk;
        }

        public static Color[] GetDummyPalette(Int32 bitsPerPixel)
        {
            return ImageUtils.GenerateDoubleRainbow(true, true, false).Entries;
        }

        public List<PaletteDropDownInfo> LoadDefaultPalettes()
        {
            List<PaletteDropDownInfo> palettes = new List<PaletteDropDownInfo>();
            // 1-bit:
            // Not gonna make those customizable. These three ought to do. People can always change the palette to view them in different colours.
            if (this.m_Settings.Generate1BitBR)
                palettes.Add(new PaletteDropDownInfo("Black-Red", 1, new Color[] { Color.FromArgb(0x00, Color.Black), Color.Red }, null, -1));
            if (this.m_Settings.Generate1BitBW)
                palettes.Add(new PaletteDropDownInfo("Black-White", 1, new Color[] { Color.FromArgb(0x00, Color.Black), Color.White }, null, -1));
            if (this.m_Settings.Generate1BitWB)
                palettes.Add(new PaletteDropDownInfo("White-Black", 1, new Color[] { Color.FromArgb(0x00, Color.White), Color.Black }, null, -1));
            // 4-bit and 8-bit
            if (this.m_Settings.Generate4BitRainbow)
                //palettes.Add(new PaletteDropDownInfo("Rainbow", 4, PaletteRainbow, null, -1));
                palettes.Add(new PaletteDropDownInfo("Rainbow", 4, ImageUtils.GenerateRainbowPalette(4, false, true, true, false).Entries, null, -1));
            if (this.m_Settings.Generate4BitWindows)
                palettes.Add(new PaletteDropDownInfo("Windows palette", 4, ImageUtils.GenerateDefFourBitPalette(true, false).Entries, null, -1));
            if (this.m_Settings.Generate4BitBW)
                palettes.Add(new PaletteDropDownInfo("Grayscale B->W", 4, ImageUtils.GenerateGrayPalette(PixelFormat.Format4bppIndexed, true, false).Entries, null, -1));
            if (this.m_Settings.Generate4BitWB)
                palettes.Add(new PaletteDropDownInfo("Grayscale W->B", 4, ImageUtils.GenerateGrayPalette(PixelFormat.Format4bppIndexed, true, true).Entries, null, -1));
            if (this.m_Settings.Generate8BitRainbow)
                palettes.Add(new PaletteDropDownInfo("Rainbow", 8, ImageUtils.GenerateDoubleRainbow(true, true, false).Entries, null, -1));
            if (this.m_Settings.Generate8BitWindows)
                palettes.Add(new PaletteDropDownInfo("Windows palette", 8, ImageUtils.GenerateRainbowPalette(8, true, false, true, false).Entries, null, -1));
            if (this.m_Settings.Generate8BitBW)
                palettes.Add(new PaletteDropDownInfo("Grayscale B->W", 8, ImageUtils.GenerateGrayPalette(PixelFormat.Format8bppIndexed, true, false).Entries, null, -1));
            if (this.m_Settings.Generate8BitWB)
                palettes.Add(new PaletteDropDownInfo("Grayscale W->B", 8, ImageUtils.GenerateGrayPalette(PixelFormat.Format8bppIndexed, true, true).Entries, null, -1));
            return palettes;
        }

        public List<PaletteDropDownInfo> LoadExtraPalettes()
        {
            List<PaletteDropDownInfo> palettes = new List<PaletteDropDownInfo>();
            String appFolder = Path.GetDirectoryName(Application.ExecutablePath);
            FileInfo[] files = new DirectoryInfo(appFolder).GetFiles("*.pal");
            foreach (FileInfo file in files)
                palettes.AddRange(LoadInfoFromPalette(file));
            return palettes;
        }

        private List<PaletteDropDownInfo> LoadInfoFromPalette(FileInfo file)
        {
            List<PaletteDropDownInfo> palettes = new List<PaletteDropDownInfo>();
            try
            {
                if (file.Length == 0x300)
                {
                    // Treat as C&C 6-bit colour palette
                    SixBitColor[] pal = ColorUtils.ReadSixBitPaletteFile(file.FullName);
                    Color[] fullPal = ColorUtils.GetEightBitColorPalette(pal);

                    String path = file.FullName.Substring(0, file.FullName.LastIndexOf(".", StringComparison.Ordinal) + 1);
                    String bareName = file.Name.Substring(0, file.Name.LastIndexOf(".", StringComparison.Ordinal));
                    String inipath = path + "ini";
                    if (File.Exists(inipath))
                    {
                        IniFile paletteConfig = new IniFile(inipath);
                        Boolean generateDefault = !paletteConfig.GetSectionNames().Contains(INI_SECTION);
                        for (Int32 i = 0; i < 16; i++)
                        {
                            String name = generateDefault ? bareName + "#" + i : paletteConfig.GetStringValue("Palette", i.ToString(), null);
                            if (!String.IsNullOrEmpty(name))
                            {
                                Color[] subPalette = new Color[16];
                                Array.Copy(fullPal, i * 16, subPalette, 0, 16);
                                if (subPalette.All(x => x.R == 0 && x.G == 0 && x.B == 0))
                                    subPalette = GetDummyPalette(4).ToArray();
                                subPalette[0] = Color.FromArgb(0x00, subPalette[0]);
                                palettes.Add(new PaletteDropDownInfo(name + " (from " + bareName + ")", 4, subPalette, file.Name, i));
                            }
                        }
                    }
                    else
                    {
                        fullPal[0] = Color.FromArgb(0x00, fullPal[0]);
                        palettes.Add(new PaletteDropDownInfo(file.Name, 8, fullPal, file.Name, 0));
                        // add as one 256 colour palette
                    }
                }
            }
            catch { /* ignore and continue */ }
            return palettes;
        }

        public List<PaletteDropDownInfo> GetPalettes(Int32 bpp)
        {
            List<PaletteDropDownInfo> allPalettes = m_DefaultPalettes.Where(p => p.BitsPerPixel == bpp).ToList();
            allPalettes.AddRange(this.m_ReadPalettes.Where(p => p.BitsPerPixel == bpp));
            return allPalettes;
        }

        public static void InitPaletteControl(Int32 bitsPerPixel, PalettePanel palPanel, Color[] palette, Int32 maxDimension)
        {
            Int32 colors = (Int32)Math.Pow(2, bitsPerPixel);
            palPanel.MaxColors = colors;
            Int32 squaresPerRow = (Int32)Math.Sqrt(colors);
            Int32 squaresPerCol = colors / squaresPerRow + ((colors % squaresPerRow) > 0 ? 1 : 0);
            squaresPerRow = Math.Max(squaresPerRow, squaresPerCol);
            Int32 sqrWidth = (Int32)Math.Ceiling(maxDimension * 7.5 / 8.5 / squaresPerRow);
            Int32 padding = (Int32)Math.Max(1, Math.Round(sqrWidth / 8.5));
            while (maxDimension < squaresPerRow * sqrWidth + (squaresPerRow - 1) * padding)
            {
                sqrWidth--;
                padding = (Int32)Math.Max(1, Math.Ceiling(sqrWidth / 8.5));
            }
            palPanel.ColorTableWidth = squaresPerRow;
            palPanel.LabelSize = new Size(sqrWidth, sqrWidth);
            palPanel.PadBetween = new Point(padding, padding);
            palPanel.Palette = palette;
        }

        private void ReloadDataGrid()
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            try
            {
                if (m_LoadedFont == null)
                {
                    this.dgrvSymbolsList.DataSource = null;
                    return;
                }
                // add as param later
                Encoding enc = ((EncodingDropDownInfo)cmbEncodings.SelectedItem).Encoding;
                ColorPalette palette = ImageUtils.MakePalette(m_CurrentPalette, m_LoadedFont.BitsPerPixel, false);
                palette.Entries[0] = Color.FromArgb(0xFF, palette.Entries[0]);
                Bitmap dummyImage = ImageUtils.GenerateBlankImage(5, 5, new Color[] { Color.Transparent }, 0);
                Int32 selectedIndex = 0;
                Int32 scrollOffset = 0;
                if (this.dgrvSymbolsList.Rows.Count > 0 && this.dgrvSymbolsList.CurrentCell != null)
                {
                    selectedIndex = this.dgrvSymbolsList.CurrentCell.RowIndex;
                    scrollOffset = this.dgrvSymbolsList.VerticalScrollbarOffset;
                }
                DataTable symbolsTable = new DataTable("Symbols");
                symbolsTable.Columns.Add(new DataColumn("Hex", typeof(String)));
                symbolsTable.Columns.Add(new DataColumn("Dec", typeof(Int32)));
                symbolsTable.Columns.Add(new DataColumn("Char", typeof(String)));
                symbolsTable.Columns.Add(new DataColumn("Pic", typeof(Bitmap)));
                FontFileSymbol[] allSymbols = m_LoadedFont.GetAllSymbols();
                for (Int32 i = 0; i < allSymbols.Length; i++)
                {
                    FontFileSymbol symbol = allSymbols[i];
                    DataRow row = symbolsTable.NewRow();
                    row[0] = "0x" + i.ToString("X2");
                    row[1] = i;
                    row[2] = enc.GetString(new Byte[] { (Byte)i });
                    Bitmap bm = symbol.GetBitmapFullSize(palette, m_LoadedFont);
                    if (bm == null)
                        bm = dummyImage;
                    row[3] = bm;
                    symbolsTable.Rows.Add(row);
                }
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[0]);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                this.dgrvSymbolsList.DataSource = symbolsTable;
                this.dgrvSymbolsList.Columns[3].DefaultCellStyle = style;
                if (selectedIndex < symbolsTable.Rows.Count)
                {
                    if (selectedIndex > 0)
                        this.dgrvSymbolsList.VerticalScrollbarOffset = scrollOffset;
                    this.dgrvSymbolsList.Rows[selectedIndex].Cells[0].Selected = true;
                }
            }
            finally
            {
                m_Loading = wasLoading;
            }
        }

        private Boolean SaveFontFile(String fileName)
        {
            if (this.m_LoadedFont == null)
                return false;
            try
            {
                Byte[] filedata = this.m_LoadedFont.SaveFont();
                File.WriteAllBytes(fileName, filedata);
                this.m_LoadedFontBackup = this.m_LoadedFont.Clone();
                this.m_FileName = fileName;
                this.Text = m_TitleText + " - \"" + Path.GetFileName(this.m_FileName) + "\" (" + m_LoadedFont.ShortTypeName + ")";
                this.revertSymbolToolStripMenuItem.Enabled = false;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Error occurred when saving:\n\n" + e.Message, m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void FrmFontEditor_Shown(object sender, EventArgs e)
        {
            if (m_FileName != null)
                LoadFontFile(m_FileName, null);
        }

        private void ReloadImageInfo(Boolean refreshEditor)
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            try
            {
                Int32 curIndex = GetSelectedIndex();
                if (this.m_LoadedFont == null)
                {
                    pxbImage.Image = null;
                    m_CurHeight = 0;
                    m_CurWidth = 0;
                    m_CurYOffset = 0;
                    numHeight.Maximum = 0;
                    numHeight.Value = 0;
                    numWidth.Maximum = 0;
                    numWidth.Value = 0;
                    numYOffset.Value = 0;
                    this.RepaintPreview();
                    return;
                }
                pxbImage.Image = this.m_LoadedFont.GetBitmap(curIndex, this.m_CurrentPalette, true);
                m_CurHeight = this.m_LoadedFont.GetSymbolHeight(curIndex);
                m_CurWidth = this.m_LoadedFont.GetSymbolWidth(curIndex);
                m_CurYOffset = this.m_LoadedFont.GetSymbolYOffset(curIndex);
                numHeight.Maximum = this.m_LoadedFont.FontHeight;
                numHeight.Value = m_CurHeight;
                numWidth.Maximum = this.m_LoadedFont.FontWidth;
                numWidth.Value = m_CurWidth;
                this.numYOffset.Value = m_CurYOffset;
                this.AdjustRevertButton();
                this.RepaintPreview();
                if (refreshEditor)
                    this.RefreshEditor();
            }
            finally
            {
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

        private void NumZoom_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.RefreshEditor();
        }

        private void RefreshEditor()
        {
            Boolean wasLoading = this.m_Loading;
            this.m_Loading = true;
            try
            {
                // Beware! Heavy grid logic abound!
                Bitmap bm = (Bitmap)pxbImage.Image;

                // False if no actual image data loaded.
                Boolean imgLoadOk = bm != null && this.m_CurWidth != 0 && this.m_CurHeight != 0;
                Boolean fntLoadOk = this.m_LoadedFont != null;
                Int32 zoom = (Int32)numZoom.Value;
                Boolean drawGrid = chkGrid.Checked;
                Boolean drawOutline = chkOutline.Checked;
                // AddGred means some kind of grid overlay needs to be drawn; either the grid itself or the outline.
                Boolean addGrid = zoom > 4 && (drawGrid || drawOutline);
                pxbImage.Visible = imgLoadOk | addGrid;
                pxbImage.Location = new Point(0, m_CurYOffset * zoom);
                pxbImage.Width = Math.Max(this.m_CurWidth * zoom, 1);
                pxbImage.Height = Math.Max(this.m_CurHeight * zoom, 1);
                Bitmap gridImageSmall = null;
                if (fntLoadOk && addGrid)
                {
                    //Draw normal grid, with or without special outline
                    Color[] palette = new Color[] { Color.Transparent, Color.Black, drawOutline ? m_Settings.EditAreaGrid : m_Settings.BackgroundGrid, m_Settings.EditAreaFrame };
                    gridImageSmall = ImageUtils.GenerateGridImage(m_CurWidth, m_CurHeight, zoom, palette, 0, drawGrid ? (Byte)2 : (Byte)0, drawOutline ? (Byte)3 : (Byte)2);
                    if (!drawOutline)
                    {
                        // If outline is disabled, restore any edges touching the full size edges to the grid colour of the outside grid.
                        ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, 0, this.m_CurHeight * zoom, 1, true); // left line
                        if (this.m_CurYOffset == 0)
                            ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, m_CurWidth * zoom, 0, 1, true); // top line
                        if (this.m_CurHeight + this.m_CurYOffset == this.m_LoadedFont.FontHeight)
                            ImageUtils.DrawRect8Bit(gridImageSmall, 0, m_CurHeight * zoom, m_CurWidth * zoom, m_CurHeight * zoom, 1, true); // bottom line
                        if (this.m_CurWidth == this.m_LoadedFont.FontWidth)
                            ImageUtils.DrawRect8Bit(gridImageSmall, m_CurWidth * zoom, 0, m_CurWidth * zoom, m_CurHeight * zoom, 1, true); // right line
                    }
                }
                pxbEditGridBehind.Visible = fntLoadOk && addGrid;
                pxbEditGridBehind.Location = new Point(0, m_CurYOffset * zoom);
                pxbEditGridBehind.Width = Math.Max(this.m_CurWidth * zoom + 1, 1);
                pxbEditGridBehind.Height = Math.Max(this.m_CurHeight * zoom + 1, 1);
                pxbEditGridBehind.Image = gridImageSmall;
                pxbEditGridFront.Visible = true;
                // Parent of pxbImage; no change needed.
                //pxbEditGridFront.Location = new Point(0, curYOffset * zoom);
                pxbEditGridFront.BackColor = Color.Transparent;
                pxbEditGridFront.BackgroundImage = addGrid ? gridImageSmall : null;
                pxbEditGridFront.Width = Math.Max(this.m_CurWidth * zoom, 1);
                pxbEditGridFront.Height = Math.Max(this.m_CurHeight * zoom, 1);

                //pxbEditGridFront.Image is the overlay image on which the currently hovered pixel is drawn. Make it null if one of the dimensions is 0.
                if (imgLoadOk)
                    this.WipeEditGridFront();
                else
                    pxbEditGridFront.Image = null;
                pxbFullSize.Visible = fntLoadOk;
                if (fntLoadOk)
                {
                    Int32 bgWidth = this.m_LoadedFont.FontWidth * zoom;
                    Int32 bgHeight = this.m_LoadedFont.FontHeight * zoom;
                    Int32 addedHeight = this.m_CurHeight + this.m_CurYOffset - this.m_LoadedFont.FontHeight;
                    Color bgColor = this.m_Settings.UsePaletteBG ? Color.FromArgb(0xFF, this.m_CurrentPalette[0]) : m_Settings.Background;
                    if (addGrid && drawGrid)
                    {
                        pxbFullSize.Image = ImageUtils.GenerateGridImage(this.m_LoadedFont.FontWidth, this.m_LoadedFont.FontHeight, zoom,
                            new Color[] { bgColor, m_Settings.BackgroundGrid, m_Settings.BackgroundFrame }, 0, 1, 2);
                        // an extra one-pixel border has been added at the bottom and right edges.
                        bgWidth++;
                        bgHeight++;
                    }
                    else
                    {
                        // No extra border since it'll deform the image
                        // ... except if the outline is drawn
                        if (drawOutline && addGrid && m_CurWidth == this.m_LoadedFont.FontWidth)
                            bgWidth++;
                        if (drawOutline && addGrid && (m_CurHeight + m_CurYOffset == this.m_LoadedFont.FontHeight || addedHeight > 0))
                            bgHeight++;
                        pxbFullSize.Image = ImageUtils.GenerateBlankImage(bgWidth, bgHeight, new Color[] { bgColor }, 0);
                        pxbFullSize.BackColor = bgColor;
                    }
                    pxbFullSize.Width = bgWidth;
                    pxbFullSize.Height = addedHeight > 0 ? bgHeight + (addedHeight * zoom) : bgHeight;
                }
            }
            finally
            {
                this.m_Loading = wasLoading;
            }
        }

        private void ImageBox_Click(object sender, MouseEventArgs e)
        {
            pnlImageScroll.Focus();
        }

        private void CheckboxGridOptionChanged(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            this.RefreshEditor();
        }

        private void pxbEditGridFront_MouseMove(object sender, MouseEventArgs e)
        {
            CheckMouse(sender, e, this.chkPaint.Checked);
        }

        private void pxbEditGridFront_MouseDown(object sender, MouseEventArgs e)
        {
            pnlImageScroll.Focus();
            // prevents bug where the closing click of a dialog is seen as valid mouse-up event on the edit grid
            m_Clicking = (e.Button & MouseButtons.Left) != 0 || (e.Button & MouseButtons.Right) != 0;
            CheckMouse(sender, e, false);
        }

        private void pxbEditGridFront_MouseUp(object sender, MouseEventArgs e)
        {
            m_Clicking = false;
            if ((e.Button & MouseButtons.Left) != 0 || (e.Button & MouseButtons.Right) != 0)
            {
                ReloadDataGrid();
                this.RepaintPreview();
                this.AdjustRevertButton();
            }
        }

        private void pxbEditGridFront_MouseLeave(object sender, EventArgs e)
        {
            m_Clicking = false;
            this.WipeEditGridFront();
            this.toolTip1.SetToolTip(this.pxbEditGridFront, null);
            this.palColorSelector.TransItemCharColor = Color.Blue;
            this.palColorSelector.ColorSelectMode = ColorSelMode.None;
            this.m_LastHoverPixelX = -1;
            this.m_LastHoverPixelY = -1;
        }

        private void CheckMouse(object sender, MouseEventArgs e, Boolean drawPreviewPixel)
        {
            Bitmap gridFront = this.pxbEditGridFront.Image as Bitmap;
            if (gridFront == null || this.m_LoadedFont == null)
                return;
            Int32 picX = e.X / (Int32)this.numZoom.Value;
            Int32 picY = e.Y / (Int32)this.numZoom.Value;
            // Optimize by aborting immediately if location is unchanged
            Boolean inBounds = picX >= 0 && picX < gridFront.Width && picY >= 0 && picY < gridFront.Height;
            Boolean hasntMoved = m_LastHoverPixelX == picX && m_LastHoverPixelY == picY;
            Boolean isLeftClick = (e.Button & MouseButtons.Left) != 0;
            Boolean isRightClick = (e.Button & MouseButtons.Right) != 0;
            if (hasntMoved && !isLeftClick && !isRightClick)
                return;
            if (drawPreviewPixel && !hasntMoved)
            {
                // Clear previous pixel
                if (m_LastHoverPixelX != -1 && m_LastHoverPixelY != -1)
                    ImageUtils.DrawRect8Bit(gridFront, m_LastHoverPixelX, m_LastHoverPixelY, m_LastHoverPixelX, m_LastHoverPixelY, 0, true);
                // set color, just in case it changed.
                if (m_CurrentPalette.Length > this.m_CurrentPaintColor1)
                    gridFront.Palette.Entries[1] = m_CurrentPalette[this.m_CurrentPaintColor1];
                // Draw new pixel
                if (inBounds)
                    ImageUtils.DrawRect8Bit(gridFront, picX, picY, picX, picY, 1, true);
                pxbEditGridFront.Invalidate();
            }
            this.m_LastHoverPixelX = picX;
            this.m_LastHoverPixelY = picY;
            if (!inBounds)
                return;
            Int32 curIndex = GetSelectedIndex();
            if (chkPaint.Checked)
            {
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
                        MessageBox.Show(this, ex.Message, m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (this.chkPicker.Checked)
            {
                Byte val = this.m_LoadedFont.GetSymbol(curIndex).GetPixelValue(picX, picY);
                // if the label is too small, remove the letter on it to more clearly show the colour.
                if (val == 0 && palColorSelector.LabelSize.Width < 10)
                    this.palColorSelector.TransItemCharColor = Color.Empty;
                else
                    this.palColorSelector.TransItemCharColor = Color.Blue;
                this.palColorSelector.ColorSelectMode = ColorSelMode.Single;
                this.palColorSelector.SelectedIndices = new Int32[] { val };
                Color c = this.m_CurrentPaintColor1 < m_CurrentPalette.Length ? m_CurrentPalette[val] : Color.Black;
                String toolTip = String.Format("#{0} ({1},{2},{3})", val, c.R, c.G, c.B);
                this.toolTip1.SetToolTip(this.pxbEditGridFront, toolTip);
                if (m_Clicking)
                {
                    if (isLeftClick)
                    {
                        this.m_CurrentPaintColor1 = val;
                        lblPaintColor1.BackColor = Color.FromArgb(0xFF, c);
                        // Since the grid only shows edit color 1, it's only needed for Left button.
                        this.WipeEditGridFront();
                    }
                    else if (isRightClick)
                    {
                        this.m_CurrentPaintColor2 = val;
                        lblPaintColor2.BackColor = Color.FromArgb(0xFF, c);
                    }
                }
            }

        }

        private Boolean CheckIsEqual()
        {
            return CheckIsEqual(GetSelectedIndex());
        }

        private Boolean CheckIsEqual(Int32 index)
        {
            if (m_LoadedFont == null || this.m_LoadedFontBackup == null)
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
            if (m_LoadedFont == null || this.m_LoadedFontBackup == null)
                return false;
            Int32 index = GetSelectedIndex();
            FontFileSymbol rawData1 = this.m_LoadedFont.GetSymbol(index);
            FontFileSymbol rawData2 = this.m_LoadedFontBackup.GetSymbol(index);
            if (rawData1 == null || rawData2 == null)
                return false;
            // they're the same; can't revert.
            if (CheckIsEqual(index))
                return false;
            // different dimensions; can't revert. Would never be equal to original.
            if (!m_LoadedFont.CustomSymbSizesForType
                && (m_LoadedFont.FontWidth != m_LoadedFontBackup.FontWidth || m_LoadedFont.FontHeight != m_LoadedFontBackup.FontHeight))
                return false;
            if (m_LoadedFont.FontWidth < rawData2.Width || m_LoadedFont.FontHeight < rawData2.Height)
                return false;
            return true;
        }

        private Boolean AdjustRevertButton()
        {
            Boolean enable = CheckCanRevert();
            this.revertSymbolToolStripMenuItem.Enabled = enable;
            return enable;
        }

        /// <summary>
        /// Regenerates the preview pixel image drawn on top of the front edit grid
        /// to get a blank slate with the correct preview pixel color set.
        /// </summary>
        private void WipeEditGridFront()
        {
            Color col = this.m_CurrentPaintColor1 < m_CurrentPalette.Length ? m_CurrentPalette[this.m_CurrentPaintColor1] : Color.Black;
            Color paintColor = Color.FromArgb(0xFF, col);
            pxbEditGridFront.Image = ImageUtils.GenerateBlankImage(this.m_CurWidth, this.m_CurHeight, new Color[] { Color.Transparent, paintColor }, 0);
        }

        private void palColorSelector_ColorLabelMouseClick(object sender, PaletteClickEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                this.m_CurrentPaintColor1 = (Byte)(e.Index & 0xFF);
                lblPaintColor1.BackColor = Color.FromArgb(0xFF, e.Color);
                // Since the grid only shows edit color 1, it's only needed for Left button.
                this.WipeEditGridFront();
            }
            if ((e.Button & MouseButtons.Right) != 0)
            {
                this.m_CurrentPaintColor2 = (Byte)(e.Index & 0xFF);
                lblPaintColor2.BackColor = Color.FromArgb(0xFF, e.Color);
            }
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("This will revert the current edits on this\nsymbol image to their original state!\n\nAre you sure you want to continue?", m_TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes)
                return;
            this.m_LoadedFont.RestorePicFromBackup(GetSelectedIndex(), this.m_LoadedFontBackup);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
            this.pnlImageScroll.Focus();
        }

        private void NumYOffset_ValueChanged(object sender, EventArgs e)
        {
            if (m_Loading)
                return;
            this.m_LoadedFont.GetSymbol(GetSelectedIndex()).YOffset = (Byte)this.numYOffset.Value;
            this.ReloadImageInfo(true);
        }

        private void CmbEncodings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Loading)
                return;
            ReloadDataGrid();
            RepaintPreview();
        }

        private void DgrvSymbolsList_SelectionChanged(object sender, EventArgs e)
        {
            if (m_Loading)
                return;
            ReloadImageInfo(true);
        }

        private void PalColorSelector_ColorLabelMouseDoubleClick(object sender, PaletteClickEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            PalettePanel palpanel = (PalettePanel)sender;
            Int32 colindex = e.Index;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = e.Color;
            cdl.FullOpen = true;
            cdl.CustomColors = this.m_CustomColors;
            DialogResult res = cdl.ShowDialog(this);
            this.m_CustomColors = cdl.CustomColors;
            if (res == DialogResult.OK)
            {
                Color paletteColor = Color.FromArgb(colindex == 0 ? 0x00 : 0xFF, cdl.Color);
                m_CurrentPalette[colindex] = paletteColor;

                ((PalettePanel)sender).Palette[colindex] = paletteColor;
                palpanel.Invalidate();
                if (colindex == m_CurrentPaintColor1)
                    lblPaintColor1.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[m_CurrentPaintColor1]);
                if (colindex == m_CurrentPaintColor2)
                    lblPaintColor2.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[m_CurrentPaintColor2]);
                ReloadDataGrid();
                ReloadImageInfo(true);
                PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
                this.btnResetPalette.Enabled = currentPal != null && currentPal.IsChanged();
            }
        }

        private void OpenFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!testUnsavedConfirm(QUESTION_OPENNEWFONT))
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            FontFileDialogItem[] items = FontFile.SupportedTypes.Select(x => new FontFileDialogItem(x)).ToArray();
            ofd.Filter = FontFileDialogItem.GetFileFilter(items, true);
            //ofd.FilterIndex = 1; // "all supported files". One-based for some fucked up reason.
            //"Westwood font files (*.fnt)|*.fnt|All Files (*.*)|*.*";
            ofd.InitialDirectory = String.IsNullOrEmpty(m_FileName) ? Path.GetFullPath(".") : Path.GetDirectoryName(m_FileName);
            //ofd.FilterIndex
            DialogResult res = ofd.ShowDialog(this);
            if (res != System.Windows.Forms.DialogResult.OK)
                return;
            Int32 index = ofd.FilterIndex - 2;
            FontFile selectedItem = index >= 0 && index < items.Length ? items[ofd.FilterIndex - 2].FontTypeObject : null;
            LoadFontFile(ofd.FileName, selectedItem);
        }

        private void SaveFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            if (this.m_LoadedFontBackup.GetType() != this.m_LoadedFont.GetType())
                SaveFontAs();
            else
                SaveFontFile(this.m_FileName);
        }


        private void SaveFontAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            SaveFontAs();
        }

        private Boolean SaveFontAs()
        {
            if (this.m_LoadedFont == null)
                return false;
            SaveFileDialog sfd = new SaveFileDialog();
            FontFileDialogItem[] items = FontFile.SupportedTypes.Select(x => new FontFileDialogItem(x)).ToArray();
            Int32 filterIndex;
            for (filterIndex = 0; filterIndex < items.Length; filterIndex++)
                if (m_LoadedFont.GetType() == items[filterIndex].FontType)
                    break;
            filterIndex++;
            sfd.Filter = FontFileDialogItem.GetFileFilter(items, false);
            sfd.FilterIndex = filterIndex;
            //sfd.Filter = "Westwood font file (*.fnt)|*.fnt";
            sfd.InitialDirectory = String.IsNullOrEmpty(m_FileName) ? Path.GetFullPath(".") : Path.GetDirectoryName(m_FileName);
            if (!String.IsNullOrEmpty(m_FileName))
                sfd.FileName = Path.GetFileName(m_FileName);
            DialogResult res = sfd.ShowDialog(this);
            if (res != System.Windows.Forms.DialogResult.OK)
                return false;
            if (sfd.FilterIndex != filterIndex)
            {
                if (!ChangeFontType(items[sfd.FilterIndex - 1].FontTypeObject))
                    return false;
            }
            return SaveFontFile(sfd.FileName);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Add changes check?
            Application.Exit();
        }

        private void ShiftCurrentImage(ShiftDirection shiftDirection, Boolean all)
        {
            if (this.m_LoadedFont == null)
                return;
            if (all)
            {
                foreach (FontFileSymbol ffs in this.m_LoadedFont.GetAllSymbols())
                    ffs.ShiftImageData(shiftDirection, chkShiftWrap.Checked);
            }
            else
            {
                Int32 curIndex = GetSelectedIndex();
                FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(curIndex);
                if (symbol == null)
                    return;
                symbol.ShiftImageData(shiftDirection, chkShiftWrap.Checked);
            }
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }

        private void BtnShiftUp_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Up, Control.ModifierKeys == Keys.Shift);
        }
        private void BtnShiftRight_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Right, Control.ModifierKeys == Keys.Shift);
        }

        private void BtnShiftDown_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Down, Control.ModifierKeys == Keys.Shift);
        }

        private void BtnShiftLeft_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Left, Control.ModifierKeys == Keys.Shift);
        }

        private void ChangeCurrentImageDimension(Byte newDimension, Boolean isHeight)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileSymbol symbol = this.m_LoadedFont.GetSymbol(curIndex);
            if (symbol == null)
                return;
            if (isHeight)
                symbol.ChangeHeight(newDimension);
            else
                symbol.ChangeWidth(newDimension);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }

        private void NumWidth_ValueChanged(object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Byte)this.numWidth.Value, false);
        }

        private void NumHeight_ValueChanged(object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Byte)this.numHeight.Value, true);
        }

        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // override of menu shortcuts to allow copying and pasting text in the preview text field.
            Boolean isCtrlC = keyData == (Keys.Control | Keys.C);
            Boolean isCtrlV = keyData == (Keys.Control | Keys.V);
            if (this.ActiveControl is TextBox && (isCtrlC || isCtrlV))
            {
                if (isCtrlC)
                    Clipboard.SetText(((TextBox)this.ActiveControl).SelectedText);
                else if (isCtrlV)
                    ((TextBox)this.ActiveControl).SelectedText = Clipboard.GetText();
                return true;
            }
            if (this.m_LoadedFont != null && (keyData & Keys.Control) != 0
                && ((keyData & Keys.Up) != 0 || (keyData & Keys.Left) != 0 || (keyData & Keys.Right) != 0 || (keyData & Keys.Down) != 0)
                && !(this.ActiveControl is DataGridView) && !(this.ActiveControl is NumericUpDown) && !(this.ActiveControl is TextBox))
            {
                Boolean processAll = (keyData & Keys.Shift) != 0;
                ShiftDirection sd = ShiftDirection.Up;
                Boolean doShift = true;
                if (keyData == (Keys.Control | Keys.Up) || keyData == (Keys.Control | Keys.Shift | Keys.Up))
                    sd = ShiftDirection.Up;
                else if (keyData == (Keys.Control | Keys.Left) || keyData == (Keys.Control | Keys.Shift | Keys.Left))
                    sd = ShiftDirection.Left;
                else if (keyData == (Keys.Control | Keys.Right) || keyData == (Keys.Control | Keys.Shift | Keys.Right))
                    sd = ShiftDirection.Right;
                else if (keyData == (Keys.Control | Keys.Down) || keyData == (Keys.Control | Keys.Shift | Keys.Down))
                    sd = ShiftDirection.Down;
                else
                    doShift = false;
                if (doShift)
                {
                    if (processAll)
                        foreach (FontFileSymbol ffs in this.m_LoadedFont.GetAllSymbols())
                            ffs.ShiftImageData(sd, chkShiftWrap.Checked);
                    else
                    {
                        Int32 selectedIndex = GetSelectedIndex();
                        FontFileSymbol ffs = this.m_LoadedFont.GetSymbol(selectedIndex);
                        ffs.ShiftImageData(sd, chkShiftWrap.Checked);
                    }
                    this.ReloadImageInfo(true);
                    this.ReloadDataGrid();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileSymbol ffs = this.m_LoadedFont.GetSymbol(curIndex);
            if (ffs == null)
                return;
            Clipboard.Clear();
            DataObject data = new DataObject();
            ColorPalette palette = ImageUtils.MakePalette(m_CurrentPalette, m_LoadedFont.BitsPerPixel, false);
            palette.Entries[0] = Color.FromArgb(0xFF, palette.Entries[0]);
            data.SetData(DataFormats.Bitmap, (Object)ffs.GetBitmapFullSize(palette, m_LoadedFont));
            data.SetData(ffs.Clone());
            Clipboard.SetDataObject(data);
        }

        private void BtnPaste_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();
            FontFileSymbol clipboard = null;
            if (retrievedData.GetDataPresent(typeof(FontFileSymbol)))
            {
                clipboard = retrievedData.GetData(typeof(FontFileSymbol)) as FontFileSymbol;
            }
            else if (retrievedData.GetDataPresent(DataFormats.Bitmap))
            {
                Image srcImage = retrievedData.GetData(DataFormats.Bitmap) as Image;
                clipboard = new FontFileSymbol(srcImage, this.m_CurrentPalette, this.m_LoadedFont);
            }
            else
            {
                MessageBox.Show("No font data found on the clipboard.", m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (clipboard == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileSymbol fc = this.m_LoadedFont.GetSymbol(curIndex);
            if (fc == null)
                return;
            Boolean canrevert = this.AdjustRevertButton();
            // if there are unsaved changes, or the image is new and not empty, ask specifically
            if (!CheckIsEqual() && !canrevert || (this.m_LoadedFontBackup.Length <= curIndex && fc.ByteData.Length > 0))
            {
                DialogResult dr = MessageBox.Show("This will completely overwrite the current symbol.\n\nAre you sure you want to continue?", m_TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }
            try
            {
                fc = clipboard.CloneFor(this.m_LoadedFont);
            }
            catch (InvalidOperationException)
            {
                FrmConvertToLowerBpp convertPopup = new FrmConvertToLowerBpp(true, this.m_LoadedFont.BitsPerPixel, this.m_CurrentPalette);
                convertPopup.StartPosition = FormStartPosition.CenterParent;
                if (convertPopup.ShowDialog() == DialogResult.OK)
                {
                    fc = clipboard.CloneFor(this.m_LoadedFont, (Byte)convertPopup.SelectedIndex);
                }
            }
            //fc = this.m_Clipboard.Clone();
            if (fc.Height > this.m_LoadedFont.FontHeight)
                fc.ChangeHeight(this.m_LoadedFont.FontHeight);
            if (fc.Width > this.m_LoadedFont.FontWidth)
                fc.ChangeWidth(this.m_LoadedFont.FontWidth);
            try
            {
                this.m_LoadedFont.RestorePicFromBackup(curIndex, fc);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }

        private void NumSymbols_ValueChanged(object sender, EventArgs e)
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
                this.ReloadDataGrid();
                if (newLen > 0)
                {
                    this.dgrvSymbolsList.Rows[newLen - 1].Cells[0].Selected = true;
                    this.dgrvSymbolsList.FirstDisplayedCell = this.dgrvSymbolsList.Rows[newLen - 1].Cells[0];
                }
                this.ReloadImageInfo(true);
            }
            finally
            {
                this.m_Loading = false;
            }
        }

        private void NumFontWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontWidth.Value, 0xFF);
            m_LoadedFont.FontWidth = newVal;
            this.ReloadDataGrid();
            this.ReloadImageInfo(true);
        }

        private void NumFontHeight_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontHeight.Value, 0xFF);
            m_LoadedFont.FontHeight = newVal;
            this.ReloadDataGrid();
            this.ReloadImageInfo(true);
        }

        private void RevertFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            if (!testUnsavedConfirm(QUESTION_RESETFONT))
                return;
            this.m_LoadedFont = this.m_LoadedFontBackup.Clone();
            ReloadUIWithSelection();
        }

        private void ReloadUIWithSelection()
        {
            Boolean wasLoading = m_Loading;
            m_Loading = true;
            try
            {
                Int32 selectedIndex = GetSelectedIndex();
                Int32 scrollOffset = 0;
                if (this.dgrvSymbolsList.SelectedRows.Count > 0)
                {
                    selectedIndex = this.dgrvSymbolsList.CurrentCell.RowIndex;
                    scrollOffset = this.dgrvSymbolsList.VerticalScrollbarOffset;
                }
                m_Loading = false;
                ReloadUi();
                if ((this.dgrvSymbolsList.DataSource as DataTable) != null && selectedIndex < ((DataTable)(this.dgrvSymbolsList.DataSource)).Rows.Count)
                {
                    if (selectedIndex > 0)
                        this.dgrvSymbolsList.VerticalScrollbarOffset = scrollOffset;
                    this.dgrvSymbolsList.Rows[selectedIndex].Cells[0].Selected = true;
                }
            }
            finally
            {
                m_Loading = wasLoading;
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, m_TitleText + "\n\nProgram icon created by Tomsons26\n\nFont format research by Nyerguds, assisted by Omniblade, CCHyper and Tomsons26", m_TitleText, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChkPaint_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.m_Loading = true;
            this.toolTip1.SetToolTip(this.pxbEditGridFront, null);
            this.palColorSelector.TransItemCharColor = Color.Blue;
            this.palColorSelector.ColorSelectMode = ColorSelMode.None;
            this.chkPicker.Checked = false;
            this.m_Loading = false;
        }

        private void ChkPick_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.m_Loading)
                return;
            this.m_Loading = true;
            chkPaint.Checked = false;
            this.m_Loading = false;
        }

        private void CmbPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
            Int32 bpp;
            if (currentPal == null)
            {
                btnSavePalette.Enabled = false;
                m_CurrentPalette = GetDummyPalette(4);
                bpp = 4;
            }
            else
            {
                Int32 nrcols = (Int32)Math.Pow(2, currentPal.BitsPerPixel);
                btnSavePalette.Enabled = currentPal.SourceFile != null && currentPal.Entry >= 0 && currentPal.Entry < 256 / nrcols;
                m_CurrentPalette = currentPal.Colors;
                bpp = currentPal.BitsPerPixel;
                this.btnResetPalette.Enabled = currentPal.IsChanged();
            }
            ReloadColors(bpp);
        }

        private void BtnResetPalette_Click(object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if (currentPal == null)
                return;
            if (currentPal.SourceFile != null && currentPal.Entry >= 0)
            {
                DialogResult dr = MessageBox.Show("This will remove all changes you have made to the palette since it was loaded!\n\nAre you sure you want to continue?", m_TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }
            currentPal.Revert();
            this.btnResetPalette.Enabled = currentPal.IsChanged();
            ReloadColors(currentPal.BitsPerPixel);
        }

        private void ReloadColors(Int32 bpp)
        {
            InitPaletteControl(bpp, this.palColorSelector, m_CurrentPalette, PALETTE_MAX_DIM);
            this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor1]);
            this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor2]);
            if (!this.m_Loading)
            {
                this.ReloadImageInfo(true);
                this.ReloadDataGrid();
            }
        }

        private void BtnSavePalette_Click(object sender, EventArgs e)
        {
            PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
            if (currentPal == null)
                return;
            Int32 nrcols = (Int32)Math.Pow(2, currentPal.BitsPerPixel);
            if (currentPal.SourceFile == null || currentPal.Entry < 0 || currentPal.Entry >= 256 / nrcols)
                return;
            FileInfo palfile = new FileInfo(GeneralUtils.GetAbsolutePath(currentPal.SourceFile, Path.GetDirectoryName(Application.ExecutablePath)));
            Color[] fullPal;
            if (palfile.Exists && palfile.Length == 0x300)
            {
                DialogResult dr = MessageBox.Show("This will overwrite the palette data on your hard disk!\n\nAre you sure you want to continue?", m_TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
                // Treat as C&C 6-bit colour palette
                SixBitColor[] pal = ColorUtils.ReadSixBitPaletteFile(palfile.FullName);
                fullPal = ColorUtils.GetEightBitColorPalette(pal);
            }
            else
            {
                fullPal = new Color[256];
                for (Int32 i = 0; i < fullPal.Length; i++)
                    fullPal[i] = Color.Black;
            }
            Array.Copy(currentPal.Colors, 0, fullPal, currentPal.Entry * nrcols, nrcols);
            ColorUtils.WriteSixBitPaletteFile(fullPal, palfile.FullName);
            currentPal.ClearRevert();
            this.btnResetPalette.Enabled = currentPal.IsChanged();
        }

        private void BtnRemap_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            FrmReplaceColor convertPopup = new FrmReplaceColor(this.m_LoadedFont.BitsPerPixel, this.m_CurrentPalette);
            convertPopup.StartPosition = FormStartPosition.CenterParent;
            if (convertPopup.ShowDialog(this) == DialogResult.OK && convertPopup.SelectedIndexSource != convertPopup.SelectedIndexTarget)
            {
                foreach (FontFileSymbol ffs in m_LoadedFont.GetAllSymbols())
                {
                    ffs.ReplaceColor((Byte)convertPopup.SelectedIndexSource, (Byte)convertPopup.SelectedIndexTarget);
                }
                this.ReloadImageInfo(true);
                this.ReloadDataGrid();
            }
        }

        private void RepaintPreview()
        {
            if (m_LoadedFont == null)
            {
                this.pxbPreview.Image = null;
                this.pxbPreview.BackColor = System.Drawing.Color.Silver;
                this.pxbPreview.Enabled = false;
                return;
            }
            this.pxbPreview.Enabled = true;
            pxbPreview.BackColor = m_Settings.Background;
            pxbPreview.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[0]);
            Int32 width = pxbPreview.ClientRectangle.Width - pxbPreview.Padding.Left - pxbPreview.Padding.Right;
            pxbPreview.Image = GeneratePreview(width, true);
        }

        private Bitmap GeneratePreview(Int32 width, Boolean transparentBg)
        {
            if (m_LoadedFont == null)
                return null;
            if (width == 0)
                width = pxbPreview.ClientRectangle.Width - pxbPreview.Padding.Left - pxbPreview.Padding.Right;
            Encoding enc = ((EncodingDropDownInfo)cmbEncodings.SelectedItem).Encoding;
            return m_LoadedFont.PrintText(txtPreview.Text, this.m_CurrentPalette, transparentBg, enc, width);
        }

        private void EditorSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings settingsFrm = new FrmSettings(this.m_CustomColors, this.m_Settings);
            PaletteDropDownInfo currentPal = cmbPalettes.SelectedItem as PaletteDropDownInfo;
            settingsFrm.StartPosition = FormStartPosition.CenterParent;
            settingsFrm.ShowDialog(this);
            this.m_CustomColors = settingsFrm.CustomColors;
            this.m_DefaultPalettes = LoadDefaultPalettes();
            List<PaletteDropDownInfo> bppPalettes = GetPalettes(currentPal.BitsPerPixel);
            if (bppPalettes.Count == 0)
                bppPalettes.Add(new PaletteDropDownInfo("Rainbow", currentPal.BitsPerPixel, GetDummyPalette(currentPal.BitsPerPixel), null, -1));
            this.cmbPalettes.DataSource = bppPalettes;
            Int32 oldIndex = bppPalettes.FindIndex(x => x.Name == currentPal.Name);
            this.RefreshEditor();
            if (oldIndex >= 0)
                this.cmbPalettes.SelectedIndex = oldIndex;
        }

        private void txtPreview_TextChanged(object sender, EventArgs e)
        {
            RepaintPreview();
        }

        private void FrmFontEditor_Resize(object sender, EventArgs e)
        {
            RepaintPreview();
        }

        private void CopyPreview(object sender, EventArgs e)
        {
            if (m_LoadedFont == null)
                return;
            Clipboard.Clear();
            DataObject data = new DataObject();
            data.SetData(DataFormats.Bitmap, GeneratePreview(0, false));
            Clipboard.SetDataObject(data);
        }

        private void CopyCharacter(object sender, EventArgs e)
        {
            if (m_LoadedFont == null)
                return;
            if (this.dgrvSymbolsList.SelectedRows.Count == 0)
                return;
            String selectedIndexChar = (String)this.dgrvSymbolsList.SelectedRows[0].Cells[2].Value;
            Clipboard.Clear();
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, selectedIndexChar);
            Clipboard.SetDataObject(data);
        }

        private void BtnValType_Click(object sender, EventArgs e)
        {
            ChangeFontType(null);
        }

        private Boolean ChangeFontType(FontFile targetFontFile)
        {
            if (this.m_LoadedFont == null)
                return false;
            FontFile sourceFontFile = this.m_LoadedFont;
            if (targetFontFile == null)
            {
                FrmConvertFontType fontConvertDialog = new FrmConvertFontType(this.m_LoadedFont);
                fontConvertDialog.StartPosition = FormStartPosition.CenterParent;
                if (fontConvertDialog.ShowDialog(this) != DialogResult.OK)
                    return false;
                targetFontFile = fontConvertDialog.TargetFontFile;
            }
            Byte replaceIndex = 0;
            Boolean tooHigh = sourceFontFile.BitsPerPixel > targetFontFile.BitsPerPixel;
            if (tooHigh)
            {
                tooHigh = false;
                Int32 colValLimit = (Int32)Math.Pow(2, targetFontFile.BitsPerPixel);
                foreach (FontFileSymbol ffs in m_LoadedFont.GetAllSymbols())
                {
                    if (ffs.ByteData.Any(x => x >= colValLimit))
                    {
                        tooHigh = true;
                        break;
                    }
                }
            }
            if (tooHigh)
            {
                FrmConvertToLowerBpp convertPopup = new FrmConvertToLowerBpp(false, targetFontFile.BitsPerPixel, this.m_CurrentPalette);
                convertPopup.StartPosition = FormStartPosition.CenterParent;
                if (convertPopup.ShowDialog() != DialogResult.OK)
                    return false;
                replaceIndex = (Byte)convertPopup.SelectedIndex;
            }
            m_LoadedFont.CloneInto(targetFontFile, replaceIndex);
            m_LoadedFont = targetFontFile;
            ReloadUIWithSelection();
            return true;
        }

        private void TextBoxSelectAll(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender != null && sender is TextBox)
                {
                    ((TextBox)sender).SelectAll();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void dgrvSymbolsList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }
            }
        }

        private void dgrvSymbolsList_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            e.ContextMenuStrip = m_tsmiCopyGridChar;
        }

        private void FrmFontEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !testUnsavedConfirm(QUESTION_EXITPROGRAM);
        }

        private Boolean testUnsavedConfirm(String question)
        {
            if (this.m_LoadedFont == null || this.m_LoadedFontBackup == null)
                return true;
            if (this.m_LoadedFont.Equals(m_LoadedFontBackup))
                return true;
            DialogResult res = MessageBox.Show(this, question, m_TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return res == System.Windows.Forms.DialogResult.Yes;
        }

    }

    public class EncodingDropDownInfo
    {
        protected static readonly Regex regex_replacename = new Regex(@"^(.+)\s*\((.+)\)$");
        public Encoding Encoding { get; private set; }

        public EncodingDropDownInfo(Encoding enc)
        {
            this.Encoding = enc;
        }

        public override String ToString()
        {
            return regex_replacename.Replace(this.Encoding.EncodingName, "$2 - $1");
        }
    }
    
    public class PaletteDropDownInfo
    {
        public String Name { get; private set; }
        public Color[] Colors { get; private set; }
        public Color[] ColorBackup { get; private set; }
        public Int32 BitsPerPixel { get; private set; }
        public String SourceFile { get; private set; }
        public Int32 Entry{ get; private set; }

        public PaletteDropDownInfo(String name, Int32 bpp, Color[] colors, String sourceFile, Int32 entry)
        {
            this.Name = name;
            this.BitsPerPixel = bpp;
            Int32 expectedcolors = (Int32)Math.Pow(2, bpp);
            Color[] palette = new Color[expectedcolors];
            Int32 copiedColors = Math.Min(colors.Length, expectedcolors);
            Array.Copy(colors, palette, copiedColors);
            for (Int32 i = copiedColors; i < expectedcolors; i++)
                palette[i] = Color.Black;
            this.Colors = palette;
            this.ColorBackup = palette.ToArray();
            this.SourceFile = sourceFile;
            this.Entry = entry;
        }


        public Boolean IsChanged()
        {
            return !this.ColorBackup.SequenceEqual(this.Colors);
        }

        public void Revert()
        {
            Array.Copy(this.ColorBackup, this.Colors, this.Colors.Length);
        }

        public void ClearRevert()
        {
            Array.Copy(this.Colors, this.ColorBackup, this.Colors.Length);
        }

        public override String ToString()
        {
            return Name;
        }
    }

    public class FontFileDialogItem
    {
        public String Extension { get; private set; }
        public String Filter { get { return "*." + Extension;} }
        public String Description { get; private set; }
        public String FullDescription
        {
            get { return String.Format("{0} (*.{1})", this.Description, this.Extension); }
        }

        public FontFile FontTypeObject { get { return (FontFile)Activator.CreateInstance(FontType); } }
        public Type FontType { get; private set; }

        public FontFileDialogItem(Type fonttype)
        {
            if (!fonttype.IsSubclassOf(typeof(FontFile)))
                throw new ArgumentException("Entries in autoDetectTypes list must all be FontFile classes!", "fonttype");
            FontType = fonttype;
            // Will immediately throw an exception if the type cannot be instantiated.
            Description = FontTypeObject.ShortTypeDescription;
            this.Extension = FontTypeObject.FileExtension;
        }

        public override String ToString()
        {
            return FontTypeObject.ShortTypeDescription;
        }

        public static String GetFileFilter(FontFileDialogItem[] fontTypes, Boolean forOpen)
        {
            String[] types = new String[fontTypes.Length + (forOpen? 2 : 0)];
            HashSet<String> allTypes = forOpen ? new HashSet<String>() : null;
            for (Int32 i = 0; i < fontTypes.Length; i++)
            {
                FontFileDialogItem fontType = fontTypes[i];
                types[i + (forOpen? 1 : 0)] = String.Format("{0} ({1})|{1}", fontType.Description, fontType.Filter);
                if (forOpen)
                    allTypes.Add(fontType.Filter);
            }
            if (forOpen)
            {
                allTypes.Add("*.fnt");
                String allTypesStr = String.Join(";", allTypes.ToArray());
                types[0] = "All supported fonts (" + allTypesStr + ")|" + allTypesStr;
                types[fontTypes.Length + 1] = "All files (*.*)|*.*";
            }
            return String.Join("|", types);
        }

    }

}
