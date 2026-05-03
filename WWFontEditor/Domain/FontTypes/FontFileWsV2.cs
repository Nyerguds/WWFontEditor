using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Old 1bpp Westwood Studios font format
    /// </summary>
    public class FontFileWsV2 : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x80; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 FontWidthTypeMax { get { return 0x8; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override Boolean CustomSymbolWidthsForType { get { return false; } }
        public override Boolean CustomSymbolHeightsForType { get { return false; } }
        public override String ShortTypeName { get { return "WWFont v2"; } }
        public override String ShortTypeDescription { get { return "WWFont v2 (BattleTech/EoB)"; } }
        public override String LongTypeDescription { get { return "A 1 BPP font with a fixed set of 128 characters, with a maximum width of 8 pixels, with the file header specifying the global width and height for all symbols."; } }
        public override String[] GamesListForType { get { return new String[]
        {
            "BattleTech - The Crescent Hawk's Revenge",
            "Eye of the Beholder",
            "Eye of the Beholder II: The Legend of Darkmoon",
            "Eye of the Beholder III Character Generator"
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            this.LoadFont(fileData, false);
        }

        public void LoadFont(Byte[] fileData, Boolean extendedFormat)
        {
            if (fileData.Length < this.SymbolsTypeMax * 2 + 4)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int16 fileSize = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x00, 2, true);
            if (fileSize != fileData.Length - 2)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            // the offset of the pixel data from the beginning of the file, the index is the ascii value (always 128 long)
            Int16[] fontDataOffsetsList = new Int16[this.SymbolsTypeMax];
            for (Int32 i = 0; i < this.SymbolsTypeMax; i++)
                fontDataOffsetsList[i] = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 2 + i * 2, 2, true);
            // Detect modified type
            if (fontDataOffsetsList[0] == 0x204 && !extendedFormat)
                throw new FileTypeLoadException(ERR_SIZEHEADER);
            // the height of a symbol in pixel
            this.m_FontHeight = fileData[this.SymbolsTypeMax * 2 + 2];
            // the width of a symbol in pixel
            this.m_FontWidth = fileData[this.SymbolsTypeMax * 2 + 3];
            for (Int32 i = 0; i < this.SymbolsTypeMax; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, this.m_FontWidth, this.m_FontHeight, start, this.BitsPerPixel, true);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel, this.TransparencyColor);
                this.m_ImageDataList.Add(fc);
            }
        }
        
        public override SaveOption[] GetSaveOptions(String targetFileName)
        {
            return new SaveOption[] { new SaveOption("OPT", SaveOptionType.Boolean, "Optimise duplicate symbols (Not advised for Eye of the Beholder 1)", "0") };
        }

        public override Byte[] SaveFont(SaveOption[] saveOptions)
        {
            Byte[][] imageData = new Byte[this.SymbolsTypeMax][];
            for (Int32 i = 0; i < this.SymbolsTypeMax; i++)
            {
                FontFileSymbol fc = this.m_ImageDataList.Count > i ? this.m_ImageDataList[i] : new FontFileSymbol(this);
                imageData[i] = ImageUtils.ConvertFrom8Bit(fc.ByteData, this.m_FontWidth, this.m_FontHeight, this.BitsPerPixel, true);
            }
            Boolean optimise = GeneralUtils.IsTrueValue(SaveOption.GetSaveOptionValue(saveOptions, "OPT"));
            Int32 afterIndex = this.SymbolsTypeMax * 2;
            Int32 fontDataOffset = afterIndex + 4;
            Int32 dataOffset = fontDataOffset;
            // Not sure if this is legal; the original fonts seem unoptimised.
            Byte[] fontDataOffsetsList = this.CreateImageIndex(imageData, 0, false, ref dataOffset, false, optimise, true);
            Byte[] fullData = new Byte[dataOffset];
            Int32 headerFileSize = dataOffset - 2;
            fullData[0x00] = (Byte)(headerFileSize & 0xFF);         //Int16 FileSize, low byte;
            fullData[0x01] = (Byte)((headerFileSize >> 8) & 0xFF);  //Int16 FileSize, high byte;
            Array.Copy(fontDataOffsetsList, 0, fullData, 0x02, fontDataOffsetsList.Length);
            fullData[afterIndex + 2] = (Byte) this.m_FontHeight;          // Byte FontHeight
            fullData[afterIndex + 3] = (Byte) this.m_FontWidth;           // Byte FontWidth
            foreach (Byte[] symbolImgData in imageData)
            {
                if (symbolImgData == null || symbolImgData.Length == 0)
                    continue;
                Array.Copy(symbolImgData, 0, fullData, fontDataOffset, symbolImgData.Length);
                fontDataOffset += symbolImgData.Length;
            }
            return fullData;
        }
    }
}