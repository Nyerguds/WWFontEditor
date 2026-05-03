using System;
using System.Linq;
using System.Windows.Forms;
using Nyerguds.Util;
using Nyerguds.Util.Ui;
using Nyerguds.Util.Ui.SaveOptions;

namespace Nyerguds.Util.Ui.SaveOptions
{
    public partial class FrmExtraOptions : Form, ListedControlController<SaveOption>
    {
        private SaveOptionInfo m_soi;
        
        public FrmExtraOptions(String title)
        {
            InitializeComponent();
            this.Text = title;
            this.m_soi = new SaveOptionInfo();
        }

        public void Init(SaveOptionInfo soi)
        {
            this.m_soi = soi;
            this.lstOptions.Populate(this.m_soi, this);
        }

        public SaveOption[] GetSaveOptions()
        {
            return this.m_soi.Properties;
        }

        public void UpdateControlInfo(SaveOption updateInfo)
        {
            SaveOption current = this.m_soi.Properties.First(x => String.Equals(x.Code, updateInfo.Code));
            if (current != null)
                current.SaveData = updateInfo.SaveData;
        }
    }
}
