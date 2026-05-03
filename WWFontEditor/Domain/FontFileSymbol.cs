using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ColorManipulation;

namespace WWFontEditor.Domain
{
    // For real clipboard support :)
    // Further info: http://stackoverflow.com/questions/9032673/clipboard-copying-objects-to-and-from
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{ToString()}")]
    public class FontFileSymbol
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
            this.ByteData = byteData;
            this.Width = width;
            this.Height = height;
            this.YOffset = yOffset;
            this.BitsPerPixel = bitsPerPixel;
        }

        public FontFileSymbol Clone()
        {
            return new FontFileSymbol(this.ByteData.ToArray(), this.Width, this.Height, this.YOffset, this.BitsPerPixel);
        }

        public FontFileSymbol CloneFor(FontFile targetVersion)
        {
            return CloneFor(targetVersion, null);
        }

        public FontFileSymbol CloneFor(FontFile targetVersion, Byte? defaultValue)
        {
            // PART ONE: COLOR CONVERSION
            // If higher bitrate, convert overflow to default if given.

            Int32 myBpp = this.BitsPerPixel;
            Byte[] newByteData;
            Int32 targetBpp = targetVersion.BitsPerPixel;
            Int32 colValLimit = (Int32)Math.Pow(2, targetBpp);
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
            if (!targetVersion.CustomSymbSizesForType)
                newSymbol.ChangeHeight(targetVersion.FontHeight);
            // Reduce width if needed
            if (targetVersion.FontWidth < newSymbol.Width || !targetVersion.CustomSymbSizesForType)
                newSymbol.ChangeWidth(targetVersion.FontWidth);
            if (targetVersion.FontWidthTypeMin > newSymbol.Width)
                newSymbol.ChangeWidth(targetVersion.FontWidthTypeMin);
            else if (targetVersion.FontWidthTypeMax < newSymbol.Width)
                newSymbol.ChangeWidth(targetVersion.FontWidthTypeMax);

            // IF all sizes in the font are fixed (V1, V2) expand symbol to full size.
            // At this point, this should only increase the size.
            if (!targetVersion.CustomSymbSizesForType)
            {
                if (targetVersion.FontWidth != newSymbol.Width)
                    newSymbol.ChangeWidth(targetVersion.FontWidth);
                if (targetVersion.FontHeight != newSymbol.Height)
                    newSymbol.ChangeWidth(targetVersion.FontHeight);
            }
            return newSymbol;
        }

        public Bitmap GetBitmapFullSize(ColorPalette palette, FontFile baseFont)
        {
            FontFileSymbol ffs = this.Clone();
            ffs.ChangeHeight(baseFont.FontHeight);
            for (Int32 i = 0; i < ffs.YOffset; i++)
                ffs.ShiftImageData(ShiftDirection.Down, false);
            return ffs.GetBitmap(palette);
        }

        public Bitmap GetBitmap(ColorPalette palette)
        {
            PixelFormat pf = PixelFormat.Format8bppIndexed;
            Int32 width = this.Width;
            Int32 height = this.Height;
            if (width == 0 || height == 0)
                return null;
            Byte[] imageData = this.ByteData;
            if (imageData.Length == 0 || width == 0 | height == 0)
                return null;
            return ImageUtils.BuildImage(imageData, width, height, width, pf, palette);
        }

        public void PaintPixel(Int32 x, Int32 y, Byte value)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return; // Ignore without error. Might accidentally occur when dragging or something I guess.
            Int32 pxf = this.BitsPerPixel;
            Int32 maxSize = (Int32)Math.Pow(2, pxf);
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
            Int32 maxSize = (Int32)Math.Pow(2, pxf);
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

    }

    public enum ShiftDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}