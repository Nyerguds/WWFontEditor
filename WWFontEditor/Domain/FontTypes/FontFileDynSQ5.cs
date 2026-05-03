using System;
using System.Linq;
using Nyerguds.Util;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Space quest font
    /// </summary>
    public class FontFileDynSQ5 : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x00; } }
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fon" }; } }
        public override String ShortTypeName { get { return "DYN SQ5"; } }
        public override String ShortTypeDescription { get { return "Dynamix Space Quest V Font"; } }
        public override String LongTypeDescription { get { return "A 1-bpp font with widths and heights specified with the symbol data."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Space Quest V" }; }
        }

        public Int16 LineHeight { get; set; }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 2)
                throw new FileTypeLoadException(ERR_NOHEADER);
            if (fileData[0] != 0x87)
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 offset = 2 + fileData[1];
            if (fileData.Length < offset + 6)
                throw new FileTypeLoadException(ERR_NOHEADER);
            //Int16 startSymbol = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset, 2, true);
            Int16 numSymbols = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 2, 2, true);
            // Not sure but I'll just preserve it...
            this.LineHeight = fileData[offset + 4];
            this.LineHeight = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offset + 4, 2, true);
            this.m_FontHeight = 0;
            this.m_FontWidth = 0;
            Int32 indexOffset = offset + 6;

            for (Int32 i = 0; i < numSymbols; ++i)
            {
                Int32 symbolOffset = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, indexOffset + (i * 2), 2, true) + offset;
                if (fileData.Length < symbolOffset + 2)
                    throw new FileTypeLoadException(ERR_SIZECHECK);
                Int32 symbolWidth = fileData[symbolOffset];
                Int32 symbolHeight = fileData[symbolOffset + 1];
                // This font type has no fixed overall size. To make the editor work right we just take the maximum in the symbols.
                if (symbolWidth > this.m_FontWidth)
                    this.m_FontWidth = symbolWidth;
                if (symbolHeight > this.m_FontHeight)
                    this.m_FontHeight = symbolHeight;
                Int32 symbolStride = (symbolWidth + 7) / 8;
                Int32 symbolSize = symbolStride * symbolHeight;
                if (fileData.Length < symbolOffset + 2 + symbolSize)
                    throw new FileTypeLoadException(ERR_SIZECHECK);
                Byte[] curData8bit = ImageUtils.ConvertTo8Bit(fileData, symbolWidth, symbolHeight, symbolOffset + 2, 1, true);
                this.m_ImageDataList.Add(new FontFileSymbol(curData8bit, symbolWidth, symbolHeight, 0, this.BitsPerPixel, this.TransparencyColor));
            }
        }

        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            // Line height. Default calculation uses the most commonly used lowest point in the font.
            Int32 lHeight = this.LineHeight;
            if (lHeight <= 0)
                lHeight = this.m_ImageDataList.Max(ffs => ffs.Height);
            return new SaveOption[]
            {
                new SaveOption("OPT", SaveOptionType.Boolean, "Optimise to remove duplicate symbols", "0"),
                new SaveOption("YOF", SaveOptionType.Number, "Font header height value", lHeight.ToString())
            };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 lineHeight;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "YOF"), out lineHeight);
            Boolean optimise = GeneralUtils.IsTrueValue(SaveOption.GetSaveOptionValue(saveOptions, "OPT"));

            this.LineHeight = (Byte)lineHeight;

            Int32 len = this.m_ImageDataList.Count;
            Byte[][] symbolData = new Byte[len][];
            for (Int32 i = 0; i < len; ++i)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                Byte[] curData1bit = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, ffs.Height, 1, true);
                Byte[] actualSymbolData = new Byte[2 + curData1bit.Length];
                actualSymbolData[0] = (Byte)ffs.Width;
                actualSymbolData[1] = (Byte)ffs.Height;
                Array.Copy(curData1bit, 0, actualSymbolData, 2, curData1bit.Length);
                symbolData[i] = actualSymbolData;
            }
            Int32 offset = 6 + len * 2;
            Int32 dataOffset = offset;
            Byte[] symbolOffsets = CreateImageIndex(symbolData, 0, true, ref dataOffset, false, optimise, true);

            // Final data array "header": 6 bytes of font info plus the index.
            Byte[] header = new Byte[offset];
            //ArrayUtils.WriteIntToByteArray(actualData, 0, 2, true, 0x0000);
            ArrayUtils.WriteIntToByteArray(header, 2, 2, true, (UInt32)len);
            ArrayUtils.WriteIntToByteArray(header, 4, 2, true, (UInt32)this.LineHeight);
            Array.Copy(symbolOffsets, 0, header, 6, len * 2);

            // Write actual font
            Byte[] fontData = new Byte[dataOffset + 0x22];
            fontData[0] = 0x87;
            fontData[1] = 0x20;
            // Reproduce weird header error.
            Array.Copy(header, 0, fontData, 0x02, 0x20);
            Array.Copy(header, 0, fontData, 0x22, header.Length);
            Int32 imageDataOffs = offset + 0x22;
            for (Int32 i = 0; i < len; ++i)
            {
                Byte[] symbolImgData = symbolData[i];
                if (symbolImgData == null || symbolImgData.Length == 0)
                    continue;
                Array.Copy(symbolImgData, 0, fontData, imageDataOffs, symbolImgData.Length);
                imageDataOffs += symbolImgData.Length;
            }
            return fontData;
        }

    }
}