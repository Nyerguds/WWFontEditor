using System;
using System.Windows.Forms;

namespace Nyerguds.Util.UI
{
    public class ComboBoxSmartWidth : ComboBox
    {
        protected override void OnDropDown(EventArgs e)
        {
            SetDropDownWidth();
            base.OnDropDown(e);
        }

        private void SetDropDownWidth()
        {
            Int32 widestStringInPixels = this.Width;
            Boolean noDisplayMember = String.IsNullOrEmpty(this.DisplayMember);
            foreach (Object o in Items)
            {
                String toCheck;
                if (noDisplayMember)
                    toCheck = o == null? String.Empty : o.ToString();
                else
                {
                    Object val = null;
                    try { val = o.GetType().GetProperty(this.DisplayMember).GetValue(o, null); }
                    catch { /* ignore; if it fails, just consider it empty. */ }
                    toCheck = val == null ? String.Empty : val.ToString();
                }
                if (toCheck.Length > 0)
                {
                    Int32 newWidth = TextRenderer.MeasureText(toCheck, this.Font).Width;
                    if (newWidth > widestStringInPixels)
                        widestStringInPixels = newWidth;
                }
            }
            if (this.Items.Count * this.ItemHeight > this.DropDownHeight)
                widestStringInPixels += SystemInformation.VerticalScrollBarWidth;
            this.DropDownWidth = widestStringInPixels;
        }
    }
}
