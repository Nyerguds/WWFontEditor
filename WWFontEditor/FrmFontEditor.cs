using WWFontEditor.Domain;
using ColorManipulation;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WWFontEditor
{
    public partial class FrmFontEditor : Form
    {
        private Color[] PaletteRainbow = new Color[]
            {
                ImageUtils.ColorFromUInt(0xFF000000),
                ImageUtils.ColorFromUInt(0xFFFF0000),
                ImageUtils.ColorFromUInt(0xFFFF5E00),
                ImageUtils.ColorFromUInt(0xFFFFBF00),
                ImageUtils.ColorFromUInt(0xFFE1FF00),
                ImageUtils.ColorFromUInt(0xFF80FF00),
                ImageUtils.ColorFromUInt(0xFF22FF00),
                ImageUtils.ColorFromUInt(0xFF00FF40),
                ImageUtils.ColorFromUInt(0xFF00FF9D),
                ImageUtils.ColorFromUInt(0xFF00FFFF),
                ImageUtils.ColorFromUInt(0xFF009DFF),
                ImageUtils.ColorFromUInt(0xFF0040FF),
                ImageUtils.ColorFromUInt(0xFF2200FF),
                ImageUtils.ColorFromUInt(0xFF8000FF),
                ImageUtils.ColorFromUInt(0xFFE100FF),
                ImageUtils.ColorFromUInt(0xFFFF00BF),
                ImageUtils.ColorFromUInt(0xFFFF005E),
            };

        
        private String filename;
        private FntFile loadedfont;
        private Int32 curHeight;
        private Int32 curWidth;
        private Int32 curYOffset;
        private Int32 lastHoverPixelX = -1;
        private Int32 lastHoverPixelY = -1;
        // TODO: Change these two to Byte later, when implementing color palette support.
        private Byte currentPaintColorFront = 1;
        private Byte currentPaintColorBack = 0;
        private Color[] currentPalette;

        private Int32[] customcolors;


        private Color GridColor = Color.Blue;
        private Color GridColorFrame = Color.Red;
        private Color GridColorOuter = Color.White;
        private Color GridColorOuterFrame = Color.Black;
        private Color GridColorBg = Color.LightGray;

        public FrmFontEditor()
        {
            currentPalette = PaletteRainbow;
            InitializeComponent();
            //*/
            pxbEditGridBehind.Parent = pxbFullSize;
            pxbEditGridBehind.BackColor = Color.Transparent;
            pxbImage.Parent = pxbFullSize;
            pxbImage.BackColor = Color.Transparent;
            pxbImage.BringToFront();
            pxbEditGridFront.Parent = pxbImage;
            pxbEditGridFront.BackColor = Color.Transparent;
            /*/
            this.pxbEditGridFront.SendToBack();
            this.pxbImage.SendToBack();
            this.pxbEditGridBehind.SendToBack();
            this.pxbFullSize.SendToBack();
            //*/
            this.lblPaintColor.BackColor = currentPalette[this.currentPaintColorFront];
            this.Text = "Westwood Font Editor " + GeneralUtils.ProgramVersion() + " - Created by Nyerguds";
        }

        public FrmFontEditor(string[] args) : this()
        {
            if (args.Length > 0 && File.Exists(args[0]))
                filename = args[0];
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
            filename = path;
            String error = null;
            Boolean loadOk = false;
            try
            {
                loadedfont = null;
                numIndex.Value = 0;
                Byte[] data = File.ReadAllBytes(path);
                loadedfont = new FntFile(data);
                loadOk = loadedfont != null;
                numIndex.Enabled = loadOk;
                if (loadOk)
                    numIndex.Maximum = loadedfont.LastIndex;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                loadedfont = null;
            }
            if (!loadOk)
            {
                filename = null;
                lblValFilename.Text = "-";
                lblValCharacters.Text = "-";
                lblValFontHeight.Text = "-";
                lblValFontWidth.Text = "-";
                pxbImage.Image = null;
                pxbFullSize.Image = null;
                pxbFullSize.Visible = false;
                RefreshImage();
                btnSave.Enabled = false;
                MessageBox.Show(this, "Font loading failed" + (error == null ? "." : ": " + error), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //pxbFullSize.BackColor = Color.Maroon;
            pxbFullSize.Visible = true;
            lblValFilename.Text = Path.GetFileName(path);
            lblValCharacters.Text = loadedfont.Length.ToString();
            lblValFontWidth.Text = loadedfont.FontWidth.ToString();
            lblValFontHeight.Text = loadedfont.FontHeight.ToString();
            ReloadImageInfo();
            //btnSave.Enabled = loadOk;
            //if (loadOk)
            //    btnSave.Focus();
        }

        private void FrmCnC64ImgViewer_Shown(object sender, EventArgs e)
        {
            if (filename != null)
                LoadFontFile(filename);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Westwood font files (*.fnt)|*.fnt|All Files (*.*)|*.*";
            ofd.InitialDirectory = String.IsNullOrEmpty(filename) ? Path.GetFullPath(".") : Path.GetDirectoryName(filename);
            DialogResult res = ofd.ShowDialog(this);
            if (res != System.Windows.Forms.DialogResult.OK)
                return;
            LoadFontFile(ofd.FileName);
        }

        private void NumIndex_ValueChanged(object sender, EventArgs e)
        {
            ReloadImageInfo();
        }

        private void ReloadImageInfo()
        {
            Int32 curIndex = (Int32)numIndex.Value;
            if (loadedfont == null)
            {
                pxbImage.Image = null;
                curHeight = 0;
                curWidth = 0;
                curYOffset = 0;
                lblValHeight.Text = "-";
                lblValWidth.Text = "-";
                lblValYOffset.Text = "-";
                return;
            }
            pxbImage.Image = loadedfont.GetBitmap(curIndex, PaletteRainbow);
            curHeight = loadedfont.GetCharHeight(curIndex);
            curWidth = loadedfont.GetCharWidth(curIndex);
            curYOffset = loadedfont.GetCharYOffset(curIndex);
            lblValHeight.Text = curHeight.ToString();
            lblValWidth.Text = curWidth.ToString();
            lblValYOffset.Text = curYOffset.ToString();
            RefreshImage();
        }

        private void NumZoom_ValueChanged(object sender, EventArgs e)
        {
            RefreshImage();
        }
        
        private void RefreshImage()
        {
            // Beware! Heavy grid logic abound!
            Bitmap bm = (Bitmap)pxbImage.Image;
            
            // False if no actual image data loaded.
            Boolean imgLoadOk = bm != null && this.curWidth != 0 && this.curHeight != 0;
            Boolean fntLoadOk = loadedfont != null;
            Int32 zoom = (Int32)numZoom.Value;
            Boolean drawGrid = chkGrid.Checked;
            Boolean drawOutline = chkOutline.Checked;
            // AddGred means some kind of grid overlay needs to be drawn; either the grid itself or the outline.
            Boolean addGrid = zoom > 4 && (drawGrid || drawOutline);
            pxbImage.Visible = imgLoadOk | addGrid;
            pxbImage.Location = new Point(0, curYOffset * zoom);
            pxbImage.Width = Math.Max(this.curWidth * zoom, 1);
            pxbImage.Height = Math.Max(this.curHeight * zoom, 1);
            Bitmap gridImageSmall = null;
            if (fntLoadOk && addGrid)
            {
                //Draw normal grid, with or without special outline
                Color[] palette = new Color[] {Color.Transparent, Color.Black, drawOutline ? GridColor : GridColorOuter, GridColorFrame};
                gridImageSmall = ImageUtils.GenerateGridImage(curWidth, curHeight, zoom, palette, 0, drawGrid ? (Byte)2 : (Byte)0, drawOutline ? (Byte)3 : (Byte)2);
                if (!drawOutline)
                {
                    // If outline is disabled, restore any edges touching the full size edges to the grid colour of the outside grid.
                    ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, 0, this.curHeight * zoom, 1, true); // left line
                    if (this.curYOffset == 0)
                        ImageUtils.DrawRect8Bit(gridImageSmall, 0, 0, curWidth * zoom, 0, 1, true); // top line
                    if (this.curHeight + this.curYOffset == loadedfont.FontHeight)
                        ImageUtils.DrawRect8Bit(gridImageSmall, 0, curHeight * zoom, curWidth * zoom, curHeight * zoom, 1, true); // bottom line
                    if (this.curWidth == loadedfont.FontWidth)
                        ImageUtils.DrawRect8Bit(gridImageSmall, curWidth * zoom, 0, curWidth * zoom, curHeight * zoom, 1, true); // right line
                }
            }
            pxbEditGridBehind.Visible = fntLoadOk && addGrid;
            pxbEditGridBehind.Location = new Point(0, curYOffset * zoom);
            pxbEditGridBehind.Width = Math.Max(this.curWidth * zoom + 1, 1);
            pxbEditGridBehind.Height = Math.Max(this.curHeight * zoom + 1, 1);
            pxbEditGridBehind.Image = gridImageSmall;
            pxbEditGridFront.Visible = true;
            // Parent of pxbImage; no change needed.
            //pxbEditGridFront.Location = new Point(0, curYOffset * zoom);
            pxbEditGridFront.BackColor = Color.Transparent;
            pxbEditGridFront.BackgroundImage = addGrid ? gridImageSmall : null;
            pxbEditGridFront.Width = Math.Max(this.curWidth * zoom, 1);
            pxbEditGridFront.Height = Math.Max(this.curHeight * zoom, 1);

            //pxbEditGridFront.Image is the overlay image on which the currently hovered pixel is drawn. Make it null if one of the dimensions is 0.
            pxbEditGridFront.Image = imgLoadOk ? ImageUtils.GenerateBlankImage(this.curWidth, this.curHeight, new Color[] { Color.Transparent, currentPalette[this.currentPaintColorFront] }, 0) : null;
            pxbFullSize.Visible = fntLoadOk;
            if (fntLoadOk)
            {
                if (addGrid && drawGrid)
                    pxbFullSize.Image = ImageUtils.GenerateGridImage(loadedfont.FontWidth, loadedfont.FontHeight, zoom, new Color[]{ GridColorBg, GridColorOuter, GridColorOuterFrame}, 0, 1, 2);
                else
                {
                    // No extra border since it'll deform the image
                    Int32 bgWidth = loadedfont.FontWidth * zoom;
                    Int32 bgHeight = loadedfont.FontHeight * zoom;
                    // ... except if the outline is drawn
                    if (drawOutline && curWidth == loadedfont.FontWidth && addGrid)
                        bgWidth++;
                    if (drawOutline && curHeight + curYOffset == loadedfont.FontHeight && addGrid)
                        bgHeight++;
                    pxbFullSize.Image = ImageUtils.GenerateBlankImage(bgWidth, bgHeight, new Color[] {GridColorBg}, 0);
                    pxbFullSize.BackColor = GridColorBg;
                    pxbFullSize.Width = loadedfont.FontWidth;
                    pxbFullSize.Height = loadedfont.FontHeight;
                }
            }
        }
        
        private void ImageBox_Click(object sender, EventArgs e)
        {
            pnlImageScroll.Focus();
        }

        private void ChkTrans_CheckedChanged(object sender, EventArgs e)
        {
            if (loadedfont == null)
                return;
            ReloadImageInfo();
        }

        private void ChkOutline_CheckedChanged(object sender, EventArgs e)
        {
            if (loadedfont == null)
                return;
            ReloadImageInfo();
        }

        private void pxbEditGridFront_MouseMove(object sender, MouseEventArgs e)
        {
            CheckMouse(sender, e, true);
        }

        private void pxbEditGridFront_MouseDown(object sender, MouseEventArgs e)
        {
            CheckMouse(sender, e, false);
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
                if (lastHoverPixelX == picX && lastHoverPixelY == picY)
                    return;
                // Clear previous pixel
                if (lastHoverPixelX != -1 && lastHoverPixelY != -1)
                    ImageUtils.DrawRect8Bit(gridFront, lastHoverPixelX, lastHoverPixelY, lastHoverPixelX, lastHoverPixelY, 0, true);
                // Draw new pixel
                if (inBounds)
                    ImageUtils.DrawRect8Bit(gridFront, picX, picY, picX, picY, 1, true);
                this.lastHoverPixelX = picX;
                this.lastHoverPixelY = picY;
                pxbEditGridFront.Invalidate();
            }
            Boolean isLeftClick = (e.Button & MouseButtons.Left) != 0;
            Boolean isRightClick = (e.Button & MouseButtons.Right) != 0;
            if (this.loadedfont!= null && inBounds && (isLeftClick || isRightClick))
            {
                Int32 curIndex = (Int32)this.numIndex.Value;
                if (isLeftClick)
                    this.loadedfont.PaintPixel(curIndex, picX, picY, currentPaintColorFront);
                else
                    this.loadedfont.PaintPixel(curIndex, picX, picY, currentPaintColorBack);
                this.pxbImage.Image = this.loadedfont.GetBitmap(curIndex, PaletteRainbow);
            }
        }

        private void pxbEditGridFront_MouseLeave(object sender, EventArgs e)
        {
            Bitmap gridFront = (Bitmap)this.pxbEditGridFront.Image;
            this.pxbEditGridFront.Image = pxbImage.Image != null ? ImageUtils.GenerateBlankImage(gridFront.Width, gridFront.Height, new Color[] { Color.Transparent, currentPalette[this.currentPaintColorFront] }, 0) : null;
            this.lastHoverPixelX = -1;
            this.lastHoverPixelY = -1;
        }
        
        private void lblPaintColor_Click(object sender, EventArgs e)
        {
            /*/
            ColorDialog cdl = new ColorDialog();
            cdl.Color = this.currentPaintColorFront;
            cdl.FullOpen = true;
            cdl.CustomColors = this.customcolors;
            DialogResult res = cdl.ShowDialog(this);
            customcolors = cdl.CustomColors;
            if (res == DialogResult.OK || res == DialogResult.Yes)
            {
                this.currentPaintColorFront = cdl.Color;
                this.lblPaintColor.BackColor = currentPaintColorFront;
                RefreshImage();
            }
            pxbEditGridFront.Image.Palette[1] = cdl.Color;
            //*/
        }

    }
}
