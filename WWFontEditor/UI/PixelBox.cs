using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RedCell.UI.Controls
{
    /// <summary>
    /// A PictureBox with configurable interpolation mode.
    /// </summary>
    public class PixelBox : PictureBox
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelBox"/> class.
        /// </summary>
        public PixelBox ()
        {
            // Set default.
            InterpolationMode = InterpolationMode.Default;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interpolation mode.
        /// </summary>
        /// <value>The interpolation mode.</value>
        [Category("Behavior")]
        [DefaultValue(InterpolationMode.Default)]
        public InterpolationMode InterpolationMode { get; set; }
        #endregion

        #region Overrides of PictureBox
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="pe">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint (PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode;
            // docs on this are wrong; putting it to Half makes it not shift the whole thing up and to the left by half a (zoomed) pixel.
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(pe);
        }
        #endregion

        #region Overrides of PictureBoxEx
        /*/
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Int32 myIndex = this.Parent.Controls.GetChildIndex(this);
            for (Int32 index = this.Parent.Controls.Count - 1; index > myIndex; index--)
            {
                PictureBox ctl = this.Parent.Controls[index] as PictureBox;
                if (ctl == null)
                    continue;
                Rectangle clip = ctl.RectangleToClient(this.RectangleToScreen(this.DisplayRectangle));
                clip.Intersect(ctl.DisplayRectangle);
                if (clip.Width == 0 || clip.Height == 0) continue;
                GraphicsState save = e.Graphics.Save();
                e.Graphics.TranslateTransform(ctl.Left - this.Left, ctl.Top - this.Top);
                using (Region rgn = new Region(clip))
                {
                    e.Graphics.Clip = rgn;
                    InvokePaintBackground(ctl, e);
                    InvokePaint(ctl, e);
                }
                e.Graphics.Restore(save);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }
        //*/
        #endregion

    }
}
