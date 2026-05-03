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
    public abstract class FontFileDynV1 : FontFile
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
        public override Boolean CustomSymbolWidthsForType { get { return false; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String LongTypeDescription { get { return "An 8-pixel wide, 96-symbol, 1 BPP font, which is saved as " + InternalBpp + "bpp, but with 0 and " + MaxValue.ToString("X") + " as only used value. Doesn't have any kind of file header."; } }

        public abstract Int32 InternalBpp { get; }
        protected Byte MaxValue { get { return (Byte)((1 << InternalBpp) - 1); } }
        
        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length == 0 || fileData.Length % (InternalBpp * 0x60) != 0)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            this.m_FontWidth = 8;
            this.m_FontHeight = fileData.Length / (InternalBpp * 0x60);
            Byte startSymbol = 32;
            Byte nrOfSymbols = 0x60;
            // fill in dummy symbols. Will need to be trimmed on save.
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[m_FontHeight * m_FontWidth], this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor));
            Int32 symbolsize = m_FontHeight * InternalBpp;
            for (Int32 i = 0; i < nrOfSymbols; i++)
            {
                Byte[] curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, symbolsize * i, InternalBpp, true);
                for (Int32 b = 0; b < curData8bit.Length; b++)
                {
                    Byte val = curData8bit[b];
                    if (val != 0)
                    {
                        if (val != MaxValue)
                            throw new FileTypeLoadException(ShortTypeDescription + " only accepts 0 and " + MaxValue + " as values");
                        else
                            curData8bit[b] = 1;
                    }
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
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
                        eightBitData[b] = MaxValue;
                }

                imageData[i] = ImageUtils.ConvertFrom8Bit(eightBitData, this.m_FontWidth, this.m_FontHeight, InternalBpp, true);
            }
            Int32 symbolsize = m_FontWidth * InternalBpp;
            Byte[] fullData = new Byte[symbolsize * 0x60];
            for (Int32 i = 0; i < 0x60; i++)
                Array.Copy(imageData[i], 0, fullData, i * symbolsize, symbolsize);
            return fullData;
        }
    }

    public class FontFileDynV1a : FontFileDynV1
    {
        public override Int32 InternalBpp { get { return 2;  } }

        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "f4" }; } }
        public override String ShortTypeName { get { return "DYN v1a"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v1a"; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Pete Rose Pennant Fever" }; }
        }
    }

    public class FontFileDynV1b : FontFileDynV1
    {
        public override Int32 InternalBpp { get { return 4; } }

        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "f16", "dat" }; } }
        public override String ShortTypeName { get { return "DYN v1b"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v1b"; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Pete Rose Pennant Fever", "Skyfox II", "Arctic Fox" }; }
        }
    }
}