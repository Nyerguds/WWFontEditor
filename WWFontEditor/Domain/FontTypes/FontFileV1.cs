using System;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Very old 1bpp Westwood font format, without file header, with fixed 8x8 symbols.
    /// </summary>
    public class FontFileV1 : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x80; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 FontWidthTypeMin { get { return 8; } }
        public override Int32 FontWidthTypeMax { get { return 8; } }
        public override Int32 FontHeightTypeMin { get { return 8; } }
        public override Int32 FontHeightTypeMax { get { return 8; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        // Not needed here: the automatic parent takes care of it
        //public override Boolean IndividualSizesAllowed { get { return false; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override String ShortTypeName { get { return "WW V1"; } }
        public override String ShortTypeDescription { get { return "WWFont v1 (WarConst, ElmStr, DrStr, CirEdg)"; } }
        public override String LongTypeDescription { get { return "A simple 1 BPP font without header data; it's always a 128-item list of 8x8 symbols."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "Wargame Construction Set",
            "A Nightmare On Elm Street",
            "DragonStrike",
            "Circuit's Edge"
        }; } }

        protected const Int32 m_FontSize = 0x400;

        public override void LoadFont(Byte[] fileData, Boolean fromAutoDetect)
        {
            if (fileData.Length != m_FontSize)
                throw new LoadFailedException("File size is not " + m_FontSize + " bytes.");
            m_FontWidth = 8;
            m_FontHeight = 8;
            for (Int32 i = 0; i < m_FontSize; i += 8)
            {
                Byte[] curData8bit = this.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, i, this.BitsPerPixel, i, true);
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
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
                Byte[] curData1bit = this.ConvertFrom8Bit(data8bit, m_FontWidth, m_FontHeight, this.BitsPerPixel, true);
                Array.Copy(curData1bit, 0, fileData, i*8, Math.Min(curData1bit.Length, 8));
            }
            return fileData;
        }
    }
}