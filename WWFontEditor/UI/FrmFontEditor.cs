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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WWFontEditor
{
    public partial class FrmFontEditor : Form
    {
        private Color[] PaletteRainbow = new Color[]
            {
                ImageUtils.ColorFromUInt(0x00000000), // 0
                ImageUtils.ColorFromUInt(0xFFFF0000), // 1
                ImageUtils.ColorFromUInt(0xFFFF5E00), // 2
                ImageUtils.ColorFromUInt(0xFFFFBF00), // 3
                ImageUtils.ColorFromUInt(0xFFFFFF00), // 4
                ImageUtils.ColorFromUInt(0xFFCCFF00), // 5
                ImageUtils.ColorFromUInt(0xFF90FF00), // 6
                ImageUtils.ColorFromUInt(0xFF00FF00), // 7
                ImageUtils.ColorFromUInt(0xFF00FF9D), // 8
                ImageUtils.ColorFromUInt(0xFF00FFFF), // 9
                ImageUtils.ColorFromUInt(0xFF009DFF), // 10
                ImageUtils.ColorFromUInt(0xFF3333FF), // 11
                ImageUtils.ColorFromUInt(0xFF8822FF), // 12
                ImageUtils.ColorFromUInt(0xFFCC22FF), // 13
                ImageUtils.ColorFromUInt(0xFFFF00FF), // 14
                ImageUtils.ColorFromUInt(0xFFFF0088), // 15
            };


        private Boolean m_loading = true;
        private String m_TitleText;
        private String m_FileName;
        private FontFile m_LoadedFont;
        private FontFile m_LoadedFontBackup;
        private FontFileCharacter m_Clipboard;
        private Int32 m_CurHeight;
        private Int32 m_CurWidth;
        private Int32 m_CurYOffset;
        private Int32 m_LastHoverPixelX = -1;
        private Int32 m_LastHoverPixelY = -1;
        // Would need to be a List<Boolean> with one status per index
        //private Boolean m_CurrentPicEdited = false;
        private Byte m_CurrentPaintColor1 = 1;
        private Byte m_CurrentPaintColor2 = 0;
        private Color[] m_CurrentPalette;

        private Int32[] m_Customcolors;

        private Color m_GridColor = Color.Blue;
        private Color m_GridColorFrame = Color.Red;
        private Color m_GridColorOuter = Color.White;
        private Color m_GridColorOuterFrame = Color.Black;
        private Color m_GridColorBg = Color.LightGray;

        public FrmFontEditor()
        {
            InitializeComponent();
            // encodings init
            List<EncodingDropDownInfo> encodings = Encoding.GetEncodings() // Get all known .Net encodings
                .Select(e => e.GetEncoding()) // From EncodingInfo to Encoding
                .Where(e => e.IsSingleByte && TextUtils.IsAsciiCompatible(e)) // Filter out single byte ASCII-compatible ones
                .Select(e => new EncodingDropDownInfo(e)) // Put in wrapper class to add a ToString() for the dropdown
                .OrderBy(n => n.ToString()) // Order by name as returned by wrapper class (with extra info first)
                .ToList();
            cmbEncodings.DataSource = encodings;
            // Select DOS-437 encoding, the one all original C&C fonts are based on.
            cmbEncodings.SelectedItem = encodings.Find(e => e.Encoding.CodePage == 437);
            
            // Colors init.
            InitColorPalette(PixelFormat.Format4bppIndexed);

            // PixelBox hierarchy init            
            pxbEditGridBehind.Parent = pxbFullSize;
            pxbEditGridBehind.BackColor = Color.Transparent;
            pxbEditGridBehind.Location = new Point(0,0);
            pxbImage.Parent = pxbFullSize;
            pxbImage.BackColor = Color.Transparent;
            pxbImage.Location = new Point(0, 0);
            pxbImage.BringToFront();
            pxbEditGridFront.Parent = pxbImage;
            pxbEditGridFront.BackColor = Color.Transparent;
            pxbEditGridFront.Location = new Point(0, 0);

            this.lblPaintColor1.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor1]);
            this.lblPaintColor2.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor2]);
            m_TitleText = "Westwood Font Editor " + GeneralUtils.ProgramVersion() + " - Created by Nyerguds";
            this.Text = m_TitleText;
            this.m_loading = false;
        }

        public FrmFontEditor(string[] args) : this()
        {
            if (args.Length > 0 && File.Exists(args[0]))
                m_FileName = args[0];
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
                    LoadFontFile(path);
            }
        }

        private void LoadFontFile(String path)
        {
            this.m_loading = true;
            try
            {
                this.m_FileName = path;
                String error = null;
                Boolean loadOk = false;
                try
                {
                    this.m_LoadedFont = null;
                    Byte[] data = File.ReadAllBytes(path);
                    this.m_LoadedFont = new FontFile(data);
                    loadOk = this.m_LoadedFont != null;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    this.m_LoadedFont = null;
                }
                this.numCharacters.Enabled = loadOk;
                this.numFontHeight.Enabled = loadOk;
                this.numFontWidth.Enabled = loadOk;
                this.numWidth.Enabled = loadOk;
                this.numHeight.Enabled = loadOk;
                this.numYOffset.Enabled = loadOk;
                this.btnShiftUp.Enabled = loadOk;
                this.btnShiftLeft.Enabled = loadOk;
                this.btnShiftRight.Enabled = loadOk;
                this.btnShiftDown.Enabled = loadOk;
                this.btnRevert.Enabled = false;
                this.btnCopy.Enabled = loadOk;
                this.copyToolStripMenuItem.Enabled = loadOk;
                this.btnPaste.Enabled = m_Clipboard != null;
                this.pasteToolStripMenuItem.Enabled = m_Clipboard != null;
                this.saveFontToolStripMenuItem.Enabled = loadOk;
                this.saveFontAsToolStripMenuItem.Enabled = loadOk;
                this.pxbFullSize.Visible = loadOk;
                if (loadOk)
                {
                    InitColorPalette(this.m_LoadedFont.GetPixelFormat());
                    this.m_LoadedFontBackup = this.m_LoadedFont.Clone();
                    this.Text = m_TitleText + " - \"" + Path.GetFileName(path) + "\" (" + m_LoadedFont.GetShortTypeCode() + ")";
                    this.lblValType.Text = m_LoadedFont.GetShortTypeCode().Replace("&", "&&");
                    this.numCharacters.Maximum = 0x100;
                    this.numCharacters.Value = this.m_LoadedFont.Length;
                    this.numFontHeight.Maximum = 0xFF;
                    this.numFontHeight.Value = this.m_LoadedFont.FontHeight;
                    this.numFontWidth.Maximum = 0xFF;
                    this.numFontWidth.Value = this.m_LoadedFont.FontWidth;
                }
                else
                {
                    this.m_FileName = null;
                    this.Text = m_TitleText;
                    this.lblValType.Text = "-";
                    this.numCharacters.Maximum = 0;
                    this.numCharacters.Value = 0;
                    this.numFontHeight.Maximum = 0;
                    this.numFontHeight.Value = 0;
                    this.numFontWidth.Maximum = 0;
                    this.numFontWidth.Value = 0;
                }
                this.ReloadImageInfo(true);
                this.ReloadDataGrid();
                // to allow index changed events on the following piece
                this.m_loading = false;
                if (loadOk && this.m_LoadedFont.Length > 32)
                {
                    //dgrvCharactersList.Rows[32].Cells[0].Selected = true;
                    this.dgrvCharactersList.FirstDisplayedCell = dgrvCharactersList.Rows[32].Cells[0];
                    this.dgrvCharactersList.Rows[32].Cells[0].Selected = true;
                    this.dgrvCharactersList.Focus();
                }
                if (!loadOk)
                    MessageBox.Show(this, "Font loading failed" + (error == null ? "." : ": " + error), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.m_loading = false;
            }
        }
        
        private void InitColorPalette(PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormat.Format4bppIndexed)
                m_CurrentPalette = PaletteRainbow;
            else
            {
                m_CurrentPalette = ImageUtils.MakePalette(null, pixelFormat, false).Entries.Select(c => Color.FromArgb(0XFF, c)) //.ToArray();
                                    .Reverse().ToArray();
                m_CurrentPalette[0] = Color.Black;
            }
            m_CurrentPalette[0] = Color.FromArgb(0x00, m_CurrentPalette[0]);
            palColorSelector.MaxColors = m_CurrentPalette.Length;
            Int32 aimDim = 134;
            Int32 squaresPerRow = (Int32)Math.Sqrt(m_CurrentPalette.Length);
            Int32 sqrWidth = (Int32)Math.Ceiling(aimDim * 7.5 / 8.5 / squaresPerRow);
            Int32 padding = (Int32)Math.Max(1, Math.Ceiling(sqrWidth / 8.5));
            while (aimDim < squaresPerRow * sqrWidth + (squaresPerRow - 1) * padding)
            {
                sqrWidth--;
                padding = (Int32)Math.Max(1, Math.Ceiling(sqrWidth / 8.5));
            }
            palColorSelector.ColorTableWidth = squaresPerRow;
            palColorSelector.LabelSize = new Size(sqrWidth, sqrWidth);
            palColorSelector.PadBetween = new Point(padding, padding);
            palColorSelector.Palette = m_CurrentPalette;
        }

        private void ReloadDataGrid()
        {
            Boolean wasLoading = this.m_loading;
            this.m_loading = true;
            try
            {
                if (m_LoadedFont == null)
                {
                    dgrvCharactersList.DataSource = null;
                    return;
                }
                // add as param later
                Encoding enc = ((EncodingDropDownInfo)cmbEncodings.SelectedItem).Encoding;
                ColorPalette palette = ImageUtils.MakePalette(m_CurrentPalette, m_LoadedFont.GetPixelFormat(), false);
                Bitmap dummyImage = ImageUtils.GenerateBlankImage(5, 5, new Color[] { Color.Transparent }, 0);
                Int32 selectedIndex = 0;
                Int32 scrollOffset = 0;
                if (dgrvCharactersList.Rows.Count > 0 && dgrvCharactersList.CurrentCell != null)
                {
                    selectedIndex = dgrvCharactersList.CurrentCell.RowIndex;
                    scrollOffset = dgrvCharactersList.VerticalScrollbarOffset;
                }
                DataTable charsTable = new DataTable("Characters");
                charsTable.Columns.Add(new DataColumn("Hex", typeof(String)));
                charsTable.Columns.Add(new DataColumn("Dec", typeof(Byte)));
                charsTable.Columns.Add(new DataColumn("Char", typeof(String)));
                charsTable.Columns.Add(new DataColumn("Pic", typeof(Bitmap)));
                FontFileCharacter[] charsInfo = m_LoadedFont.GetAllRawData();
                for (Int32 i = 0; i < charsInfo.Length; i++)
                {
                    FontFileCharacter charInfo = charsInfo[i];
                    DataRow row = charsTable.NewRow();
                    row[0] = "0x" + i.ToString("X2");
                    row[1] = i;
                    row[2] = enc.GetString(new Byte[] { (Byte)i });
                    Bitmap bm = charInfo.GetBitmap(palette);
                    if (bm == null)
                        bm = dummyImage;
                    row[3] = bm;
                    charsTable.Rows.Add(row);
                }
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.FromArgb(0xFF, m_CurrentPalette[0]);
                style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgrvCharactersList.DataSource = charsTable;
                dgrvCharactersList.Columns[3].DefaultCellStyle = style;
                if (selectedIndex < charsTable.Rows.Count)
                {
                    if (selectedIndex > 0)
                        dgrvCharactersList.VerticalScrollbarOffset = scrollOffset;
                    dgrvCharactersList.Rows[selectedIndex].Cells[0].Selected = true;
                }
            }
            finally
            {
                m_loading = wasLoading;
            }
        }

        private void SaveFontFile(String fileName)
        {
            if (this.m_LoadedFont == null)
                return;
            //FntFileVersion ver = m_Loadedfont.Unknown0E == 0x1012 ? FntFileVersion.CnC : FntFileVersion.Kyrandia;
            Byte[] filedata = this.m_LoadedFont.WriteFntFile();
            File.WriteAllBytes(fileName, filedata);
        }

        private void FrmFontEditor_Shown(object sender, EventArgs e)
        {
            if (m_FileName != null)
                LoadFontFile(m_FileName);
        }
        
        private void ReloadImageInfo(Boolean refreshEditor)
        {
            Boolean wasLoading = this.m_loading;
            this.m_loading = true;
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
                    return;
                }
                Bitmap charImage = this.m_LoadedFont.GetBitmap(curIndex, this.m_CurrentPalette, true);
                pxbImage.Image = charImage;
                m_CurHeight = this.m_LoadedFont.GetCharHeight(curIndex);
                m_CurWidth = this.m_LoadedFont.GetCharWidth(curIndex);
                m_CurYOffset = this.m_LoadedFont.GetCharYOffset(curIndex);
                numHeight.Maximum = this.m_LoadedFont.FontHeight;
                numHeight.Value = m_CurHeight;
                numWidth.Maximum = this.m_LoadedFont.FontWidth;
                numWidth.Value = m_CurWidth;
                this.numYOffset.Value = m_CurYOffset;
                this.CheckCanRevert();
                if (refreshEditor)
                    this.RefreshEditor();
            }
            finally
            {
                this.m_loading = wasLoading;
            }
        }

        private Int32 GetSelectedIndex()
        {
            Int32 selectedIndex = 0;
            if (dgrvCharactersList.SelectedRows.Count > 0)
                selectedIndex = dgrvCharactersList.SelectedRows[0].Index;
            return selectedIndex;
        }

        private void NumZoom_ValueChanged(object sender, EventArgs e)
        {
            this.RefreshEditor();
        }

        private void RefreshEditor()
        {
            Boolean wasLoading = this.m_loading;
            this.m_loading = true;
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
                    Color[] palette = new Color[] {Color.Transparent, Color.Black, drawOutline ? m_GridColor : m_GridColorOuter, m_GridColorFrame};
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
                    if (addGrid && drawGrid)
                    {
                        pxbFullSize.Image = ImageUtils.GenerateGridImage(this.m_LoadedFont.FontWidth, this.m_LoadedFont.FontHeight, zoom, new Color[] { m_GridColorBg, m_GridColorOuter, m_GridColorOuterFrame }, 0, 1, 2);
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
                        pxbFullSize.Image = ImageUtils.GenerateBlankImage(bgWidth, bgHeight, new Color[] { m_GridColorBg }, 0);
                        pxbFullSize.BackColor = m_GridColorBg;
                    }
                    pxbFullSize.Width = bgWidth;
                    pxbFullSize.Height = addedHeight > 0 ? bgHeight + (addedHeight * zoom) : bgHeight;
                }
            }
            finally
            {
                this.m_loading = wasLoading;
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
            CheckMouse(sender, e, false);
        }

        private void pxbEditGridFront_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0 || (e.Button & MouseButtons.Right) != 0)
            {
                ReloadDataGrid();
                this.CheckCanRevert();
            }
        }

        private void CheckMouse(object sender, MouseEventArgs e, Boolean drawPreviewPixel)
        {
            Bitmap gridFront = this.pxbEditGridFront.Image as Bitmap;
            if (gridFront == null)
                return;
            Int32 picX = e.X / (Int32)this.numZoom.Value;
            Int32 picY = e.Y / (Int32)this.numZoom.Value;
            Boolean inBounds = picX >= 0 && picX < gridFront.Width && picY >= 0 && picY < gridFront.Height;
            if (drawPreviewPixel)
            {
                // Optimize by aborting immediately if location is unchanged
                if (m_LastHoverPixelX == picX && m_LastHoverPixelY == picY)
                    return;
                // Clear previous pixel
                if (m_LastHoverPixelX != -1 && m_LastHoverPixelY != -1)
                    ImageUtils.DrawRect8Bit(gridFront, m_LastHoverPixelX, m_LastHoverPixelY, m_LastHoverPixelX, m_LastHoverPixelY, 0, true);
                // set color, just in case it changed.
                if (m_CurrentPalette.Length > this.m_CurrentPaintColor1)
                    gridFront.Palette.Entries[1] = m_CurrentPalette[this.m_CurrentPaintColor1];
                // Draw new pixel
                if (inBounds)
                    ImageUtils.DrawRect8Bit(gridFront, picX, picY, picX, picY, 1, true);
                this.m_LastHoverPixelX = picX;
                this.m_LastHoverPixelY = picY;
                pxbEditGridFront.Invalidate();
            }
            Boolean isLeftClick = (e.Button & MouseButtons.Left) != 0;
            Boolean isRightClick = (e.Button & MouseButtons.Right) != 0;
            if (this.m_LoadedFont!= null && inBounds && (isLeftClick || isRightClick))
            {
                try
                {
                    Int32 curIndex = GetSelectedIndex();
                    if (chkPaint.Checked)
                    {
                        if (isLeftClick)
                            this.m_LoadedFont.PaintPixel(curIndex, picX, picY, this.m_CurrentPaintColor1);
                        else
                            this.m_LoadedFont.PaintPixel(curIndex, picX, picY, this.m_CurrentPaintColor2);
                        this.pxbImage.Image = this.m_LoadedFont.GetBitmap(curIndex, this.m_CurrentPalette, true);
                    }
                    else if (this.chkPicker.Checked)
                    {
                        Byte val = this.m_LoadedFont.GetRawData(curIndex).GetPixelValue(picX, picY);
                        if (isLeftClick)
                        {
                            this.m_CurrentPaintColor1 = val;
                            lblPaintColor1.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[val]);
                            // Since the grid only shows edit color 1, it's only needed for Left button.
                            this.WipeEditGridFront();
                        }
                        else
                        {
                            this.m_CurrentPaintColor2 = val;
                            lblPaintColor2.BackColor = Color.FromArgb(0xFF, this.m_CurrentPalette[val]);
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    // Trying to draw a >15 color index on a 4-bit image. Shouldn't happen in the final version.
                    MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Boolean CheckCanRevert()
        {
            Int32 index = GetSelectedIndex();
            Boolean equal = m_LoadedFont != null && this.m_LoadedFontBackup != null;
            FontFileCharacter rawData2 = null;
            if (equal)
            {
                FontFileCharacter rawData1 = this.m_LoadedFont.GetRawData(index);
                rawData2 = this.m_LoadedFontBackup.GetRawData(index);
                equal = rawData1 != null && rawData2 != null;
                if (equal)
                {
                    equal = this.m_LoadedFont.GetCharWidth(index) == this.m_LoadedFontBackup.GetCharWidth(index)
                                   && this.m_LoadedFont.GetCharHeight(index) == this.m_LoadedFontBackup.GetCharHeight(index)
                                   && this.m_LoadedFont.GetCharYOffset(index) == this.m_LoadedFontBackup.GetCharYOffset(index)
                                   && rawData1.ByteData.SequenceEqual(rawData2.ByteData);
                }
            }
            Boolean enable = !equal && rawData2 != null;
            this.btnRevert.Enabled = enable;
            this.revertToolStripMenuItem.Enabled = enable;
            return enable;

        }

        /// <summary>
        /// Regenerates the preview pixel image drawn on top of the front edit grid
        /// to get a blank slate with the correct preview pixel color set.
        /// </summary>
        private void WipeEditGridFront()
        {
            Color paintColor = Color.FromArgb(0xFF, m_CurrentPalette[this.m_CurrentPaintColor1]);
            pxbEditGridFront.Image = ImageUtils.GenerateBlankImage(this.m_CurWidth, this.m_CurHeight, new Color[] { Color.Transparent, paintColor }, 0);
        }

        private void pxbEditGridFront_MouseLeave(object sender, EventArgs e)
        {
            this.WipeEditGridFront();
            this.m_LastHoverPixelX = -1;
            this.m_LastHoverPixelY = -1;
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
            DialogResult dr = MessageBox.Show("This will revert the current edits on this\ncharacter image to their original state!\n\nAre you sure you want to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes)
                return;
            this.m_LoadedFont.RestorePicFromBackup(GetSelectedIndex(), this.m_LoadedFontBackup);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
            this.pnlImageScroll.Focus();
            this.btnRevert.Enabled = false;
        }

        private void numYOffset_ValueChanged(object sender, EventArgs e)
        {
            if (m_loading)
                return;
            this.m_LoadedFont.GetRawData(GetSelectedIndex()).YOffset = (Byte)this.numYOffset.Value;
            this.ReloadImageInfo(true);
        }

        private void cmbEncodings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_loading)
                return;
            ReloadDataGrid();
        }

        private void dgrvCharactersList_SelectionChanged(object sender, EventArgs e)
        {
            if (m_loading)
                return;
            ReloadImageInfo(true);
        }

        private void palColorSelector_ColorLabelMouseDoubleClick(object sender, PaletteClickEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            PalettePanel palpanel = (PalettePanel)sender;
            Int32 colindex = e.Index;
            ColorDialog cdl = new ColorDialog();
            cdl.Color = e.Color;
            cdl.FullOpen = true;
            cdl.CustomColors = m_Customcolors;
            DialogResult res = cdl.ShowDialog();
            m_Customcolors = cdl.CustomColors;
            if (res == DialogResult.OK || res == DialogResult.Yes)
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
            }
        }

        private void OpenFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Westwood font files (*.fnt)|*.fnt|All Files (*.*)|*.*";
            ofd.InitialDirectory = String.IsNullOrEmpty(m_FileName) ? Path.GetFullPath(".") : Path.GetDirectoryName(m_FileName);
            DialogResult res = ofd.ShowDialog(this);
            if (res != System.Windows.Forms.DialogResult.OK)
                return;
            LoadFontFile(ofd.FileName);
        }

        private void SaveFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFontFile(this.m_FileName);
            this.m_LoadedFontBackup = this.m_LoadedFont.Clone();
            // no need to even check; it's 100% absolutely unchanged.
            this.btnRevert.Enabled = false;
        }


        private void SaveFontAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Westwood font files (*.fnt)|*.fnt|All Files (*.*)|*.*";
            sfd.InitialDirectory = String.IsNullOrEmpty(m_FileName) ? Path.GetFullPath(".") : Path.GetDirectoryName(m_FileName);
            if (!String.IsNullOrEmpty(m_FileName))
                sfd.FileName = Path.GetFileName(m_FileName);
            DialogResult res = sfd.ShowDialog(this);
            if (res != System.Windows.Forms.DialogResult.OK)
                return;
            SaveFontFile(sfd.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Add changes check?
            Application.Exit();
        }

        private void ShiftCurrentImage(ShiftDirection shiftDirection)
        {
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileCharacter fc = this.m_LoadedFont.GetRawData(curIndex);
            if (fc == null)
                return;
            fc.ShiftImageData(shiftDirection);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }
        
        private void btnShiftUp_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Up);
        }
        private void btnShiftRight_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Right);
        }

        private void btnShiftDown_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Down);
        }

        private void btnShiftLeft_Click(object sender, EventArgs e)
        {
            ShiftCurrentImage(ShiftDirection.Left);
        }

        private void ChangeCurrentImageDimension(Byte newDimension, Boolean isHeight)
        {
            if (this.m_loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileCharacter fc = this.m_LoadedFont.GetRawData(curIndex);
            if (fc == null)
                return;
            if (isHeight)
                fc.ChangeHeight(newDimension);
            else
                fc.ChangeWidth(newDimension);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Byte)this.numWidth.Value, false);
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            this.ChangeCurrentImageDimension((Byte)this.numHeight.Value, true);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileCharacter fc = this.m_LoadedFont.GetRawData(curIndex);
            if (fc == null)
                return;
            this.m_Clipboard = fc.Clone();
            this.btnPaste.Enabled = m_Clipboard != null;
            this.pasteToolStripMenuItem.Enabled = m_Clipboard != null;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (this.m_LoadedFont == null || this.m_Clipboard == null)
                return;
            Int32 curIndex = GetSelectedIndex();
            FontFileCharacter fc = this.m_LoadedFont.GetRawData(curIndex);
            if (fc == null)
                return;
            Boolean canrevert = this.CheckCanRevert();
            // if there are unsaved changes, or the image is new and not empty, ask specifically
            if (canrevert || (this.m_LoadedFontBackup.Length <= curIndex && fc.ByteData.Length > 0))
            {
                DialogResult dr = MessageBox.Show("This will completely overwrite the current character\n\nAre you sure you want to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != DialogResult.Yes)
                    return;
            }
            fc = this.m_Clipboard.Clone();
            if (fc.Height > this.m_LoadedFont.FontHeight)
                fc.ChangeHeight(this.m_LoadedFont.FontHeight);
            if (fc.Width > this.m_LoadedFont.FontWidth)
                fc.ChangeWidth(this.m_LoadedFont.FontWidth);
            this.m_LoadedFont.RestorePicFromBackup(curIndex, fc);
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
        }

        private void numCharacters_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            this.m_loading = true;
            try
            {
                Int32 newLen = (Int32)numCharacters.Value;
                this.m_LoadedFont.Length = newLen;
                newLen = this.m_LoadedFont.Length;
                numCharacters.Value = newLen;
                this.ReloadDataGrid();
                if (newLen > 0)
                {
                    this.dgrvCharactersList.Rows[newLen - 1].Cells[0].Selected = true;
                    this.dgrvCharactersList.FirstDisplayedCell = this.dgrvCharactersList.Rows[newLen - 1].Cells[0];
                }
                this.ReloadImageInfo(true);
            }
            finally
            {
                this.m_loading = false;
            }
        }

        private void numFontWidth_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontWidth.Value, 0xFF);
            m_LoadedFont.FontWidth = newVal;
            this.ReloadDataGrid();
            this.ReloadImageInfo(true);
        }

        private void numFontHeight_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_loading)
                return;
            if (this.m_LoadedFont == null)
                return;
            Byte newVal = (Byte)Math.Min(this.numFontHeight.Value, 0xFF);
            m_LoadedFont.FontHeight = newVal;
            this.ReloadDataGrid();
            this.ReloadImageInfo(true);
        }

        private void revertFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("This will remove all changes you have made to the font since it was loaded!\n\nAre you sure you want to continue?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes)
                return;
            this.m_LoadedFont = this.m_LoadedFontBackup.Clone();
            this.ReloadImageInfo(true);
            this.ReloadDataGrid();
            this.pnlImageScroll.Focus();
            this.btnRevert.Enabled = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Text + "\n\nProgram icon created by Tomsons26\n\nWestwood font format research by Nyerguds, assisted by Omniblade, CCHyper and Tomsons26", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkPaint_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.m_loading)
                return;
            this.m_loading = true;
            this.chkPicker.Checked = false;
            this.m_loading = false;
        }

        private void chkPick_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.m_loading)
                return;
            this.m_loading = true;
            chkPaint.Checked = false;
            this.m_loading = false;
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

        public override string ToString()
        {
            return regex_replacename.Replace(this.Encoding.EncodingName, "$2 - $1");
        }

    }
}
