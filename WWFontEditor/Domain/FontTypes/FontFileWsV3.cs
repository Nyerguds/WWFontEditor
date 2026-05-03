using System;
using System.Collections.Generic;
using System.Linq;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Main 4-bpp Westwood Studios font format
    /// </summary>
    public class FontFileWsV3 : FontFile
    {

        public override Int32 SymbolsTypeMax { get { return 0x100; } }
        public override Int32 FontWidthTypeMax { get { return 0xFF; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0xFF; } }
        public override Int32 BitsPerPixel { get { return 4; } }
        public override String ShortTypeName { get { return "WWFont v3"; } }
        public override String ShortTypeDescription { get { return "WWFont v3 (D2/C&C1/RA1/LoL/Kyr)"; } }
        public override String LongTypeDescription { get { return "A 4-bpp font with variable amount of characters, which allows separate symbols to specify their width, height and Y-offset. It is optimised by saving duplicate symbols only one time."; } }
        public override String[] GamesListForType
        {
            get
            {
                return new String[]
                {
                    "Dune II",
                    "Lands of Lore The Throne of Chaos",
                    "The Legend of Kyrandia Hand of Fate",
                    "The Legend of Kyrandia Malcolm's Revenge",
                    "The Legend of Kyrandia Malcolm's Revenge Installer",
                    "Command & Conquer",
                    "Command & Conquer Installer",
                    "Command & Conquer Red Alert",
                    "Command & Conquer Red Alert Installer",
                    "Lands of Lore Guardians of Destiny",
                    "Lands of Lore Guardians of Destiny Installer",
                    "Command & Conquer Sole Survivor",
                    "Lands of Lore III",
                };
            }
        }

        public override void LoadFont(Byte[] fileData)
        {
            this.LoadV3V4Font(fileData, false);
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            return this.SaveV3V4Font(false);
        }


        #region V3 / V4 loading and saving

        protected void LoadV3V4Font(Byte[] fileData, Boolean forV4)
        {
            Int32 fileLength = fileData.Length;
            if (fileLength < 0x14)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int32 fontHeaderLength = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x00, 2, true);
            if (fontHeaderLength != fileLength)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            Byte fontHeaderCompress = fileData[0x02];
            //Byte dataBlocks = fileData[0x03];
            //Int16 infoBlockOffset = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 2, true);
            Int32 fontHeaderOffsetBlockOffset = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x06, 2, true);
            Int32 fontHeaderWidthBlockOffset = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x08, 2, true);
            // use this for pos on TS format
            Int32 fontHeaderDataBlockOffset = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0A, 2, true);
            Int32 fontHeaderHeightOffset = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 2, true);
            //UInt16 unknown0E = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0E, 2, true);
            //Byte AlwaysZero = fileData[0x10];
            Int32 nrOfSymbols;
            Boolean isV4 = fontHeaderCompress == 0x02;
            if (isV4)
            {
                if (!forV4)
                    throw new FileTypeLoadException("Load type identifies as v4.");
                // "last symbol" byte 0x11 is not filled in on TS fonts, so instead, calculate it from the header offsets. Sort by offset and take the lowest two.
                Int32[] headerVals = new Int32[] {fontHeaderOffsetBlockOffset, fontHeaderWidthBlockOffset, fontHeaderDataBlockOffset, fontHeaderHeightOffset}.OrderBy(n => n).Take(2).ToArray();
                // The difference between these two, divided by the item length in that particular list, is the amount of symbols.
                Int32 divval = 1;
                if (headerVals[0] == fontHeaderOffsetBlockOffset || headerVals[0] == fontHeaderHeightOffset)
                    divval = 2;
                nrOfSymbols = (headerVals[1] - headerVals[0]) / divval;
            }
            else if (fontHeaderCompress == 0x00)
            {
                if (forV4)
                    throw new FileTypeLoadException("Load type identifies as v3.");
                nrOfSymbols = fileData[0x11] + 1; // "last symbol" byte, so actual amount is this value + 1.
            }
            else
                throw new FileTypeLoadException(String.Format("Unknown font type identifier, '{0}'.", fontHeaderCompress));
            this.m_FontHeight = fileData[0x12]; // MaxHeight
            this.m_FontWidth = fileData[0x13]; // MaxWidth
            if (fontHeaderOffsetBlockOffset + nrOfSymbols * 2 > fileLength)
                throw new FileTypeLoadException("File data too short for offsets list!");
            if (fontHeaderWidthBlockOffset + nrOfSymbols > fileLength)
                throw new FileTypeLoadException("File data too short for symbol widths list starting from offset!");
            if (fontHeaderHeightOffset + nrOfSymbols * 2 > fileLength)
                throw new FileTypeLoadException("File data too short for symbol heights list!");

            //FontDataOffset
            Int32[] fontDataOffsetsList = new Int32[nrOfSymbols];
            for (Int32 i = 0; i < nrOfSymbols; ++i)
                fontDataOffsetsList[i] = (UInt16)ArrayUtils.ReadIntFromByteArray(fileData, fontHeaderOffsetBlockOffset + i * 2, 2, true) + (isV4 ? fontHeaderDataBlockOffset : 0);
            List<Byte> widthsList = new List<Byte>();
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                Byte width = fileData[fontHeaderWidthBlockOffset + i];
                if (width > this.FontWidth)
                    throw new FileTypeLoadException(String.Format("Illegal value '{0}' in symbol widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                widthsList.Add(width);
            }
            List<Byte> yOffsetsList = new List<Byte>();
            List<Byte> heightsList = new List<Byte>();
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                yOffsetsList.Add(fileData[fontHeaderHeightOffset + i * 2]);
                Byte height = fileData[fontHeaderHeightOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new FileTypeLoadException(String.Format("Illegal value '{0}' in symbol heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                heightsList.Add(height);
            }
            // End of FileTypeLoadExceptions. After this, assume the type is identified.
            this.m_ImageDataList = new List<FontFileSymbol>();
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < nrOfSymbols; ++i)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte width = widthsList[i];
                Byte height = heightsList[i];
                Byte[] data8Bit;
                try
                {
                    data8Bit = ImageUtils.ConvertTo8Bit(fileData, width, height, start, bitsLength, false);
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i), ex);
                }
                FontFileSymbol fc = new FontFileSymbol(data8Bit, width, height, yOffsetsList[i], bitsLength, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }

        protected Byte[] SaveV3V4Font(Boolean forV4)
        {
            // Y-optimization.
            Int32 imagesCount = this.m_ImageDataList.Count;
            for (Int32 i = 0; i < imagesCount; ++i)
                this.m_ImageDataList[i].OptimizeYHeight(this.YOffsetTypeMax);
            Byte[][] imageData = new Byte[imagesCount][];
            Byte[] widthsList = new Byte[imagesCount];
            Byte[] heightsList = new Byte[imagesCount * 2];
            // header + UInt16 index + Byte heights
            Int32 offsetsListOffset = 0x14;
            Int32 widthsListOffset = offsetsListOffset + imagesCount * 2;
            Int32 heightsListOffset = 0;
            // V4 (TS) has its Y/height list before the image data.
            if (forV4)
                heightsListOffset = widthsListOffset + imagesCount;
            Int32 fontOffsetStart = (!forV4) ? widthsListOffset + imagesCount : heightsListOffset + imagesCount * 2;
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < imagesCount; ++i)
            {
                FontFileSymbol fc = this.m_ImageDataList[i];
                Byte[] imgData8bit = fc.ByteData;
                Byte imgWidth = (Byte)fc.Width;
                Byte imgHeight = (Byte)fc.Height;
                // Small optimization; no need to go converting the TS stuff; it doesn't change.
                if (bitsLength < 8)
                    imageData[i] = ImageUtils.ConvertFrom8Bit(imgData8bit, imgWidth, imgHeight, bitsLength, false);
                else
                    imageData[i] = imgData8bit.ToArray();
                widthsList[i] = imgWidth;
                heightsList[i * 2] = (Byte)fc.YOffset;
                heightsList[i * 2 + 1] = imgHeight;
            }
            Int32 fontOffset = forV4 ? 0 : fontOffsetStart;
            Byte[] fontDataOffsetsList = this.CreateImageIndex(imageData, 0, false, ref fontOffset, true, true, true);
            // V3 (C&C/RA) has its Y/height list after the image data.
            if (!forV4)
                heightsListOffset = fontOffset;
            Int32 fullLength = !forV4 ? (heightsListOffset + imagesCount * 2) : (fontOffset + fontOffsetStart);
            if (fullLength > UInt16.MaxValue)
                throw new OverflowException("The full font data size exceeds the maximum of " + UInt16.MaxValue + " bytes supported for " + ShortTypeName + ".");
            Byte[] fullData = new Byte[fullLength];

            // write header
            ArrayUtils.WriteIntToByteArray(fullData, 0, 2, true, (UInt32)fullLength);
            fullData[0x02] = (Byte)(forV4 ? 0x02 : 0x00);       // Byte DataFormat
            fullData[0x03] = (Byte)(forV4 ? 0 : 5);             // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = 0x0e;                              // UInt16 Unknown04, low byte; (always 0x0e)
            fullData[0x05] = 0x00;                              // UInt16 Unknown04, high byte; (always 0x00)
            ArrayUtils.WriteIntToByteArray(fullData, 0x06, 2, true, (UInt16)offsetsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x08, 2, true, (UInt16)widthsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0A, 2, true, (UInt16)fontOffsetStart);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0C, 2, true, (UInt16)heightsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0E, 2, true, (UInt16)(forV4 ? 0 : 0x1012));
            fullData[0x10] = 0x00;                              // Byte AlwaysZero (Always 0x00)
            fullData[0x11] = (Byte)(forV4 ? 0 : imagesCount - 1);  // Byte LastSymbolIndex (for non-TS)
            fullData[0x12] = (Byte) this.m_FontHeight;                // Byte FontHeight
            fullData[0x13] = (Byte) this.m_FontWidth;                 // Byte FontWidth
            Array.Copy(fontDataOffsetsList, 0, fullData, offsetsListOffset, fontDataOffsetsList.Length);
            Array.Copy(widthsList, 0, fullData, widthsListOffset, widthsList.Length);
            Int32 imageDataOffs = fontOffsetStart;
            for (Int32 i = 0; i < imagesCount; ++i)
            {
                Byte[] symbolImgData = imageData[i];
                if (symbolImgData == null || symbolImgData.Length == 0)
                    continue;
                Array.Copy(symbolImgData, 0, fullData, imageDataOffs, symbolImgData.Length);
                imageDataOffs += symbolImgData.Length;
            }
            // at this point, heightsListOffset should equal imageDataOffs, and the next operation should exactly fill up the array.
            Array.Copy(heightsList, 0, fullData, heightsListOffset, heightsList.Length);
            // return data
            return fullData;
        }
        #endregion

        /// <summary>
        /// any actions to be taken after conversion to this type.
        /// </summary>
        protected override void PostConvertCleanup()
        {
            // Y-optimization.
            Int32 imagesCount = this.m_ImageDataList.Count;
            for (Int32 i = 0; i < imagesCount; ++i)
                this.m_ImageDataList[i].OptimizeYHeight(this.YOffsetTypeMax);
        }
    }
}