using System;
using System.Collections.Generic;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>

    public class FontFileDynV5 : FontFileDynV4
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return this.m_bpp; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v5"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v5 (Krondor)"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font with compression support, with width definable for each symbol. It is optimized by only saving the used range of symbols. Identical to v3, but 8-bit."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Betrayal at Krondor", "Front Page Sports Football" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFont(fileData, true);
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            return SaveFont(disableCompression, true);
        }

    }
}