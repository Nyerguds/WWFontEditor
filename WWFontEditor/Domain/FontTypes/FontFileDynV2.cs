using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format without header.
    /// </summary>
    public class FontFileDynV2 : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override Boolean CustomSymbolWidthsForType { get { return false; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v2"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v2 (Abrams)"; } }
        public override String LongTypeDescription { get { return "A 1 BPP font with the file header specifying the global width and height for all symbols and the amount of symbols. It is optimized by only saving the used range of symbols. Identical to v3, but without Dynamix chunk."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Abrams Battle Tank", "The Train: Escape to Normandy" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            LoadFont(fileData, 0);
        }

        protected void LoadFont(Byte[] fileData, Int32 offset)
        {
            if (fileData.Length - offset < 0x04)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 dataOffset = offset;
            if (fileData[dataOffset] > 0x80)
                throw new FileTypeLoadException("Complex Dynamix font detected.");
            this.m_FontWidth = fileData[dataOffset];
            this.m_FontHeight = fileData[dataOffset + 1];
            Byte startSymbol = fileData[dataOffset + 2];
            Byte nrOfSymbols = fileData[dataOffset + 3];
            if (startSymbol + nrOfSymbols > 0x100)
                throw new FileTypeLoadException(ERR_SYMBCHECK);
            dataOffset += 4;
            Int32 symbolSize = ((m_FontWidth + 7) / 8) * m_FontHeight;
            if ((fileData.Length - dataOffset) != symbolSize * nrOfSymbols)
                throw new FileTypeLoadException(ERR_SIZECHECK);

            // fill in dummy symbols. Will need to be checked and trimmed on save (until 0x20 that is.)
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor));
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, dataOffset + (symbolSize * i), this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return SaveFont(saveOptions, false);
        }

        protected Byte[] SaveFont(SaveOption[] saveOptions, Boolean withheader)
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
                imageData[i] = ImageUtils.ConvertFrom8Bit(eightBitData, this.m_FontWidth, this.m_FontHeight, this.BitsPerPixel, true);
            }
            Int32 nrOfSymbols = fullNrOfSymbols - startSymbol;
            Int32 symbolSize = ((m_FontWidth + 7) / 8) * m_FontHeight;
            Int32 startOffset = withheader ? 8 : 0;
            Int32 fullSize = startOffset + 4 + symbolSize * nrOfSymbols;

            Byte[] fullData = new Byte[fullSize];
            if (withheader)
            {
                Array.Copy(Encoding.ASCII.GetBytes("FNT:"), 0, fullData, 0, 4);
                ArrayUtils.WriteIntToByteArray(fullData, 4, 4, true, (UInt32)(fullSize - 8));
            }
            fullData[startOffset + 0x00] = (Byte)this.m_FontWidth;
            fullData[startOffset + 0x01] = (Byte)this.m_FontHeight;
            fullData[startOffset + 0x02] = (Byte)startSymbol;
            fullData[startOffset + 0x03] = (Byte)nrOfSymbols;
            Int32 fontDataOffset = startOffset + 4;
            for (Int32 i = startSymbol; i < fullNrOfSymbols; i++)
            {
                Array.Copy(imageData[i], 0, fullData, fontDataOffset, symbolSize);
                fontDataOffset += symbolSize;
            }
            return fullData;
        }

    }
}