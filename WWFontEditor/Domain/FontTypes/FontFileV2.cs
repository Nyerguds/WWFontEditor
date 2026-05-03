using Nyerguds.Util;
using System;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Old 1bpp Westwood font format
    /// </summary>
    public class FontFileV2 : FontFile
    {
        public override Int32 SymbolsTypeMin { get { return 0x80; } }
        public override Int32 SymbolsTypeMax { get { return 0x80; } }
        public override Int32 FontWidthTypeMax { get { return 0x8; } }
        public override Int32 FontHeightTypeMax { get { return 0xFF; } }
        public override Int32 YOffsetTypeMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override Boolean CustomSymbXForType { get { return false; } }
        public override Boolean CustomSymbYForType { get { return false; } }
        public override String ShortTypeName { get { return "WW v2"; } }
        public override String ShortTypeDescription { get { return "WWFont v2 (BattleTech, EoB)"; } }
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
            if (fileData.Length < 0x104)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int16 fileSize = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x00, 2, true);
            if (fileSize != fileData.Length - 2)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            // the size of the file: already read. Skip this.
            //Int16 filesize = ArrayUtils.GetLEShortFromByteArray(fileData, 0x00);
            // the offset of the pixel data from the beginning of the file, the index is the ascii value (always 128 long)
            Int16[] fontDataOffsetsList = new Int16[0x80];
            for (Int32 i = 0; i < 0x80; i++)
                fontDataOffsetsList[i] = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 2 + i * 2, 2, true);
            // the height of a symbol in pixel
            this.m_FontHeight = fileData[0x102];
            // the width of a symbol in pixel
            this.m_FontWidth = fileData[0x103];
            for (Int32 i = 0; i < 0x80; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte[] curData8bit;
                try
                {
                    curData8bit = ImageUtils.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, start, this.BitsPerPixel);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, this.BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont(Boolean disableCompression)
        {
            Byte[][] imageData = new Byte[0x80][];
            for (Int32 i = 0; i < 0x80; i++)
            {
                FontFileSymbol fc = m_ImageDataList.Count > i ? this.m_ImageDataList[i] : new FontFileSymbol(this);
                imageData[i] = ImageUtils.ConvertFrom8Bit(fc.ByteData, this.m_FontWidth, this.m_FontHeight, this.BitsPerPixel);
            }
            Int32 fontDataOffset = 0x104;
            Int32 dataOffset = fontDataOffset;
            // Not sure if this is legal; the original fonts seem unoptimised.
            Byte[] fontDataOffsetsList = this.OptimizeImagesList(imageData, ref dataOffset);
            Byte[] fullData = new Byte[dataOffset];
            Int32 headerFileSize = dataOffset - 2;
            fullData[0x00] = (Byte)(headerFileSize & 0xFF);         //Int16 FileSize, low byte;
            fullData[0x01] = (Byte)((headerFileSize >> 8) & 0xFF);  //Int16 FileSize, high byte;
            Array.Copy(fontDataOffsetsList, 0, fullData, 0x02, fontDataOffsetsList.Length);
            fullData[0x102] = (Byte)m_FontHeight;                // Byte FontHeight
            fullData[0x103] = (Byte)m_FontWidth;                 // Byte FontWidth
            foreach (Byte[] symbolImgData in imageData)
            {
                if (symbolImgData.Length == 0)
                    continue;
                Array.Copy(symbolImgData, 0, fullData, fontDataOffset, symbolImgData.Length);
                fontDataOffset += symbolImgData.Length;
            }
            return fullData;
        }
    }
}