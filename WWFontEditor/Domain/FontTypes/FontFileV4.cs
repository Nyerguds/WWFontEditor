using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Tiberian Sun format
    /// </summary>
    public class FontFileV4 : FontFile
    {
        public override Int32 CharactersMax { get { return 0x100; } }
        public override Int32 FontWidthMax { get { return 0xFF; } }
        public override Int32 FontHeightMax { get { return 0xFF; } }
        public override Int32 YOffsetMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        public override String ShortTypeCode { get { return "WW V4"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 4"; } }
        public override String[] GamesList { get { return new String[]
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