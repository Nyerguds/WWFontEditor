using Nyerguds.Util;
using System;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Very old 1bpp Westwood Studios font format, without file header, with fixed 8x8 symbols.
    /// </summary>
    public class FontFileTran : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x9F; } }
        public override Int32 SymbolsTypeMax { get { return 0x9F; } }
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 8; } }
        public override Int32 FontWidthTypeMax { get { return 8; } }
        public override Int32 FontHeightTypeMin { get { return 8; } }
        public override Int32 FontHeightTypeMax { get { return 8; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "GDA" }; } }
        public override String ShortTypeName { get { return "TranFont"; } }
        public override String ShortTypeDescription { get { return "Translvania 1 & 2 Font"; } }
        public override String LongTypeDescription { get { return "A simple 1 BPP font with a tiny header of seemingly fixed values."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "Transylvania",
            "Transylvania II: The Crimson Crown",
        }; } }

        protected const Int32 m_FontSize = 0x400;

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length != m_FontSize)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            if (ArrayUtils.ReadIntFromByteArray(fileData,0,4,true) != 0x03001100)
                throw new FileTypeLoadException(ERR_BADHEADER);
            m_FontWidth = 8;
            m_FontHeight = 8;

            for (Int32 i = 0; i < SymbolsTypeFirst; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel));
            for (Int32 i = 4; i + 4 < m_FontSize; i += 8)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, i, this.BitsPerPixel, false);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i / 8));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
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
                Byte[] curData1bit = ImageUtils.ConvertFrom8Bit(data8bit, m_FontWidth, m_FontHeight, this.BitsPerPixel, true);
                Array.Copy(curData1bit, 0, fileData, i*8, Math.Min(curData1bit.Length, 8));
            }
            return fileData;
        }
    }
}