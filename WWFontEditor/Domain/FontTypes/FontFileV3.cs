using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Main 4bpp Westwood font format
    /// </summary>
    public class FontFileV3 : FontFile
    {

        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 4; } }
        public override String ShortTypeName { get { return "WW v3"; } }
        public override String ShortTypeDescription { get { return "WWFont v3 (D2/C&C1/RA1/LoL/Kyr)"; } }
        public override String LongTypeDescription { get { return "A 4 BPP font with variable amount of characters, which allows separate symbols to specify their width, height and Y-offset."; } }
        public override String[] GamesListForType
        {
            get
            {
                return new String[]
        {
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
            this.LoadV3V4Font(fileData, false);
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            return this.SaveV3V4Font(false);
        }

        // any actions to be taken after conversion to this type.
        protected override void PostConvertCleanup()
        {
            // Y-optimization.
            foreach (FontFileSymbol ffs in m_ImageDataList)
                ffs.OptimizeYHeight();
        }
    }
}