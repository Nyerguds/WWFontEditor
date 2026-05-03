using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Tiberian Sun format
    /// </summary>
    public class FontFileV4 : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        public override String ShortTypeCode { get { return "WW V4"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 4"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font which allows separate symbols to specify their width, height and Y-offset."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "Command & Conquer Tiberian Sun",
            "Command & Conquer Tiberian Sun Installer",
            "Command & Conquer Tiberian Sun Firestorm",
            "Command & Conquer Tiberian Sun Firestorm Installer",
            "Lands of Lore III Installer"
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFromFileData(fileData, FontFileVersion.WW_V4);
        }

        public override Byte[] SaveFont()
        {
            return this.WriteFntFileV3V4(FontFileVersion.WW_V4);
        }
    }
}