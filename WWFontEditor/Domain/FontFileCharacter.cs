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
    public class FontFileCharacter
    {
        public Byte[] ByteData { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }
        public Int32 YOffset { get; set; }
        public Int32 BitsPerPixel { get; set; }


        public FontFileCharacter(Int32 bitsPerPixel)
        {
            this.ByteData = new Byte[0];
            this.BitsPerPixel = bitsPerPixel;
        }

        public FontFileCharacter(Byte[] byteData, Int32 width, Int32 height, Int32 yOffset, Int32 bitsPerPixel)
        {
            this.ByteData = byteData;
            this.Width = width;
            this.Height = height;
            this.YOffset = yOffset;
            this.BitsPerPixel = bitsPerPixel;
        }

        public FontFileCharacter Clone()
        {
            return new FontFileCharacter(this.ByteData.ToArray(), this.Width, this.Height, this.YOffset, this.BitsPerPixel);
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
                Byte fill = (Byte)(wrap ? newSource[(left ? 0 : length)] : 0);
                Array.Copy(newSource, i + srcStart, source, i + tarStart, length);
                // clear shifted pixel
                source[i + (stride - 1) * srcStart] = fill;
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
    }

    public enum ShiftDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}