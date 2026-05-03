using Nyerguds.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 8-bpp Dynamix font format
    /// </summary>

    public class FontFileDynV6 : FontFile
    {
        public const String CHAR_OVERFLOW = "This font format can save only 255 symbols. Reduce the amount of symbols, or remove symbol #0 by reducing its width to 0, to allow saving the file.";

        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 8; } }
        /// <summary>File extensions typically used for this font type.</summary>
        public override String[] FileExtensions { get { return new String[] { "fon" }; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v6"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v6 (FPS-Fball-Pro)"; } }
        public override String LongTypeDescription { get { return "An 8-bpp font with width definable for each symbol. It is optimized by only saving the used range of symbols."; } }
        public override String[] GamesListForType
        {
            get { return new String[] { "Front Page Sports Football Pro" }; }
        }

        protected Byte m_unkn1;

        public Byte LineHeight { get; set; }
        
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
            Int32 dataIndexOffset = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset, 4, true) + dataOffset;
            if (dataIndexOffset < 0 || dataIndexOffset > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            Int32 widthsOffset = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 4, 4, true) + dataOffset;
            if (widthsOffset  < 0 || widthsOffset > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            Int32 symbolDataStart = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 8, 4, true) + dataOffset;
            if (symbolDataStart < 0 || symbolDataStart > fileData.Length)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            this.m_unkn1 = fileData[dataOffset + 0x0C];
            this.LineHeight = fileData[dataOffset + 0x0D];
            Int32 startSymbol = fileData[dataOffset + 0x0E];
            Int32 nrOfSymbols = fileData[dataOffset + 0x0F];
            if (startSymbol < 0 || nrOfSymbols < 0)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            if(startSymbol + nrOfSymbols > 0x100)
                throw new FileTypeLoadException(ERR_MAXSYMB);
            this.m_FontWidth = fileData[dataOffset + 0x10];
            this.m_FontHeight = fileData[dataOffset + 0x11];
            if (this.m_FontWidth <= 0 || this.m_FontHeight <= 0)
                throw new FileTypeLoadException(ERR_BADHEADERDATA);
            // Read symbol information
            Int16[] offsets = new Int16[nrOfSymbols];
            for (Int32 i = 0; i < nrOfSymbols; ++i)
                offsets[i] = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, dataIndexOffset + i * 2, 2, true);
            Byte[] widths = new Byte[nrOfSymbols];
            Array.Copy(fileData, widthsOffset, widths, 0, nrOfSymbols);
            // Read symbols
            for (Int32 i = 0; i < startSymbol; ++i)
                this.m_ImageDataList.Add(new FontFileSymbol(this));
            for (Int32 i = 0; i < offsets.Length; ++i)
            {
                Byte[] curData8bit;
                Byte symbWidth = widths[i];
                if (symbWidth > this.m_FontWidth)
                    throw new FileTypeLoadException(ERR_MAXWIDTH);
                try
                {
                    curData8bit = new Byte[symbWidth * this.m_FontHeight];
                    Array.Copy(fileData, symbolDataStart + offsets[i], curData8bit, 0, curData8bit.Length);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new FileTypeLoadException(String.Format("{0}: Data for font entry #{1} exceeds file bounds!", this.ShortTypeName, i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, symbWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }
        
        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            if (this.m_ImageDataList.Count == 0x100)
            {
                FontFileSymbol first = this.m_ImageDataList[0];
                FontFileSymbol last = this.m_ImageDataList[0xFF];
                if (first.Width > 0 && last.Width > 0)
                    throw new InvalidOperationException(CHAR_OVERFLOW);
            }
            // Line height. Default calculation uses the most commonly used lowest point in the font.
            Int32 lHeight = this.LineHeight;
            if (lHeight == 0)
                lHeight = (Byte)FontFileDynV4.CalculateLineHeight(this.m_ImageDataList, this.BitsPerPixel, this.YOffsetTypeMax, this.TransparencyColor);
            return new SaveOption[]
            {
                new SaveOption("OPT", SaveOptionType.Boolean, "Optimise to remove duplicate symbols", "0"),
                new SaveOption("YOF", SaveOptionType.Number, "Font base line Y-offset", lHeight.ToString())
            };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Int32 lineHeight;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "YOF"), out lineHeight);
            Boolean optimise = GeneralUtils.IsTrueValue(SaveOption.GetSaveOptionValue(saveOptions, "OPT"));
            this.LineHeight = (Byte)lineHeight;
            Boolean foundStart = false;
            Int32 startSymbol = 0;
            Int32 fullNrOfSymbols = this.m_ImageDataList.Count;
            Byte[] symbolWidths = new Byte[fullNrOfSymbols];
            Byte[][] symbolData = new Byte[fullNrOfSymbols][];
            for (Int32 i = 0; i < fullNrOfSymbols; ++i)
            {
                FontFileSymbol ffs = this.m_ImageDataList[i];
                if (!foundStart)
                {
                    if (i < 0x20 && ffs.Width == 0)
                        continue;
                    foundStart = true;
                    startSymbol = i;
                }
                symbolData[i] = ffs.ByteData;
                symbolWidths[i] = (Byte)ffs.Width;
            }
            Int32 fontOffset = 0;
            Byte[] fontDataOffsetsList = this.CreateImageIndex(symbolData, startSymbol, true, ref fontOffset, true, optimise, true);
            Int32 chunkOffset = 8;
            Int32 actualSymbols = fullNrOfSymbols - startSymbol;
            Int32 offsetsIndex = 0x12;
            Int32 widthsIndex = offsetsIndex + actualSymbols * 2;
            Int32 dataIndex = widthsIndex + actualSymbols;
            Byte[] fileData = new Byte[chunkOffset + dataIndex + fontOffset];
            Array.Copy(Encoding.ASCII.GetBytes("FNT:"), 0, fileData, 0, 4);
            ArrayUtils.WriteIntToByteArray(fileData, 4, 4, true, (UInt32)(fileData.Length - 8));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x00, 4, true, (UInt32)(offsetsIndex));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x04, 4, true, (UInt32)(widthsIndex));
            ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + 0x08, 4, true, (UInt32)(dataIndex));
            fileData[chunkOffset + 0x0C] = (Byte)(this.m_FontWidth - this.LineHeight);
            fileData[chunkOffset + 0x0D] = this.LineHeight;
            fileData[chunkOffset + 0x0E] = (Byte)startSymbol;
            fileData[chunkOffset + 0x0F] = (Byte)(actualSymbols);
            fileData[chunkOffset + 0x10] = (Byte)this.m_FontWidth;
            fileData[chunkOffset + 0x11] = (Byte)this.m_FontHeight;
            // First: data index
            offsetsIndex += chunkOffset;
            Array.Copy(fontDataOffsetsList, 0, fileData, offsetsIndex, fontDataOffsetsList.Length);
            // Second: image widths
            widthsIndex += chunkOffset;
            Array.Copy(symbolWidths, startSymbol, fileData, widthsIndex, actualSymbols);
            // Third: actual font data.
            dataIndex += chunkOffset;
            for (Int32 i = startSymbol; i < fullNrOfSymbols; ++i)
            {
                Byte[] image = symbolData[i];
                if (image == null || image.Length == 0)
                    continue;
                Array.Copy(image, 0, fileData, dataIndex, image.Length);
                dataIndex += image.Length;
            }
            return fileData;
        }

        public Byte[] SaveFontOld(SaveOption[] saveOptions)
        {
            Int32 lineHeight;
            Int32.TryParse(SaveOption.GetSaveOptionValue(saveOptions, "YOF"), out lineHeight);

            this.LineHeight = (Byte)lineHeight;
            Int32 len = this.m_ImageDataList.Count;
            Int32[] symbolOffsets = new Int32[len];
            Byte[] symbolWidths = new Byte[len];
            Byte[][] symbolData = new Byte[len][];
            Int32 indexOffset = 0;
            Boolean foundStart = false;
            Int32 startSymbol = 0;
            for (Int32 i = 0; i < len; ++i)
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
            fileData[chunkOffset + 0x0C] = (Byte)(this.m_FontWidth - this.LineHeight);
            fileData[chunkOffset + 0x0D] = this.LineHeight;
            fileData[chunkOffset + 0x0E] = (Byte)startSymbol;
            fileData[chunkOffset + 0x0F] = (Byte)(actualSymbols);
            fileData[chunkOffset + 0x10] = (Byte) this.m_FontWidth;
            fileData[chunkOffset + 0x11] = (Byte) this.m_FontHeight;
            Array.Copy(symbolWidths, startSymbol, fileData, chunkOffset + widthsIndex, actualSymbols);
            for (Int32 i = startSymbol; i < len; ++i)
            {
                Int32 symbIndex = i - startSymbol;
                ArrayUtils.WriteIntToByteArray(fileData, chunkOffset + offsetsIndex + symbIndex * 2, 2, true, (UInt32)symbolOffsets[i]);
                Array.Copy(symbolData[i], 0, fileData, chunkOffset + dataIndex + symbolOffsets[i], symbolData[i].Length);
            }
            return fileData;
        }

    }
}