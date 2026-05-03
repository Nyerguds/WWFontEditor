using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Main 4bpp Westwood font format
    /// </summary>
    public class FontFileV3 : FontFile
    {

        public override Int32 CharactersMax { get { return 0x100; } }
        public override Int32 FontWidthMax { get { return 0xFF; } }
        public override Int32 FontHeightMax { get { return 0xFF; } }
        public override Int32 YOffsetMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 4; } }
        public override String ShortTypeCode { get { return "WW V3"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 3"; } }
        public override String[] GamesList { get { return new String[]
        {
            "The Legend of Kyrandia",
            "Dune II",
            "Lands of Lore The Throne of Chaos",
            "The Legend of Kyrandia Hand of Fate",
            "The Legend of Kyrandia Malcolm's Revenge",
            "The Legend of Kyrandia Malcolm's Revenge Installer",
            "Command & Conquer",
            "Command & Conquer Installer",
            "Command & Conquer Red Alert",
            "Command & Conquer Red Alert Installer",
            "Lands of Lore Guardians of Destiny",
            "Lands of Lore Guardians of Destiny Installer",
            "Command & Conquer Sole Survivor",
            "Lands of Lore III",
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFromFileData(fileData, FontFileVersion.WW_V3);
        }

        public override Byte[] SaveFont()
        {
            return this.WriteFntFileV3V4(FontFileVersion.WW_V3);
        }

    }
}