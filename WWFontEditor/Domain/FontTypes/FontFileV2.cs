using System;
using System.Linq;

namespace WWFontEditor.Domain.FontTypes
{
    /// <summary>
    /// Old 1bpp Westwood font format
    /// </summary>
    public class FontFileV2 : FontFile
    {
        public override Int32 CharactersMin { get { return 0x80; } }
        public override Int32 CharactersMax { get { return 0x80; } }
        public override Int32 FontWidthMax { get { return 0x8; } }
        public override Int32 FontHeightMax { get { return 0xFF; } }
        public override Int32 YOffsetMax { get { return 0; } }
        public override Int32 BitsPerPixel { get { return 1; } }
        public override String ShortTypeCode { get { return "WW V2"; } }
        public override String LongTypeCode { get { return "Westwood Font Version 2"; } }
        public override String[] GamesList { get { return new String[]
        {
            "BattleTech - The Crescent Hawk's Revenge",
            "Eye of the Beholder",
            "Eye of the Beholder II: The Legend of Darkmoon"
        }; } }

        public override void LoadFont(Byte[] fileData)
        {
            if (fileData.Length < 104)
                throw new LoadFailedException(ERR_NOHEADER);
            Int16 fileSize = ArrayUtils.GetLEShortFromByteArray(fileData, 0x00);
            if (fileSize != fileData.Length - 2)
                throw new LoadFailedException(ERR_SIZECHECK);
            // the size of the file: already read. Skip this.
            //Int16 filesize = ArrayUtils.GetLEShortFromByteArray(fileData, 0x00);
            // the offset of the pixel data from the beginning of the file, the index is the ascii value (always 128 long)
            Int16[] fontDataOffsetsList = new Int16[0x80];
            for (Int32 i = 0; i < 0x80; i++)
                fontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, 2 + i * 2);
            // the height of a character in pixel
            this.m_FontHeight = fileData[0x102];
            // the width of a character in pixel
            this.m_FontWidth = fileData[0x103];
            for (Int32 i = 0; i < 0x80; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte[] curData8bit = this.ConvertTo8Bit(fileData, m_FontWidth, m_FontHeight, start, BitsPerPixel, i, true);
                FontFileCharacter fc = new FontFileCharacter(curData8bit, this.m_FontWidth, this.m_FontHeight, 0, BitsPerPixel);
                this.m_ImageDataList.Add(fc);
            }
        }

        public override Byte[] SaveFont()
        {
            Int32 startIndex = 0x104;
            Byte[][] imageData = new Byte[0x80][];
            for (Int32 i = 0; i < 0x80; i++)
            {
                FontFileCharacter fc = m_ImageDataList.Count > i ? this.m_ImageDataList[i] : new FontFileCharacter(this.BitsPerPixel);
                imageData[i] = ConvertFrom8Bit(fc.ByteData, this.m_FontWidth, this.m_FontHeight, this.BitsPerPixel, true);
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
            foreach (Byte[] charImgData in imageData)
            {
                if (charImgData.Length == 0)
                    continue;
                Array.Copy(charImgData, 0, fullData, fontDataOffset, charImgData.Length);
                fontDataOffset += charImgData.Length;
            }
            return fullData;
        }
    }
}