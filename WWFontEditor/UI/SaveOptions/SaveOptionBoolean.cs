using System;
using Nyerguds.Util;

namespace Nyerguds.Util.Ui.SaveOptions
{
    public partial class SaveOptionBoolean : SaveOptionControl
    {

        public SaveOptionBoolean() : this(null, null) { }

        public SaveOptionBoolean(SaveOption info, ListedControlController<SaveOption> controller)
        {
            InitializeComponent();
            Init(info, controller);
        }

        public override void UpdateInfo(SaveOption info)
        {
            this.Info = info;
            this.chkOption.Text = GeneralUtils.DoubleFirstAmpersand(this.Info.UiString);
            this.chkOption.Checked = GeneralUtils.IsTrueValue(this.Info.SaveData);
        }
        
        public override void FocusValue()
        {
            this.chkOption.Focus();
        }

        private void chkOption_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Info == null)
                return;
            this.Info.SaveData = this.chkOption.Checked ? "1" : "0";
            if (this.m_Controller != null)
                this.m_Controller.UpdateControlInfo(Info);
        }
    }
}
