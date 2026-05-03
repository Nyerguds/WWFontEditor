using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Nyerguds.Util.UI
{
    public class ComboBoxSmartWidth : ComboBox
    {
        protected override void OnDropDown(EventArgs e)
        {
            SetDropDownWidth(e);
            base.OnDropDown(e);
        }

        private void SetDropDownWidth(EventArgs e)
        {
            Int32 widestStringInPixels = this.Width;
            Int32 count = this.Items.Count;
            Boolean hasScrollBar = count * this.ItemHeight > this.DropDownHeight;
            if (hasScrollBar)
                widestStringInPixels -= SystemInformation.VerticalScrollBarWidth;
            Boolean noDisplayMember = String.IsNullOrEmpty(this.DisplayMember);
            for (Int32 i = 0; i < count; ++i)
            {
                Object o = this.Items[i];
                String toCheck;
                if (noDisplayMember)
                    toCheck = o == null ? String.Empty : o.ToString();
                else
                {
                    Object val = null;
                    if (o != null)
                    {
                        try
                        {
                            PropertyInfo po = o.GetType().GetProperty(this.DisplayMember);
                            if (po != null)
                                val = po.GetValue(o, null);
                        }
                        catch
                        {
                            /* ignore; if it fails, just consider it empty. */
                        }
                    }
                    toCheck = val == null ? String.Empty : val.ToString();
                }
                if (toCheck.Length > 0)
                {
                    Int32 newWidth = TextRenderer.MeasureText(toCheck, this.Font).Width;
                    Int32 newWidth2;
                    using (Graphics g = this.CreateGraphics())
                        newWidth2 = g.MeasureString(toCheck, this.Font).ToSize().Width;
                    newWidth = Math.Max(newWidth, newWidth2);
                    if (this.DrawMode == DrawMode.OwnerDrawFixed)
                        newWidth += 4;
                    if (newWidth > widestStringInPixels)
                        widestStringInPixels = newWidth;
                }
            }
            if (hasScrollBar)
                widestStringInPixels += SystemInformation.VerticalScrollBarWidth;
            this.DropDownWidth = widestStringInPixels;
        }
    }
}
