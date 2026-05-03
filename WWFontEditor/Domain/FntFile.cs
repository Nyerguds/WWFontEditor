using ColorManipulation;
using System;
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
        public Int16 Unknown0E { get; private set; }          // [Unused?] Unknown entry (always 0x1011 or 0x1012{ get; private set; } 0x0000 in TS) (font version?)
        public Byte AlwaysZero { get; private set; }          // [Unused?] Align byte. Always 0x00
        public Byte LastIndex { get; private set; }           // Last (0-based) character index. Add 1 to get the amount of characters.
        public Byte FontHeight { get; private set; }          // Overall maximum font height.
        public Byte FontWidth { get; private set; }           // Overall maximum font width.

        private Int16[] m_FontDataOffsetsList;  // array with the positions of all font entries
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
        private PixelFormat GetPixelFormat()
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
            this.m_FontDataOffsetsList = new Int16[length];
            for (Int32 i = 0; i < length; i++)
                this.m_FontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, this.FontDataOffsetsListOffset + i * 2);
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
                Int32 start = this.m_FontDataOffsetsList[i];
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
}