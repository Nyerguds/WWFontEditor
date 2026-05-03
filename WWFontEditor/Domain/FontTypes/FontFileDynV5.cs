using System;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 8-bpp Dynamix font format
    /// </summary>

    public class FontFileDynV5 : FontFileDynV4
    {
        public override Int32 BitsPerPixel { get { return 8; } }
        public override String ShortTypeName { get { return "DYN v5"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v5 (Krondor/FPS-Fball)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with compression support, with width definable for each symbol. It is optimized by only saving the used range of symbols. Identical to v4, but 8-bit."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Betrayal at Krondor", "Front Page Sports Football", "Front Page Sports Football Pro" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            this.LoadFont(fileData, true);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return this.SaveFont(saveOptions, true);
        }

    }
}