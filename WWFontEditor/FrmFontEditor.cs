using WWFontEditor.Domain;
using WWFontEditor.Ui;
using ColorManipulation;
using Nyerguds.Util.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWFontEditor
{
    public partial class FrmFontEditor : Form
    {
        private String filename;
        private FntFile loadedfont;
        private Int32 curHeight;
        private Int32 curWidth;
        private Int32 curYOffset;

        private Color GridColor = Color.Blue;
        private Color GridColorFrame = Color.Red;
        private Color GridColorOuter = Color.White;
        private Color GridColorOuterFrame = Color.Black;
        private Color GridColorBg = Color.LightGray;

        public FrmFontEditor()
        {
            InitializeComponent();
            pxbEditGridBehind.Parent = pxbFullSize;
            pxbEditGridBehind.BackColor = Color.Transparent;
            pxbImage.Parent = pxbFullSize;
            pxbImage.BackColor = Color.Transparent;
            pxbImage.BringToFront();
            pxbEditGridFront.Parent = pxbImage;
            pxbEditGridFront.BackColor = Color.Transparent;
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
                {
                    numIndex.Maximum = loadedfont.LastIndex;
                }
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
                lblValColors.Text = "-";
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
            lblValColors.Text = (loadedfont.LastIndex + 1).ToString();
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

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
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

        private void numIndex_ValueChanged(object sender, EventArgs e)
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
            pxbImage.Image = loadedfont.GetBitmap(curIndex, null);
            curHeight = loadedfont.HeightsList[curIndex];
            curWidth = loadedfont.WidthsList[curIndex];
            curYOffset = loadedfont.OffsetYList[curIndex];
            lblValHeight.Text = curHeight.ToString();
            lblValWidth.Text = curWidth.ToString();
            lblValYOffset.Text = curYOffset.ToString();
            RefreshImage();
        }

        private void numZoom_ValueChanged(object sender, EventArgs e)
        {
            RefreshImage();
        }
        
        private void RefreshImage()
        {
            Bitmap bm = (Bitmap)pxbImage.Image;
            Boolean imgLoadOk = bm != null;
            Boolean fntLoadOk = loadedfont != null;
            Int32 zoom = (Int32)numZoom.Value;
            Boolean addGrid = zoom > 4 && chkGrid.Checked;
            pxbImage.Visible = imgLoadOk;
            pxbImage.Location = new Point(0, curYOffset * zoom);
            pxbImage.Width = imgLoadOk ? bm.Width * zoom : 100;
            pxbImage.Height = imgLoadOk ? bm.Height * zoom : 100;
            Bitmap GridImageSmall = fntLoadOk && addGrid ? ImageUtils.GenerateGridImage(curWidth, curHeight, zoom, Color.Transparent, GridColor, GridColorFrame) : null;
            pxbEditGridBehind.Visible = fntLoadOk && addGrid;
            pxbEditGridBehind.Location = new Point(0, curYOffset * zoom);
            pxbEditGridBehind.Image = GridImageSmall;
            pxbEditGridFront.Visible = imgLoadOk && addGrid;
            pxbEditGridFront.BackColor = Color.Transparent;
            pxbEditGridFront.Image = imgLoadOk && addGrid ? GridImageSmall : null;
            pxbFullSize.Visible = fntLoadOk;
            if (fntLoadOk)
            {
                if (addGrid)
                    pxbFullSize.Image = ImageUtils.GenerateGridImage(loadedfont.FontWidth, loadedfont.FontHeight, zoom, GridColorBg, GridColorOuter, GridColorOuterFrame);
                else
                {
                    // No extra border since it'll deform the image
                    pxbFullSize.Image = ImageUtils.GenerateBlankImage(loadedfont.FontWidth * zoom, loadedfont.FontHeight * zoom, GridColorBg);
                    pxbFullSize.BackColor = GridColorBg;
                    pxbFullSize.Width = loadedfont.FontWidth;
                    pxbFullSize.Height = loadedfont.FontHeight;
                }
            }
        }
        
        private void picImage_Click(object sender, EventArgs e)
        {
            pnlImageScroll.Focus();
        }

        private void chkTrans_CheckedChanged(object sender, EventArgs e)
        {
            if (loadedfont == null)
                return;
            ReloadImageInfo();
        }

    }
}
