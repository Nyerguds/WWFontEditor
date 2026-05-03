using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Kyrandia format
    /// </summary>
    public class FontFileV3_1 : FontFileV3
    {
        public override String ShortTypeCode { get { return "WW V3 (Kyr)"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 3.1 "; } }
        public override String[] GamesList { get { return new String[] { "The Legend of Kyrandia" }; } }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFromFileData(fileData, FontFileVersion.WW_V3_1);
        }

        public override Byte[] SaveFont()
        {
            return this.WriteFntFileV3V4(FontFileVersion.WW_V3_1);
        }
    }
}