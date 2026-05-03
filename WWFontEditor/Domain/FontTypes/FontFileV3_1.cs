using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Kyrandia format
    /// </summary>
    public class FontFileV3_1 : FontFileV3
    {
        public override String ShortTypeName { get { return "WW V3 (Kyr)"; } }
        public override String ShortTypeDescription { get { return "WWFont v3.1 (Legend of Kyrandia)"; } }
        public override String LongTypeDescription { get { return "A 4 BPP font which allows separate symbols to specify their width, height and Y-offset. Kyrandia's fonts have some small difference in the unknown bytes in the file header."; } }
        public override String[] GamesListForType { get { return new String[] { "The Legend of Kyrandia" }; } }

        public override void LoadFont(Byte[] fileData, Boolean fromAutoDetect)
        {
            LoadV3V4Font(fileData, FontFileVersion.WW_V3_1, fromAutoDetect);
        }

        public override Byte[] SaveFont()
        {
            return this.SaveV3V4Font(FontFileVersion.WW_V3_1);
        }
    }
}