using Nyerguds.ImageManipulation;
using Nyerguds.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using WWFontEditor.Domain.FontTypes;

namespace WWFontEditor.Domain
{
    public abstract class FontFile : IEquatable<FontFile>, FileTypeBroadcaster
    {
        protected const String ERR_NOHEADER = "File data too short to contain header.";
        protected const String ERR_BADHEADER = "Identifying bytes in header do not match.";
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
        public virtual Boolean CustomSymbXForType { get { return this.FontWidthTypeMin != this.FontWidthTypeMax; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font. Automatically disables if max and min for both dimensions are the same.</summary>
        public virtual Boolean CustomSymbYForType { get { return this.FontHeightTypeMin != this.FontHeightTypeMax; } }
        /// <summary>Bits per pixel of the data in this font.</summary>
        public abstract Int32 BitsPerPixel { get; }
        /// <summary>File extensions typically used for this font type.</summary>
        public virtual String[] FileExtensions { get { return new String[] { "fnt" }; } }
        /// <summary>File extension set for this specific file.</summary>
        public String FileExtension { get; set; }
        /// <summary>Very short code name for this font type.</summary>
        public abstract String ShortTypeName { get; }
        /// <summary>Brief name and description of the overall file type, for the types dropdown in the open file dialog.</summary>
        public abstract String ShortTypeDescription { get; }
        /// <summary>Brief name and description of the specific types for all extensions, for the types dropdown in the save file dialog.</summary>
        public virtual String[] DescriptionsForExtensions { get { return Enumerable.Repeat(this.ShortTypeDescription, this.FileExtensions.Length).ToArray(); } }
        /// <summary>Longer description of the font format.</summary>
        public abstract String LongTypeDescription { get; }
        /// <summary>List of games and other programs this font type is used by.</summary>
        public abstract String[] GamesListForType { get; }
        /// <summary>Supported types can always be loaded, but this indicates if save functionality to this type is also available.</summary>
        public virtual Boolean CanSave { get { return true; } }

        /// <summary>
        /// Loads the font from file data. Throws a FileTypeLoadException if the format is not recognised. Might throw other exceptions if the actual load failed after validation.
        /// </summary>
        /// <param name="fileData">The file data to read the font from.</param>
        /// <returns>False if the font was not identified as this type.</returns>
        public abstract void LoadFont(Byte[] fileData);

        /// <summary>
        /// Saves the font data to a byte array and returns it.
        /// </summary>
        /// <param name="disableCompression">True to disable any optional compression that might obfuscate the binary readability of the font.</param>
        /// <returns>The font data to be written to disk.</returns>
        public abstract Byte[] SaveFont(Boolean disableCompression);

        // any actions to be taken after conversion to this type. Free to override by subclasses.
        protected virtual void PostConvertCleanup() { }

        #endregion

        #region General functions and properties
        /// <summary>Adjustable maximum height of the loaded font.</summary>
        public Int32 FontHeight
        {
            get { return m_FontHeight; }
            set
            {
                this.m_FontHeight = Math.Max(Math.Min(value, this.FontHeightTypeMax), this.FontHeightTypeMin);
                foreach (FontFileSymbol symbol in this.m_ImageDataList)
                    if (symbol.Height > m_FontHeight || !this.CustomSymbXForType)
                        symbol.ChangeHeight(m_FontHeight);
            }
        }

        /// <summary>Adjustable maximum width of the loaded font.</summary>
        public Int32 FontWidth
        {
            get { return m_FontWidth; }
            set
            {
                this.m_FontWidth = Math.Max(Math.Min(value, this.FontWidthTypeMax), this.FontWidthTypeMin);
                foreach (FontFileSymbol symbol in this.m_ImageDataList)
                    if (symbol.Width > m_FontWidth || !this.CustomSymbXForType)
                        symbol.ChangeWidth(m_FontWidth);
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
                        m_ImageDataList.Add(new FontFileSymbol(this));
                    }
                }
            }
        }
        
        /// <summary>
        /// All supported types. Never put types in here that don't derive from FontFile.
        /// This list is used for open / save / convert dialogs, and should have the items in a logical order.
        /// </summary>
        public static Type[] SupportedTypes =
        {
            typeof(FontFileV1),
            typeof(FontFileV2),
            typeof(FontFileV3),
            typeof(FontFileV4),
            typeof(FontFileD2K),
            typeof(FontFileDynV1),
            typeof(FontFileDynV2),
            //typeof(FontFileMK), //DO NOT ENABLE. HAS NO SAVE.
        };

        /// <summary>
        /// All supported types. Never put types in here that don't derive from FontFile.
        /// Ordered in a logical way for autodetection, from complex to simple, to prevent false positives.
        /// </summary>
        public static Type[] AutoDetectTypes =
        {
            // Dynamix fonts have a very specific "FNT:" header start so I prefer putting them first.
            typeof(FontFileDynV1),
            typeof(FontFileDynV2),
            typeof(FontFileV4),
            typeof(FontFileV3),
            typeof(FontFileV2),
            // V1's "check" is file size only; leave it at the end.
            typeof(FontFileV1),
            // Can safely be put behind V1, since its minimum size is more than V1's fixed size.
            typeof(FontFileD2K),
            //typeof(FontFileMK), //DO NOT ENABLE. HAS NO LOAD FAIL CONDITIONS.
        };

        /// <summary>
        /// Attempts to load the given data as one of the known font types.
        /// </summary>
        /// <param name="fileData">File data</param>
        /// <param name="loadErrors">Load errors detailing failed attempts at identification.</param>
        /// <returns>An instance of the detected font, or null if not found.</returns>
        public static FontFile LoadFontFile(Byte[] fileData, out List<FileTypeLoadException> loadErrors)
        {
            Type fontType = typeof (FontFile);
            foreach (Type t in AutoDetectTypes)
                if (!t.IsSubclassOf(fontType))
                    throw new Exception("Entries in autoDetectTypes list must all be FontFile classes!");
            loadErrors = new List<FileTypeLoadException>();
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
                catch (FileTypeLoadException e)
                {
                    e.AttemptedLoadedType = fontInstance.ShortTypeName;
                    loadErrors.Add(e);
                }
            }
            return null;
        }

        public static List<String> GetSupportedExtensions()
        {
            List<String> extensions = new List<String>();
            List<Type> types = SupportedTypes.Union(AutoDetectTypes).ToList();
            foreach (Type type in types)
            {
                FontFile fontInstance = null;
                try
                {
                    fontInstance = (FontFile)Activator.CreateInstance(type);
                }
                catch { /* Ignore; programmer error. */ }
                if (fontInstance == null)
                    continue;
                foreach (String ext in fontInstance.FileExtensions)
                    if (!String.IsNullOrEmpty(ext) && !extensions.Contains(ext))
                        extensions.Add(ext);
            }
            return extensions;
        }

        public Boolean HasTooHighDataFor(Int32 bitsPerPixel)
        {
            if (this.BitsPerPixel <= bitsPerPixel)
                return false;
            foreach (FontFileSymbol ffs in m_ImageDataList)
                if (ffs.HasTooHighDataFor(bitsPerPixel))
                    return true;
            return false;
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

        /// <summary>
        /// Deep-clones the current font into a provided new font object, possibly of a different type.
        /// </summary>
        /// <param name="newFont">The new object to clone into.</param>
        /// <param name="overflowColor">Default value for overflow bytes on the font data in case newFont is of a lower color depth</param>
        /// <param name="targetBpp">Target bit per pixel. Might be artificially limited below th maximum for 8-bit palettes.</param>
        public void CloneInto(FontFile newFont, Byte overflowColor, Int32 targetBpp)
        {
            Int32 colValLimit = 1 << targetBpp;
            if (overflowColor >= colValLimit)
                throw new InvalidOperationException(String.Format("Cannot use value {0} as default on a {1} bit per pixel font.", overflowColor, targetBpp));
            // automatically adjusts the images to the given font width.
            newFont.FontWidth = this.FontWidth;
            // automatically adjusts the images to the given font height.
            newFont.FontHeight = this.FontHeight;
            newFont.m_ImageDataList = new List<FontFileSymbol>();

            for (Int32 i = 0; i < newFont.SymbolsTypeMin; i++)
            {
                FontFileSymbol image = i < m_ImageDataList.Count? this.m_ImageDataList[i] : new FontFileSymbol(newFont);
                newFont.m_ImageDataList.Add(image.CloneFor(newFont, overflowColor, targetBpp));
            }
            for (Int32 i = newFont.SymbolsTypeMin; i < Math.Min(m_ImageDataList.Count, newFont.SymbolsTypeMax); i++)
            {
                newFont.m_ImageDataList.Add(this.m_ImageDataList[i].CloneFor(newFont, overflowColor, targetBpp));
            }
            newFont.PostConvertCleanup();
        }

        public void RestorePicFromBackup(Int32 index, FontFile backup, Int32 targetBpp)
        {
            if (index < 0 || backup.Length <= index || this.Length <= index)
                return;
            RestorePicFromBackup(index, backup.m_ImageDataList[index], targetBpp);
        }

        public void RestorePicFromBackup(Int32 index, FontFileSymbol backup, Int32 targetBpp)
        {
            if (index < 0 || this.Length <= index)
                return;
            this.m_ImageDataList[index] = backup.CloneFor(this, targetBpp);
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
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero, Color.Black);
            return this.m_ImageDataList[index].GetBitmap(palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors, Boolean addTransparentZero)
        {
            Bitmap[] allPics = new Bitmap[this.Length];
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, addTransparentZero);
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
                if (val.Length != 1 || val[0] >= this.Length)
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
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, true);
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

        protected void LoadV3V4Font(Byte[] fileData, Boolean forV4)
        {
            Int32 fileLength = fileData.Length;
            if (fileLength < 0x14)
                throw new FileTypeLoadException(ERR_NOHEADER);
            Int16 fileSize = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x00, 2, true);
            if (fileSize != fileLength)
                throw new FileTypeLoadException(ERR_SIZECHECK);
            Byte dataFormat = fileData[0x02];
            //Byte unknown03 = fileData[0x03];
            //this.Unknown04 = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x04, 2, true);
            Int16 fontDataOffsetsListOffset = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x06, 2, true);
            Int16 widthsListOffset = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x08, 2, true);
            // use this for pos on TS format
            Int16 fontDataOffset = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0A, 2, true);
            Int16 heightsListOffset = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0C, 2, true);
            //Int16 unknown0E = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, 0x0E, 2, true);
            //Byte AlwaysZero = fileData[0x10];
            Byte lastIndex = fileData[0x11];
            this.m_FontHeight = fileData[0x12];
            this.m_FontWidth = fileData[0x13];

            Int32 length = lastIndex;
            Boolean isV4 = dataFormat == 0x02;
            if (isV4)
            {
                if (!forV4)
                    throw new FileTypeLoadException("Load type identifies as v4.");
                // isn't in the header? Calculate.
                Int32[] headerVals = new Int32[] {fontDataOffsetsListOffset, widthsListOffset, fontDataOffset, heightsListOffset}.OrderBy(n => n).Take(2).ToArray();
                Int32 divval = 1;
                if (headerVals[0] == fontDataOffsetsListOffset || headerVals[0] == heightsListOffset)
                    divval = 2;
                length = (headerVals[1] - headerVals[0]) / divval;
            }
            else if (dataFormat == 0x00)
            {
                if (forV4)
                    throw new FileTypeLoadException("Load type identifies as v3.");
                length++;
            }
            else
                throw new FileTypeLoadException(String.Format("Unknown font type identifier, '{0}'.", dataFormat));
            if (fontDataOffsetsListOffset + length * 2 > fileLength)
                throw new FileTypeLoadException("File data too short for offsets list!");
            if (widthsListOffset + length > fileLength)
                throw new FileTypeLoadException("File data too short for symbol widths list starting from offset !");
            if (heightsListOffset + length * 2 > fileLength)
                throw new FileTypeLoadException("File data too short for symbol heights list!");

            //FontDataOffset
            Int32[] fontDataOffsetsList = new Int32[length];
            for (Int32 i = 0; i < length; i++)
                fontDataOffsetsList[i] = (Int16)ArrayUtils.ReadIntFromByteArray(fileData, fontDataOffsetsListOffset + i * 2, 2, true) + (isV4 ? fontDataOffset : 0);
            List<Byte> widthsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                Byte width = fileData[widthsListOffset + i];
                if (width > this.FontWidth)
                    throw new FileTypeLoadException(String.Format("Illegal value '{0}' in symbol widths list at entry #{1}: the value is larger than global width '{2}'.", width, i, this.FontWidth));
                widthsList.Add(width);
            }
            List<Byte> yOffsetsList = new List<Byte>();
            List<Byte> heightsList = new List<Byte>();
            for (Int32 i = 0; i < length; i++)
            {
                yOffsetsList.Add(fileData[heightsListOffset + i * 2]);
                Byte height = fileData[heightsListOffset + i * 2 + 1];
                if (height > this.FontHeight)
                    throw new FileTypeLoadException(String.Format("Illegal value '{0}' in symbol heights list at entry #{1}: the value is larger than global height '{2}'.", height, i, this.FontHeight));
                heightsList.Add(height);
            }
            // End of FileTypeLoadExceptions. After this, assume the type is identified.
            this.m_ImageDataList = new List<FontFileSymbol>();
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < length; i++)
            {
                Int32 start = fontDataOffsetsList[i];
                Byte width = widthsList[i];
                Byte height = heightsList[i];
                Byte[] data8Bit;
                try
                {
                    data8Bit = ImageUtils.ConvertTo8Bit(fileData, width, height, start, bitsLength, false);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(String.Format("Data for font entry #{0} exceeds file bounds!", i));
                }
                FontFileSymbol fc = new FontFileSymbol(data8Bit, width, height, yOffsetsList[i], bitsLength);
                this.m_ImageDataList.Add(fc);
            }
        }
                
        protected Byte[] SaveV3V4Font(Boolean forV4)
        {
            // Y-optimization.
            foreach (FontFileSymbol ffs in m_ImageDataList)
                ffs.OptimizeYHeight();
            Int32 imagesCount = this.m_ImageDataList.Count;
            Byte[][] imageData = new Byte[imagesCount][];
            Byte[] widthsList = new Byte[imagesCount];
            Byte[] heightsList = new Byte[imagesCount * 2];
            // header + Int16 index + Byte heights
            Int32 offsetsListOffset = 0x14;
            Int32 widthsListOffset = offsetsListOffset + imagesCount * 2;
            Int32 heightsListOffset = 0;
            // V3 (TS) has its Y/height list before the image data.
            if (forV4)
                heightsListOffset = widthsListOffset + imagesCount;
            Int32 fontOffsetStart = (!forV4) ? widthsListOffset + imagesCount : heightsListOffset + imagesCount * 2;
            Int32 bitsLength = this.BitsPerPixel;
            for (Int32 i = 0; i < imagesCount; i++)
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
            Byte[] fontDataOffsetsList = this.OptimizeImagesList(imageData, ref fontOffset);
            // V2 (C&C) has its Y/height list before the image data.
            if (!forV4)
                heightsListOffset = fontOffset;
            Int32 fullLength = !forV4 ? (heightsListOffset + imagesCount * 2) : (fontOffset + fontOffsetStart);
            Byte[] fullData = new Byte[fullLength];
            
            // write header
            ArrayUtils.WriteIntToByteArray(fullData, 0, 2, true, (UInt32)fullLength);
            fullData[0x02] = (Byte)(forV4 ? 0x02 : 0x00);       // Byte DataFormat
            fullData[0x03] = (Byte)(forV4 ? 0 : 5);             // Byte Unknown03 (0x05 in EOB/C&C/RA1, 0x00 in TS)
            fullData[0x04] = 0x0e;                              // Int16 Unknown04, low byte; (always 0x0e)
            fullData[0x05] = 0x00;                              // Int16 Unknown04, high byte; (always 0x00)
            ArrayUtils.WriteIntToByteArray(fullData, 0x06, 2, true, (UInt32)offsetsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x08, 2, true, (UInt32)widthsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0A, 2, true, (UInt32)fontOffsetStart);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0C, 2, true, (UInt32)heightsListOffset);
            ArrayUtils.WriteIntToByteArray(fullData, 0x0E, 2, true, (UInt32)(forV4 ? 0 : 0x1012));
            fullData[0x10] = 0x00;                              // Byte AlwaysZero (Always 0x00)
            fullData[0x11] = (Byte)(forV4 ? 0 : imagesCount - 1);  // Byte LastSymbolIndex (for non-TS)
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
                    ArrayUtils.WriteIntToByteArray(fontDataOffsetsList, i * 2, 2, true, (UInt32)fontOffset);
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
        #endregion

        public Boolean Equals(FontFile other)
        {
            if (this.GetType() != other.GetType())
                return false;
            if (this.FontWidth != other.FontWidth || this.FontHeight != other.FontHeight || this.Length != other.Length)
                return false;
            for (Int32 i = 0; i < this.Length; i++)
            {
                if (!this.m_ImageDataList[i].Equals(other.m_ImageDataList[i]))
                    return false;
            }
            return true;
        }
    }
}