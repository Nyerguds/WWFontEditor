using Nyerguds.Util;

namespace Nyerguds.Util.Ui.SaveOptions
{
    public class SaveOptionsList : ControlsList<SaveOptionControl, SaveOption>
    {
        protected override void FocusItem(SaveOptionControl control)
        {
            control.FocusValue();
        }
    }
}