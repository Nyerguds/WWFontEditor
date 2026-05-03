using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;

namespace WWFontEditor.UI.Tools
{
    public class DataGridViewScrollSupport : DataGridView
    {
        private Int32 BORDERWIDTH = 2;
        private Boolean m_VertScrollAlwaysVisible = false;

        public event EventHandler AlwaysShowVerticalScrollbarChanged;

        public DataGridViewScrollSupport()
        {
            //make scrollbar visible & hook up handlers
            this.VerticalScrollBar.VisibleChanged += ShowScrollBars;
            this.AlwaysShowVerticalScrollbarChanged += ShowScrollBars;
        }

        [DefaultValue(ScrollBars.Both)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new ScrollBars ScrollBars
        {
            get { return base.ScrollBars; }
            set
            {
                base.ScrollBars = value;
                ShowScrollBars(this, new EventArgs());
                this.PerformLayout();
            }
        }


        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Boolean AlwaysShowVerticalScrollbar
        {
            get { return this.m_VertScrollAlwaysVisible; }
            set
            {
                this.m_VertScrollAlwaysVisible = value;
                if (this.AlwaysShowVerticalScrollbarChanged != null)
                    this.AlwaysShowVerticalScrollbarChanged(this, new EventArgs());
                this.PerformLayout();
            }
        }
        
        public Int32 VerticalScrollbarOffset
        {
            get { return this.VerticalScrollBar.Value; }
            set
            {
                if (this.ScrollBars == ScrollBars.Both || this.ScrollBars == ScrollBars.Vertical && this.VerticalScrollBar.Visible && this.VerticalScrollBar.Enabled)
                {
                    Int32 scroll = Math.Min(this.VerticalScrollBar.Maximum, Math.Max(this.VerticalScrollBar.Minimum, value));
                    this.VerticalScrollBar.Value = scroll;
                    PropertyInfo verticalOffsetProp = this.GetType().GetProperty("VerticalOffset", BindingFlags.NonPublic | BindingFlags.Instance);
                    // Because fuck you, whoever thought making this readonly was clever.
                    verticalOffsetProp.SetValue(this, scroll, null);
                }
                this.PerformLayout();                
            }
        }

        private void ShowScrollBars(object sender, EventArgs e)
        {
            if ((this.ScrollBars != ScrollBars.Both && this.ScrollBars != ScrollBars.Vertical) || !this.AlwaysShowVerticalScrollbar || this.VerticalScrollBar.Visible)
                return;
            Int32 width = this.VerticalScrollBar.Width;
            this.VerticalScrollBar.Location = new Point(this.ClientRectangle.Width - width - this.BORDERWIDTH, this.BORDERWIDTH);
            this.VerticalScrollBar.Size = new Size(width, this.ClientRectangle.Height - this.BORDERWIDTH*2);
            this.VerticalScrollBar.Show();
        }

    }
}
