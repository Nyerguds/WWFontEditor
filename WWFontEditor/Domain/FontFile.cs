using ColorManipulation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace WWFontEditor.Domain
{
    public class FontFile
    {
        public FontFileType FontFileType { get; set; }
        
        protected Byte m_FontHeight;
        /// <summary>Overall maximum font height.</summary>
        public Byte FontHeight
        {
            get { return m_FontHeight; }
            set
            {
                this.m_FontHeight = value;
                foreach (FontFileCharacter fontchar in this.m_ImageDataList)
                    if (fontchar.Height > value)
                        fontchar.ChangeHeight(value);
            }
        }

        protected Byte m_FontWidth;
        /// <summary>Overall maximum font width.</summary>
        public Byte FontWidth
        {
            get { return m_FontWidth; }
            set
            {
                this.m_FontWidth = value;
                foreach (FontFileCharacter fontchar in this.m_ImageDataList)
                    if (fontchar.Width > value)
                        fontchar.ChangeWidth(value);
            }
        }

        /// <summary> array with the actual image data (as 8-bit) as byte arrays</summary>
        private List<FontFileCharacter> m_ImageDataList = new List<FontFileCharacter>();

        public FontFile(Byte[] fileData)
        {
            LoadFromFileData(fileData);
        }

        /// <summary>
        /// Creates a deep clone of this font.
        /// </summary>
        /// <returns>A deep clone of this font.</returns>
        public FontFile Clone()
        {
            FontFile clone = (FontFile)this.MemberwiseClone();
            clone.m_ImageDataList = new List<FontFileCharacter>();
            foreach (FontFileCharacter image in this.m_ImageDataList)
                clone.m_ImageDataList.Add(image.Clone());
            return clone;
        }


        public String GetShortTypeCode()
        {
            switch (this.FontFileType)
            {
                case FontFileType.WWClassic1:
                    return "LOL/KYR/D2";
                case FontFileType.WWClassicCnC:
                    return "C&C1/RA1";
                case FontFileType.WWTibSun:
                    return "TS";
                case FontFileType.EDDune2k:
                    return "D2K";
            }
            return String.Empty;
        }

        public void RestorePicFromBackup(Int32 index, FontFile backup)
        {
            if (index < 0 || backup.Length <= index || this.Length <= index)
                return;
            RestorePicFromBackup(index, backup.m_ImageDataList[index]);
        }

        public void RestorePicFromBackup(Int32 index, FontFileCharacter backup)
        {
            if (index < 0 || this.Length <= index)
                return;
            FontFileCharacter fontchar = backup.Clone();
            if (fontchar.Height > this.FontHeight)
                fontchar.ChangeHeight(this.FontHeight);
            if (fontchar.Width > this.FontWidth)
                fontchar.ChangeWidth(this.FontWidth);
            this.m_ImageDataList[index] = fontchar;
        }

        public Int32 Length
        {
            get { return m_ImageDataList.Count; }
            set
            {
                value = Math.Min(value, 0x100);
                if (value < m_ImageDataList.Count)
                    m_ImageDataList = this.m_ImageDataList.Take(value).ToList();
                else
                {
                    for (Int32 i = m_ImageDataList.Count; i < value; i++)
                    {
                        m_ImageDataList.Add(new FontFileCharacter());
                    }
                }
            }
        }

        public Byte GetCharWidth(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Width;
        }

        public Byte GetCharHeight(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Height; 
        }

        public Byte GetCharYOffset(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].YOffset;
        }

        public FontFileCharacter[] GetAllRawData()
        {
            return this.m_ImageDataList.ToArray();
        }
        public FontFileCharacter GetRawData(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return null;
            return this.m_ImageDataList[index];
        }

        public Bitmap GetBitmap(Int32 index, Color[] colors, Boolean addTransparentZero)
        {
            if (index < 0 || index >= this.Length)
                return null;
            ColorPalette palette = ImageUtils.MakePalette(colors, GetPixelFormat(), addTransparentZero);
            return this.m_ImageDataList[index].GetBitmap(palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors, Boolean addTransparentZero)
        {
            Bitmap[] allChars = new Bitmap[this.Length];
            ColorPalette palette = ImageUtils.MakePalette(colors, GetPixelFormat(), addTransparentZero);
            for (Int32 i = 0; i < allChars.Length; i++)
                allChars[i] = this.m_ImageDataList[i].GetBitmap(palette);
            return allChars;
        }

        public void PaintPixel(Int32 index, Int32 x, Int32 y, Byte value)
        {
            if (index < 0 || index >= this.Length)
                throw new IndexOutOfRangeException("Bad character index '" + index + "'.");
            FontFileCharacter character = GetRawData(index);
            character.PaintPixel(x, y, value, this.GetPixelFormat());
        }

        public Byte[] WriteFntFile()
        {
            Int32 imagesCount = this.m_ImageDataList.Count;
            FontFileType fft = this.FontFileType;
            Boolean isTibSun = fft == FontFileType.WWTibSun;
            Byte[] fontDataOffsetsList = new Byte[imagesCount*2];
            Byte[][] imageData = new Byte[imagesCount][];
            Byte[] widthsList = new Byte[imagesCount];
            Byte[] heightsList = new Byte[imagesCount*2];
            // header + Int16 index + Byte heights
            Int32 offsetsListOffset = 0x14;
            Int32 widthsListOffset = offsetsListOffset + imagesCount * 2;
            Int32 heightsListOffset = 0;
            if (isTibSun)
                heightsListOffset = widthsListOffset + +imagesCount;

            Int32 fontOffsetStart = (!isTibSun) ? widthsListOffset + imagesCount : heightsListOffset + imagesCount * 2;
            for (Int32 i = 0; i < imagesCount; i++)
            {
                FontFileCharacter fc = this.m_ImageDataList[i];
                Byte[] imgData8bit = fc.ByteData;
                Byte imgWidth = fc.Width;
                Byte imgHeight = fc.Height;
                if (!isTibSun)
                {
                    Int32 stride = (imgWidth / 2) + (imgWidth % 2);
                    Int32 dubstride = stride * 2;
                    Byte[] imgData4bit = new Byte[stride*imgHeight];
                    for (Int32 y = 0; y < imgHeight; y++)
                    {
                        for (Int32 x = 0; x < dubstride; x += 2)
                        {
                            Int32 nybLo = (imgData8bit[y * imgWidth + x] & 0x0F);
                            Int32 nybHi = (x + 1) == imgWidth ? 0 : ((imgData8bit[y * imgWidth + x + 1] << 0x4) & 0xF0);
                            imgData4bit[y * stride + x / 2] = (Byte)(nybHi | nybLo);
                        }
                    }
                    imageData[i] = imgData4bit;
                }
                imageData[i] = imgData8bit.ToArray();
                widthsList[i] = imgWidth;
                heightsList[i * 2] = fc.YOffset;
                heightsList[i * 2 + 1] = imgHeight;
            }
            Int32[] refslist = CreateRefsList(imageData);
            Int32 fontOffset = isTibSun ? 0 : fontOffsetStart;
            for (Int32 i = 0; i < imagesCount; i++)
            {
                Int32 replacei = refslist[i];
                if (replacei == i)
                {
                    fontDataOffsetsList[i * 2] = (Byte)(fontOffset & 0xFF);
                    fontDataOffsetsList[i * 2 + 1] = (Byte)((fontOffset >> 8) & 0xFF);
                    fontOffset += imageData[i].Length;
                }
                else
                {
                    imageData[i] = new Byte[0];
                    fontDataOffsetsList[i * 2] = fontDataOffsetsList[replacei * 2];
                    fontDataOffsetsList[i * 2 + 1] = fontDataOffsetsList[replacei * 2 + 1];
                }
            }
            if (!isTibSun)
                heightsListOffset = fontOffset;
            Int32 fullLength = !isTibSun ? (heightsListOffset + imagesCount * 2) : (fontOffset + fontOffsetStart);
            Byte[] fullData = new Byte[fullLength];

            Byte unknown03 = (Byte)(isTibSun ? 0 : 5);
            Int16 unknown0E;
            if (fft == FontFileType.WWClassic1)
                unknown0E = 0x1011;
            else if (fft == FontFileType.WWClassicCnC)
                unknown0E = 0x1012;
            else
                unknown0E = 0;
            // write header
            fullData[0x00] = (Byte)(fullLength & 0xFF);         //Int16 FileSize, low byte;
            fullData[0x01] = (Byte)((fullLength >> 8) & 0xFF);  //Int16 FileSize, high byte;
            fullData[0x02] = (Byte)(isTibSun? 0x02 : 0x00);     // Byte DataFormat
            fullData[0x03] = unknown03;                         // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = (Byte)0x0e;                        // Int16 Unknown04, low byte; (always 0x0e)
            fullData[0x05] = (Byte)0x00;                        // Int16 Unknown04, high byte; (always 0x00)
            fullData[0x06] = (Byte)(offsetsListOffset & 0xFF);        // Int16 FontDataListOffset, low byte;
            fullData[0x07] = (Byte)((offsetsListOffset >> 8) & 0xFF); // Int16 FontDataListOffset, high byte;
            fullData[0x08] = (Byte)(widthsListOffset & 0xFF);         // Int16 WidthsListOffset, low byte
            fullData[0x09] = (Byte)((widthsListOffset >> 8) & 0xFF);  // Int16 WidthsListOffset, high byte
            fullData[0x0A] = (Byte)(fontOffsetStart & 0xFF);          // Int16 FontDataOffset, low byte
            fullData[0x0B] = (Byte)((fontOffsetStart >> 8) & 0xFF);   // Int16 FontDataOffset, high byte
            fullData[0x0C] = (Byte)(heightsListOffset & 0xFF);        // Int16 HeightsListOffset, low byte
            fullData[0x0D] = (Byte)((heightsListOffset >> 8) & 0xFF); // Int16 HeightsListOffset, high byte
            fullData[0x0E] = (Byte)(unknown0E & 0xFF);          // Int16 Unknown0E, low byte (0x11 for pre-C&C WW games?)
            fullData[0x0F] = (Byte)((unknown0E >> 8) & 0xFF);   // Int16 Unknown0E, high byte (always 0x10)
            fullData[0x10] = 0x00;                              // Byte AlwaysZero (Always 0x00)
            fullData[0x11] = (Byte)(isTibSun? 0 : imagesCount - 1); // Byte LastCharIndex (for non-TS)
            fullData[0x12] = m_FontHeight;                        // Byte FontHeight
            fullData[0x13] = m_FontWidth;                         // Byte FontWidth
            Array.Copy(fontDataOffsetsList, 0, fullData, offsetsListOffset, fontDataOffsetsList.Length);
            Array.Copy(widthsList, 0, fullData, widthsListOffset, widthsList.Length);
            Int32 imageDataOffs = fontOffsetStart;
            foreach (Byte[] charImgData in imageData)
            {
                if (charImgData.Length == 0)
                    continue;
                Array.Copy(charImgData, 0, fullData, imageDataOffs, charImgData.Length);
                imageDataOffs += charImgData.Length;
            }
            // at this point, heightsListOffset should equal imageDataOffs, and the next operation should exactly fill up the array.
            Array.Copy(heightsList, 0, fullData, heightsListOffset, heightsList.Length);
            // return data
            return fullData;
        }

        /// <summary>
        /// File size optimization. This function makes a map to re-map duplicate entries to the first found occurrence.
        /// In the final images array, any index not referencing itself is deemed a copy and will be removed in favour of the reference.
        /// </summary>
        /// <param name="imageData">Image data array</param>
        /// <returns></returns>
        private Int32[] CreateRefsList(Byte[][] imageData)
        {
            Int32 imagesCount = imageData.Length;
            Int32[] refsList = new Int32[imagesCount];
            for (Int32 checkedEntry = 0; checkedEntry < imagesCount; checkedEntry++)
            {
                for (Int32 dupetest = 0; dupetest < imagesCount; dupetest++)
                {
                    if (dupetest == checkedEntry || imageData[checkedEntry].SequenceEqual(imageData[dupetest]))
                    {
                        // reached the own index, or the data matches. Either way, set ref and continue with next one.
                        refsList[checkedEntry] = dupetest;
                        break;
                    }
                }
            }
            return refsList;
        }

        /// <summary>
        /// Gets the pixel format of the loaded font. The image handling internally is 8 bit,
        /// but this will restrict the size of the values that can be painted on the image.
        /// </summary>
        /// <returns>the pixel format of the loaded font.</returns>
        public PixelFormat GetPixelFormat()
        {
            switch(this.FontFileType)
            {
                case FontFileType.WWClassic1:
                case FontFileType.WWClassicCnC:
                    return PixelFormat.Format4bppIndexed;
                case FontFileType.WWTibSun:
                case FontFileType.EDDune2k:
                    return PixelFormat.Format8bppIndexed;
            }
            throw new NotSupportedException("Not supported!");
        }

        protected void LoadFromFileData(Byte[] fileData)
        {
            Int32 fileLength = fileData.Length;
            if (fileLength < 0x14)
                throw new Exception("File data too short enough to be a valid FNT file.");
                     
            Int16 fileSize = ArrayUtils.GetLEShortFromByteArray(fileData, 0x00);
            if (fileSize != fileLength)
            {
                // put checks for D2K file format here.
                // Will require UI updates though, to support a start index for the characters.
                // Though that could be automated by detecting empty entries if the game doesn't count on a specific start offset.
                throw new Exception("File size in header does not match file data!");
            }
            Byte dataFormat = fileData[0x02];
            //Byte unknown03 = fileData[0x03];
            //this.Unknown04 = ArrayUtils.GetLEShortFromByteArray(fileData, 0x04);
            Int16 fontDataOffsetsListOffset = ArrayUtils.GetLEShortFromByteArray(fileData, 0x06);
            Int16 widthsListOffset = ArrayUtils.GetLEShortFromByteArray(fileData, 0x08);
            // use this for pos on TS format
            Int16 fontDataOffset = ArrayUtils.GetLEShortFromByteArray(fileData, 0x0A);
            Int16 heightsListOffset = ArrayUtils.GetLEShortFromByteArray(fileData, 0x0C);
            Int16 unknown0E = ArrayUtils.GetLEShortFromByteArray(fileData, 0x0E);
            //Byte AlwaysZero = fileData[0x10];
            Byte lastIndex = fileData[0x11];
            this.m_FontHeight = fileData[0x12];
            this.m_FontWidth = fileData[0x13];

            Int32 length = lastIndex;
            Boolean isTibSun = dataFormat == 0x02;
            if (isTibSun)
            {
                this.FontFileType = FontFileType.WWTibSun;
                // isn't in the header? Calculate.
                Int32[] headerVals = new Int32[] { fontDataOffsetsListOffset, widthsListOffset, fontDataOffset, heightsListOffset }.OrderBy(n => n).Take(2).ToArray();
                Int32 divval = 1;
                if (headerVals[0] == fontDataOffsetsListOffset || headerVals[0] == heightsListOffset)
                    divval = 2;
                length = (headerVals[1] - headerVals[0]) / divval;
            }
            else
            {
                if (unknown0E == 0x1011)
                    this.FontFileType = FontFileType.WWClassic1;
                else
                    this.FontFileType = FontFileType.WWClassicCnC;
                length++;
            }
            if (dataFormat != 0x00 && !isTibSun)
                throw new NotImplementedException(String.Format("Unknown font type identifier, '{0}'.", dataFormat));
            if (fontDataOffsetsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for offsets list!");
            if (widthsListOffset + length > fileLength)
                throw new Exception("File data too short for character widths list starting from offset !");
            if (heightsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for character heights list!");
            //FontDataOffset
            Int32[] fontDataOffsetsList = new Int32[length];
            for (Int32 i = 0; i < length; i++)
                fontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, fontDataOffsetsListOffset + i * 2) + (isTibSun ? fontDataOffset: 0);
            List<Byte> widthsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[widthsListOffset + i];
                if (width > this.FontWidth)
                    throw new Exception(String.Format("Illegal value '{0}' in character widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                widthsList.Add(width);
            }
            List<Byte> yOffsetsList = new List<Byte>();
            List<Byte> heightsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                yOffsetsList.Add(fileData[heightsListOffset + i * 2]);
                Byte height = fileData[heightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new Exception(String.Format("Illegal value '{0}' in character heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                heightsList.Add(height);
            }
            this.m_ImageDataList = new List<FontFileCharacter>();
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte width = widthsList[i];
                Byte height = heightsList[i];
                Int32 stride = Image.GetPixelFormatSize(this.GetPixelFormat()) * width;
                stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
                Int32 size = stride * height;
                if (start + size > fileLength)
                    throw new Exception(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                Byte[] curData8bit = new Byte[width*height];
                if (!isTibSun)
                {
                    // Convert to 8-bit data. So much easier to edit the data as one byte per pixel.
                    Byte[] curData = new Byte[size];
                    Array.Copy(fileData, start, curData, 0, size);
                    for (Int32 y = 0; y < height; y++)
                    {
                        for (Int32 x = 0; x < width; x++)
                        {
                            Int32 index4bit = y * stride + x / 2;
                            Int32 index8bit = y * width + x;
                            if (x % 2 == 0)
                                curData8bit[index8bit] = (Byte)(curData[index4bit] & 0x0F);
                            else
                                curData8bit[index8bit] = (Byte)((curData[index4bit] & 0xF0) >> 4);
                        }
                    }
                }
                else
                {
                    Array.Copy(fileData, start, curData8bit, 0, size);
                }
                FontFileCharacter fc = new FontFileCharacter();
                fc.Width = width;
                fc.Height = height;
                fc.YOffset = yOffsetsList[i];
                fc.ByteData = curData8bit;
                this.m_ImageDataList.Add(fc);
            }
        }
    }

    public enum FontFileType
    {
        /// <summary>Legend of Kyrandia and other older games</summary>
        WWClassic1,
        /// <summary>command & Conquer, Red Alert, etc.</summary>
        WWClassicCnC,
        /// <summary>command & Conquer Tiberian Sun</summary>
        WWTibSun,
        /// <summary>Dune 2000</summary>
        EDDune2k
    }

}