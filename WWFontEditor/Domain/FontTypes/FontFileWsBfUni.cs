using System;
using System.Linq;
using System.Text;
using Nyerguds.Util;
using Nyerguds.ImageManipulation;
using System.Collections.Generic;
using System.IO;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Westwood Studios RA2 font format.
    /// </summary>
    public class FontFileWsBfUni : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x10000; } }
        public override Int32 SymbolsTypeMax { get { return 0x10000; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override Int32 YOffsetTypeMax { get { return 0x0; } }
        /// <summary>Padding between the characters of the font. Used for the preview function and to determine if padding is needed when automatically optimizing symbol widths.</summary>
        public override Int32 FontTypePaddingRight { get { return 1; } }
        public override Int32 BitsPerPixel { get { return 1; } }

        public override String ShortTypeName { get { return "WW BitFont (Unicode)"; } }
        public override String ShortTypeDescription { get { return "WW BitFont (Unicode) (RA2)"; } }
        public override String LongTypeDescription { get { return "A 1-bpp font which supports unicode."; } }
        public override String[] GamesListForType { get { return new String[] { "Command & Conquer Red Alert 2", }; } }
        /// <summary>Indicates that the font file is unicode, and is thus not limited to 256 characters.</summary>
        public override Boolean IsUnicode { get { return true; } }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 0x1C)
                throw new FileTypeLoadException(ERR_NOHEADER);
            String format = Encoding.ASCII.GetString(fileData, 0, 4);
            if (!String.Equals(format, "fonT", StringComparison.InvariantCulture))
                throw new FileTypeLoadException(ERR_BADHEADER);
            //UInt32 dataStart = (UInt32) ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            Int32 stride = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x08, 4, true);
            this.m_FontHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 4, true);
            this.m_FontWidth = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x10, 4, true);
            // count: highest encountered ID. But all IDs are +1.
            Int32 count = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x14, 4, true);
            Int32 symbolDataSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x18, 4, true);
            Int32 symbolImageSize = symbolDataSize - 1;
            if (stride * m_FontHeight != symbolImageSize)
                throw new FileTypeLoadException("Symbol size does not match width * height!");
            Int32 readOffset = 0x1C;
            List<Int32>[] symbolUsage = new List<Int32>[count];
            if (fileData.Length <= readOffset + 0x20000)
                throw new FileTypeLoadException(ERR_NOHEADER);
            for (Int32 i = 0; i <= 0xFFFF; i++)
            {
                Int32 symbolIndex = (UInt16)(ArrayUtils.ReadIntFromByteArray(fileData, readOffset, 2, true)) - 1;
                if (symbolIndex >= count)
                    throw new FileTypeLoadException("Symbol index exceeds number of symbols!");
                if (symbolIndex >= 0)
                {
                    if (symbolUsage[symbolIndex] == null)
                        symbolUsage[symbolIndex] = new List<Int32>();
                    symbolUsage[symbolIndex].Add(i);
                }
                readOffset += 2;
            }
            FontFileSymbol[] symbols = new FontFileSymbol[0x10000];
            for (Int32 i = 0; i < count; i++)
            {
                if (readOffset >= fileData.Length)
                    throw new FileTypeLoadException("File is not long enough to contain all symbols!");
                List<Int32> curSymbolUsage = symbolUsage[i];
                if (curSymbolUsage == null)
                    break;
                Byte symbolWidth = fileData[readOffset++];
                // Technically the read font width is irrelevant, and thus it might be wrong.
                if (symbolWidth > m_FontWidth && ImageUtils.GetMinimumStride(symbolWidth, 1) <= stride)
                    m_FontWidth = symbolWidth;
                Byte[] symbolData = new Byte[symbolImageSize];
                Array.Copy(fileData, readOffset, symbolData, 0, symbolImageSize);
                readOffset += symbolImageSize;
                Int32 symbolStride = stride;
                Byte[] symbolData8Bit = ImageUtils.ConvertTo8Bit(symbolData, symbolWidth, this.m_FontHeight, 0, 1, true, ref symbolStride);
                foreach (Int32 index in curSymbolUsage)
                    symbols[index] = new FontFileSymbol(symbolData8Bit, symbolWidth, this.m_FontHeight, 0, 1, this.TransparencyColor);
            }
            for (Int32 i = 0; i <= 0xFFFF; i++)
                if (symbols[i] == null)
                    symbols[i] = new FontFileSymbol(this);
            m_ImageDataList = new List<FontFileSymbol>(symbols);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            
            Int32 stride = ImageUtils.GetMinimumStride(this.m_FontWidth, 1);
            Int32 dataLength = stride * m_FontHeight;
            Int32 blockLength = dataLength + 1;
            Int32 imageListcount = m_ImageDataList.Count; // should always be 0x10000
            Byte[][] fontListBin = new Byte[0x10000][];
            // Make list of binary entries, skipping any with width == 0
            for (Int32 i = 0; i < imageListcount; i++)
            {
                FontFileSymbol ffs = m_ImageDataList[i];
                Int32 symbWidth = ffs.Width;
                if (symbWidth == 0)
                    continue;
                Byte[] output = new Byte[blockLength];
                output[0] = (Byte)symbWidth;
                Int32 symbStride = symbWidth;
                Byte[] oneBppArr = ImageUtils.ConvertFrom8Bit(ffs.ByteData, symbWidth, ffs.Height, 1, true, ref symbStride);
                oneBppArr = ImageUtils.ChangeStride(oneBppArr, symbStride, ffs.Height, stride, false, 0);
                Array.Copy(oneBppArr, 0, output, 1, dataLength);
                fontListBin[i] = output;
            }
            List<Byte[]> optimisedList = new List<Byte[]>();
            // Make optimised list, and write all entries to the index.
            Byte[] index = new Byte[0x20000];
            for (Int32 i = 0; i < imageListcount; i++)
            {
                Byte[] curWritesymbol = fontListBin[i];
                if (curWritesymbol == null)
                    continue;
                optimisedList.Add(curWritesymbol);
                UInt64 curNum = (UInt64)optimisedList.Count;
                if (curNum > 0xFFFF)
                    throw new NotSupportedException("WWFont v5 can only contain 65535 (0xFFFF) characters!");
                ArrayUtils.WriteIntToByteArray(index, i << 1, 2, true, curNum);
                // Start at i; everything before it is already checked. This means the inner loop becomes shorter as this progresses.
                for (Int32 j = i + 1; j < imageListcount; j++)
                {
                    Byte[] curChecksymbol = fontListBin[j];
                    if (curChecksymbol == null)
                        continue;
                    Boolean isEqual = true;
                    // Seems x.SequenceEquals(y) is about 4x as slow as a simple 'for' loop, so I stopped using it.
                    // Since they're stride-adjusted, the arrays are all of equal length at this point anyway.
                    for (Int32 b = 0; b < blockLength; b++)
                    {
                        if (curWritesymbol[b] == curChecksymbol[b])
                            continue;
                        isEqual = false;
                        break;
                    }
                    if (isEqual)
                    {
                        ArrayUtils.WriteIntToByteArray(index, j << 1, 2, true, curNum);
                        // Remove it from any following equal checks, to further increase speed.
                        fontListBin[j] = null;
                    }
                }
                // I originally nulled fontListBin[i] here, but the inner loops only starting at i makes this unnecessary.
                // I guess this means the final fontListBin will contain only the originals. Not that it matters; it's no longer used.
            }
            Int32 count = optimisedList.Count;
            Byte[] outputArray = new Byte[0x1C + index.Length + count * blockLength];
            Array.Copy(Encoding.ASCII.GetBytes("fonT"), outputArray, 4);
            // not sure what this is; just gonna hardcode it to what's in 'game.fnt'.
            ArrayUtils.WriteIntToByteArray(outputArray, 0x04, 4, true, 0x14);

            //UInt32 dataStart? = (UInt32) ArrayUtils.WriteIntToByteArray(fileData, 0x04, 4, true);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x08, 4, true, (UInt64)stride);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x0C, 4, true, (UInt64)this.m_FontHeight);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x10, 4, true, (UInt64)this.m_FontWidth);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x14, 4, true, (UInt64)count);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x18, 4, true, (UInt32)blockLength);
            // currently at 0x1C.
            Array.Copy(index, 0, outputArray, 0x1C, index.Length);
            Int32 curIndex = 0x1C + index.Length;
            foreach (Byte[] ffs in optimisedList)
            {
                Array.Copy(ffs, 0, outputArray, curIndex, blockLength);
                curIndex += blockLength;
            }
            return outputArray;
        }

    }
}