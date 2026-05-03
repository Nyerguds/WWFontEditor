using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WWFontEditor.Domain;

namespace WWFontEditor
{
    public partial class FrmFontEditTest : Form
    {
        private String filename;
        private FntFile loadedfont;

        public FrmFontEditTest()
        {
            InitializeComponent();
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
                    LoadImage(path);
            }
        }


        private void LoadImage(String path)
        {
            filename = path;
            String error = null;
            Bitmap bm = null;
            Boolean loadOk = false;
            try
            {
                Byte[] data = File.ReadAllBytes(path);
                loadedfont = new FntFile(data);
                loadOk = loadedfont != null;
                if (loadOk)
                {
                    bm = loadedfont.GetBitmap(0, null);
                    if (bm == null)
                        loadOk = false;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                loadedfont = null;
            }
            if (!loadOk)
            {
                pixelBox1.Image = null;
                MessageBox.Show(this, "Font loading failed" + (error == null ? "." : ": " + error), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            pixelBox1.Image = bm;
            pixelBox1.Width = bm.Width * 5;
            pixelBox1.Height = bm.Width * 5;
            
        }

    }
}
