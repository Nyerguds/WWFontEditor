using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WWFontEditor.UI.Tools
{
    public class EnhRichTextBox : RichTextBox
    {
        private const Int32 WM_SETFOCUS = 0x0007;
        private const Int32 WM_KILLFOCUS = 0x0008;
        private const Int32 WM_SETCURSOR = 0x20;
        
        private Boolean m_AllowScrollWheelZoom = true;
        private Boolean m_AllowKeyZoom = true;
        private Boolean m_AllowSelecting = true;

        [Description("Allow adjusting zoom with [Ctrl]+[Scrollwheel]"), Category("Behavior")]
        [DefaultValue(true)]
        public Boolean AllowScrollWheelZoom
        {
            get { return m_AllowScrollWheelZoom; }
            set { m_AllowScrollWheelZoom = value; }
        }

        [Description("Allow adjusting zoom with [Ctrl]+[Shift]+[,] and [Ctrl]+[Shift]+[.]"), Category("Behavior")]
        [DefaultValue(true)]
        public Boolean AllowKeyZoom
        {
            get { return m_AllowKeyZoom; }
            set { m_AllowKeyZoom = value; }
        }

        [Description("Allow selecting text in the textbox"), Category("Behavior")]
        [DefaultValue(true)]
        public bool Selectable
        {
            get { return m_AllowSelecting; }
            set
            {
                m_AllowSelecting = value;
                //this.Cursor = value ? Cursors.IBeam : Cursors.Arrow;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Prevent flicker in Win10
            if (!m_AllowSelecting && m.Msg == WM_SETCURSOR)
                return;
            if (!m_AllowSelecting && m.Msg == WM_SETFOCUS)
                m.Msg = WM_KILLFOCUS;
            else if (!m_AllowScrollWheelZoom && (m.Msg == 0x115 || m.Msg == 0x20a) && (Control.ModifierKeys & Keys.Control) != 0)
                return;
            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.m_AllowKeyZoom && e.Control && e.Shift && (e.KeyValue == (Int32)Keys.Oemcomma || e.KeyValue == (Int32)Keys.OemPeriod))
                return;
            base.OnKeyDown(e);
        }
    }
}