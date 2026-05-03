using System;
using Nyerguds.Util;

namespace Nyerguds.Util.Ui.SaveOptions
{
    public class SaveOptionInfo : CustomControlInfo<SaveOptionControl, SaveOption>
    {
        public override SaveOptionControl MakeControl(SaveOption property, ListedControlController<SaveOption> controller)
        {
            switch (property.Type)
            {
                case SaveOptionType.Number:
                    return new SaveOptionNumber(property, controller);
                case SaveOptionType.Boolean:
                    return new SaveOptionBoolean(property, controller);
                case SaveOptionType.String:
                    return new SaveOptionString(property, controller);
                case SaveOptionType.ChoicesList:
                    return new SaveOptionChoices(property, controller);
                case SaveOptionType.FileOpen:
                    throw new NotImplementedException("Not yet implemented.");
                case SaveOptionType.FileSave:
                    throw new NotImplementedException("Not yet implemented.");
                case SaveOptionType.Folder:
                    throw new NotImplementedException("Not yet implemented.");
                default:
                    return null;
            }
        }
    }
}