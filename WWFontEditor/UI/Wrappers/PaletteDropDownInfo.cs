using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WWFontEditor.UI.Wrappers
{
    public class PaletteDropDownInfo
    {
        public String Name { get; private set; }
        public Color[] Colors { get; private set; }
        public Color[] ColorBackup { get; private set; }
        public Int32 BitsPerPixel { get; private set; }
        public String SourceFile { get; private set; }
        public Int32 Entry { get; private set; }

        public PaletteDropDownInfo(String name, Int32 bpp, Color[] colors, String sourceFile, Int32 entry)
        {
            this.Name = name;
            this.BitsPerPixel = bpp;
            Int32 expectedcolors = (Int32)Math.Pow(2, bpp);
            Color[] palette = new Color[expectedcolors];
            Int32 copiedColors = Math.Min(colors.Length, expectedcolors);
            Array.Copy(colors, palette, copiedColors);
            for (Int32 i = copiedColors; i < expectedcolors; i++)
                palette[i] = Color.Black;
            this.Colors = palette;
            this.ColorBackup = palette.ToArray();
            this.SourceFile = sourceFile;
            this.Entry = entry;
        }


        public Boolean IsChanged()
        {
            return !this.ColorBackup.SequenceEqual(this.Colors);
        }

        public void Revert()
        {
            Array.Copy(this.ColorBackup, this.Colors, this.Colors.Length);
        }

        public void ClearRevert()
        {
            Array.Copy(this.Colors, this.ColorBackup, this.Colors.Length);
        }

        public override String ToString()
        {
            return Name;
        }
    }
}
