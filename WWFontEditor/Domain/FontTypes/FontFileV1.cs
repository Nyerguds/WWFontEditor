using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Very old 1bpp Westwood font format, without file header, with fixed 8x8 characters.
    /// </summary>
    public class FontFileV1 : FontFile
    {
        public override Int32 CharactersMin { get { return 0x80; } }
        public override Int32 CharactersMax { get { return 0x80; } }
        public override Int32 FontWidthMin { get { return 8; } }
        public override Int32 FontWidthMax { get { return 8; } }
        public override Int32 FontHeightMin { get { return 8; } }
        public override Int32 FontHeightMax { get { return 8; } }
        public override Int32 YOffsetMax { get { return 0; } }
        public override Boolean IndividualSizesAllowed { get { return false; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override String ShortTypeCode { get { return "WW V1"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 1"; } }
        public override String[] GamesList { get { return new String[]
        {
            "Wargame Construction Set",
            "A Nightmare On Elm Street",
            "DragonStrike",
            "Circuit's Edge"
        }; } }

        protected const Int32 m_FontSize = 0x400;

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length != m_FontSize)
                throw new LoadFailedException("File size is not " + m_FontSize + " bytes.");
            m_FontWidth = 8;
            m_FontHeight = 8;
            for (Int32 i = 0; i < m_FontSize; i += 8)
            {
                Byte[] curData8bit = this.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, i, BitsPerPixel, i, true);
                FontFileCharacter fc = new FontFileCharacter(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont()
        {
            Byte[] fileData = new Byte[m_FontSize];
            Int32 imagesCount = Math.Min(128, this.m_ImageDataList.Count);
            for (Int32 i = 0; i < imagesCount; i++)
            {
                if (this.Length <= i)
                    break;
                Byte[] data8bit = this.m_ImageDataList[i].ByteData;
                if (data8bit == null)
                    continue;
                Byte[] curData1bit = this.ConvertFrom8Bit(data8bit, m_FontWidth, m_FontHeight, BitsPerPixel, true);
                Array.Copy(curData1bit, 0, fileData, i*8, Math.Min(curData1bit.Length, 8));
            }
            return fileData;
        }
    }
}