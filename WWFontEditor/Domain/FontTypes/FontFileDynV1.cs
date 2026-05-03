using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>
    public class FontFileDynV1 : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override Boolean CustomSymbXForType { get { return false; } }
        public override Boolean CustomSymbYForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v1"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v1"; } }
        public override String LongTypeDescription { get { return "A 1 BPP font with the file header specifying the global width and height for all symbols and the amount of symbols. It is optimized by only saving the used range of symbols."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Rise of the Dragon", "Heart of China", "The Adventures of Willy Beamish" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x0C)
                throw new FileTypeLoadException(ERR_NOHEADER);
            // Poor Man's bytes-to-string conversion. Will give incorrect chars on values > 0x7F,
            // but it doesn't matter here since the string that is compared with doesn't contain those.
            if (!"FNT:".Equals(new String(fileData.Take(4).Select(x => (Char)x).ToArray())))
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 fileSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            if (fileSize != fileData.Length - 8)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            Int32 dataOffset = 0x08;
            if (fileData[dataOffset] == 0xFF)
                throw new FileTypeLoadException("Complex Dynamix font detected.");
            this.m_FontWidth = fileData[dataOffset];
            this.m_FontHeight = fileData[dataOffset+1];
            //if (newType) dataOffset++;
            Byte startSymbol = fileData[dataOffset+2];
            Byte nrOfSymbols = fileData[dataOffset+3];
            Int32 symbolSize = ((m_FontWidth + 7) / 8) * m_FontHeight;

            // fill in dummy symbols. Will need to be checked and trimmed on save (until 0x20 that is.)
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel));

            Int32 start = dataOffset+4;
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, start + (symbolSize * i), this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            Boolean foundStart = false;
            Int32 startSymbol = 0;
            Int32 fullNrOfSymbols = m_ImageDataList.Count;
            Byte[][] imageData = new Byte[fullNrOfSymbols][];
            for (Int32 i = 0; i < fullNrOfSymbols; i++)
            {
                if (!foundStart)
                {
                    if (i < 0x20 && m_ImageDataList[i].ByteData.All(x => x == 0))
                        continue;
                    foundStart = true;
                    startSymbol = i;
                }
                Byte[] eightBitData = this.m_ImageDataList[i].ByteData;
                imageData[i] = ImageUtils.ConvertFrom8Bit(eightBitData, this.m_FontWidth, this.m_FontHeight, this.BitsPerPixel);
            }
            Int32 nrOfSymbols = fullNrOfSymbols - startSymbol;
            Int32 symbolSize = ((m_FontWidth + 7) / 8) * m_FontHeight;
            Byte[] fullData = new Byte[0x0C + symbolSize * nrOfSymbols];
            Array.Copy(Encoding.ASCII.GetBytes("FNT:"), 0, fullData, 0, 4);
            ArrayUtils.WriteIntToByteArray(fullData, 4, 4, true, (UInt32)(fullData.Length - 8));
            fullData[0x08] = (Byte)this.m_FontWidth;
            fullData[0x09] = (Byte)this.m_FontHeight;
            fullData[0x0A] = (Byte)startSymbol;
            fullData[0x0B] = (Byte)nrOfSymbols;
            Int32 fontDataOffset = 0x0C;
            for (Int32 i = startSymbol; i < fullNrOfSymbols; i++)
            {
                Array.Copy(imageData[i], 0, fullData, fontDataOffset, symbolSize);
                fontDataOffset += symbolSize;
            }
            return fullData;
        }
    }
}