using System;
using System.Windows.Forms;

namespace Nyerguds.Util.Ui
{
    public abstract class CustomControlInfo<TControl, TInfoObject> where TControl : Control
    {
        public String Name { get; set; }
        public String ClassName { get; set; }
        public TInfoObject[] Properties { get; set; }

        public abstract TControl MakeControl(TInfoObject property, ListedControlController<TInfoObject> controller);

        public override String ToString()
        {
            return this.Name;
        }
    }
}
