using System;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Westwood Studios Tiberian Sun format. Though this is technically 8bpp, the games only seem to use the first 16 colours in it.
    /// </summary>
    public class FontFileWsV4 : FontFileWsV3
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        public override String ShortTypeName { get { return "WWFont v4"; } }
        public override String ShortTypeDescription { get { return "WWFont v4 (Tiberian Sun)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with variable amount of characters, which allows separate symbols to specify their width, height and Y-offset. It is optimised by saving duplicate symbols only one time."; } }
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
            this.LoadV3V4Font(fileData, true);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return this.SaveV3V4Font(true);
        }

        /// <summary>
        /// any actions to be taken after conversion to this type.
        /// </summary>
        protected override void PostConvertCleanup()
        {

            // Y-optimization.
            Int32 imagesCount = this.m_ImageDataList.Count;
            for (Int32 i = 0; i < imagesCount; ++i)
                this.m_ImageDataList[i].OptimizeYHeight(this.YOffsetTypeMax);
        }

    }
}