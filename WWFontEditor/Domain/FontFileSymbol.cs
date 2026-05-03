using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Nyerguds.ImageManipulation;

namespace WWFontEditor.Domain
{
    // For real clipboard support :)
    // Further info: http://stackoverflow.com/questions/9032673/clipboard-copying-objects-to-and-from
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public class FontFileSymbol : IEquatable<FontFileSymbol>
    {
        public Byte[] ByteData { get; set; }
        /// <summary>Only used for initialisation. Use ChangeWidth for editing the image!</summary>
        public Int32 Width { get; private set; }
        /// <summary>Only uses for initialisation. Use ChangeHeight for editing the image!</summary>
        public Int32 Height { get; private set; }
        public Int32 YOffset { get; set; }
        public Int32 BitsPerPixel { get; private set; }
        public Byte TransparencyColor { get; private set; }
        
        /// <summary>Creates a new symbol for the given font type.</summary>
        /// <param name="source">Font the symbol is meant for; the symbol will adhere to the restrictions of this font.</param>
        public FontFileSymbol(FontFile source)
        {
            this.ByteData = new Byte[0];
            this.Width = source.CustomSymbolWidthsForType ? source.FontWidthTypeMin : source.FontWidth;
            this.Height = 0;
            // only need to do this from 1 dimension since you start from (?)x0
            this.ChangeHeight(source.CustomSymbolHeightsForType ? source.FontHeightTypeMin : source.FontHeight, source.TransparencyColor);
            this.BitsPerPixel = source.BitsPerPixel;
            this.TransparencyColor = source.TransparencyColor;
        }

        /// <summary>
        /// Creates a new font file symbol for a font file, starting from an external image.
        /// This constructor is used for clipboard input.
        /// </summary>
        /// <param name="image">Source image</param>
        /// <param name="palette">Colour palette</param>
        /// <param name="source">Font file to create this symbol for.</param>
        public FontFileSymbol(Image image, Color[] palette, FontFile source)
        {
            this.BitsPerPixel = source.BitsPerPixel;
            this.TransparencyColor = source.TransparencyColor;
            this.Width = Math.Min(source.FontWidth, image.Width);
            this.Height = Math.Min(source.FontHeight, image.Height);
            this.ByteData = new Byte[this.Width * this.Height];
            Byte[] hiColImg;
            Int32 stride;
            Boolean hasTrans;
            using (Bitmap srcImage = ImageUtils.PaintOn32bpp(image, null))
            {
                hasTrans = ImageUtils.HasTransparency(srcImage);
                hiColImg = ImageUtils.GetImageData(srcImage, out stride);
                //Byte[] pngdata = BitmapHandler.GetPngImageData(srcImage, 0);
                //System.IO.File.WriteAllBytes("imgDump.png", pngdata);
            }
            List<Int32> trans = null;
            if (hasTrans)
            {
                // Only filter out the transparency colour if the image has alpha.
                trans = new List<Int32>();
                trans.Add(this.TransparencyColor);
            }
            for (Int32 y = 0; y < this.Height; y++)
            {
                Int32 inputOffs = y * stride;
                for (Int32 x = 0; x < this.Width; x++)
                {
                    Color col = Color.FromArgb(hiColImg[inputOffs + 3], hiColImg[inputOffs + 2], hiColImg[inputOffs + 1], hiColImg[inputOffs]);
                    inputOffs += 4;
                    if (hasTrans && col.A < 128)
                        this.ByteData[y * this.Width + x] = this.TransparencyColor;
                    else
                        this.ByteData[y * this.Width + x] = (Byte)ColorUtils.GetClosestPaletteIndexMatch(col, palette, trans);
                }
            }
            this.AdaptSizeToFont(this, source);
        }

        public FontFileSymbol(Byte[] byteData, Int32 width, Int32 height, Int32 yOffset, Int32 bitsPerPixel, Byte transparencyColor)
        {
            this.ByteData = new Byte[byteData.Length];
            Array.Copy(byteData, 0, this.ByteData, 0, byteData.Length);
            this.Width = width;
            this.Height = height;
            this.YOffset = yOffset;
            this.BitsPerPixel = bitsPerPixel;
            this.TransparencyColor = transparencyColor;
        }

        public FontFileSymbol Clone()
        {
            return new FontFileSymbol(this.ByteData.ToArray(), this.Width, this.Height, this.YOffset, this.BitsPerPixel, this.TransparencyColor);
        }

        public FontFileSymbol CloneFor(FontFile targetVersion, Int32 targetBpp)
        {
            return this.CloneFor(targetVersion, null, targetBpp);
        }

        public Boolean HasTooHighDataFor(Int32 bitsPerPixel)
        {
            // Shouldn't. Let's assume that's implemented correctly ;)
            if (this.BitsPerPixel <= bitsPerPixel)
                return false;
            Int32 colValLimit = 1 << bitsPerPixel;
            return this.ByteData.Any(x => x >= colValLimit);
        }

        public FontFileSymbol CloneFor(FontFile font, Byte? defaultValue, Int32 targetBpp)
        {
            // PART ONE: COLOR CONVERSION
            // If higher bitrate, convert overflow to default if given.
            Byte[] newByteData = this.ConvertDataToBpp(defaultValue, targetBpp);
            FontFileSymbol newSymbol = new FontFileSymbol(newByteData, this.Width, this.Height, this.YOffset, targetBpp, font.TransparencyColor);
            this.AdaptSizeToFont(newSymbol, font);
            return newSymbol;
        }

        private void AdaptSizeToFont(FontFileSymbol symbol, FontFile newFont)
        {
            if (this.YOffset > newFont.YOffsetTypeMax)
            {
                Int32 diff = symbol.YOffset - newFont.YOffsetTypeMax;
                symbol.ChangeHeight(this.Height + diff, newFont.TransparencyColor);
                symbol.YOffset = 0;
                // Not ideal, I know, but I haven't adapted the shift function to accept an amount to shift.
                for (Int32 i = 0; i < diff; i++)
                    symbol.ShiftImageData(ShiftDirection.Down, false, newFont.TransparencyColor);
            }
            // Adapt to font width
            if (!newFont.CustomSymbolWidthsForType || newFont.FontWidth < symbol.Width)
                symbol.ChangeWidth(newFont.FontWidth, newFont.TransparencyColor);
            // Adapt to font height
            if (!newFont.CustomSymbolHeightsForType || newFont.FontHeight < symbol.Height)
                symbol.ChangeHeight(newFont.FontHeight, newFont.TransparencyColor);
        }

        /// <summary>
        /// Converts the data to the maximum value restrictions of a new bits per pixel value.
        /// This will not compact the array to that bpp format; the buffer remains 8-bit.
        /// </summary>
        /// <param name="defaultValue">Default value when exceeding the maximum of the target format.</param>
        /// <param name="targetBpp">Target bits per pixel format.</param>
        public void ConvertToBpp(Byte? defaultValue, Int32 targetBpp)
        {
            if (this.BitsPerPixel == targetBpp)
                return;
            this.ByteData = this.ConvertDataToBpp(defaultValue, targetBpp);
            this.BitsPerPixel = targetBpp;
        }

        /// <summary>
        /// Converts the data to the maximum value restrictions of a new bits per pixel value.
        /// This will not compact the array to that bpp format; the buffer remains 8-bit.
        /// </summary>
        /// <param name="defaultValue">Default value when exceeding the maximum of the target format.</param>
        /// <param name="targetBpp">Target bits per pixel format.</param>
        /// <returns>The adapted buffer.</returns>
        private Byte[] ConvertDataToBpp(Byte? defaultValue, Int32 targetBpp)
        {
            Int32 myBpp = this.BitsPerPixel;
            Byte[] newByteData;
            Int32 colValLimit = 1 << targetBpp;
            if (defaultValue.HasValue && defaultValue.Value >= colValLimit)
                throw new InvalidOperationException(String.Format("Cannot use value {0} as default on a {1} bit per pixel font.", defaultValue, targetBpp));
            if (myBpp > targetBpp && this.ByteData.Any(x => x >= colValLimit))
            {
                if (defaultValue == null)
                    throw new InvalidOperationException(String.Format("Cannot insert a {0} bit per pixel image into a {1} bit per pixel font.", myBpp, targetBpp));
                newByteData = this.ByteData.Select(x => x >= colValLimit ? defaultValue.Value : x).ToArray();
            }
            else
                newByteData = this.ByteData.ToArray();
            return newByteData;
        }

        /// <summary>
        /// Gets a bitmap of the symbol at full font height.
        /// </summary>
        /// <param name="palette">Colour palette for the image.</param>
        /// <param name="baseFont">The base font, so transparency colour and overall dimensions can be fetched.</param>
        /// <param name="expandToY">True to expand the image to the full shifted-down size, false to cut it off at the original font height.</param>
        /// <returns></returns>
        public Bitmap GetBitmapFullSize(Color[] palette, FontFile baseFont, Boolean expandToY)
        {
            FontFileSymbol ffs = this.Clone();
            ffs.ChangeHeight(baseFont.FontHeight + ffs.YOffset, baseFont.TransparencyColor);
            for (Int32 i = 0; i < ffs.YOffset; i++)
                ffs.ShiftImageData(ShiftDirection.Down, false, baseFont.TransparencyColor);
            ffs.YOffset = 0;
            ffs.ChangeHeight(expandToY ? Math.Max(baseFont.FontHeight, this.Height + this.YOffset) : baseFont.FontHeight, baseFont.TransparencyColor);
            return ffs.GetBitmap(palette);
        }

        public Bitmap GetBitmap(Color[] palette)
        {
            Int32 width = this.Width;
            Int32 height = this.Height;
            if (width == 0 || height == 0)
                return null;
            Byte[] imageData = this.ByteData;
            if (imageData.Length == 0 || width == 0 | height == 0)
                return null;
            return ImageUtils.BuildImage(imageData, width, height, width, PixelFormat.Format8bppIndexed, palette, Color.Empty);
        }

        public void PaintPixel(Int32 x, Int32 y, Byte value)
        {
            if (x < 0 || x >= this.Width || y < 0 || y >= this.Height)
                return; // Ignore without error. Might accidentally occur when dragging or something I guess.
            Int32 pxf = this.BitsPerPixel;
            Int32 maxSize = 1 << pxf;
            if (maxSize <= value)
                throw new IndexOutOfRangeException("Byte value too large for " + pxf + " bit image!");
            this.ByteData[y * this.Width + x] = value;
        }

        public Byte GetPixelValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= this.Width || y < 0 || y >= this.Height)
                return 0; // Ignore without error. Might accidentally occur when dragging or something I guess.
            return this.ByteData[y * this.Width + x];
        }

        public void ChangeHeight(Int32 newHeight, Byte backColor)
        {
            if (this.Height == newHeight)
                return;
            this.ByteData = ImageUtils.ChangeHeight(this.ByteData, this.Width, this.Height, newHeight, false, this.TransparencyColor);
            this.Height = newHeight;
        }

        public void ChangeWidth(Int32 newWidth, Byte backColor)
        {
            if (this.Width == newWidth)
                return;
            this.ByteData = ImageUtils.ChangeStride(this.ByteData, this.Width, this.Height, newWidth, false, backColor);
            this.Width = newWidth;
        }

        public Boolean TryExpandImage(ShiftDirection direction, FontFile parentFont)
        {
            Int32 maxWidth = parentFont.FontWidth;
            Int32 maxHeight = parentFont.FontHeight;
            switch (direction)
            {
                case ShiftDirection.Up:
                    if (this.Height >= maxHeight || this.YOffset <= 0)
                        return false;
                    this.ChangeHeight(this.Height + 1, parentFont.TransparencyColor);
                    this.YOffset--;
                    return false;
                case ShiftDirection.Down:
                    if (this.Height >= maxHeight)
                        return false;
                    this.ChangeHeight(this.Height + 1, parentFont.TransparencyColor);
                    break;
                case ShiftDirection.Left:
                    // can't expand to the left.
                    return false;
                case ShiftDirection.Right:
                    if (this.Width >= maxWidth)
                        return false;
                    this.ChangeWidth(this.Width + 1, parentFont.TransparencyColor);
                    break;
            }
            return true;
        }

        public void ShiftImageData(ShiftDirection direction, Boolean wrap, Byte backColor)
        {
            if (this.ByteData.Length == 0)
                return;
            switch (direction)
            {
                case ShiftDirection.Up:
                case ShiftDirection.Down:
                    ImageUtils.Shift8BitRowVert(this.ByteData, this.Width, direction == ShiftDirection.Up, wrap, backColor);
                    break;
                case ShiftDirection.Left:
                case ShiftDirection.Right:
                    ImageUtils.Shift8BitRowHor(this.ByteData, this.Width, direction == ShiftDirection.Left, wrap, backColor);
                    break;
            }
        }

        public void ReplaceColor(Byte sourceVal, Byte targetVal)
        {
            Int32 pxf = this.BitsPerPixel;
            Int32 maxSize = 1 << pxf;
            if (maxSize <= targetVal)
                throw new IndexOutOfRangeException("Byte value too large for " + pxf + " bit image!");
            this.ByteData = this.ByteData.Select(x => x == sourceVal? targetVal : x).ToArray();
        }

        public override String ToString()
        {
            return String.Format("{0}x{1} (Y={2}), {3} bytes", this.Width, this.Height, this.YOffset, this.ByteData == null ? 0 : this.ByteData.Length);
        }
        
        public Boolean Equals(FontFileSymbol other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            // left out bpp; it isn't really relevant since it should get set explicitly on any non-internal clone operation anyway.
            if (this.Width != other.Width || this.Height != other.Height || this.YOffset != other.YOffset)
                return false;
            return this.ByteData.SequenceEqual(other.ByteData);
        }
        
        /// <summary>
        /// Crop the image in Y-dimension and adjust the Y offset instead.
        /// This can not be performed on fonts that don't support Y-offset!
        /// </summary>
        /// <param name="yOffsetMax">Maximum value for Y-offset; related to how the Y is saved. Use -1 to ignore.</param>
        public void OptimizeYHeight(Int32 yOffsetMax)
        {
            if (yOffsetMax == 0)
                return;
            if (yOffsetMax < 0)
                yOffsetMax = Int32.MaxValue;
            Int32 height = this.Height;
            Int32 yoffSet = this.YOffset;
            this.ByteData = ImageUtils.OptimizeYHeight(this.ByteData, this.Width, ref height, ref yoffSet, true, this.TransparencyColor, yOffsetMax, true);
            this.Height = height;
            this.YOffset = yoffSet;
        }


        /// <summary>
        /// Crop the image in x-dimension and return the amount it was shifted to the left.
        /// This can not be performed on fonts that don't support custom width!
        /// </summary>
        /// <returns>The amound the image was shifted to the left.</returns>
        public Int32 OptimizeXWidth(Boolean alsoTrimRight)
        {
            Int32 width = this.Width;
            Int32 xoffSet = 0;
            this.ByteData = ImageUtils.OptimizeXWidth(this.ByteData, ref width, this.Height, ref xoffSet, alsoTrimRight, this.TransparencyColor, 0, true);
            this.Width = width;
            return xoffSet;
        }

        /// <summary>
        /// Crop the right side of the image in x-dimension.
        /// This can not be performed on fonts that don't support custom width!
        /// </summary>
        /// <returns>The amound the image was shifted to the left.</returns>
        public void CropRightSide()
        {
            Int32 width = this.Width;
            this.ByteData = ImageUtils.TrimXWidth(this.ByteData, ref width, this.Height, this.TransparencyColor);
            this.Width = width;
        }

        /// <summary>
        /// Crop the bottom side of the image in x-dimension.
        /// This can not be performed on fonts that don't support custom width!
        /// </summary>
        /// <returns>The amound the image was shifted to the left.</returns>
        public void CropBottomSide()
        {
            Int32 height = this.Height;
            this.ByteData = ImageUtils.TrimYHeight(this.ByteData, this.Width, ref height, this.TransparencyColor);
            this.Height= height;
        }

        internal static FontFileSymbol Combine(FontFileSymbol firstLayer, FontFileSymbol secondLayer, FontFile fontFile, Color[] transparencyGuide)
        {
            Int32 trueFcHeight = firstLayer.Height + firstLayer.YOffset;
            Int32 trueClHeight = secondLayer.Height+ secondLayer.YOffset;
            Int32 newWidth = Math.Max(secondLayer.Width, firstLayer.Width);
            Int32 newHeight = Math.Max(trueClHeight, trueFcHeight);
            Byte[] newSymbolData = new Byte[newWidth * newHeight];
            newSymbolData = ImageUtils.PasteOn8bpp(newSymbolData, newWidth, newHeight, newWidth, firstLayer.ByteData, firstLayer.Width, firstLayer.Height, firstLayer.Width,
                new Rectangle(0, firstLayer.YOffset, firstLayer.Width, firstLayer.Height), null, true);
            newSymbolData = ImageUtils.PasteOn8bpp(newSymbolData, newWidth, newHeight, newWidth, secondLayer.ByteData, secondLayer.Width, secondLayer.Height, secondLayer.Width,
                new Rectangle(0, secondLayer.YOffset, secondLayer.Width, secondLayer.Height), transparencyGuide, true);
            secondLayer = new FontFileSymbol(newSymbolData, newWidth, newHeight, 0, firstLayer.BitsPerPixel, fontFile.TransparencyColor);
            if (fontFile.YOffsetTypeMax != 0)
                secondLayer.OptimizeYHeight(fontFile.YOffsetTypeMax);
            return secondLayer.CloneFor(fontFile, fontFile.BitsPerPixel);
        }
    }

    public enum ShiftDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}