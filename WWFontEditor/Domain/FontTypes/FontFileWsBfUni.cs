using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nyerguds.Util;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Westwood Studios RA2 Unicode font format.
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
        public override Int32 FontTypePaddingHorizontal { get { return -1; } }
        public override Int32 BitsPerPixel { get { return 1; } }

        public override String ShortTypeName { get { return "WW BitFont (Unicode)"; } }
        public override String ShortTypeDescription { get { return "WW BitFont (Unicode) (RA2)"; } }
        public override String LongTypeDescription { get { return "A 1-bpp font which supports unicode. It is optimised by saving duplicate symbols only one time."; } }
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
            Int32 fontDataHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 4, true);
            this.m_FontHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x10, 4, true);
            // Start at 0
            this.m_FontWidth = 0;
            // count: highest encountered ID. But all IDs are +1.
            Int32 count = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x14, 4, true);
            Int32 symbolDataSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x18, 4, true);
            Int32 symbolImageSize = symbolDataSize - 1;
            if (stride * fontDataHeight != symbolImageSize)
                throw new FileTypeLoadException("Symbol size does not match width * height!");
            Int32 readOffset = 0x1C;
            List<Int32>[] symbolUsage = new List<Int32>[count];
            if (fileData.Length <= readOffset + 0x20000)
                throw new FileTypeLoadException(ERR_NOHEADER);
            for (Int32 i = 0; i <= 0xFFFF; ++i)
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
            for (Int32 i = 0; i < count; ++i)
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
                Byte[] symbolData8Bit = ImageUtils.ConvertTo8Bit(symbolData, symbolWidth, fontDataHeight, 0, 1, true, ref symbolStride);
                // Debug: view symbol.
                //String symbol = this.VisualiseOneBitData(symbolData, Int32 symbolWidth, this.m_FontHeight);
                Int32 usageCount = curSymbolUsage.Count;
                for (Int32 use = 0; use < usageCount; ++use)
                {
                    FontFileSymbol ffs = new FontFileSymbol(symbolData8Bit, symbolWidth, fontDataHeight, 0, 1, this.TransparencyColor);
                    ffs.ChangeHeight(m_FontHeight);
                    symbols[curSymbolUsage[use]] = ffs;
                }
            }
            for (Int32 i = 0; i <= 0xFFFF; ++i)
                if (symbols[i] == null)
                    symbols[i] = new FontFileSymbol(this);
            m_ImageDataList = new List<FontFileSymbol>(symbols);
        }

        private String VisualiseOneBitData(Byte[] data, Int32 width, Int32 height)
        {
            return String.Join("\r\n", Enumerable.Range(0, height).Select(line => String.Join("", data.Skip(line * width).Take(width).Select(c => c > 0 ? "X" : "_").ToArray())).ToArray());
        }


        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            Int32 width;
            //if (this.m_ImageDataList[0x20] != null && this.m_ImageDataList[0x20].Width > 0)
            //    width = this.m_ImageDataList[0x20].Width*2;
            //else if (this.m_ImageDataList[0x3000] != null && this.m_ImageDataList[0x3000].Width > 0)
            //    width = this.m_ImageDataList[0x3000].Width;
            //else
            width = 0x14;
            return new SaveOption[] { new SaveOption("SPC", SaveOptionType.Number, "Default for the standard ideographic width", width.ToString()) };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 imageListcount = m_ImageDataList.Count; // should always be 0x10000
            Int32 fontDataHeight = 0;
            Int32 fontDataWidth = 0;
            for (Int32 i = 0; i < imageListcount; ++i)
            {
                FontFileSymbol ffs = m_ImageDataList[i];
                Int32 symbWidth = ffs.Width;
                if (symbWidth == 0)
                    continue;
                if (fontDataWidth < symbWidth)
                    fontDataWidth = symbWidth;
                Int32 yoffSet = 0;
                Int32 height = ffs.Height;
                ImageUtils.OptimizeYHeight(ffs.ByteData, ffs.Width, ref height, ref yoffSet, true, this.TransparencyColor, this.FontHeight, false);
                Int32 newHeight = height + yoffSet;
                if (newHeight > fontDataHeight)
                    fontDataHeight = newHeight;
            }
            Int32 spaceWidth;
            if (!Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "SPC"), out spaceWidth))
                spaceWidth = 0x14;

            Int32 stride = ImageUtils.GetMinimumStride(fontDataWidth, 1);
            Boolean hasHeightDiff = m_FontHeight > fontDataHeight;
            Int32 dataLength = stride * fontDataHeight;
            Int32 blockLength = dataLength + 1;
            Byte[][] fontListBin = new Byte[0x10000][];
            // Make list of binary entries, skipping any with width == 0
            for (Int32 i = 0; i < imageListcount; ++i)
            {
                FontFileSymbol ffs = m_ImageDataList[i];
                Int32 symbWidth = ffs.Width;
                if (symbWidth == 0)
                    continue;
                if (hasHeightDiff)
                {
                    // Clone, so we don't edit the loaded font for saving.
                    // FontFileSymbol is not IDisposable, so the garbage collector will take care of the clones.
                    ffs = ffs.Clone();
                    // Reduce height to actually used height.
                    ffs.ChangeHeight(fontDataHeight);
                }
                Byte[] output = new Byte[blockLength];
                output[0] = (Byte)symbWidth;
                Int32 symbStride = symbWidth;
                Byte[] oneBppArr = ImageUtils.ConvertFrom8Bit(ffs.ByteData, symbWidth, ffs.Height, 1, true, ref symbStride);
                oneBppArr = ImageUtils.ChangeStride(oneBppArr, symbStride, ffs.Height, stride, false, 0);
                Array.Copy(oneBppArr, 0, output, 1, dataLength);
                fontListBin[i] = output;
            }
            // Optimise list by removing all duplicates, and write all entries to the index.
            // this list is an array of 2-byte Words. It's treated as simple byte array for convenience.
            Byte[] index = new Byte[0x20000];
            Int32 curNum = 0;
            for (Int32 i = 0; i < imageListcount; ++i)
            {
                Byte[] curWritesymbol = fontListBin[i];
                if (curWritesymbol == null)
                    continue;
                curNum++;
                if (curNum > 0xFFFF)
                    throw new NotSupportedException("WWFont v5 can only contain 65535 (0xFFFF) characters!");
                ArrayUtils.WriteIntToByteArray(index, i << 1, 2, true, (UInt64)curNum);
                // Find any duplicates of this symbol in the following data, set their index to the same as this one,
                // and mark them as "ignore" by setting them to null. Start at i; everything before it is already checked.
                // This means the inner loop becomes shorter as this progresses. The checked block includes the symbol width.
                for (Int32 j = i + 1; j < imageListcount; ++j)
                {
                    Byte[] curChecksymbol = fontListBin[j];
                    if (curChecksymbol == null)
                        continue;
                    Boolean isEqual = true;
                    // Seems x.SequenceEquals(y) is about 4x as slow as a simple 'for' loop here, so I stopped using it.
                    // Since they're stride-adjusted, the arrays are all of equal length at this point anyway.
                    for (Int32 b = 0; b < blockLength; ++b)
                    {
                        if (curWritesymbol[b] == curChecksymbol[b])
                            continue;
                        isEqual = false;
                        break;
                    }
                    if (!isEqual)
                        continue;
                    ArrayUtils.WriteIntToByteArray(index, j << 1, 2, true, (UInt64)curNum);
                    // Remove it from any following equal checks, to further increase speed,
                    // and to end up with a list containing only uniques.
                    fontListBin[j] = null;
                }
            }
            Byte[] outputArray = new Byte[0x1C + index.Length + curNum * blockLength];
            Array.Copy(Encoding.ASCII.GetBytes("fonT"), outputArray, 4);
            // ideographic width.
            ArrayUtils.WriteIntToByteArray(outputArray, 0x04, 4, true, (UInt64)spaceWidth);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x08, 4, true, (UInt64)stride);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x0C, 4, true, (UInt64)fontDataHeight);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x10, 4, true, (UInt64)this.m_FontHeight);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x14, 4, true, (UInt64)curNum);
            ArrayUtils.WriteIntToByteArray(outputArray, 0x18, 4, true, (UInt32)blockLength);
            // currently at 0x1C.
            Array.Copy(index, 0, outputArray, 0x1C, index.Length);
            Int32 curIndex = 0x1C + index.Length;
            // Go over fontListBin and write all symbols that remain in it;
            // that should be exactly and only the remaining non-duplicates.
            for (Int32 i = 0; i < fontListBin.Length; ++i)
            {
                Byte[] symbolBytes = fontListBin[i];
                if (symbolBytes == null)
                    continue;
                Array.Copy(symbolBytes, 0, outputArray, curIndex, blockLength);
                curIndex += blockLength;
            }
            return outputArray;
        }

    }
}