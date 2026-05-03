using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;
using System.Text;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Space quest font
    /// </summary>
    public class FontFileDyn96 : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x80; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fon" }; } }
        public override String ShortTypeName { get { return "DYN'96"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font '96"; } }
        public override String LongTypeDescription { get { return "A 1bpp font with widths and heights specified with the symbol data."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Space Quest V" }; }
        }

        private Int32 m_lineHeight = -1;

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 2)
                throw new FileTypeLoadException(ERR_NOHEADER);
            if (fileData[0] != 0x87)
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 offset = 2 + fileData[1];
            if (fileData.Length < offset + 6)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 symbols = fileData[offset + 2];
            // Not sure but I'll just preserve it...
            this.m_lineHeight = fileData[offset + 4];
            this.m_FontHeight = 0;
            this.m_FontWidth = 0;
            Int32 indexOffset = offset + 6;

            for (Int32 i = 0; i < symbols; i++)
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

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 len = this.m_ImageDataList.Count;
            Byte[] symbolWidths = new Byte[len];
            Byte[] symbolHeights = new Byte[len];
            Byte[][] symbolData = new Byte[len][];
            Int32[] symbolOffsets = new Int32[len];
            Int32 offset = len * 2 + 6;
            Int32 indexOffset = offset;
            for (Int32 i = 0; i < len; i++)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                symbolWidths[i] = (Byte)ffs.Width;
                symbolHeights[i] = (Byte)ffs.Height;
                Byte[] curData1bit = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, ffs.Height, 1, true);
                symbolData[i] = curData1bit;
                symbolOffsets[i] = indexOffset;
                indexOffset += 2 + curData1bit.Length;
            }
            Byte[] actualData = new Byte[indexOffset];
            actualData[2] = (Byte)len;
            // Just restoring this for now, or taking the font height as substitute.
            actualData[4] = (Byte)(this.m_lineHeight != -1 ? this.m_lineHeight : this.m_FontHeight);
            Int32 writeOffset = 0x06;
            for (Int32 i = 0; i < len; i++)
            {
                UInt32 offs = (UInt32)symbolOffsets[i];
                ArrayUtils.WriteIntToByteArray(actualData, writeOffset + i * 2, 2, true, offs);
                actualData[offs] = symbolWidths[i];
                actualData[offs + 1] = symbolHeights[i];
                Byte[] curData1bit = symbolData[i];
                Array.Copy(curData1bit, 0, actualData, offs + 0x02, curData1bit.Length);
            }
            Byte[] fontData = new Byte[indexOffset+0x22];
            fontData[0] = 0x87;
            fontData[1] = 0x20;
            // Reproduce weird header error.
            Array.Copy(actualData, 0, fontData, 0x02, actualData.Length);
            Array.Copy(actualData, 0, fontData, 0x22, actualData.Length);
            return fontData;
        }

    }
}