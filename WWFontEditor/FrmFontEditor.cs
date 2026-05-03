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

        
        private String m_FileName;
        private FntFile m_Loadedfont;
        private Int32 m_CurHeight;
        private Int32 m_CurWidth;
        private Int32 m_CurYOffset;
        private Int32 m_LastHoverPixelX = -1;
        private Int32 m_LastHoverPixelY = -1;
        // TODO: Change these two to Byte later, when implementing color palette support.
        private Byte m_CurrentPaintColorFront = 1;
        private Byte m_CurrentPaintColorBack = 0;
        private Color[] m_CurrentPalette;

        private Int32[] m_Customcolors;


        private Color m_GridColor = Color.Blue;
        private Color m_GridColorFrame = Color.Red;
        private Color m_GridColorOuter = Color.White;
        private Color m_GridColorOuterFrame = Color.Black;
        private Color m_GridColorBg = Color.LightGray;

        public FrmFontEditor()
        {
            m_CurrentPalette = PaletteRainbow;
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
            this.lblPaintColor.BackColor = m_CurrentPalette[this.m_CurrentPaintColorFront];
            this.Text = "Westwood Font Editor " + GeneralUtils.ProgramVersion() + " - Created by Nyerguds";
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
            m_FileName = path;
            String error = null;
            Boolean loadOk = false;
            try
            {
                m_Loadedfont = null;
                numIndex.Value = 0;
                Byte[] data = File.ReadAllBytes(path);
                m_Loadedfont = new FntFile(data);
                loadOk = m_Loadedfont != null;
                numIndex.Enabled = loadOk;
                if (loadOk)
                {
                    numIndex.Maximum = m_Loadedfont.LastIndex;
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                m_Loadedfont = null;
            }
            if (!loadOk)
            {
                m_FileName = null;
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
            lblValCharacters.Text = m_Loadedfont.Length.ToString();
            lblValFontWidth.Text = m_Loadedfont.FontWidth.ToString();
            lblValFontHeight.Text = m_Loadedfont.FontHeight.ToString();
            ReloadImageInfo();
            //btnSave.Enabled = loadOk;
            //if (loadOk)
            //    btnSave.Focus();
        }


        private void SaveFontFile(String fileName)
        {
            if (m_Loadedfont == null)
                return;
            FntFileVersion ver = m_Loadedfont.Unknown0E == 0x1012 ? FntFileVersion.CnC : FntFileVersion.Kyrandia;
            Byte[] filedata = m_Loadedfont.WriteFntFile(ver);
            File.WriteAllBytes(fileName, filedata);
        }

        private void FrmFontEditor_Shown(object sender, EventArgs e)
        {
            if (m_FileName != null)
                LoadFontFile(m_FileName);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (m_Loadedfont == null)
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

        private void BtnOpen_Click(object sender, EventArgs e)
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

        private void NumIndex_ValueChanged(object sender, EventArgs e)
        {
            ReloadImageInfo();
        }

        private void ReloadImageInfo()
        {
            Int32 curIndex = (Int32)numIndex.Value;
            if (m_Loadedfont == null)
            {
                pxbImage.Image = null;
                m_CurHeight = 0;
                m_CurWidth = 0;
                m_CurYOffset = 0;
                lblValHeight.Text = "-";
                lblValWidth.Text = "-";
                lblValYOffset.Text = "-";
                return;
            }
            pxbImage.Image = m_Loadedfont.GetBitmap(curIndex, PaletteRainbow);
            m_CurHeight = m_Loadedfont.GetCharHeight(curIndex);
            m_CurWidth = m_Loadedfont.GetCharWidth(curIndex);
            m_CurYOffset = m_Loadedfont.GetCharYOffset(curIndex);
            lblValHeight.Text = m_CurHeight.ToString();
            lblValWidth.Text = m_CurWidth.ToString();
            lblValYOffset.Text = m_CurYOffset.ToString();
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
            Boolean imgLoadOk = bm != null && this.m_CurWidth != 0 && this.m_CurHeight != 0;
            Boolean fntLoadOk = m_Loadedfont != null;
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
                    if (this.m_CurHeight + this.m_CurYOffset == m_Loadedfont.FontHeight)
                        ImageUtils.DrawRect8Bit(gridImageSmall, 0, m_CurHeight * zoom, m_CurWidth * zoom, m_CurHeight * zoom, 1, true); // bottom line
                    if (this.m_CurWidth == m_Loadedfont.FontWidth)
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
            pxbEditGridFront.Image = imgLoadOk ? ImageUtils.GenerateBlankImage(this.m_CurWidth, this.m_CurHeight, new Color[] { Color.Transparent, m_CurrentPalette[this.m_CurrentPaintColorFront] }, 0) : null;
            pxbFullSize.Visible = fntLoadOk;
            if (fntLoadOk)
            {
                if (addGrid && drawGrid)
                    pxbFullSize.Image = ImageUtils.GenerateGridImage(m_Loadedfont.FontWidth, m_Loadedfont.FontHeight, zoom, new Color[]{ m_GridColorBg, m_GridColorOuter, m_GridColorOuterFrame}, 0, 1, 2);
                else
                {
                    // No extra border since it'll deform the image
                    Int32 bgWidth = m_Loadedfont.FontWidth * zoom;
                    Int32 bgHeight = m_Loadedfont.FontHeight * zoom;
                    // ... except if the outline is drawn
                    if (drawOutline && m_CurWidth == m_Loadedfont.FontWidth && addGrid)
                        bgWidth++;
                    if (drawOutline && m_CurHeight + m_CurYOffset == m_Loadedfont.FontHeight && addGrid)
                        bgHeight++;
                    pxbFullSize.Image = ImageUtils.GenerateBlankImage(bgWidth, bgHeight, new Color[] {m_GridColorBg}, 0);
                    pxbFullSize.BackColor = m_GridColorBg;
                    pxbFullSize.Width = m_Loadedfont.FontWidth;
                    pxbFullSize.Height = m_Loadedfont.FontHeight;
                }
            }
        }
        
        private void ImageBox_Click(object sender, EventArgs e)
        {
            pnlImageScroll.Focus();
        }

        private void ChkTrans_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Loadedfont == null)
                return;
            ReloadImageInfo();
        }

        private void ChkOutline_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Loadedfont == null)
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
                if (m_LastHoverPixelX == picX && m_LastHoverPixelY == picY)
                    return;
                // Clear previous pixel
                if (m_LastHoverPixelX != -1 && m_LastHoverPixelY != -1)
                    ImageUtils.DrawRect8Bit(gridFront, m_LastHoverPixelX, m_LastHoverPixelY, m_LastHoverPixelX, m_LastHoverPixelY, 0, true);
                // Draw new pixel
                if (inBounds)
                    ImageUtils.DrawRect8Bit(gridFront, picX, picY, picX, picY, 1, true);
                this.m_LastHoverPixelX = picX;
                this.m_LastHoverPixelY = picY;
                pxbEditGridFront.Invalidate();
            }
            Boolean isLeftClick = (e.Button & MouseButtons.Left) != 0;
            Boolean isRightClick = (e.Button & MouseButtons.Right) != 0;
            if (this.m_Loadedfont!= null && inBounds && (isLeftClick || isRightClick))
            {
                Int32 curIndex = (Int32)this.numIndex.Value;
                if (isLeftClick)
                    this.m_Loadedfont.PaintPixel(curIndex, picX, picY, m_CurrentPaintColorFront);
                else
                    this.m_Loadedfont.PaintPixel(curIndex, picX, picY, m_CurrentPaintColorBack);
                this.pxbImage.Image = this.m_Loadedfont.GetBitmap(curIndex, PaletteRainbow);
            }
        }

        private void pxbEditGridFront_MouseLeave(object sender, EventArgs e)
        {
            Bitmap gridFront = (Bitmap)this.pxbEditGridFront.Image;
            this.pxbEditGridFront.Image = pxbImage.Image != null ? ImageUtils.GenerateBlankImage(gridFront.Width, gridFront.Height, new Color[] { Color.Transparent, m_CurrentPalette[this.m_CurrentPaintColorFront] }, 0) : null;
            this.m_LastHoverPixelX = -1;
            this.m_LastHoverPixelY = -1;
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
