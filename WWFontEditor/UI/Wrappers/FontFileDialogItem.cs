using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WWFontEditor.Domain;

namespace WWFontEditor.UI.Wrappers
{

    public class FontFileDialogItem
    {
        public String Extension { get; private set; }
        public String Filter { get { return "*." + Extension; } }
        public String Description { get; private set; }
        public String FullDescription
        {
            get { return String.Format("{0} (*.{1})", this.Description, this.Extension); }
        }

        public FontFile FontTypeObject { get { return (FontFile)Activator.CreateInstance(FontType); } }
        public Type FontType { get; private set; }

        public FontFileDialogItem(Type fonttype)
        {
            if (!fonttype.IsSubclassOf(typeof(FontFile)))
                throw new ArgumentException("Entries in autoDetectTypes list must all be FontFile classes!", "fonttype");
            FontType = fonttype;
            // Will immediately throw an exception if the type cannot be instantiated.
            Description = FontTypeObject.ShortTypeDescription;
            this.Extension = FontTypeObject.FileExtension;
        }

        public override String ToString()
        {
            return FontTypeObject.ShortTypeDescription;
        }

        public static String GetFileFilter(FontFileDialogItem[] fontTypes, Boolean forOpen)
        {
            String[] types = new String[fontTypes.Length + (forOpen ? 2 : 0)];
            HashSet<String> allTypes = forOpen ? new HashSet<String>() : null;
            for (Int32 i = 0; i < fontTypes.Length; i++)
            {
                FontFileDialogItem fontType = fontTypes[i];
                types[i + (forOpen ? 1 : 0)] = String.Format("{0} ({1})|{1}", fontType.Description, fontType.Filter);
                if (forOpen)
                    allTypes.Add(fontType.Filter);
            }
            if (forOpen)
            {
                allTypes.Add("*.fnt");
                String allTypesStr = String.Join(";", allTypes.ToArray());
                types[0] = "All supported fonts (" + allTypesStr + ")|" + allTypesStr;
                types[fontTypes.Length + 1] = "All files (*.*)|*.*";
            }
            return String.Join("|", types);
        }

    }
}
