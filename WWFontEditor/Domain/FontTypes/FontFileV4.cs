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
        public override String ShortTypeName { get { return "WW v4"; } }
        public override String ShortTypeDescription { get { return "WWFont v4 (Tiberian Sun)"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font with variable amount of characters, which allows separate symbols to specify their width, height and Y-offset."; } }
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
            LoadV3V4Font(fileData, true);
        }

        public override Byte[] SaveFont()
        {
            return this.SaveV3V4Font(true);
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