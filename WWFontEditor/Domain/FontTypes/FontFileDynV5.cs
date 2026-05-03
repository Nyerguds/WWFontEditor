using System;
using System.Collections.Generic;
using System.Text;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>

    public class FontFileDynV5 : FontFileDynV4
    {
        public override String ShortTypeName { get { return "DYN v5"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v5 (Krondor)"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font with compression support, with width definable for each symbol. It is optimized by only saving the used range of symbols. Identical to v4, but 8-bit."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Betrayal at Krondor", "Front Page Sports Football" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFont(fileData, true);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return SaveFont(saveOptions, true);
        }

    }
}