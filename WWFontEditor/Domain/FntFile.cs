using ColorManipulation;
using System;
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
        public Byte LastIndex { get; private set; }       // Last (0-based) character index. Add 1 to get the amount of characters.
        public Byte FontHeight { get; private set; }          // Overall maximum font height.
        public Byte FontWidth { get; private set; }           // Overall maximum font width.

        private Int16[] FontDataOffsetsList;  // array with the positions of all font entries
        public Byte[] WidthsList;   //  array with the widths of all font entries
        public Byte[] HeightsList;  // array with the heights of all font entries
        public Byte[] OffsetYList;  // array with the vertical offsets of all font entries

        private Byte[][] ImageDataList;  // array with actual read font data

        public FntFile(Byte[] fileData)
        {
            LoadFromFileData(fileData);
        }

        public Bitmap GetBitmap(Int32 index, Color[] palette)
        {
            if(index < 0 || index > LastIndex)
                throw new ArgumentOutOfRangeException();

            ColorPalette pal = GeneratePalette(palette);

            PixelFormat pf = this.GetPixelFormat();
            Int32 width = WidthsList[index];
            Int32 height = HeightsList[index];
            if (width == 0 || height == 0)
                return null;
            Byte[] imageData = ImageDataList[index];
            if (imageData.Length == 0 || width == 0 | height == 0)
                return new Bitmap(FontWidth, FontHeight, pf);

            Int32 stride = Image.GetPixelFormatSize(pf) * width;
            stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
            if (height * stride != imageData.Length)
                throw new Exception("You dun goofed, programmer!");
            return ImageUtils.BuildImage(imageData, width, height, stride, pf, pal);
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
                return pal;
            }
            // generate greyscale palette. Ignore original value here.
            Double fraction = 256.0 / (Double)(palSize - 1);
            Int32 steps = 255 / (palSize - 1);
            for (Int32 i = 0; i < pal.Entries.Length; i++)
            {
                Int32 offs = i * 4;
                Byte grayval = (Byte)Math.Min(255, Math.Round((Double)i * steps, MidpointRounding.AwayFromZero));
                pal.Entries[i] = Color.FromArgb(255, grayval, grayval, grayval);
            }
            // make color 0 transparent
            pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;            
        }

        private PixelFormat GetPixelFormat()
        {
            if (this.DataFormat == 0)
                return PixelFormat.Format4bppIndexed;
            else if (this.DataFormat == 2)
                return PixelFormat.Format8bppIndexed;
            else throw new NotSupportedException("Not supported!");
        }

        public Bitmap[] GetAllBitmaps()
        {
            return new Bitmap[0];
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
            Int32 length = this.LastIndex + 1;

            if (this.FontDataOffsetsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for offsets list!");
            if (WidthsListOffset + length > fileLength)
                throw new Exception("File data too short for character widths list starting from offset !");
            if (HeightsListOffset + length * 2 > fileLength)
                throw new Exception("File data too short for character heights list!");
            //FontDataOffset
            this.FontDataOffsetsList = new Int16[length];
            for (Int32 i = 0; i < length; i++)
                this.FontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, this.FontDataOffsetsListOffset + i * 2);
            WidthsList = new Byte[length];
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[this.WidthsListOffset + i];
                if (width > this.FontWidth)
                    throw new Exception(String.Format("Illegal value '{0}' in character widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                this.WidthsList[i] = width;
            }
            OffsetYList = new Byte[length];
            HeightsList = new Byte[length];
            for (Int32 i = 0; i < length; i++)
            {
                this.OffsetYList[i] = fileData[this.HeightsListOffset + i * 2];
                Byte height = fileData[this.HeightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new Exception(String.Format("Illegal value '{0}' in character heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                this.HeightsList[i] = height;
            }
            ImageDataList = new Byte[length][];
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = this.FontDataOffsetsList[i];
                Int32 width = this.WidthsList[i];
                Int32 height = this.HeightsList[i];
                Int32 stride = Image.GetPixelFormatSize(this.GetPixelFormat()) * width;
                stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
                Int32 size = height * stride;
                if (start + size > fileLength)
                    throw new Exception(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                Byte[] curData = new Byte[size];
                Array.Copy(fileData, start, curData, 0, size);
                for (Int32 dataOffs = 0; dataOffs < curData.Length; dataOffs++)
                    curData[dataOffs] = (Byte)((curData[dataOffs] << 4) | (curData[dataOffs] >> 4));
                ImageDataList[i] = curData;
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