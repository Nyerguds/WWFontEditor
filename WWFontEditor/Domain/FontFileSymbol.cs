using System;
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
        /// <summary>Only use this for initialisation! Use ChangeWidth for editing the image!</summary>
        public Int32 Width { get; set; }
        /// <summary>Only use this for initialisation! Use ChangeHeight for editing the image!</summary>
        public Int32 Height { get; set; }
        public Int32 YOffset { get; set; }
        public Int32 BitsPerPixel { get; private set; }
        
        public FontFileSymbol(Int32 bitsPerPixel)
        {
            this.ByteData = new Byte[0];
            this.BitsPerPixel = bitsPerPixel;
        }
        public FontFileSymbol(Image image, Color[] palette, FontFile source)
        {
            this.BitsPerPixel = source.BitsPerPixel; 
            this.Width = Math.Min(source.FontWidth, image.Width);
            this.Height = Math.Min(source.FontHeight, image.Height);
            this.ByteData = new Byte[Width * Height];
            Bitmap srcImage = new Bitmap(image);
            for (Int32 y = 0; y < Height; y++)
            {
                for (Int32 x = 0; x < Width; x++)
                {
                    Color col = srcImage.GetPixel(x, y);
                    this.ByteData[y * Width + x] = (Byte)ColorUtils.GetClosestPaletteIndexMatch(col, palette, null);
                }
            }
        }

        public FontFileSymbol(Byte[] byteData, Int32 width, Int32 height, Int32 yOffset, Int32 bitsPerPixel)
        {
            this.ByteData = new Byte[byteData.Length];
            Array.Copy(byteData, 0, this.ByteData, 0, byteData.Length);
            this.Width = width;
            this.Height = height;
            this.YOffset = yOffset;
            this.BitsPerPixel = bitsPerPixel;
        }

        public FontFileSymbol Clone()
        {
            return new FontFileSymbol(this.ByteData.ToArray(), this.Width, this.Height, this.YOffset, this.BitsPerPixel);
        }

        public FontFileSymbol CloneFor(FontFile targetVersion, Int32 targetBpp)
        {
            return CloneFor(targetVersion, null, targetBpp);
        }

        public Boolean HasTooHighDataFor(Int32 bitsPerPixel)
        {
            // shouldn't. Let's assume that's implemented correctly ;)
            if (this.BitsPerPixel <= bitsPerPixel)
                return false;
            Int32 colValLimit = 1 << bitsPerPixel;
            return this.ByteData.Any(x => x >= colValLimit);
        }

        public FontFileSymbol CloneFor(FontFile targetVersion, Byte? defaultValue, Int32 targetBpp)
        {
            // PART ONE: COLOR CONVERSION
            // If higher bitrate, convert overflow to default if given.
            Byte[] newByteData = ConvertDataToBpp(defaultValue, targetBpp);

            FontFileSymbol newSymbol = new FontFileSymbol(newByteData, this.Width, this.Height, this.YOffset, targetBpp);

            // PART TWO: SIZE ADJUSTMENT

            if (targetVersion.FontHeight == newSymbol.Height)
                newSymbol.YOffset = 0;
            else if (targetVersion.FontHeight < newSymbol.Height)
            {
                newSymbol.YOffset = 0;
                newSymbol.ChangeHeight(targetVersion.FontHeight);
            }
            else if (targetVersion.FontHeight < newSymbol.Height + newSymbol.YOffset)
            {
                // target has enough space, but Y is too large.
                newSymbol.YOffset = targetVersion.FontHeight - newSymbol.Height;
            }
            // If there is no suppport for Y, reduce Y to 0 and shift down symbol.
            if (targetVersion.YOffsetTypeMax == 0 && newSymbol.YOffset > 0)
            {
                // Increase size of image
                if (newSymbol.Height < targetVersion.FontHeight)
                    newSymbol.ChangeHeight(Math.Min(targetVersion.FontHeight, newSymbol.Height + newSymbol.YOffset));
                // Shift down to Y offset
                for (int i = 0; i < newSymbol.YOffset; i++)
                    newSymbol.ShiftImageData(ShiftDirection.Down, false);
                // Remove Y offset; it's been replaced by actual offset
                newSymbol.YOffset = 0;
            }
            if (!targetVersion.CustomSymbYForType && targetVersion.FontHeight != newSymbol.Height)
                newSymbol.ChangeHeight(targetVersion.FontHeight);
            // Reduce width if needed
            if (targetVersion.FontWidth < newSymbol.Width || !targetVersion.CustomSymbXForType)
                newSymbol.ChangeWidth(targetVersion.FontWidth);
            if (targetVersion.FontWidthTypeMin > newSymbol.Width)
                newSymbol.ChangeWidth(targetVersion.FontWidthTypeMin);
            else if (targetVersion.FontWidthTypeMax < newSymbol.Width)
                newSymbol.ChangeWidth(targetVersion.FontWidthTypeMax);

            // If all sizes in the font are fixed (V1, V2) expand symbol to full size.
            // At this point, this should only increase the size.
            if (!targetVersion.CustomSymbXForType && targetVersion.FontWidth != newSymbol.Width)
                newSymbol.ChangeWidth(targetVersion.FontWidth);
            return newSymbol;
        }

        public void ConvertToBpp(Byte? defaultValue, Int32 targetBpp)
        {
            if (this.BitsPerPixel == targetBpp)
                return;
            this.ByteData = ConvertDataToBpp(defaultValue, targetBpp);
            this.BitsPerPixel = targetBpp;
        }

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

        public Bitmap GetBitmapFullSize(Color[] palette, FontFile baseFont)
        {
            FontFileSymbol ffs = this.Clone();
            ffs.ChangeHeight(baseFont.FontHeight);
            for (Int32 i = 0; i < ffs.YOffset; i++)
                ffs.ShiftImageData(ShiftDirection.Down, false);
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
            return ImageUtils.BuildImage(imageData, width, height, width, PixelFormat.Format8bppIndexed, palette);
        }

        public void PaintPixel(Int32 x, Int32 y, Byte value)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return; // Ignore without error. Might accidentally occur when dragging or something I guess.
            Int32 pxf = this.BitsPerPixel;
            Int32 maxSize = 1 << pxf;
            if (maxSize <= value)
                throw new IndexOutOfRangeException("Byte value too large for " + pxf + " bit image!");
            this.ByteData[y * Width + x] = value;
        }

        public Byte GetPixelValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return 0; // Ignore without error. Might accidentally occur when dragging or something I guess.
            return this.ByteData[y * Width + x];
        }

        public void ChangeHeight(Int32 newHeight)
        {
            Byte[] newData = new Byte[this.Width * newHeight];
            Array.Copy(this.ByteData, 0, newData, 0, Math.Min(this.ByteData.Length, newData.Length));
            this.ByteData = newData;
            this.Height = newHeight;
        }

        public void ChangeWidth(Int32 newWidth)
        {
            Byte[] newData = ChangeStride(this.ByteData, this.Width, this.Height, newWidth, false);
            this.ByteData = newData;
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
                    ChangeHeight(this.Height + 1);
                    this.YOffset--;
                    return false;
                case ShiftDirection.Down:
                    if (this.Height >= maxHeight)
                        return false;
                    ChangeHeight(this.Height + 1);
                    break;
                case ShiftDirection.Left:
                    // can't expand to the left.
                    return false;
                case ShiftDirection.Right:
                    if (this.Width >= maxWidth)
                        return false;
                    ChangeWidth(this.Width + 1);
                    break;
            }
            return true;
        }

        public void ShiftImageData(ShiftDirection direction, Boolean wrap)
        {
            if (ByteData.Length == 0)
                return;
            switch (direction)
            {
                case ShiftDirection.Up:
                case ShiftDirection.Down:
                    ShiftRowVert(this.ByteData, this.Width, direction == ShiftDirection.Up, wrap);
                    break;
                case ShiftDirection.Left:
                case ShiftDirection.Right:
                    ShiftRowHor(this.ByteData, this.Width, direction == ShiftDirection.Left, wrap);
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

        private static void ShiftRowVert(Byte[] source, Int32 stride, Boolean up, Boolean wrap)
        {
            Byte[] newSource = source.ToArray();
            Byte[] emptyRow = new Byte[stride];
            Int32 length = source.Length - stride;
            Int32 srcStart = up ? stride : 0;
            Int32 tarStart = up ? 0 : stride;
            if (wrap)
                Array.Copy(source, up ? 0 : length, emptyRow, 0, stride);
            Array.Copy(newSource, srcStart, source, tarStart, length);
            // clear shifted row
            Array.Copy(emptyRow, 0, source, up ? length : 0, stride);
        }

        private static void ShiftRowHor(Byte[] source, Int32 stride, Boolean left, Boolean wrap)
        {
            Byte[] newSource = source.ToArray();
            Int32 length = stride -1;
            Int32 srcStart = left ? 1 : 0;
            Int32 tarStart = left ? 0 : 1;
            for (Int32 i = 0; i < source.Length; i += stride)
            {
                Byte fill = (Byte)(wrap ? newSource[i + (left ? 0 : length)] : 0);
                Array.Copy(newSource, i + srcStart, source, i + tarStart, length);
                // clear shifted pixel
                source[i + length * srcStart] = fill;
            }
        }

        private static Byte[] ChangeStride(Byte[] source, Int32 origStride, Int32 height, Int32 targetStride, Boolean fromLeft)
        {
            Int32 sourcePos = 0;
            Int32 destPos = 0;
            Int32 minStride = Math.Min(origStride, targetStride);
            Int32 length = source.Length;
            Byte[] target = new Byte[height * targetStride];
            Int32 diff = origStride - targetStride;
            while (length >= origStride && length > 0)
            {
                Int32 sourcePos1 = sourcePos;
                Int32 destPos1 = destPos;
                if (fromLeft)
                {
                    if (diff > 0)
                        sourcePos1 += diff;
                    else
                        destPos1 -= diff;
                }
                Array.Copy(source, sourcePos1, target, destPos1, minStride);
                length -= origStride;
                sourcePos += origStride;
                destPos += targetStride;
            }
            if (length > 0)
                Array.Copy(source, sourcePos, target, destPos, length);
            return target;
        }

        public override String ToString()
        {
            return String.Format("{0}x{1} (Y={2}), {3} bytes", this.Width, this.Height, this.YOffset, this.ByteData == null ? 0 : this.ByteData.Length);
        }
        
        public Boolean Equals(FontFileSymbol other)
        {
            // left out bpp; it isn't really relevant since it should get set explicitly on any non-internal clone operation anyway.
            if (this.Width != other.Width || this.Height != other.Height || this.YOffset != other.YOffset)
                return false;
            return this.ByteData.SequenceEqual(other.ByteData);
        }

        /// <summary>
        /// Crop the image in Y-dimension and adjust the Y offset instead.
        /// This can not be performed on fonts that don't support Y-offset!
        /// </summary>
        public void OptimizeYHeight()
        {
            Int32 addedY = 0;
            Int32 cutHeightBottom = 0;
            Byte[] tempArray = new Byte[Width];
            for (Int32 y = 0; y < Height; y ++)
            {
                Array.Copy(ByteData, Width * y, tempArray, 0, Width);
                if (tempArray.All(x => x == 0))
                    addedY++;
                else
                    break;
            }
            for (Int32 y = Height - 1; y >= this.YOffset + addedY; y--)
            {
                Array.Copy(ByteData, Width * y, tempArray, 0, Width);
                if (tempArray.All(x => x == 0))
                    cutHeightBottom++;
                else
                    break;
            }
            for (Int32 i = 0; i < addedY; i++)
                this.ShiftImageData(ShiftDirection.Up, false);
            this.ChangeHeight(this.Height - addedY - cutHeightBottom);
            // Optimization: no need to keep Y if data is empty.
            if (this.Height == 0)
                this.YOffset = 0;
            else
                this.YOffset += addedY;
        }

        internal static FontFileSymbol Combine(FontFileSymbol firstLayer, FontFileSymbol secondLayer, FontFile fontFile, Color[] transparencyGuide)
        {
            Int32 trueFcHeight = firstLayer.Height + firstLayer.YOffset;
            Int32 trueClHeight = secondLayer.Height+ secondLayer.YOffset;
            Int32 newWidth = Math.Max(secondLayer.Width, firstLayer.Width);
            Int32 newHeight = Math.Max(trueClHeight, trueFcHeight);
            Byte[] newSymbolData = new Byte[newWidth * newHeight];
            Color[] pal = transparencyGuide.ToArray();
            pal[0] = Color.FromArgb(0, pal[0]);
            newSymbolData = ImageUtils.PasteOn8bpp(newSymbolData, newWidth, newHeight, newWidth, firstLayer.ByteData, firstLayer.Width, firstLayer.Height, firstLayer.Width,
                new Rectangle(0, firstLayer.YOffset, firstLayer.Width, firstLayer.Height), null);
            newSymbolData = ImageUtils.PasteOn8bpp(newSymbolData, newWidth, newHeight, newWidth, secondLayer.ByteData, secondLayer.Width, secondLayer.Height, secondLayer.Width,
                new Rectangle(0, secondLayer.YOffset, secondLayer.Width, secondLayer.Height), pal);
            secondLayer = new FontFileSymbol(newSymbolData, newWidth, newHeight, 0, firstLayer.BitsPerPixel);
            if (fontFile.YOffsetTypeMax != 0)
                secondLayer.OptimizeYHeight();
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