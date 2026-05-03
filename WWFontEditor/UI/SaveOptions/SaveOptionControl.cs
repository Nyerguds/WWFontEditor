using System.Windows.Forms;
using Nyerguds.Util;

namespace Nyerguds.Util.Ui
{
    public class SaveOptionControl : UserControl
    {
        public SaveOption Info = null;
        protected ListedControlController<SaveOption> m_Controller = null;

        protected void Init(SaveOption info, ListedControlController<SaveOption> controller)
        {
            UpdateInfo(info);
            this.m_Controller = controller;
        }
        public virtual void FocusValue() { this.Focus(); }
        public virtual void UpdateInfo(SaveOption info) { this.Info = info; }
    }
}
