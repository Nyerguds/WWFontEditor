using Nyerguds.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>

    public class FontFileDynV6 : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fon" }; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v6"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v6"; } }
        public override String LongTypeDescription { get { return "An 8 BPP font with width definable for each symbol. It is optimized by only saving the used range of symbols."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Front Page Sports Football Pro" }; }
        }

        protected Byte m_unkn1 = 0;
        protected Byte m_lineHeight = 0;
        
        public override void LoadFont(Byte[] fileData)
        {
            // Read header data
            if (fileData.Length < 0x1A)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte[] sectionId = new Byte[4];
            Array.Copy(fileData, 0, sectionId, 0, 4);
            if (!sectionId.SequenceEqual(Encoding.ASCII.GetBytes("FNT:")))
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 fileSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            if (fileSize != fileData.Length - 8)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            Int32 dataOffset = 0x08;
            Int32 offsetsIndex = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset, 4, true) + dataOffset;
            if (offsetsIndex < 0 || offsetsIndex > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            Int32 widthsIndex = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 4, 4, true) + dataOffset;
            if (widthsIndex  < 0 || widthsIndex > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            Int32 symbolDataStartOffset = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 8, 4, true) + dataOffset;
            if (symbolDataStartOffset < 0 || symbolDataStartOffset > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            m_unkn1 = fileData[dataOffset + 0x0C];
            m_lineHeight = fileData[dataOffset + 0x0D];
            Int32 startSymbol = fileData[dataOffset + 0x0E];
            Int32 nrOfSymbols = fileData[dataOffset + 0x0F];
            if (startSymbol < 0 || nrOfSymbols < 0)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            if(startSymbol + nrOfSymbols > 0x100)
                throw new FileTypeLoadException(ERR_SYMBCHECK);
            m_FontWidth = fileData[dataOffset + 0x10];
            m_FontHeight = fileData[dataOffset + 0x11];
            if (m_FontWidth <= 0 || m_FontHeight <= 0)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            // Read symbol information
            Int16[] offsets = new Int16[nrOfSymbols];
            for (Int32 i = 0; i < nrOfSymbols; i++)
                offsets[i] = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, offsetsIndex + i * 2, 2, true);
            Byte[] widths = new Byte[nrOfSymbols];
            Array.Copy(fileData, widthsIndex, widths, 0, nrOfSymbols);
            // Read symbols
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 0, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor));
            for (int i = 0; i < offsets.Length; i++)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, widths[i], this.m_FontHeight, symbolDataStartOffset + offsets[i], this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new FileTypeLoadException(String.Format("{0}: Data for font entry #{1} exceeds file bounds!", ShortTypeName, i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, widths[i], this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            this.m_lineHeight = (Byte)FontFileDynV4.CalculateLineHeight(m_ImageDataList, this.BitsPerPixel, this.YOffsetTypeMax);
            Int32 len = this.m_ImageDataList.Count;
            Int32[] symbolOffsets = new Int32[len];
            Byte[] symbolWidths = new Byte[len];
            Byte[][] symbolData = new Byte[len][];
            Int32 indexOffset = 0;
            Boolean foundStart = false;
            Int32 startSymbol = 0;
            for (Int32 i = 0; i < len; i++)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                if (!foundStart)
                {
                    if (i < 0x20 && ffs.Width == 0)
                        continue;
                    foundStart = true;
                    startSymbol = i;
                }
                symbolOffsets[i] = indexOffset;
                symbolWidths[i] = (Byte)ffs.Width;
                symbolData[i] = ffs.ByteData;
                indexOffset += symbolData[i].Length;
            }
            Int32 chunkOffset = 8;
            Int32 actualSymbols = len - startSymbol;
            Int32 offsetsIndex = 0x12;
            Int32 widthsIndex = offsetsIndex + actualSymbols * 2;
            Int32 dataIndex = widthsIndex + actualSymbols;
            Byte[] fileData = new Byte[chunkOffset + dataIndex + indexOffset];
            Array.Copy(Encoding.ASCII.GetBytes("FNT:"), 0, fileData, 0, 4);
            ArrayUtils.WriteIntToByteArray(fileData, 4, 4, true, (UInt32)(fileData.Length - 8));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x00, 4, true, (UInt32)(offsetsIndex));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x04, 4, true, (UInt32)(widthsIndex));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x08, 4, true, (UInt32)(dataIndex));
            fileData[chunkOffset + 0x0C] = (Byte)(m_FontWidth - m_lineHeight);
            fileData[chunkOffset + 0x0D] = m_lineHeight;
            fileData[chunkOffset + 0x0E] = (Byte)startSymbol;
            fileData[chunkOffset + 0x0F] = (Byte)(actualSymbols);
            fileData[chunkOffset + 0x10] = (Byte)m_FontWidth;
            fileData[chunkOffset + 0x11] = (Byte)m_FontHeight;
            Array.Copy(symbolWidths, startSymbol, fileData, chunkOffset + widthsIndex, actualSymbols);
            for (Int32 i = startSymbol; i < len; i++)
            {
                Int32 symbIndex = i - startSymbol;
                ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + offsetsIndex + symbIndex * 2, 2, true, (UInt32)symbolOffsets[i]);
                Array.Copy(symbolData[i], 0, fileData, chunkOffset + dataIndex + symbolOffsets[i], symbolData[i].Length);
            }
            return fileData;
        }

    }
}