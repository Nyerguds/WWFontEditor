using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ColorManipulation;

namespace WWFontEditor.Domain
{
    public class FontFileCharacter
    {
        public Byte[] ByteData { get; set; }
        public Byte Width { get; set; }
        public Byte Height { get; set; }
        public Byte YOffset { get; set; }

        public FontFileCharacter()
        {
            ByteData = new Byte[0];
        }

        public FontFileCharacter Clone()
        {
            FontFileCharacter clone = new FontFileCharacter();
            clone.Width = this.Width;
            clone.Height = this.Height;
            clone.YOffset = this.YOffset;
            clone.ByteData = this.ByteData.ToArray();
            return clone;
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

        public void PaintPixel(Int32 x, Int32 y, Byte value, PixelFormat sourcePixelFormat)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return; // Ignore without error. Might accidentally occur when dragging or something I guess.
            Int32 pxf = Image.GetPixelFormatSize(sourcePixelFormat);
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

        public void ChangeHeight(Byte newHeight)
        {
            Byte[] newData = new Byte[this.Width * newHeight];
            Array.Copy(this.ByteData, 0, newData, 0, Math.Min(this.ByteData.Length, newData.Length));
            this.ByteData = newData;
            this.Height = newHeight;
        }

        public void ChangeWidth(Byte newWidth)
        {
            Byte[] newData = ChangeStride(this.ByteData, this.Width, this.Height, newWidth, false);
            this.ByteData = newData;
            this.Width = newWidth;
        }

        public void ShiftImageData(ShiftDirection direction)
        {
            if (ByteData.Length == 0)
                return;
            switch (direction)
            {
                case ShiftDirection.Up:
                case ShiftDirection.Down:
                    ShiftRowVert(this.ByteData, this.Width, direction == ShiftDirection.Up);
                    break;
                case ShiftDirection.Left:
                case ShiftDirection.Right:
                    ShiftRowHor(this.ByteData, this.Width, direction == ShiftDirection.Left);
                    break;
            }
        }

        private static void ShiftRowVert(Byte[] source, Int32 stride, Boolean up)
        {
            Byte[] newSource = source.ToArray();
            Byte[] emptyRow = new Byte[stride];
            Int32 length = source.Length - stride;
            Int32 srcStart = up ? stride : 0;
            Int32 tarStart = up ? 0 : stride;
            Array.Copy(newSource, srcStart, source, tarStart, length);
            // clear shifted row
            Array.Copy(emptyRow, 0, source, up ? length : 0, stride);
        }

        private static void ShiftRowHor(Byte[] source, Int32 stride, Boolean left)
        {
            Byte[] newSource = source.ToArray();
            Int32 length = stride -1;
            Int32 srcStart = left ? 1 : 0;
            Int32 tarStart = left ? 0 : 1;
            for (Int32 i = 0; i < source.Length; i += stride)
            {
                Array.Copy(newSource, i + srcStart, source, i + tarStart, length);
                // clear shifted pixel
                source[i + (stride - 1) * srcStart] = 0;
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
            while (length > targetStride)
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