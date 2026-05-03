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
    public class FontFileWsV5 : FontFile
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
        public override String ShortTypeName { get { return "WWFont v5"; } }
        public override String ShortTypeDescription { get { return "WWFont v5 (RA2)"; } }
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
            //UInt32 dataStart? = (UInt32) ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            Int32 stride = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x08, 4, true);
            this.m_FontHeight = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 4, true);
            this.m_FontWidth = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x10, 4, true);
            // count: highest encountered ID. But all IDs are +1.
            Int32 count = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x14, 4, true);
            Int32 symbolSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x18, 4, true);
            Int32 symbolDataSize = symbolSize - 1;
            if (stride * m_FontHeight != symbolDataSize)
                throw new FileTypeLoadException("Symbol size does not match width * height!");
            Int32 dataStart = 0x1C;
            List<Int32>[] symbolUsage = new List<Int32>[count];
            if (fileData.Length <= dataStart + 0x20000)
                throw new FileTypeLoadException(ERR_NOHEADER);
            for (Int32 i = 0; i <= 0xFFFF; i++)
            {
                Int32 symbolIndex = (UInt16)(ArrayUtils.ReadIntFromByteArray(fileData, dataStart, 2, true)) - 1;
                if (symbolIndex >= count)
                    throw new FileTypeLoadException("Symbol index exceeds number of symbols!");
                if (symbolIndex >= 0)
                {
                    if (symbolUsage[symbolIndex] == null)
                        symbolUsage[symbolIndex] = new List<Int32>();
                    symbolUsage[symbolIndex].Add(i);
                }
                dataStart += 2;
            }
            FontFileSymbol[] symbols = new FontFileSymbol[0x10000];
            for (Int32 i = 0; i < count; i++)
            {
                if (dataStart >= fileData.Length)
                    throw new FileTypeLoadException("File is not long enough to contain all referenced symbols!");
                List<Int32> curSymbolUsage = symbolUsage[i];
                if (curSymbolUsage == null)
                    break;
                Byte symbolWidth = fileData[dataStart++];
                Byte[] symbolData = new Byte[symbolDataSize];
                Array.Copy(fileData, dataStart, symbolData, 0, symbolDataSize);
                dataStart += symbolDataSize;
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
            for (Int32 i = 0; i < imageListcount; i++)
            {
                FontFileSymbol ffs = m_ImageDataList[i];
                if (ffs.Width == 0)
                    continue;
                Byte[] output = new Byte[blockLength];
                output[0] = (Byte)ffs.Width;
                Int32 symbStride = ffs.Width;
                Byte[] oneBppArr = ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, ffs.Height, 1, true, ref symbStride);
                oneBppArr = ImageUtils.ChangeStride(oneBppArr, symbStride, ffs.Height, stride, false, 0);
                Array.Copy(oneBppArr, 0, output, 1, dataLength);
                fontListBin[i] = output;
            }
            List<Byte[]> optimisedList = new List<Byte[]>();
            // Remove all empty symbols
            //FontFileSymbol blep = copiedList[65510];
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
                for (Int32 j = i + 1; j < imageListcount; j++)
                {
                    Byte[] curChecksymbol = fontListBin[j];
                    if (curChecksymbol == null || curWritesymbol[0] != curChecksymbol[0])
                        continue;
                    if (curWritesymbol.SequenceEqual(curChecksymbol))
                    {
                        ArrayUtils.WriteIntToByteArray(index, j << 1, 2, true, curNum);
                        fontListBin[j] = null;
                    }
                }
                fontListBin[i] = null;
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