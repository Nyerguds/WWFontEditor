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
    public class FontFileDynV1a : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x80; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 8; } }
        public override Int32 FontWidthTypeMax { get { return 8; } }
        public override Int32 FontHeightTypeMin { get { return 1; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "f4" }; } }
        public override Boolean CustomSymbolWidthsForType { get { return false; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v1a"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v1a"; } }
        public override String LongTypeDescription { get { return "An 8-pixel wide, 96-symbol, 1 BPP font, which is saved as 2bpp, but with 0 and 3 as only used value. Doesn't have any kind of file header."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Pete Rose Pennant Fever" }; }
        }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length == 0 || fileData.Length % (2 * 0x60) != 0)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            this.m_FontWidth = 8;
            this.m_FontHeight = fileData.Length / (2 * 0x60);
            Byte startSymbol = 32;
            Byte nrOfSymbols = 0x60;
            // fill in dummy symbols. Will need to be checked and trimmed on save (until 0x20 that is.)
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel));
            Int32 symbolsize = m_FontHeight * 2;
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Byte[] curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, symbolsize * i, 2, true);
                for (Int32 b = 0; b < curData8bit.Length; b++)
                {
                    Byte val = curData8bit[b];
                    if (val != 0)
                    {
                        if (val != 0x03)
                            throw new FileTypeLoadException("Dynamix v1a only accepts 0 and 3 as values");
                        else
                            curData8bit[b] = 1;
                    }
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);

            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            Byte[][] imageData = new Byte[0x60][];
            for (Int32 i = 0; i < 0x60; i++)
            {
                Byte[] eightBitData = this.m_ImageDataList[i + 0x20].ByteData;
                for (Int32 b = 0; b < eightBitData.Length; b++)
                {
                    if (eightBitData[b] != 0)
                        eightBitData[b] = 0x03;
                }

                imageData[i] = ImageUtils.ConvertFrom8Bit(eightBitData, this.m_FontWidth, this.m_FontHeight, 2, true);
            }
            Int32 symbolsize = m_FontWidth * 4;
            Byte[] fullData = new Byte[symbolsize * 0x60];
            for (Int32 i = 0; i < 0x60; i++)
                Array.Copy(imageData[i], 0, fullData, i * symbolsize, symbolsize);
            return fullData;
        }

    }
}