// ===========================================================================
// This control is licensed under the Code Project Open License (CPOL) 1.02.
// See https://www.codeproject.com/info/cpol10.aspx for more info.
// Code taken from https://www.codeproject.com/articles/717312/pixelbox
// Written by Yvan Rodrigues, 28 Jan 2014.
// ===========================================================================

using System;
using System.ComponentModel;
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
            // docs on this are wrong; putting it to Half PREVENTS it from shifting the whole thing up and to the left by half a (zoomed) pixel.
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(pe);
        }
        #endregion

    }
}
