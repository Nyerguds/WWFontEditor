using Nyerguds.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Nyerguds.ImageManipulation;
using System.Text;
using Compression;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// 1bpp Dynamix font format
    /// </summary>
    
    public class FontFileDynV4 : FontFile
    {
        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return this.m_bpp; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "DYN v4"; } }
        public override String ShortTypeDescription { get { return "Dynamix Font v4 (RBar/RotD/HoC/WBeam/Kron)"; } }
        public override String LongTypeDescription { get { return "A 1 BPP font with compression support, with width definable for each symbol. It is optimized by only saving the used range of symbols."; } }

        public override String[] GamesListForType
        {
            get
            {
                return new String[]
                {
                    "Red Baron",
                    "Rise of the Dragon",
                    "Heart of China",
                    "The Adventures of Willy Beamish",
                    "Betrayal at Krondor",
                    "A-10 Tank Killer v1.5",
                    "Stellar 7",
                    "Nova 9: The Return of Gir Draxon",
                    "The Incredible Machine",
                    "Sid & Al's Incredible Toons",
                    "Front Page Sports Football"
                };
            }
        }

        public Int32 lineHeight;
        protected Int32 m_bpp = 1;

        public override void LoadFont(Byte[] fileData)
        {
            LoadFont(fileData, false);
        }

        public void LoadFont(Byte[] fileData, Boolean asV5)
        {
            if (fileData.Length < 0x15)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Byte[] sectionId = new Byte[4];
            Array.Copy(fileData, 0, sectionId, 0, 4);
            if (!sectionId.SequenceEqual(Encoding.ASCII.GetBytes("FNT:")))
                throw new FileTypeLoadException(ERR_BADHEADER);
            Int32 fileSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 4, true);
            if (fileSize != fileData.Length - 8)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            Int32 dataOffset = 0x08;
            if (asV5)
            {
                if (fileData[dataOffset] != 0xFD)
                    throw new FileTypeLoadException("Not a v5 Dynamix font!");
                m_bpp = 8;
            }
            else
            {
                if (fileData[dataOffset] != 0xFF)
                    throw new FileTypeLoadException("Not a v4 Dynamix font!");
                m_bpp = 1;
            }
            this.m_FontWidth = fileData[dataOffset + 1];
            this.m_FontHeight = fileData[dataOffset + 2];
            this.lineHeight = fileData[dataOffset + 3];
            Byte startSymbol = fileData[dataOffset + 4];
            Byte nrOfSymbols = fileData[dataOffset + 5];

            Int32 symbolDataSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 6, 2, true);
            Int32 compressionMethod = fileData[dataOffset + 8];
            Int32 uncompressedSize = (Int32)ArrayUtils.ReadIntFromByteArray(fileData, dataOffset + 9, 2, true);
            if (uncompressedSize != symbolDataSize)
                throw new FileTypeLoadException("Error in complex-type Dynamix font: font data size doesn't match uncompressed data size!");

            Int32 dataStart = dataOffset + 13;
            Byte[] compressedData = new Byte[fileData.Length - dataStart];
            Array.Copy(fileData, dataStart, compressedData, 0, compressedData.Length);
            Byte[] data;

            switch (compressionMethod)
            {
                case 0:
                    if (symbolDataSize > compressedData.Length)
                        throw new IndexOutOfRangeException("Data length does not match actual data!");
                    data = compressedData;
                    break;
                case 1:
                    data = DynamixCompression.RleDecode(compressedData, uncompressedSize);
                    break;
                case 2:
                    data = DynamixCompression.LzwDecode(compressedData, uncompressedSize);
                    break;
                default:
                    throw new FileTypeLoadException(String.Format("Unknown compression type \"{0}\"", compressionMethod));
            }
            Int16[] offsets = new Int16[nrOfSymbols];
            //File.WriteAllBytes("fontdump.fnt", data);
            for (Int32 i = 0; i < nrOfSymbols; i++)
                offsets[i] = (Int16)ArrayUtils.ReadIntFromByteArray(data, i * 2, 2, true);
            Int32 readStart = nrOfSymbols * 2;

            Byte[] widths = new Byte[nrOfSymbols];
            Array.Copy(data, readStart, widths, 0, nrOfSymbols);
            readStart += nrOfSymbols;
            // fill in dummy symbols. Will need to be checked and trimmed on save (until 0x20 that is.)
            for (Int32 i = 0; i < startSymbol; i++)
                this.m_ImageDataList.Add(new FontFileSymbol(new Byte[0], 0, this.m_FontHeight, 0, this.BitsPerPixel));
            for (int i = 0; i < offsets.Length; i++)
            {
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(data, widths[i], this.m_FontHeight, readStart + offsets[i], this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new FileTypeLoadException(String.Format("{0}: Data for font entry #{1} exceeds file bounds!", ShortTypeName, i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, widths[i], this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            return SaveFont(disableCompression, false);
        }

        public Byte[] SaveFont(Boolean disableCompression, Boolean asV5)
        {
            // Not sure about this value; there is no support in the editor for indicating anything like this.
            // But the most commonly used lowest point in the font seems like a logical value. It matches the existing fonts.
            this.lineHeight = CalculateLineHeight(m_ImageDataList, this.BitsPerPixel);
            Boolean foundStart = false;
            Int32 startSymbol = 0;
            Int32 fullNrOfSymbols = m_ImageDataList.Count;
            Byte[][] imageData = new Byte[fullNrOfSymbols][];
            Byte[] imageWidths = new Byte[fullNrOfSymbols];
            //Int32 fontDataSize = 0;
            for (Int32 i = 0; i < fullNrOfSymbols; i++)
            {
                FontFileSymbol ffs = m_ImageDataList[i];
                if (!foundStart)
                {
                    if (i < 0x20 && ffs.Width == 0)
                        continue;
                    foundStart = true;
                    startSymbol = i;
                }
                imageData[i] = asV5? ffs.ByteData : ImageUtils.ConvertFrom8Bit(ffs.ByteData, ffs.Width, ffs.Height, this.BitsPerPixel, true);
                imageWidths[i] = (Byte)ffs.Width;
            }
            Int32 fontOffset = 0;
            Byte[] fontDataOffsetsList = this.OptimizeImagesList(imageData, startSymbol, ref fontOffset);
            Int32 nrOfSymbols = fullNrOfSymbols - startSymbol;
            //Int32 fullDataSize = fontDataSize + nrOfSymbols * 3;
            Int32 fullDataSize = fontOffset + fontDataOffsetsList.Length * 3;
            Byte[] fullData = new Byte[fullDataSize];
            // Reserve space for index, and skip it.
            //Int32 indexOffset = 0;
            Int32 dataOffset = 0;
            Array.Copy(fontDataOffsetsList, 0, fullData, dataOffset, fontDataOffsetsList.Length);
            dataOffset += fontDataOffsetsList.Length;
            // Write image widths
            Array.Copy(imageWidths, startSymbol, fullData, dataOffset, nrOfSymbols);
            dataOffset += nrOfSymbols;
            UInt32 offset = 0;
            for (Int32 i = startSymbol; i < fullNrOfSymbols; i++)
            {
                Byte[] image = imageData[i];
                if (image == null || image.Length == 0)
                    continue;
                Array.Copy(image, 0, fullData, dataOffset + offset, image.Length);
                offset += (UInt32)image.Length;
            }
            Byte compression = 0;
            Byte[] writeData = fullData;
            if (!disableCompression)
            {
                /*/
                // Not implemented
                Byte[] compressLzw = DynamixCompression.LzwEncode(fullData);
                if (compressLzw != null && compressLzw.Length < writeData.Length)
                {
                    compression = 2;
                    writeData = compressLzw;
                }
                //*/
                Byte[] compressRle = DynamixCompression.RleEncode(fullData, 3);
                if (compressRle != null && compressRle.Length < writeData.Length)
                {
                    compression = 1;
                    writeData = compressRle;
                }
            }
            // offset to start writing data. Initialized on header length.
            Int32 writeOffset = 0x15;
            Byte[] fileData = new Byte[writeOffset + writeData.Length];
            Array.Copy(Encoding.ASCII.GetBytes("FNT:"), 0, fileData, 0, 4);
            ArrayUtils.WriteIntToByteArray(fileData, 4, 4, true, (UInt32)(fileData.Length - 8));
            // Indicator for v2 format
            fileData[0x08] = (Byte)(asV5 ? 0xFD : 0xFF);
            fileData[0x09] = (Byte)this.m_FontWidth;
            fileData[0x0A] = (Byte)this.m_FontHeight;
            // Line height value. Not sure what to do with it tbh... the editor doesn't really support setting this.
            fileData[0x0B] = (Byte)this.lineHeight;
            fileData[0x0C] = (Byte)startSymbol;
            fileData[0x0D] = (Byte)nrOfSymbols;
            // Full added size: font size + symbols index + symbol widths.
            ArrayUtils.WriteIntToByteArray(fileData, 0x0E, 2, true, (UInt32)fullDataSize);
            // Compression method For now, let's leave that.
            fileData[0x10] = compression;
            ArrayUtils.WriteIntToByteArray(fileData, 0x11, 4, true, (UInt32)fullDataSize);
            Array.Copy(writeData, 0, fileData, writeOffset, writeData.Length);
            return fileData;
        }

        public static Int32 CalculateLineHeight(List<FontFileSymbol> imageDataList, Int32 bitsPerPixel)
        {
            Dictionary<Int32,Int32> frequencies = new Dictionary<Int32, Int32>();
            foreach (FontFileSymbol symbol in imageDataList)
            {
                FontFileSymbol ffs = new FontFileSymbol(symbol.ByteData, symbol.Width, symbol.Height, 0, bitsPerPixel);
                ffs.OptimizeYHeight();
                Int32 fullHeight = ffs.Height == 0? 0 : ffs.YOffset + ffs.Height;
                if (fullHeight == 0)
                    continue;
                Int32 curVal;
                if (!frequencies.TryGetValue(fullHeight, out curVal))
                    curVal = 0;
                frequencies[fullHeight] = curVal+1;
            }
            Int32 max = 0;
            Int32 maxKey = -1;
            foreach (KeyValuePair<Int32, Int32> kvp in frequencies)
            {
                if (kvp.Value <= max)
                    continue;
                maxKey = kvp.Key;
                max = kvp.Value;
            }
            return maxKey == -1 ? 0 : maxKey;
        }
    }
}