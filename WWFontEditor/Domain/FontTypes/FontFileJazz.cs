using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    public class FontFileJazz : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x5B; } }
        public override Int32 SymbolsTypeMax { get { return 0x5B; } }
        public override Int32 SymbolsTypeFirst { get { return 0x20; } }
        public override Int32 FontWidthTypeMin { get { return 0x00; } }
        public override Int32 FontWidthTypeMax { get { return 0x3FFFF; } }
        public override Int32 FontHeightTypeMin { get { return 0x00; } }
        public override Int32 FontHeightTypeMax { get { return 0xFFFF; } }
        public override Int32 FontTypePaddingHorizontal { get { return 0; } }
        public override Int32 FontTypePaddingVertical { get { return 0; } }

        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        public override Byte TransparencyColor { get { return 0xFE; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extension typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "000" }; } }
        public override String ShortTypeName { get { return "JazzFnt"; } }
        public override String ShortTypeDescription { get { return "Jazz Jackrabbit font (Uncompressed)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with an unknown text encoding, and variable sizes per symbol. Only A-Z, 0-9 and the % symbol are editable."; } }
        public override String[] GamesListForType { get { return new String[] { "Jazz Jackrabbit" }; } }

        protected readonly Byte[] ReorderTable =
        {
            //_0, 0x_1, 0x_2, 0x_3, 0x_4, 0x_5, 0x_6, 0x_7, 0x_8, 0x_9, 0x_A, 0x_B, 0x_C, 0x_D, 0x_E, 0x_F
            0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50,
            0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35,
            0x36, 0x37, 0x38, 0x39, 0x25
        };

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 2)
                throw new FileTypeLoadException(ERR_NOHEADER);
            UInt32 symbols = (UInt32)ArrayUtils.ReadIntFromByteArray(fileData, 0, 2, true);
            if (symbols > 0x100)
                throw new FileTypeLoadException(ERR_MAXSYMB);
            Int32 offset = 2;
            Dictionary<Int32, Int32> widthFreq = new Dictionary<Int32, Int32>();
            List<FontFileSymbol> imageDataList = new List<FontFileSymbol>();
            for (Int32 i = 0; i < symbols; ++i)
            {
                if (offset + 8 > fileData.Length)
                    throw new FileTypeLoadException(ERR_SIZETOOSMALL);
                Int32 stride = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, offset, 2, true);
                Int32 width = stride * 4;
                if (width != 0)
                {
                    Int32 curAmount;
                    if (!widthFreq.TryGetValue(width, out curAmount))
                        curAmount = 0;
                    widthFreq[width] = curAmount + 1;
                }
                Int32 height = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, offset + 2, 2, true);
                Int32 size = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, offset + 4, 2, true);
                Int32 empty = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, offset + 6, 2, true);
                if (stride * height != size)
                    throw new FileTypeLoadException("Symbol data size does not match width and height.");
                offset += 8;
                if (empty != 0)
                    throw new FileTypeLoadException("Reserved bytes don't match");
                Byte[] symbol = new Byte[size * 4];
                for (Int32 j = 0; j < 4; ++j)
                {
                    for (Int32 k = 0; k < size; k++)
                        symbol[(k << 2) + j] = fileData[offset + k];
                    offset += size;
                }
                FontFileSymbol ffs = new FontFileSymbol(symbol, width, height, 0, this.BitsPerPixel, this.TransparencyColor);
                if (width > this.m_FontWidth)
                    this.m_FontWidth = width;
                if (height > this.m_FontHeight)
                    this.m_FontHeight = height;
                imageDataList.Add(ffs);
            }
            Int32 maxFound = 0;
            Int32 maxFoundAt = -1;
            Int32[] widths = widthFreq.Keys.ToArray();
            Array.Sort(widths);
            for (Int32 i = widths.Length - 1; i >= 0; --i)
            {
                Int32 width = widths[i];
                Int32 widthAmount = widthFreq[width];
                if (widthAmount > maxFound)
                {
                    maxFound = widthAmount;
                    maxFoundAt = width;
                }
            }
            // Dummy space is generated at 2/3rd of the most commonly found width.
            Int32 spaceWidth = Math.Max(0, maxFoundAt) * 2 / 3;
            this.m_ImageDataList = this.RearrangeLoad(imageDataList);
            // Set dummy space
            this.m_ImageDataList[0x20].ChangeWidth(spaceWidth);
        }

        private List<FontFileSymbol> RearrangeLoad(List<FontFileSymbol> imageDataList)
        {
            Int32 length = imageDataList.Count;
            FontFileSymbol[] symbols = new FontFileSymbol[this.SymbolsTypeMin];
            for (Int32 i = 0; i < length; ++i)
                symbols[this.ReorderTable[i]] = imageDataList[i];
            for (Int32 i = 0; i < symbols.Length; ++i)
                if (symbols[i] == null)
                    symbols[i] = new FontFileSymbol(this);
            return new List<FontFileSymbol>(symbols);
        }

        private FontFileSymbol[] RearrangeSave(List<FontFileSymbol> imageDataList)
        {
            Int32 sourceLength = imageDataList.Count;
            Int32 length = this.ReorderTable.Length;
            FontFileSymbol[] symbols = new FontFileSymbol[length];
            for (Int32 i = 0; i < length; ++i)
            {
                Int32 sourceIndex = this.ReorderTable[i];
                if (sourceIndex < sourceLength)
                    symbols[i] = imageDataList[sourceIndex];
            }
            return symbols;
        }

        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            return new SaveOption[0];
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            FontFileSymbol[] symbols = this.RearrangeSave(this.m_ImageDataList);
            Int32 length = symbols.Length;
            Int32 fullSize = 2;
            for (Int32 i = 0; i < length; ++i)
            {
                FontFileSymbol ffs = symbols[i];
                if (ffs.Width % 4 != 0)
                    ffs.ChangeWidth((ffs.Width + 3) / 4 * 4);
                fullSize += 8 + ffs.ByteData.Length;
            }
            Byte[] fileData = new Byte[fullSize];
            ArrayUtils.WriteIntToByteArray(fileData, 0, 2, true, (UInt64) length);
            Int32 offset = 2;
            for (Int32 i = 0; i < length; ++i)
            {
                FontFileSymbol ffs = symbols[i];
                Int32 width = ffs.Width / 4;
                Int32 height = ffs.Height;
                Int32 size = width * height;
                ArrayUtils.WriteIntToByteArray(fileData, offset, 2, true, (UInt64) width);
                ArrayUtils.WriteIntToByteArray(fileData, offset + 2, 2, true, (UInt64) height);
                ArrayUtils.WriteIntToByteArray(fileData, offset + 4, 2, true, (UInt64) size);
                //ArrayUtils.WriteIntToByteArray(fileData, offset + 6, 2, true, 0);
                offset += 8;
                Byte[] symbolData = ffs.ByteData;
                for (Int32 j = 0; j < 4; ++j)
                {
                    for (Int32 k = 0; k < size; k++)
                        fileData[offset + k] = symbolData[(k << 2) + j];
                    offset += size;
                }
            }
            return fileData;
        }
        
    }
}