using ColorManipulation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace WWFontEditor.Domain
{
    public class FntFile
    {
        public Int16 FileSize { get; private set; }           // Size of the file
        public Byte DataFormat { get; private set; }          // Data format for the image data: 00 for 4-bit image data, 02 for 8-bit.
        public Byte Unknown03 { get; private set; }           // Unknown entry (0x05 in C&C/RA1, 0x00 in TS)
        public Int16 Unknown04 { get; private set; }          // [Unused?] Unknown entry (always 0x000e) (font version position?)
        public Int16 FontDataOffsetsListOffset { get; private set; } // Absolute offset of the start of FontDataList (Normally 0x14)
        public Int16 WidthsListOffset { get; private set; }   // Absolute offset of the start of WidthsList
        public Int16 FontDataOffset { get; private set; }     // [Unused?] Start of the actual font data? Should equals the first entry in FontDataList, but doesn't in TS.
        public Int16 HeightsListOffset { get; private set; }  // Absolute offset of the start of HeightsList
        public Int16 Unknown0E { get; private set; }          // [Unused?] Unknown entry (always 0x1011 or 0x1012; 0x0000 in TS) (font version?)
        public Byte AlwaysZero { get; private set; }          // [Unused?] Align byte. Always 0x00
        public Byte LastIndex { get; private set; }           // Last (0-based) character index. Add 1 to get the amount of characters.
        public Byte FontHeight { get; private set; }          // Overall maximum font height.
        public Byte FontWidth { get; private set; }           // Overall maximum font width.

                                                     private List<Byte> m_WidthsList;   //  array with the widths of all font entries
        private List<Byte> m_HeightsList;  // array with the heights of all font entries
        private List<Byte> m_OffsetYList;  // array with the vertical offsets of all font entries
        private List<Byte[]> ImageDataList;

        public Int32 Length { get { return LastIndex + 1; } }

        public Byte GetCharWidth(Int32 index)
        {
            return this.m_WidthsList[index];
        }

        public Byte GetCharHeight(Int32 index)
        {
            return this.m_HeightsList[index]; 
        }

        public Byte GetCharYOffset(Int32 index)
        {
            return this.m_OffsetYList[index];
        }

        public FntFile(Byte[] fileData)
        {
            LoadFromFileData(fileData);
        }

        public Bitmap GetBitmap(Int32 index, Color[] colors)
        {
            if (index < 0 || index > LastIndex)
                throw new ArgumentOutOfRangeException();
            ColorPalette palette = GeneratePalette(colors);
            return GetBitmap(index, palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors)
        {
            ColorPalette palette = GeneratePalette(colors);
            Bitmap[] allChars = new Bitmap[this.Length];
            for (Int32 i = 0; i < allChars.Length; i++)
                allChars[i] = GetBitmap(i, palette);
            return allChars;
        }

        public void PaintPixel(Int32 index, Int32 x, Int32 y, Byte value)
        {
            if (index < 0 || index > LastIndex)
                throw new IndexOutOfRangeException("Bad character index '" + index + "'.");
            Byte chWidth = GetCharWidth(index);
            Byte chHeight = this.GetCharHeight(index);
            if (x < 0 || x >= chWidth || y < 0 || y >= chHeight)
                return; // Ignore. without error. Might accidentally occur when dragging or something I guess.
            Int32 pxf = Image.GetPixelFormatSize(this.GetPixelFormat());
            Int32 maxSize = (Int32)Math.Pow(2, pxf);
            if (maxSize <= value)
                throw new IndexOutOfRangeException("Byte value too large for " + pxf + " bit image!");
            ImageDataList[index][y * chWidth + x] = value;
        }

        public Byte[] WriteFntFile()
        {
            Int32 imagesCount = this.ImageDataList.Count;
            Byte[] fontDataOffsetsList = new Byte[imagesCount*2];
            Byte[][] imageData = new Byte[imagesCount][];
            Byte[] widthsList = new Byte[imagesCount];
            Byte[] heightsList = new Byte[imagesCount*2];
            // header + Int16 index + Byte heights
            Int32 offsetsListOffset = 0x14;
            Int32 widthListOffset = offsetsListOffset + imagesCount * 2;
            Int32 fontOffsetStart = widthListOffset + imagesCount;
            for (Int32 i = 0; i < imagesCount; i++)
            {
                Byte[] imgData8bit = this.ImageDataList[i];
                Byte imgWidth = GetCharWidth(i);
                Byte imgHeight = GetCharHeight(i);
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
                widthsList[i] = imgWidth;
                heightsList[i * 2] = this.GetCharYOffset(i);
                heightsList[i * 2 + 1] = imgHeight;
            }
            Int32[] refslist = CreateRefsList(imageData);
            Int32 fontOffset = fontOffsetStart;
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
            Int32 heightsListOffset = fontOffset;
            Int32 fullLength = heightsListOffset + imagesCount * 2;
            Byte[] fullData = new Byte[fullLength];
            // write header
            fullData[0x00] = (Byte)(fullLength & 0xFF);         //Int16 FileSize, byte 1;
            fullData[0x01] = (Byte)((fullLength >> 8) & 0xFF);  //Int16 FileSize, byte 2;
            fullData[0x02] = 0x00;                              // Byte DataFormat
            fullData[0x03] = Unknown03;                         // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = (Byte)(Unknown04 & 0xFF);          // Int16 Unknown04, byte 1; (always 0x0e)
            fullData[0x05] = (Byte)((Unknown04 >> 8) & 0xFF);   // Int16 Unknown04, byte 2; (always 0x00)
            fullData[0x06] = (Byte)(offsetsListOffset & 0xFF);        // Int16 FontDataListOffset, byte 1;
            fullData[0x07] = (Byte)((offsetsListOffset >> 8) & 0xFF); // Int16 FontDataListOffset, byte 2;
            fullData[0x08] = (Byte)(widthListOffset & 0xFF);          // Int16 WidthsListOffset, byte 1
            fullData[0x09] = (Byte)((widthListOffset >> 8) & 0xFF);   // Int16 WidthsListOffset, byte 2
            fullData[0x0A] = fontDataOffsetsList[0];            // Int16 FontDataOffset, byte 1
            fullData[0x0B] = fontDataOffsetsList[1];             // Int16 FontDataOffset, byte 2
            fullData[0x0C] = (Byte)(heightsListOffset & 0xFF);        // Int16 HeightsListOffset, byte 1
            fullData[0x0D] = (Byte)((heightsListOffset >> 8) & 0xFF); // Int16 HeightsListOffset, byte 2
            fullData[0x0E] = (Byte)(Unknown0E & 0xFF);          // Int16 Unknown0E, byte 1 (0x11 for pre-C&C WW games?)
            fullData[0x0F] = (Byte)((Unknown0E >> 8) & 0xFF);   // Int16 Unknown0E, byte 2 (always 0x10)
            fullData[0x10] = 0x00;                              // Byte AlwaysZero (Always 0x00)
            fullData[0x11] = (Byte)(imagesCount - 1);           // Byte LastCharIndex
            fullData[0x12] = FontHeight;                        // Byte FontHeight
            fullData[0x13] = FontWidth;                         // Byte FontWidth
            Array.Copy(fontDataOffsetsList, 0, fullData, offsetsListOffset, fontDataOffsetsList.Length);
            Array.Copy(widthsList, 0, fullData, widthListOffset, widthsList.Length);
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

        protected Bitmap GetBitmap(Int32 index, ColorPalette palette)
        {
            if(index < 0 || index > LastIndex)
                throw new ArgumentOutOfRangeException();
            PixelFormat pf = PixelFormat.Format8bppIndexed;
            Int32 width = m_WidthsList[index];
            Int32 height = m_HeightsList[index];
            if (width == 0 || height == 0)
                return null;
            Byte[] imageData = ImageDataList[index];
            if (imageData.Length == 0 || width == 0 | height == 0)
                return new Bitmap(FontWidth, FontHeight, pf);
            return ImageUtils.BuildImage(imageData, width, height, width, pf, palette);
        }

        private ColorPalette GeneratePalette(Color[] sourcePalette)
        {
            Int32 palSize = (Int32)Math.Pow(2, Image.GetPixelFormatSize(this.GetPixelFormat()));
            ColorPalette pal = new Bitmap(10, 10, GetPixelFormat()).Palette;
            if (sourcePalette != null)
            {
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    if (i < sourcePalette.Length)
                        pal.Entries[i] = sourcePalette[i];
                    else
                        pal.Entries[i] = Color.Empty;
                }
            }
            else
            {
                // generate greyscale palette.
                Int32 steps = 255 / (palSize - 1);
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    Byte grayval = (Byte)Math.Min(255, Math.Round((Double)i * steps, MidpointRounding.AwayFromZero));
                    pal.Entries[i] = Color.FromArgb(255, grayval, grayval, grayval);
                }
            }
            // make color 0 transparent
            pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;            
        }

        /// <summary>
        /// Gets the pixel format of the loaded font. The image handling internally is 8 bit,
        /// but this will restrict the size of the values that can be painted on the image.
        /// </summary>
        /// <returns>the pixel format of the loaded font.</returns>
        public PixelFormat GetPixelFormat()
        {
            if (this.DataFormat == 0)
                return PixelFormat.Format4bppIndexed;
            if (this.DataFormat == 2)
                return PixelFormat.Format8bppIndexed;
            throw new NotSupportedException("Not supported!");
        }

        protected void LoadFromFileData(Byte[] fileData)
        {
            Int32 fileLength = fileData.Length;
            if (fileLength < 0x14)
                throw new Exception("File data too short enough to be a valid FNT file.");
            ReadHeader(fileData);
            if (this.FileSize != fileLength)
                throw new Exception("File size in header does not match file data!");
            if (this.DataFormat != 0x00)
                throw new NotImplementedException(String.Format("Font type {0} is not supported.", this.DataFormat));
            Int32 length = this.Length;
            if (this.FontDataOffsetsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for offsets list!");
            if (WidthsListOffset + length > fileLength)
                throw new Exception("File data too short for character widths list starting from offset !");
            if (HeightsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for character heights list!");
            //FontDataOffset
            Int16[] fontDataOffsetsList = new Int16[length];
            for (Int32 i = 0; i < length; i++)
                fontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, this.FontDataOffsetsListOffset + i * 2);
            m_WidthsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[this.WidthsListOffset + i];
                if (width > this.FontWidth)
                    throw new Exception(String.Format("Illegal value '{0}' in character widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                this.m_WidthsList.Add(width);
            }
            m_OffsetYList = new List<Byte>();
            m_HeightsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                this.m_OffsetYList.Add(fileData[this.HeightsListOffset + i * 2]);
                Byte height = fileData[this.HeightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new Exception(String.Format("Illegal value '{0}' in character heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                this.m_HeightsList.Add(height);
            }
            ImageDataList = new List<Byte[]>();
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Int32 width = this.m_WidthsList[i];
                Int32 height = this.m_HeightsList[i];
                Int32 stride = Image.GetPixelFormatSize(this.GetPixelFormat()) * width;
                stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
                Int32 size = height * stride;
                if (start + size > fileLength)
                    throw new Exception(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                Byte[] curData = new Byte[size];
                // Convert to 8-bit data. So much easier to edit the data as one byte per pixel.
                Byte[] curData8bit = new Byte[width*height];
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
                ImageDataList.Add(curData8bit);
            }
        }

        protected void ReadHeader(Byte[] headerBytes)
        {
            if (headerBytes.Length < 0x14)
                return;
            this.FileSize = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x00);
            this.DataFormat = headerBytes[0x02];
            this.Unknown03 = headerBytes[0x03];
            this.Unknown04 = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x04);
            this.FontDataOffsetsListOffset = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x06);
            this.WidthsListOffset = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x08);
            this.FontDataOffset = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x0A);
            this.HeightsListOffset = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x0C);
            this.Unknown0E = ArrayUtils.GetLEShortFromByteArray(headerBytes, 0x0E);
            this.AlwaysZero = headerBytes[0x10];
            this.LastIndex = headerBytes[0x11];
            this.FontHeight = headerBytes[0x12];
            this.FontWidth = headerBytes[0x13];
        }

    }

    public enum FntFileVersion
    {
        Kyrandia,
        CnC
    }
}