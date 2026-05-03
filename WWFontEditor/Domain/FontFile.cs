using ColorManipulation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using WWFontEditor.Domain.FontTypes;
using System.Text;
using System.Drawing.Drawing2D;

namespace WWFontEditor.Domain
{
    public abstract class FontFile
    {                              
        protected const String ERR_NOHEADER = "File data too short to contain header.";
        protected const String ERR_SIZECHECK = "File size value in header does not match file data length.";

        #region protected variables
        /// <summary>Overall maximum font height.</summary>
        protected Int32 m_FontHeight;
        /// <summary>Overall maximum font width.</summary>
        protected Int32 m_FontWidth;

        /// <summary> array with the actual image data (as 8-bit) as byte arrays</summary>
        protected List<FontFileSymbol> m_ImageDataList = new List<FontFileSymbol>();
        #endregion

        #region overridable properties and functions
        /// <summary>Lower limit for the amount of symbols in the font.</summary>
        public virtual Int32 SymbolsTypeMin { get {return 0;} }
        /// <summary>Upper limit for the amount of symbols in the font.</summary>
        public abstract Int32 SymbolsTypeMax { get; }
        /// <summary>Lower limit for the overall width of the symbols in the font.</summary>
        public virtual Int32 FontWidthTypeMin { get { return 0; } }
        /// <summary>Upper limit for the overall width of the symbols in the font.</summary>
        public abstract Int32 FontWidthTypeMax { get; }
        /// <summary>Lower limit for the overall height of the symbols in the font.</summary>
        public virtual Int32 FontHeightTypeMin { get { return 0; } }
        /// <summary>Upper limit for the overall height of the symbols in the font.</summary>
        public abstract Int32 FontHeightTypeMax { get; }
        /// <summary>Upper limit for the Y-offset of the symbols in the font. Zero means the font format does not support Y offsets</summary>
        public abstract Int32 YOffsetTypeMax { get; }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font. Automatically disables if max and min for both dimensions are the same.</summary>
        public virtual Boolean CustomSymbSizesForType { get { return this.FontHeightTypeMin != this.FontHeightTypeMax || this.FontWidthTypeMin != this.FontWidthTypeMax; } }
        /// <summary>Bits per pixel of the data in this font.</summary>
        public abstract Int32 BitsPerPixel { get; }
        /// <summary>File extension typically used for this font type.</summary>
        public virtual String FileExtension { get { return "fnt"; } }
        /// <summary>Very short code name for this font type.</summary>
        public abstract String ShortTypeName { get; }
        /// <summary>Brief name and description of the font type, for the file types dropdown in the open file dialog.</summary>
        public abstract String ShortTypeDescription { get; }
        /// <summary>Longer description of the font format.</summary>
        public abstract String LongTypeDescription { get; }
        /// <summary>List of games and other programs this font type is used by.</summary>
        public abstract String[] GamesListForType { get; }

        /// <summary>
        /// Loads the font from file data. Throws a LoadFailedException if the format is not recognised. Might throw other exceptions if the actual load failed after validation.
        /// </summary>
        /// <param name="fileData">The file data to read the font from.</param>
        /// <param name="fromAutoDetect">From autodetect; means stricter failure conditions. Always fail if not detected as exactly this type.</param>
        /// <returns>False if the font was not identified as this type.</returns>
        public abstract void LoadFont(Byte[] fileData, Boolean fromAutoDetect);

        /// <summary>
        /// Saves the font data to a byte array and returns it.
        /// </summary>
        /// <returns>The font data to be written to disk.</returns>
        public abstract Byte[] SaveFont();
        #endregion

        #region General functions and properties
        /// <summary>Adjustable maximum height of the loaded font.</summary>
        public Int32 FontHeight
        {
            get { return m_FontHeight; }
            set
            {
                this.m_FontHeight = Math.Min(value, this.FontHeightTypeMax);
                foreach (FontFileSymbol symbol in this.m_ImageDataList)
                    if (symbol.Height > value || !this.CustomSymbSizesForType)
                        symbol.ChangeHeight(value);
            }
        }

        /// <summary>Adjustable maximum width of the loaded font.</summary>
        public Int32 FontWidth
        {
            get { return m_FontWidth; }
            set
            {
                this.m_FontWidth = Math.Min(value, this.FontWidthTypeMax);
                foreach (FontFileSymbol symbol in this.m_ImageDataList)
                    if (symbol.Width > value || !this.CustomSymbSizesForType)
                        symbol.ChangeWidth(value);
            }
        }

        /// <summary>Amound of symbols in the font.</summary>
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
                        m_ImageDataList.Add(new FontFileSymbol(this.BitsPerPixel));
                    }
                }
            }
        }
        
        /// <summary>All supported types lined up for autodetection. Ordered from complex to simple to prevent false positives.</summary>
        public static Type[] AutoDetectTypes =
        {
            typeof (FontFileV4),
            typeof (FontFileV3),
            //typeof (FontFileV3_1),
            typeof (FontFileV2),
            // V0's "check" is file size only; leave it last.
            typeof (FontFileV1),
            // Can safely be put behind V0, since its size is always more than V1's fixed size.
            typeof (FontFileD2K),
        };

        public static FontFile[] GetAutoDetectTypeInstances()
        {
            List<FontFile> fonttypes = new List<FontFile>();
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
                fonttypes.Add(fontInstance);
            }
            return fonttypes.ToArray();
        }

        /// <summary>
        /// Attempts to load the given data as one of the known font types.
        /// </summary>
        /// <param name="AutoDetectTypes">List of fonts to try detection on.</param>
        /// <param name="fileData">File data</param>
        /// <param name="loadErrors">Load errors detailing failed attempts at identification.</param>
        /// <returns>An instance of the detected font, or null if not found.</returns>
        public static FontFile LoadFontFile(Byte[] fileData, out List<LoadFailedException> loadErrors)
        {
            Type fontType = typeof (FontFile);
            foreach (Type t in AutoDetectTypes)
                if (!t.IsSubclassOf(fontType))
                    throw new Exception("Entries in autoDetectTypes list must all be FontFile classes!");
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
                    fontInstance.LoadFont(fileData, true);
                    return fontInstance;
                }
                catch (LoadFailedException e)
                {
                    e.AttemptedLoadedType = fontInstance.ShortTypeName;
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
            clone.m_ImageDataList = new List<FontFileSymbol>();
            foreach (FontFileSymbol image in this.m_ImageDataList)
                clone.m_ImageDataList.Add(image.Clone());
            return clone;
        }

        public FontFile CloneInto(FontFile newFont, Byte overflowColor)
        {
            Int32 targetBpp = newFont.BitsPerPixel;
            Int32 colValLimit = (Int32)Math.Pow(2, targetBpp);
            if (overflowColor >= colValLimit)
                throw new InvalidOperationException(String.Format("Cannot use value {0} as default on a {1} bit per pixel font.", overflowColor, targetBpp));
            newFont.FontWidth = this.FontWidth;
            newFont.FontHeight = this.FontHeight;
            newFont.m_ImageDataList = new List<FontFileSymbol>();

            //if newFont.SymbolsTypeMax this.newFont.SymbolsTypeMin < thisnewFont.SymbolsTypeMax;

            for (Int32 i = 0; i < newFont.SymbolsTypeMin; i++)
            {
                FontFileSymbol image = i < m_ImageDataList.Count? this.m_ImageDataList[i] : new FontFileSymbol(targetBpp);
                newFont.m_ImageDataList.Add(image.CloneFor(newFont, overflowColor));
            }
            for (Int32 i = newFont.SymbolsTypeMin; i < Math.Min(m_ImageDataList.Count, newFont.SymbolsTypeMax); i++)
            {
                newFont.m_ImageDataList.Add(this.m_ImageDataList[i].CloneFor(newFont, overflowColor));
            }
            return newFont;
        }

        public void RestorePicFromBackup(Int32 index, FontFile backup)
        {
            if (index < 0 || backup.Length <= index || this.Length <= index)
                return;
            RestorePicFromBackup(index, backup.m_ImageDataList[index]);
        }

        public void RestorePicFromBackup(Int32 index, FontFileSymbol backup)
        {
            if (index < 0 || this.Length <= index)
                return;
            this.m_ImageDataList[index] = backup.CloneFor(this);
        }

        public Int32 GetSymbolWidth(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Width;
        }

        public Int32 GetSymbolHeight(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].Height;
        }

        public Int32 GetSymbolYOffset(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return 0;
            return this.m_ImageDataList[index].YOffset;
        }

        public FontFileSymbol GetSymbol(Int32 index)
        {
            if (index < 0 || index >= this.Length)
                return null;
            return this.m_ImageDataList[index];
        }

        public FontFileSymbol[] GetAllSymbols()
        {
            return this.m_ImageDataList.ToArray();
        }

        public Bitmap GetBitmap(Int32 index, Color[] colors, Boolean addTransparentZero)
        {
            if (index < 0 || index >= this.Length)
                return null;
            ColorPalette palette = ImageUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero, Color.Black);
            return this.m_ImageDataList[index].GetBitmap(palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors, Boolean addTransparentZero)
        {
            Bitmap[] allPics = new Bitmap[this.Length];
            ColorPalette palette = ImageUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero);
            for (Int32 i = 0; i < allPics.Length; i++)
                allPics[i] = this.m_ImageDataList[i].GetBitmap(palette);
            return allPics;
        }

        public void PaintPixel(Int32 index, Int32 x, Int32 y, Byte value)
        {
            if (index < 0 || index >= this.Length)
                throw new IndexOutOfRangeException("Bad symbol index '" + index + "'.");
            FontFileSymbol symbol = this.GetSymbol(index);
            symbol.PaintPixel(x, y, value);
        }

        public Bitmap PrintText(String text, Color[] colors, Boolean transparentBg, Encoding enc, Int32 wrapAt)
        {
            // just to be sure this never overflows to infinite height
            wrapAt = Math.Max(wrapAt, this.FontWidth);
            Int32 fullWidth = 0;
            Int32 fullHeight = this.FontHeight;
            Int32 curWidth = 0;
            List<FontFileSymbol> symbols = new List<FontFileSymbol>();
            text = text.Trim().Trim('\r', '\n').Replace("\r\n", "\n");

            foreach (Char c in text)
            {
                if (c == '\n')
                {
                    fullWidth = Math.Max(fullWidth, curWidth);
                    // special case: since symbol data itself doesn't contain data about which character
                    // they are (shouldn't, either; it'd complicate copying), use "null" for a line break.
                    symbols.Add(null);
                    curWidth = 0;
                    fullHeight += this.FontHeight;
                    continue;
                }
                Byte[] val = enc.GetBytes(new Char[]{c});
                if (val.Length != 1 || val[0] > this.Length)
                    continue;
                FontFileSymbol ffs = GetSymbol(val[0]);
                symbols.Add(ffs);
                if (wrapAt != -1 && curWidth + ffs.Width > wrapAt)
                {
                    fullWidth = Math.Max(fullWidth, curWidth);
                    curWidth = 0;
                    fullHeight += this.FontHeight;
                }
                curWidth += ffs.Width;
            }
            // the minimum of 1 is added to prevent empty text from crashing
            fullWidth = Math.Max(1, Math.Max(fullWidth, curWidth));
            ColorPalette palette = ImageUtils.MakePalette(colors, this.BitsPerPixel, true);
            Bitmap fullBm = new Bitmap(fullWidth, fullHeight, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(fullBm))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(transparentBg? 0x00 : 0xFF, colors[0])))
                    g.FillRectangle(brush, 0, 0, fullWidth, fullHeight);
                curWidth = 0;
                Int32 curHeight = 0;
                foreach (FontFileSymbol ffs in symbols)
                {
                    if (ffs == null)
                    {
                        // special case: Line break. Increase height, reset width, and go to next symbol.
                        curHeight += this.FontHeight;
                        curWidth = 0;
                        continue;
                    }
                    if (wrapAt != -1 && curWidth + ffs.Width > fullWidth)
                    {
                        curWidth = 0;
                        curHeight += this.FontHeight;
                    }
                    if (ffs.Width != 0)
                    {
                        Bitmap symbol = ffs.GetBitmapFullSize(palette, this);
                        g.DrawImage(symbol, new Point(curWidth, curHeight));
                        curWidth += ffs.Width;
                    }
                }                
            }
            return fullBm;
        }

        #endregion

        #region V3 / V4 loading and saving

        protected void LoadV3V4Font(Byte[] fileData, FontFileVersion checkType, Boolean doV3Checks)
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
            Boolean isV4 = dataFormat == 0x02;
            if (isV4)
            {
                if (checkType != FontFileVersion.WW_V4)
                    throw new LoadFailedException("Load type identifies as v4.");
                // isn't in the header? Calculate.
                Int32[] headerVals = new Int32[] {fontDataOffsetsListOffset, widthsListOffset, fontDataOffset, heightsListOffset}.OrderBy(n => n).Take(2).ToArray();
                Int32 divval = 1;
                if (headerVals[0] == fontDataOffsetsListOffset || headerVals[0] == heightsListOffset)
                    divval = 2;
                length = (headerVals[1] - headerVals[0]) / divval;
            }
            else if (dataFormat == 0x00)
            {
                if (checkType == FontFileVersion.WW_V4)
                    throw new LoadFailedException("Load type identifies as v4.");
                if (doV3Checks)
                {
                    if (unknown0E == 0x1011)
                    {
                        if (checkType != FontFileVersion.WW_V3_1)
                            throw new LoadFailedException("Load identifies as v3.1.");
                    }
                    else if (unknown0E == 0x1012)
                    {
                        if (checkType != FontFileVersion.WW_V3)
                            throw new LoadFailedException("Load identifies as v3.");
                    }
                    // else... just let it pass. It'll come out as 3.2.
                }
                length++;
            }
            else
                throw new LoadFailedException(String.Format("Unknown font type identifier, '{0}'.", dataFormat));
            if (fontDataOffsetsListOffset + length * 2 > fileLength)
                throw new LoadFailedException("File data too short for offsets list!");
            if (widthsListOffset + length > fileLength)
                throw new LoadFailedException("File data too short for symbol widths list starting from offset !");
            if (heightsListOffset + length * 2 > fileLength)
                throw new LoadFailedException("File data too short for symbol heights list!");

            //FontDataOffset
            Int32[] fontDataOffsetsList = new Int32[length];
            for (Int32 i = 0; i < length; i++)
                fontDataOffsetsList[i] = ArrayUtils.GetLEShortFromByteArray(fileData, fontDataOffsetsListOffset + i * 2) + (isV4 ? fontDataOffset : 0);
            List<Byte> widthsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[widthsListOffset + i];
                if (width > this.FontWidth)
                    throw new LoadFailedException(String.Format("Illegal value '{0}' in symbol widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                widthsList.Add(width);
            }
            List<Byte> yOffsetsList = new List<Byte>();
            List<Byte> heightsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                yOffsetsList.Add(fileData[heightsListOffset + i * 2]);
                Byte height = fileData[heightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new LoadFailedException(String.Format("Illegal value '{0}' in symbol heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                heightsList.Add(height);
            }
            // End of LoadFailedExceptions. After this, assume the type is identified.
            this.m_ImageDataList = new List<FontFileSymbol>();
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte width = widthsList[i];
                Byte height = heightsList[i];
                Byte[] data8Bit = this.ConvertTo8Bit(fileData, width, height, start, bitsLength, i, false);
                FontFileSymbol fc = new FontFileSymbol(data8Bit, width, height, yOffsetsList[i], bitsLength);
                this.m_ImageDataList.Add(fc);
            }
        }
                
        protected Byte[] SaveV3V4Font(FontFileVersion fontver)
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
                FontFileSymbol fc = this.m_ImageDataList[i];
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
            ArrayUtils.SetLEShortInByteArray(fullData, 0, (Int16)fullLength);
            fullData[0x02] = (Byte)(isTibSun ? 0x02 : 0x00);    // Byte DataFormat
            fullData[0x03] = unknown03;                         // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = 0x0e;                              // Int16 Unknown04, low byte; (always 0x0e)
            fullData[0x05] = 0x00;                              // Int16 Unknown04, high byte; (always 0x00)
            ArrayUtils.SetLEShortInByteArray(fullData, 0x06, (Int16)offsetsListOffset);
            ArrayUtils.SetLEShortInByteArray(fullData, 0x08, (Int16)widthsListOffset);
            ArrayUtils.SetLEShortInByteArray(fullData, 0x0A, (Int16)fontOffsetStart);
            ArrayUtils.SetLEShortInByteArray(fullData, 0x0C, (Int16)heightsListOffset);
            ArrayUtils.SetLEShortInByteArray(fullData, 0x0E, (Int16)unknown0E);
            fullData[0x10] = 0x00;                              // Byte AlwaysZero (Always 0x00)
            fullData[0x11] = (Byte)(isTibSun ? 0 : imagesCount - 1);  // Byte LastSymbolIndex (for non-TS)
            fullData[0x12] = (Byte)m_FontHeight;                // Byte FontHeight
            fullData[0x13] = (Byte)m_FontWidth;                 // Byte FontWidth
            Array.Copy(fontDataOffsetsList, 0, fullData, offsetsListOffset, fontDataOffsetsList.Length);
            Array.Copy(widthsList, 0, fullData, widthsListOffset, widthsList.Length);
            Int32 imageDataOffs = fontOffsetStart;
            foreach (Byte[] symbolImgData in imageData)
            {
                if (symbolImgData.Length == 0)
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

        #region internal data loading/saving methods

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
                    ArrayUtils.SetLEShortInByteArray(fontDataOffsetsList, i * 2, (Int16)fontOffset);
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
        #endregion

        /// <summary>
        /// Basic FontFile contains the reading and writing implementation of V3 and V4 because they are similar,
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