using ColorManipulation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using WWFontEditor.Domain.FontTypes;

namespace WWFontEditor.Domain
{
    public abstract class FontFile
    {                              
        protected const String ERR_NOHEADER = "File data too short to contain header.";
        protected const String ERR_SIZECHECK = "File size value in header does not match file data length.";

        /// <summary>Lower limit for the amount of characters in the font.</summary>
        public virtual Int32 CharactersMin { get {return 0;} }
        /// <summary>Upper limit for the amount of characters in the font.</summary>
        public abstract Int32 CharactersMax { get; }
        /// <summary>Lower limit for the overall width of the characters in the font.</summary>
        public virtual Int32 FontWidthMin { get { return 0; } }
        /// <summary>Upper limit for the overall width of the characters in the font.</summary>
        public abstract Int32 FontWidthMax { get; }
        /// <summary>Lower limit for the overall height of the characters in the font.</summary>
        public virtual Int32 FontHeightMin { get { return 0; } }
        /// <summary>Upper limit for the overall height of the characters in the font.</summary>
        public abstract Int32 FontHeightMax { get; }
        /// <summary>Upper limit for the Y-offset of the characters in the font. Zero means the font format does not support Y offsets</summary>
        public abstract Int32 YOffsetMax { get; }
        /// <summary>Bits per pixel of the data in this font.</summary>
        public abstract Int32 BitsPerPixel { get; }
        /// <summary></summary>
        public abstract String ShortTypeCode { get; }
        public abstract String LongTypeCode { get; }
        public abstract String[] GamesList { get; }

        /// <summary>
        /// Loads the font from file data. Throws a LoadFailedException if the format is not recognised. Might throw other exceptions if the actual load failed after validation.
        /// </summary>
        /// <param name="fileData">The file data to read the font from</param>
        /// <returns>False if the font was not identified as this type.</returns>
        public abstract void LoadFont(Byte[] fileData);
        public abstract Byte[] SaveFont();
        
        protected Int32 m_FontHeight;
        /// <summary>Overall maximum font height.</summary>
        public Int32 FontHeight
        {
            get { return m_FontHeight; }
            set
            {
                this.m_FontHeight = Math.Min(value, this.FontHeightMax);
                foreach (FontFileCharacter fontchar in this.m_ImageDataList)
                    if (fontchar.Height > value)
                        fontchar.ChangeHeight(value);
            }
        }

        protected Int32 m_FontWidth;
        /// <summary>Overall maximum font width.</summary>
        public Int32 FontWidth
        {
            get { return m_FontWidth; }
            set
            {
                this.m_FontWidth = Math.Min(value, this.FontWidthMax);
                foreach (FontFileCharacter fontchar in this.m_ImageDataList)
                    if (fontchar.Width > value)
                        fontchar.ChangeWidth(value);
            }
        }

        /// <summary>Ordered from complex to simple to prevent false positives.</summary>
        protected static Type[] AutoDetectTypes =
        {
            typeof (FontFileV4),
            typeof (FontFileV3),
            typeof (FontFileV3_1),
            typeof (FontFileV2),
            // V0's "check" is file size only; ALWAYS leave it last.
            typeof (FontFileV1),
        };



        /// <summary> array with the actual image data (as 8-bit) as byte arrays</summary>
        protected List<FontFileCharacter> m_ImageDataList = new List<FontFileCharacter>();

        /// <summary>
        /// Attempts to load the given data as one of the known font types.
        /// </summary>
        /// <param name="fileData">File data</param>
        /// <param name="loadErrors">Load errors detailing failed attempts at identification.</param>
        /// <returns></returns>
        public static FontFile LoadFontFile(Byte[] fileData, out List<LoadFailedException> loadErrors)
        {
            loadErrors = new List<LoadFailedException>();
            //List<Exception> processErrors = new List<Exception>();
            foreach (Type type in AutoDetectTypes)
            {
                FontFile fontInstance = null;
                try
                {
                    fontInstance = (FontFile)Activator.CreateInstance(type);
                }
                catch { /* Ignore; programmer error. */ }
                if (fontInstance == null)
                    continue;
                try
                {
                    fontInstance.LoadFont(fileData);
                    return fontInstance;
                }
                catch (LoadFailedException e)
                {
                    e.AttemptedLoadedType = fontInstance.ShortTypeCode;
                    loadErrors.Add(e);
                }
                /*/
                // Let this one slip; catch on UI level.
                catch (Exception e)
                {
                    processErrors.Add(e);
                }
                //-*/
            }
            return null;
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
            Int32 backupBpp = fontchar.BitsPerPixel;
            Int32 thisBpp = this.BitsPerPixel;
            if (backupBpp > thisBpp)
            {
                Int32 colValLimit = (Int32)Math.Pow(2, thisBpp);
                if (fontchar.ByteData.Any(x => x >= colValLimit))
                    throw new InvalidOperationException(String.Format("Cannot insert a {0} bit per pixel image into a {1} bit per pixel font.", backupBpp, thisBpp));
            }
            if (fontchar.Height > this.FontHeight)
                fontchar.ChangeHeight(this.FontHeight);
            if (fontchar.Width > this.FontWidth)
                fontchar.ChangeWidth(this.FontWidth);
            if (this.YOffsetMax == 0 && fontchar.Height + fontchar.YOffset < this.FontHeight)
            {
                // No Y support: hardcode the Y by shifting down the image to its Y value.
                for (int i = 0; i < fontchar.YOffset; i++)
                    fontchar.ShiftImageData(ShiftDirection.Down, false);
            }
            fontchar.YOffset = Math.Min(this.YOffsetMax, fontchar.YOffset);
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
                        m_ImageDataList.Add(new FontFileCharacter(this.BitsPerPixel));
                    }
                }
            }
        }

        public Int32 GetCharWidth(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Width;
        }

        public Int32 GetCharHeight(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Height;
        }

        public Int32 GetCharYOffset(Int32 index)
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
            ColorPalette palette = ImageUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero);
            return this.m_ImageDataList[index].GetBitmap(palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors, Boolean addTransparentZero)
        {
            Bitmap[] allChars = new Bitmap[this.Length];
            ColorPalette palette = ImageUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero);
            for (Int32 i = 0; i < allChars.Length; i++)
                allChars[i] = this.m_ImageDataList[i].GetBitmap(palette);
            return allChars;
        }

        public void PaintPixel(Int32 index, Int32 x, Int32 y, Byte value)
        {
            if (index < 0 || index >= this.Length)
                throw new IndexOutOfRangeException("Bad character index '" + index + "'.");
            FontFileCharacter character = GetRawData(index);
            character.PaintPixel(x, y, value);
        }
        
        protected Byte[] WriteFntFileV3V4(FontFileVersion fontver)
        {
            Int32 imagesCount = this.m_ImageDataList.Count;
            Boolean isTibSun = fontver == FontFileVersion.WW_V4;
            Byte[][] imageData = new Byte[imagesCount][];
            Byte[] widthsList = new Byte[imagesCount];
            Byte[] heightsList = new Byte[imagesCount * 2];
            // header + Int16 index + Byte heights
            Int32 offsetsListOffset = 0x14;
            Int32 widthsListOffset = offsetsListOffset + imagesCount * 2;
            Int32 heightsListOffset = 0;
            // V3 (TS) has its Y/height list before the image data.
            if (isTibSun)
                heightsListOffset = widthsListOffset + +imagesCount;
            Int32 fontOffsetStart = (!isTibSun) ? widthsListOffset + imagesCount : heightsListOffset + imagesCount * 2;
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < imagesCount; i++)
            {
                FontFileCharacter fc = this.m_ImageDataList[i];
                Byte[] imgData8bit = fc.ByteData;
                Byte imgWidth = (Byte)fc.Width;
                Byte imgHeight = (Byte)fc.Height;
                // Small optimization; no need to go converting the TS stuff; it doesn't change.
                if (bitsLength < 8)
                    imageData[i] = ConvertFrom8Bit(imgData8bit, imgWidth, imgHeight, bitsLength, false);
                else
                    imageData[i] = imgData8bit.ToArray();
                widthsList[i] = imgWidth;
                heightsList[i * 2] = (Byte)fc.YOffset;
                heightsList[i * 2 + 1] = imgHeight;
            }
            Int32 fontOffset = isTibSun ? 0 : fontOffsetStart;
            Byte[] fontDataOffsetsList = this.OptimizeImagesList(imageData, ref fontOffset);
            // V2 (C&C) has its Y/height list before the image data.
            if (!isTibSun)
                heightsListOffset = fontOffset;
            Int32 fullLength = !isTibSun ? (heightsListOffset + imagesCount * 2) : (fontOffset + fontOffsetStart);
            Byte[] fullData = new Byte[fullLength];

            Byte unknown03 = (Byte)(isTibSun ? 0 : 5);
            Int16 unknown0E;
            if (fontver == FontFileVersion.WW_V3_1)
                unknown0E = 0x1011;
            else if (fontver == FontFileVersion.WW_V3)
                unknown0E = 0x1012;
            else // V3
                unknown0E = 0;

            // write header
            fullData[0x00] = (Byte)(fullLength & 0xFF);         //Int16 FileSize, low byte;
            fullData[0x01] = (Byte)((fullLength >> 8) & 0xFF);  //Int16 FileSize, high byte;
            fullData[0x02] = (Byte)(isTibSun ? 0x02 : 0x00);    // Byte DataFormat
            fullData[0x03] = unknown03;                         // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = 0x0e;                              // Int16 Unknown04, low byte; (always 0x0e)
            fullData[0x05] = 0x00;                              // Int16 Unknown04, high byte; (always 0x00)
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
            fullData[0x11] = (Byte)(isTibSun ? 0 : imagesCount - 1);  // Byte LastCharIndex (for non-TS)
            fullData[0x12] = (Byte)m_FontHeight;                // Byte FontHeight
            fullData[0x13] = (Byte)m_FontWidth;                 // Byte FontWidth
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
        ///     Optimizes the image data list, and returns a list of reference addresses, starting from the given fontOffset.
        ///     After the procedure, fontOffset will have the address behind the last data to write.
        /// </summary>
        /// <param name="imageData">Image data. Duplicate arrays in this are set to 0-sized ones.</param>
        /// <param name="fontOffset">Start offset of the addressing.</param>
        /// <returns></returns>
        protected Byte[] OptimizeImagesList(Byte[][] imageData, ref Int32 fontOffset)
        {
            Int32[] refslist = CreateRefsList(imageData);
            Byte[] fontDataOffsetsList = new Byte[imageData.Length * 2];
            for (Int32 i = 0; i < imageData.Length; i++)
            {
                Int32 replacei = refslist[i];
                if (imageData[i].Length == 0)
                {
                    // Data is null: just write 0
                    fontDataOffsetsList[i * 2] = 0;
                    fontDataOffsetsList[i * 2 + 1] = 0;
                }
                else if (replacei == i)
                {
                    // Data is not null and not a duplicate: write offset and advance offset ptr.
                    fontDataOffsetsList[i * 2] = (Byte)(fontOffset & 0xFF);
                    fontDataOffsetsList[i * 2 + 1] = (Byte)((fontOffset >> 8) & 0xFF);
                    fontOffset += imageData[i].Length;
                }
                else
                {
                    // Data is duplicate: clear data and copy previously written offset.
                    imageData[i] = new Byte[0];
                    fontDataOffsetsList[i * 2] = fontDataOffsetsList[replacei * 2];
                    fontDataOffsetsList[i * 2 + 1] = fontDataOffsetsList[replacei * 2 + 1];
                }
            }
            return fontDataOffsetsList;
        }

        /// <summary>
        /// File size optimization. This function makes a map to re-map duplicate entries to the first found occurrence.
        /// In the final images array, any index not referencing itself is deemed a copy and should be removed in favour of the reference.
        /// </summary>
        /// <param name="imageData">Image data array</param>
        /// <returns></returns>
        protected Int32[] CreateRefsList(Byte[][] imageData)
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
        protected void LoadFromFileData(Byte[] fileData, FontFileVersion checkType)
        {
            Int32 fileLength = fileData.Length;
            if (fileLength < 0x14)
                throw new LoadFailedException(ERR_NOHEADER);
            Int16 fileSize = ArrayUtils.GetLEShortFromByteArray(fileData, 0x00);
            if (fileSize != fileLength)
                throw new LoadFailedException(ERR_SIZECHECK);
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
            Boolean isV3 = dataFormat == 0x02;
            if (isV3)
            {
                if (checkType != FontFileVersion.WW_V4)
                    throw new LoadFailedException("Load type identifies as V3.");
                // isn't in the header? Calculate.
                Int32[] headerVals = new Int32[] {fontDataOffsetsListOffset, widthsListOffset, fontDataOffset, heightsListOffset}.OrderBy(n => n).Take(2).ToArray();
                Int32 divval = 1;
                if (headerVals[0] == fontDataOffsetsListOffset || headerVals[0] == heightsListOffset)
                    divval = 2;
                length = (headerVals[1] - headerVals[0]) / divval;
            }
            else if (dataFormat == 0x00)
            {
                if (unknown0E == 0x1011)
                {
                    if (checkType != FontFileVersion.WW_V3_1)
                        throw new LoadFailedException("Load identifies as V2.1.");
                }
                else if (unknown0E == 0x1012)
                {
                    if (checkType != FontFileVersion.WW_V3)
                        throw new LoadFailedException("Load identifies as V2.2.");
                }
                // else... just let it pass. It'll come out as 2.2.

                length++;
            }
            else
                throw new LoadFailedException(String.Format("Unknown font type identifier, '{0}'.", dataFormat));
            if (fontDataOffsetsListOffset + length * 2 > fileLength)
                throw new LoadFailedException("File data too short for offsets list!");
            if (widthsListOffset + length > fileLength)
                throw new LoadFailedException("File data too short for character widths list starting from offset !");
            if (heightsListOffset + length * 2 > fileLength)
                throw new LoadFailedException("File data too short for character heights list!");

            //FontDataOffset
            Int32[] fontDataOffsetsList = new Int32[length];
            for (Int32 i = 0; i < length; i++)
                fontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, fontDataOffsetsListOffset + i * 2) + (isV3 ? fontDataOffset : 0);
            List<Byte> widthsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[widthsListOffset + i];
                if (width > this.FontWidth)
                    throw new LoadFailedException(String.Format("Illegal value '{0}' in character widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                widthsList.Add(width);
            }
            List<Byte> yOffsetsList = new List<Byte>();
            List<Byte> heightsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                yOffsetsList.Add(fileData[heightsListOffset + i * 2]);
                Byte height = fileData[heightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new LoadFailedException(String.Format("Illegal value '{0}' in character heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                heightsList.Add(height);
            }
            // End of LoadFailedExceptions. After this, assume the type is identified.
            this.m_ImageDataList = new List<FontFileCharacter>();
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte width = widthsList[i];
                Byte height = heightsList[i];
                Byte[] data8Bit = this.ConvertTo8Bit(fileData, width, height, start, bitsLength, i, false);
                FontFileCharacter fc = new FontFileCharacter(data8Bit, width, height, yOffsetsList[i], bitsLength);
                this.m_ImageDataList.Add(fc);
            }
        }

        protected Byte[] ConvertTo8Bit(Byte[] fileData, Int32 width, Int32 height, Int32 start, Int32 bitsLength, Int32 index, Boolean bigEndian)
        {
            // Full array
            Byte[] data8bit = new Byte[width * height];
            // Amount of runs that end up on the same pixel
            Int32 parts = 8 / bitsLength;
            // Amount of bytes to read per width
            Int32 stride = bitsLength * width;
            stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
            // Bit mask for reducing read and shifted data to actual bits length
            Int32 bitmask = (Int32)Math.Pow(2, bitsLength) - 1;
            Int32 size = stride * height;
            // File check, and getting actual data.
            if (start + size > fileData.Length)
                throw new Exception(String.Format("Data for font entry #{0} exceeds file bounds!", index));
            Byte[] curData = new Byte[size];
            Array.Copy(fileData, start, curData, 0, size);
            // Actual conversion porcess.
            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexXbit = y * stride + x / parts;
                    // This will always get a new index
                    Int32 index8bit = y * width + x;
                    // Amount of bits to shift the data to get to the current pixel data
                    Int32 shift = (x % parts) * bitsLength;
                    // Reversed for big-endian
                    if (bigEndian)
                        shift = parts - 1 - shift;
                    // Get data and store it.
                    data8bit[index8bit] = (Byte)((curData[indexXbit] >> shift) & bitmask);
                }
            }
            return data8bit;
        }

        protected Byte[] ConvertFrom8Bit(Byte[] data8bit, Int32 width, Int32 height, Int32 bitsLength, Boolean bigEndian)
        {
            Int32 parts = 8 / bitsLength;
            // Amount of bytes to write per width
            Int32 stride = bitsLength * width;
            stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
            // Bit mask for reducing original data to actual bits maximum.
            // Should not be needed if data is correct, but eh.
            Int32 bitmask = (Int32)Math.Pow(2, bitsLength) - 1;
            Byte[] dataXbit = new Byte[stride * height];
            // Actual conversion porcess.
            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexXbit = y * stride + x / parts;
                    // This will always get a new index
                    Int32 index8bit = y * width + x;
                    // Amount of bits to shift the data to get to the current pixel data
                    Int32 shift = (x % parts) * bitsLength;
                    // Reversed for big-endian
                    if (bigEndian)
                        shift = parts - 1 - shift;
                    // Get data, reduce to bit rate, shift it and store it.
                    dataXbit[indexXbit] |= (Byte)((data8bit[index8bit] & bitmask) << shift);
                }
            }
            return dataXbit;
        }

        /// <summary>
        /// Basic FontFile contains the reading and writing implementation of V2 and V3 because they are similar,
        /// and the originally supported type. This enum distinguishes between them inside common operations.
        /// </summary>
        protected enum FontFileVersion
        {
            /// <summary>Legend of Kyrandia</summary>
            WW_V3_1,
            /// <summary>Dune II, Lands of Lore, Command & Conquer, Red Alert, etc.</summary>
            WW_V3,
            /// <summary>Tiberian Sun</summary>
            WW_V4,
        }
    }

    /// <summary>Load failed exceptions. These are typically ignored in favour of checking the next type to try.</summary>
    public class LoadFailedException: Exception
    {
        public String AttemptedLoadedType { get; set; }

        public LoadFailedException() { }
        public LoadFailedException(string message) : base(message) { }
        public LoadFailedException(string message, Exception innerException): base(message, innerException) {}
        public LoadFailedException(string message, String attemptedLoadedType) : base(message)
        {
            this.AttemptedLoadedType = attemptedLoadedType;
        }
        public LoadFailedException(string message, String attemptedLoadedType, Exception innerException)
            : base(message, innerException)
        {
            this.AttemptedLoadedType = attemptedLoadedType;
        }
    }

}