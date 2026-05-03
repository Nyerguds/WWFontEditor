using Nyerguds.ImageManipulation;
using Nyerguds.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Nyerguds.Util.UI;
using WWFontEditor.Domain.FontTypes;

namespace WWFontEditor.Domain
{
    public abstract class FontFile : IEquatable<FontFile>, FileTypeBroadcaster
    {
        protected const String ERR_NOHEADER = "File data too short to contain header.";
        protected const String ERR_BADHEADER = "Identifying bytes in header do not match.";
        protected const String ERR_BADHEADERDATA = "Bad values in header.";
        protected const String ERR_SIZEHEADER = "File size value in header does not match file data length.";
        protected const String ERR_SIZECHECK = "File size does not match expected data length.";
        protected const String ERR_SYMBCHECK = "Amount of symbols exceeds 256!";

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
        /// <summary>The first symbol that is saved. This hides all symbols before this index from the editor.</summary>
        public virtual Int32 SymbolsTypeFirst { get { return 0; } }
        /// <summary>Lower limit for the width of the overall font. This does not mean symbols themselves are limited to this minimum.</summary>
        public virtual Int32 FontWidthTypeMin { get { return 0; } }
        /// <summary>Upper limit for the width of the overall font.</summary>
        public abstract Int32 FontWidthTypeMax { get; }
        /// <summary>Lower limit for the overall height of the overall font.</summary>
        public virtual Int32 FontHeightTypeMin { get { return 0; } }
        /// <summary>Upper limit for the overall height of the overall font.</summary>
        public abstract Int32 FontHeightTypeMax { get; }
        /// <summary>Upper limit for the Y-offset of the symbols in the font. Zero means the font format does not support Y offsets</summary>
        public abstract Int32 YOffsetTypeMax { get; }
        /// <summary>The index on the font that is treated as transparent colour.</summary>
        public virtual Byte TransparencyColor { get { return 0;} }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public virtual Boolean CustomSymbolWidthsForType { get { return this.FontWidthTypeMin != this.FontWidthTypeMax; } }
        /// <summary> Set this to False if individual symbols cannot have different sizes than their parent font.</summary>
        public virtual Boolean CustomSymbolHeightsForType { get { return this.FontHeightTypeMin != this.FontHeightTypeMax; } }
        /// <summary>Padding at the bottom of the font. Only used for the preview function.</summary>
        public virtual Int32 FontTypePaddingBottom { get { return 0; } }
        /// <summary>Padding between the characters of the font. Used for the preview function and to determine if padding is needed when automatically optimizing symbol widths.</summary>
        public virtual Int32 FontTypePaddingRight { get { return 0; } }
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

        public virtual SaveOption[] GetSaveOptions(String targetFileName) { return new SaveOption[0]; }
        
        /// <summary>
        /// Saves the font data to a byte array and returns it.
        /// </summary>
        /// <param name="saveOptions"></param>
        /// <returns>The font data to be written to disk.</returns>
        public abstract Byte[] SaveFont(SaveOption[] saveOptions);

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
                    if (symbol.Height > m_FontHeight || !this.CustomSymbolHeightsForType)
                        symbol.ChangeHeight(m_FontHeight, this.TransparencyColor);
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
                    if (symbol.Width > m_FontWidth || !this.CustomSymbolWidthsForType)
                        symbol.ChangeWidth(m_FontWidth, this.TransparencyColor);
            }
        }

        /// <summary>Amount of symbols in the font.</summary>
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
            typeof(FontFileWsV1),
            typeof(FontFileWsV2),
            typeof(FontFileWsV3),
            typeof(FontFileWsV4),
            typeof(FontFileD2K),
            typeof(FontFileTran),
            typeof(FontFileDynV1a),
            typeof(FontFileDynV1b),
            typeof(FontFileDynV2),
            typeof(FontFileDynV3),
            typeof(FontFileDynV4),
            typeof(FontFileDynV5),
            typeof(FontFileDynV6),
            typeof(FontFileDynSQ5),
            typeof(FontFileCent),
            typeof(FontFileKort),
            typeof(FontFileMythos),
            typeof(FontFileKotB),
            //typeof(FontFileMK), //DO NOT ENABLE. HAS NO SAVE.
        };

        /// <summary>
        /// All supported types. Never put types in here that don't derive from FontFile.
        /// Ordered in a logical way for autodetection, starting with those that are easy to identify with certainty,
        /// and going down to more simple types that rely on size calculations, to prevent false positives.
        /// </summary>
        public static Type[] AutoDetectTypes =
        {
            // Dynamix fonts starting from v3 have a very specific "FNT:" header start so I prefer putting them first.
            typeof(FontFileDynV3),
            typeof(FontFileDynV4),
            typeof(FontFileDynV5),
            typeof(FontFileDynV6),
            typeof(FontFileWsV4),
            typeof(FontFileWsV3),
            typeof(FontFileWsV2),
            typeof(FontFileD2K),
            typeof(FontFileKort),
            // rather weak file size / content based checks.
            typeof(FontFileDynSQ5),
            typeof(FontFileCent),
            typeof(FontFileDynV2),
            typeof(FontFileDynV1b),
            typeof(FontFileDynV1a),
            typeof(FontFileMythos),
            typeof(FontFileKotB),
            typeof(FontFileTran),
            // File size only; leave it at the end.
            typeof(FontFileWsV1),
            //typeof(FontFileMK), //DO NOT ENABLE. HAS NO LOAD FAIL CONDITIONS.
        };

        /// <summary>
        /// Attempts to load the given data as one of the known font types.
        /// </summary>
        ///<param name="path">Path the file was loaded from.</param>
        /// <param name="fileData">File data</param>
        /// <param name="loadErrors">Load errors detailing failed attempts at identification.</param>
        /// <returns>An instance of the detected font, or null if not found.</returns>
        public static FontFile LoadFontFile(String path, Byte[] fileData, out List<FileTypeLoadException> loadErrors)
        {
            Type fontType = typeof (FontFile);
            foreach (Type t in AutoDetectTypes)
                if (!t.IsSubclassOf(fontType))
                    throw new Exception("Entries in autoDetectTypes list must all be FontFile classes!");
            loadErrors = new List<FileTypeLoadException>();
            //List<Exception> processErrors = new List<Exception>();
            FontFile[] possibleTypes = FileDialogGenerator.IdentifyByExtension<FontFile>(AutoDetectTypes, path);
            foreach (FontFile typeObj in possibleTypes)
            {
                try
                {
                    typeObj.LoadFont(fileData);
                    return typeObj;
                }
                catch (FileTypeLoadException e)
                {
                    e.AttemptedLoadedType = typeObj.ShortTypeName;
                    loadErrors.Add(e);
                }
            }
            foreach (Type type in AutoDetectTypes)
            {
                Boolean knownType = false;
                foreach (FontFile typeObj in possibleTypes)
                {
                    if (typeObj.GetType() == type)
                    {
                        knownType = true;
                        break;
                    }
                }
                if (knownType)
                    continue;
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
                FontFileSymbol symbol = i < m_ImageDataList.Count? this.m_ImageDataList[i] : new FontFileSymbol(newFont);
                newFont.m_ImageDataList.Add(symbol.CloneFor(newFont, overflowColor, targetBpp));
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

        public Bitmap GetBitmap(Int32 index, Color[] colors, Boolean addTransparentcy)
        {
            if (index < 0 || index >= this.Length)
                return null;
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, PaletteUtils.MakeTransparencyGuide(this.BitsPerPixel, this.TransparencyColor), Color.Black);
            return this.m_ImageDataList[index].GetBitmap(palette);
        }

        public Bitmap[] GetAllBitmaps(Color[] colors, Boolean addTransparentZero)
        {
            Bitmap[] allPics = new Bitmap[this.Length];
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, PaletteUtils.MakeTransparencyGuide(this.BitsPerPixel, this.TransparencyColor));
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
            Int32 fullWidth = 0;
            Int32 fullHeight = this.m_FontHeight;
            Int32 curWidth = 0;
            List<FontFileSymbol> symbols = new List<FontFileSymbol>();
            String printText = text.Replace("\r\n", "\n").Replace('\r', '\n');
            // Makes the list of the font file symbols to paint, with null substituting for the line break.
            // Also calculates the required width without wrapping.
            Boolean isStart = true;
            foreach (Char c in printText)
            {
                if (c == '\n')
                {
                    fullWidth = Math.Max(fullWidth, curWidth);
                    // special case: since symbol data itself doesn't contain data about which character
                    // they are (shouldn't, either; it'd complicate copying), use "null" for a line break.
                    symbols.Add(null);
                    curWidth = 0;
                    isStart = true;
                    continue;
                }
                if (isStart)
                    isStart = false;
                else
                    curWidth += this.FontTypePaddingRight;
                Byte[] val = enc.GetBytes(new Char[] {c});
                Char[] newc = enc.GetChars(val);
                // Can only handle one byte per character fonts.
                if (val.Length != 1 || newc.Length != 1 || newc[0] != c)
                    continue;
                Byte code = val[0];
                // Font symbol is not implemented!
                if (code >= this.Length)
                {
                    symbols.Add(new FontFileSymbol(this));
                    continue;
                }
                FontFileSymbol ffs = this.GetSymbol(code);
                curWidth += ffs.Width;
                symbols.Add(ffs);
            }
            // If wrapping is enabled, this applies wrapping by making a new list with extra null entries.
            // Also calculates the required width with wrapping.
            if (wrapAt > -1)
            {
                curWidth = 0;
                fullWidth = 0;
                if (symbols.Count > 0)
                {
                    // Ensure that the wrap width is at least as wide as the widest used symbol in the string.
                    Int32 maxWidth = symbols.Max(x => x == null ? Int32.MinValue : x.Width);
                    wrapAt = Math.Max(wrapAt, maxWidth);
                }
                List<FontFileSymbol> wrappedSymbols = new List<FontFileSymbol>();
                isStart = true;
                foreach (FontFileSymbol ffs in symbols)
                {
                    // Add padding behind previous symbol
                    Boolean wasStart = isStart;
                    if (isStart)
                        isStart = false;
                    else
                        curWidth += this.FontTypePaddingRight;
                    Boolean isBreak = ffs == null;
                    if (isBreak || curWidth + ffs.Width > wrapAt)
                    {
                        // Remove padding since symbol isn't added
                        if (!wasStart)
                            curWidth -= this.FontTypePaddingRight;
                        fullWidth = Math.Max(fullWidth, curWidth);
                        // A wrap break never puts IsStart back to true since it immediately add the character behind the break.
                        curWidth = 0;
                        wrappedSymbols.Add(null);
                        if (isBreak)
                        {
                            isStart = true;
                            continue;
                        }
                    }
                    wrappedSymbols.Add(ffs);
                    curWidth += ffs.Width;
                }
                symbols = wrappedSymbols;
            }
            // Calculates the required line height, including any Y offsets sticking out that could extend the bottom.
            // This goes over all lines, just to be sure.
            Int32 curLineTop = 0;
            foreach (FontFileSymbol ffs in symbols)
            {
                if (ffs == null) // Line break
                    curLineTop += this.m_FontHeight + this.FontTypePaddingBottom;
                else
                    fullHeight = Math.Max(fullHeight, curLineTop + ffs.Height + ffs.YOffset);
            }
            // Comparison of the final line's curWidth after the loop.
            //The minimum of 1 is added to prevent empty text from crashing
            fullWidth = Math.Max(1, Math.Max(fullWidth, curWidth));
            fullHeight = Math.Max(1, fullHeight);
            Color[] palette = PaletteUtils.MakePalette(colors, this.BitsPerPixel, PaletteUtils.MakeTransparencyGuide(this.BitsPerPixel, this.TransparencyColor));
            Bitmap fullBm = new Bitmap(fullWidth, fullHeight, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(fullBm))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(transparentBg ? 0x00 : 0xFF, colors[this.TransparencyColor])))
                    g.FillRectangle(brush, 0, 0, fullWidth, fullHeight);
                curWidth = 0;
                Int32 curHeight = 0;
                foreach (FontFileSymbol ffs in symbols)
                {
                    if (ffs == null)
                    {
                        // special case: Line break. Increase height, reset width, and go to next symbol.
                        curHeight += this.m_FontHeight + this.FontTypePaddingBottom;
                        curWidth = 0;
                        continue;
                    }
                    if (ffs.Width != 0)
                    {
                        using (Bitmap symbol = ffs.GetBitmapFullSize(palette, this, true))
                        {
                            if (symbol != null)
                                g.DrawImage(symbol, new Point(curWidth, curHeight));
                            curWidth += ffs.Width;
                        }
                    }
                    curWidth += this.FontTypePaddingRight;
                }
            }
            return fullBm;
        }

        #endregion

        #region internal data loading/saving methods

        /// <summary>
        ///     Creates a 16-bit little endian index of reference addresses, starting from the given fontOffset.
        ///     After the procedure, fontOffset will have the address behind the last data to write.
        ///     If "optimize" is enabled this will remove duplicate images in the process.
        /// </summary>
        /// <param name="imageData">Image data. Duplicate arrays in this are set to 0-sized ones.</param>
        /// <param name="startIndex">Start index in the imageData array.</param>
        /// <param name="reduce">True to only start the index from the start offset. False generates the full index with 0 on the empty spots.</param>
        /// <param name="fontOffset">Start offset of the addressing. Adjusted to the end offset.</param>
        /// <param name="usesNullOffset">Use 0 value for symbols with no data.</param>
        /// <param name="optimise">Optimise to remove duplicate indices.</param>
        /// <param name="unsigned">True if the Int16 values in the index are seen as unsigned.</param>
        /// <returns>The list of reference addresses, relative to the given font offset.</returns>
        protected Byte[] CreateImageIndex(Byte[][] imageData, Int32 startIndex, Boolean reduce, ref Int32 fontOffset, Boolean usesNullOffset, Boolean optimise, Boolean unsigned)
        {
            Int32 maxValue = unsigned ? (Int32) UInt16.MaxValue : Int16.MaxValue;
            Int32[] refslist = optimise ? this.CreateOptimizedRefsList(imageData, startIndex) : null;
            Int32 symbols = imageData.Length;
            Int32 writeDiff = reduce ? -startIndex : 0;
            Byte[] fontDataOffsetsList = new Byte[(reduce ? symbols - startIndex : symbols) * 2];

            for (Int32 i = startIndex; i < symbols; i++)
            {
                Int32 replacei = optimise ? refslist[i] : i;
                if (usesNullOffset && imageData[i].Length == 0)
                {
                    // Data is null: just write 0
                    fontDataOffsetsList[(i + writeDiff) * 2] = 0;
                    fontDataOffsetsList[(i + writeDiff) * 2 + 1] = 0;
                }
                else if (replacei == i)
                {
                    if (fontOffset > maxValue)
                        throw new OverflowException("Data too large: this format cannot address data that exceeds " + maxValue + " bytes!");
                    // Data is not null and not a duplicate: write offset and advance offset ptr.
                    ArrayUtils.WriteIntToByteArray(fontDataOffsetsList, (i + writeDiff) * 2, 2, true, (UInt32)fontOffset);
                    fontOffset += imageData[i].Length;
                }
                else
                {
                    // Data is duplicate: clear data and copy previously written offset.
                    imageData[i] = new Byte[0];
                    fontDataOffsetsList[(i + writeDiff) * 2] = fontDataOffsetsList[(replacei + writeDiff) * 2];
                    fontDataOffsetsList[(i + writeDiff) * 2 + 1] = fontDataOffsetsList[(replacei + writeDiff) * 2 + 1];
                }
            }
            return fontDataOffsetsList;
        }

        /// <summary>
        /// File size optimization. This function makes a map to re-map duplicate entries to the first found occurrence.
        /// In the final images array, any index not referencing itself is deemed a copy and should be removed in favour of the reference.
        /// If startindex is greater than 0, the returned references list will not be smaller; the ones before the start will simply not be processed.
        /// </summary>
        /// <param name="imageData">Image data array</param>
        /// <param name="startIndex">Start index in the array.</param>
        /// <returns></returns>
        protected Int32[] CreateOptimizedRefsList(Byte[][] imageData, Int32 startIndex)
        {
            Int32 imagesCount = imageData.Length;
            Int32[] refsList = new Int32[imagesCount];
            for (Int32 checkedEntry = startIndex; checkedEntry < imagesCount; checkedEntry++)
            {
                for (Int32 dupetest = startIndex; dupetest < imagesCount; dupetest++)
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
            if (ReferenceEquals(this, other))
                return true;
            if (other == null || this.GetType() != other.GetType())
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