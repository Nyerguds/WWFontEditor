using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace Nyerguds.ImageManipulation
{
    public static class ImageUtils
    {
        public static void SaveImage(Bitmap image, String filename)
        {
            String ext = Path.GetExtension(filename);
            ImageFormat saveFormat = ImageFormat.Png;
            if (".bmp".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                saveFormat = ImageFormat.Bmp;
            else if (".gif".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                saveFormat = ImageFormat.Gif;
            else if (".jpg".Equals(ext, StringComparison.InvariantCultureIgnoreCase) || ".jpeg".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                saveFormat = ImageFormat.Jpeg;
            else if (!".png".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                filename += ".png";

            if (saveFormat.Guid == ImageFormat.Jpeg.Guid)
            {
                // What a mess just to have non-crappy jpeg. Scratch that; jpeg is always crappy.
                ImageCodecInfo jpegEncoder = null;
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
                Guid formatId = ImageFormat.Jpeg.Guid;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == formatId)
                    {
                        jpegEncoder = codec;
                        break;
                    }
                }
                System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encparams = new EncoderParameters(1);
                encparams.Param[0] = new EncoderParameter(qualityEncoder, 90L);
                image.Save(filename, jpegEncoder, encparams);
            }
            else
                image.Save(filename, saveFormat);
        }
        
        /// <summary>
        /// Loads an image without locking the underlying file.
        /// </summary>
        /// <param name="path">Path of the image to load</param>
        /// <returns>The image</returns>
        public static Bitmap LoadImageSafe(String path)
        {
            Byte[] fileData = File.ReadAllBytes(path);
            using (MemoryStream ms = new MemoryStream(fileData))
            {
                Bitmap bm = new Bitmap(ms);
                bm = new Bitmap(bm);
                ms.Close();
                return bm;
            }
        }

        /// <summary>
        /// Clones an image object.
        /// Code taken from http://stackoverflow.com/a/3661892/ with some extra fixes.
        /// </summary>
        /// <param name="sourceImage">The image to clone</param>
        /// <returns>The cloned image</returns>
        public static Bitmap CloneImage(Bitmap sourceImage)
        {
            Bitmap targetImage = new Bitmap(sourceImage.Width, sourceImage.Height, sourceImage.PixelFormat);
            BitmapData sourceData = sourceImage.LockBits(
                new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            BitmapData targetData = targetImage.LockBits(
                new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                ImageLockMode.WriteOnly, targetImage.PixelFormat);

            CopyMemory(targetData.Scan0, sourceData.Scan0, sourceData.Stride * sourceData.Height, 1024, 1024);

            sourceImage.UnlockBits(sourceData);
            targetImage.UnlockBits(targetData);
            // For 8-bit images, restore the palette. This is not linking to a referenced
            // object in the original image; the getter creates a new object when called.
            if (sourceImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                targetImage.Palette = sourceImage.Palette;
            return targetImage;
        }


        /// <summary>
        /// Creates a bitmap based on data, width, height, stride and pixel format.
        /// </summary>
        /// <param name="sourceData">Byte array of raw source data</param>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="stride">Scanline length inside the data</param>
        /// <param name="pixelFormat"></param>
        /// <param name="palette"></param>
        /// <returns>The new image</returns>
        public static Bitmap BuildImage(Byte[] sourceData, Int32 width, Int32 height, Int32 stride, PixelFormat pixelFormat, Color[] palette)
        {
            if (width == 0 || height == 0)
                return null;
            Bitmap newImage = new Bitmap(width, height, pixelFormat);
            BitmapData targetData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            CopyMemory(targetData.Scan0, sourceData, sourceData.Length, stride, targetData.Stride);
            newImage.UnlockBits(targetData);
            // For 8-bit images, set the palette.
            if ((pixelFormat == PixelFormat.Format8bppIndexed || pixelFormat == PixelFormat.Format4bppIndexed) && palette != null)
            {
                ColorPalette pal = newImage.Palette;
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                    if (i < palette.Length)
                    pal.Entries[i] = palette[i];
                newImage.Palette = pal;
            }
            return newImage;
        }

        public static void CopyMemory(IntPtr target, Byte[] sourceBytes, Int32 length, Int32 origStride, Int32 targetStride)
        {
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(sourceBytes.Length);
            Marshal.Copy(sourceBytes, 0, unmanagedPointer, sourceBytes.Length);
            CopyMemory(target, unmanagedPointer, length, origStride, targetStride);
            Marshal.FreeHGlobal(unmanagedPointer);
        }

        public static void CopyMemory(IntPtr target, IntPtr source, Int32 length, Int32 origStride, Int32 targetStride)
        {
            IntPtr sourcePos = source;
            IntPtr destPos = target;
            Int32 minStride = Math.Min(origStride, targetStride);
            Byte[] imageData = new Byte[targetStride];
            while (length >= origStride && length > 0)
            {
                Marshal.Copy(sourcePos, imageData, 0, minStride);
                Marshal.Copy(imageData, 0, destPos, targetStride);
                length -= origStride;
                sourcePos = new IntPtr(sourcePos.ToInt64() + origStride);
                destPos = new IntPtr(destPos.ToInt64() + targetStride);
            }
            if (length > 0)
            {
                Marshal.Copy(sourcePos, imageData, 0, length);
                Marshal.Copy(imageData, 0, destPos, length);
            }
        }

        public static Boolean HasTransparency(Bitmap bitmap)
        {
            // not an alpha-capable color format.
            if ((bitmap.Flags & (Int32)ImageFlags.HasAlpha) == 0)
                return false;
            // Indexed formats. Special case because one index on their palette is configured as THE transparent color.
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed || bitmap.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                ColorPalette pal = bitmap.Palette;
                // Find the transparent indexea on the palette.
                List<Int32> transCols = new List<Int32>();
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                {
                    Color col = pal.Entries[i];
                    if (col.A != 255)
                    {
                        // Color palettes should only have one index acting as transparency. Not sure if there's a better way of getting it...
                        transCols.Add(i);
                        break;
                    }
                }
                // none of the entries in the palette have transparency information.
                if (transCols.Count == 0)
                    return false;
                // Check pixels for existence of the transparent index.
                Int32 colDepth = Image.GetPixelFormatSize(bitmap.PixelFormat);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                Int32 stride = data.Stride;
                Byte[] bytes = new Byte[bitmap.Height * stride];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                bitmap.UnlockBits(data);
                if (colDepth == 8)
                {
                    // Last line index.
                    Int32 lineMax = bitmap.Width - 1;
                    for (Int32 i = 0; i < bytes.Length; i++)
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if (linepos > lineMax)
                            continue;
                        Byte b = bytes[i];
                        if (transCols.Contains(b))
                            return true;
                    }
                }
                else if (colDepth == 4)
                {
                    // line size in bytes. 1-indexed for the moment.
                    Int32 lineMax = bitmap.Width / 2;
                    // Check if end of line ends on half a byte.
                    Boolean halfByte = bitmap.Width % 2 != 0;
                    // If it ends on half a byte, one more needs to be processed.
                    // We subtract in the other case instead, to make it 0-indexed right away.
                    if (!halfByte)
                        lineMax--;
                    for (Int32 i = 0; i < bytes.Length; i++)
                    {
                        // Last position to process.
                        Int32 linepos = i % stride;
                        // Passed last image byte of the line. Abort and go on with loop.
                        if (linepos > lineMax)
                            continue;
                        Byte b = bytes[i];
                        if (transCols.Contains((b & 0x0F)))
                            return true;
                        if (halfByte && linepos == lineMax) // reached last byte of the line. If only half a byte to check on that, abort and go on with loop.
                            continue;
                        if (transCols.Contains((b & 0xF0) >> 4))
                            return true;
                    }
                }
                return false;
            }
            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
            {
                
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                Byte[] bytes = new Byte[bitmap.Height * data.Stride];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                bitmap.UnlockBits(data);
                for (Int32 p = 3; p < bytes.Length; p += 4)
                {
                    if (bytes[p] != 255)
                    {
                        return true;
                    }
                }
                return false;
            }

            // Final "screw it all" method. This is pretty slow, but it won't ever be used, unless you
            // encounter some really esoteric types not handled above, like 16bppArgb1555 and 64bppArgb.
            for (Int32 i = 0; i < bitmap.Width; i++)
            {
                for (Int32 j = 0; j < bitmap.Height; j++)
                {
                    if (bitmap.GetPixel(i, j).A != 255)
                        return true;
                }
            }
            return false;
        }

        private static Color[] GeneratePalette(Color[] colors, Color def)
        {
            Color[] pal = new Color[256];
            for (Int32 i = 0; i < pal.Length; i++)
                if (i < colors.Length)
                    pal[i] = colors[i];
                else
                    pal[i] = def;
            return pal;
        }

        public static Bitmap GenerateBlankImage(Int32 width, Int32 height, Color[] colors, Byte paintColor)
        {
            if (width == 0 || height == 0)
                return null;
            Color[] pal = GeneratePalette(colors, Color.Empty);
            Byte[] blankArray = new Byte[width * height];
            if (paintColor != 0)
                for (Int32 i = 0; i < blankArray.Length; i++)
                    blankArray[i] = paintColor;
            return BuildImage(blankArray, width, height, width, PixelFormat.Format8bppIndexed, pal);
        }

        public static Bitmap GenerateCheckerboardImage(Int32 width, Int32 height, Color[] colors, Byte color1, Byte color2)
        {
            if (width == 0 || height == 0)
                return null;
            Color[] pal = GeneratePalette(colors, Color.Empty);
            Byte[] patternArray = new Byte[width * height];
            for (Int32 y = 0; y < width; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    Int32 offset = x + y * height;
                    patternArray[offset] = (Byte)(((x + y) % 2 == 0) ? 1 : 0);
                }
            }
            return BuildImage(patternArray, width, height, width, PixelFormat.Format8bppIndexed, pal);
        }

        public static Bitmap GenerateGridImage(Int32 origWidth, Int32 origHeight, Int32 zoomFactor, Color[] colors, Byte bgColor, Byte gridcolor, Byte outLineColor)
        {
            if (zoomFactor <= 0)
                throw new ArgumentOutOfRangeException("zoomFactor");
            Color[] pal = GeneratePalette(colors, Color.Empty);
            Int32 width1 = origWidth * zoomFactor;
            Int32 height1 = origHeight * zoomFactor;
            Int32 width = width1 + 1;
            Int32 height = height1 + 1;
            if (width == 0 || height == 0)
                return null;
            Byte[] patternArray = new Byte[width * height];
            if (bgColor != 0)
                for (Int32 i = 0; i < patternArray.Length; i++)
                    patternArray[i] = bgColor;
            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    Int32 offset = x + y * width;
                    if (x == 0 || x == width1 || y == 0 || y == height1)
                        patternArray[offset] = outLineColor;
                    else if (x % zoomFactor == 0 || y % zoomFactor == 0)
                        patternArray[offset] = gridcolor;
                }
            }
            return BuildImage(patternArray, width, height, width, PixelFormat.Format8bppIndexed, pal);
        }

        /// <summary>
        ///     Gets an 8-bit image's internal byte array for editing, executes a given function with that data, and writes the edited array back to the image afterwards.
        /// </summary>
        /// <param name="source">The source image</param>
        /// <param name="editDelegate">A delegate to edit the resulting byte array, with the byte array's stride as second argument.</param>
        public static void EditRawImageBytes(Bitmap source, Action<Byte[], Int32> editDelegate)
        {
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            // Could technically design this to edit the bytes directly instead of copying, but this way doesn't require (technically) unsafe code.
            Byte[] picData = new Byte[sourceData.Stride * sourceData.Height];
            Int32 sourceStride = sourceData.Stride;
            Marshal.Copy(sourceData.Scan0, picData, 0, picData.Length);
            source.UnlockBits(sourceData);
            // =======================================
            // Call delegate function to perform the actual actions.
            editDelegate(picData, sourceStride);
            // =======================================
            BitmapData destData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.WriteOnly, source.PixelFormat);
            CopyMemory(destData.Scan0, picData, picData.Length, sourceStride, destData.Stride);
            source.UnlockBits(destData);
        }

        public static void DrawRect8Bit(Bitmap source, Int32 startX, Int32 startY, Int32 endX, Int32 endY, Byte colorIndex, Boolean fill)
        {
            if (source.PixelFormat != PixelFormat.Format8bppIndexed)
                return;
            EditRawImageBytes(source, (arr, stride) => DrawRect8Bit(arr, source.Width, source.Height, stride, startX, startY, endX, endY, colorIndex, fill));
        }

        public static void DrawRect8Bit(Byte[] dataArray, Int32 width, Int32 height, Int32 stride, Int32 startX, Int32 startY, Int32 endX, Int32 endY, Byte colorIndex, Boolean fill)
        {
            // Switch incorrect start and end positions
            if (startX > endX)
            {
                Int32 tmp = startX;
                startX = endX;
                endX = tmp;
            }
            if (startY > endY)
            {
                Int32 tmp = startY;
                startY = endY;
                endY = tmp;
            }
            // Check if bounds are completely outside image
            if ((startX < 0 && endX < 0) || (startX >= width && endX >= width)
                || (startY < 0 && endY < 0) || (startY >= height && endX >= height))
                return;
            // Restrict bounds to image.
            Int32 maxw = width - 1;
            Int32 maxh = height - 1;
            startX = Math.Min(maxw, Math.Max(0, startX));
            endX = Math.Min(maxw, Math.Max(0, endX));
            startY = Math.Min(maxh, Math.Max(0, startY));
            endY = Math.Min(maxh, Math.Max(0, endY));
            for (Int32 y = startY; y <= endY; y++)
            {
                if (fill)
                {
                    for (Int32 x = startX; x <= endX; x++)
                        dataArray[x + y * stride] = colorIndex;
                }
                else
                {
                    if (y == startY || y == endY)
                        for (Int32 x = startX; x <= endX; x++)
                            dataArray[x + y * stride] = colorIndex;
                    else
                    {
                        dataArray[startX + y * stride] = colorIndex;
                        dataArray[endX + y * stride] = colorIndex;
                    }
                }
            }
        }

        public static Int32 GetMinStride(Int32 width, Int32 bitsLength)
        {
            // Amount of bytes to read per width
            Int32 stride = bitsLength * width;
            stride = (stride / 8) + ((stride % 8) > 0 ? 1 : 0);
            return stride;
        }

        public static Byte[] CopyFrom8bpp(Byte[] fileData, Int32 width, Int32 height, ref Int32 stride, Rectangle copyArea)
        {
            Byte[] copiedPicture = new Byte[copyArea.Width * copyArea.Height];
            Int32 maxY = Math.Min(height - copyArea.Y, copyArea.Height);
            Int32 maxX = Math.Min(width - copyArea.X, copyArea.Width);

            for (Int32 y = 0; y < maxY; y++)
            {
                for (Int32 x = 0; x < maxX; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexSource = (copyArea.Y + y) * stride + copyArea.X + x;
                    // This will always get a new index
                    Int32 indexDest = y * copyArea.Width + x;
                    copiedPicture[indexDest] = fileData[indexSource];
                }
            }
            stride = copyArea.Width;
            return copiedPicture;
        }

        public static Byte[] PasteOn8bpp(Byte[] fileData1, Int32 width1, Int32 height1, Int32 stride1, Byte[] fileData2, Int32 width2, Int32 height2, Int32 stride2, Rectangle targetPos, Color[] transparencyGuide)
        {
            if (targetPos.Width != width2 || targetPos.Height != height2)
                fileData2 = CopyFrom8bpp(fileData2, width2, height2, ref stride2, new Rectangle(0, 0, targetPos.Width, targetPos.Height));

            Byte[] finalFileData = new Byte[fileData1.Length];
            Array.Copy(fileData1, finalFileData, fileData1.Length);

            Boolean[] isTransparent = new Boolean[256];
            if (transparencyGuide != null)
            {
                Int32 len = Math.Min(isTransparent.Length, transparencyGuide.Length);
                for (Int32 i = 0; i < len; i++)
                    isTransparent[i] = transparencyGuide[i].A < 128;
            }
            Int32 maxY = Math.Min(height1 - targetPos.Y, targetPos.Height);
            Int32 maxX = Math.Min(width1 - targetPos.X, targetPos.Width);
            for (Int32 y = 0; y < maxY; y++)
            {
                for (Int32 x = 0; x < maxX; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexDest = (targetPos.Y + y) * stride1 + targetPos.X + x;
                    // This will always get a new index
                    Int32 indexSource = y * targetPos.Width + x;
                    Byte data = fileData2[indexSource];
                    if (!isTransparent[data])
                        finalFileData[indexDest] = data;
                }
            }
            return finalFileData;
        }

        /// <summary>
        /// Converts given raw image data for a paletted image to 8-bit, so we have a simple one-byte-per-pixel format to work with.
        /// Stride is assumed to be the minimum needed to contain the data. Output stride will be the same as the width.
        /// </summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="start">Start offset of the image data in the fileData parameter.</param>
        /// <param name="bitsLength">Amount of bits used by one pixel.</param>
        /// <param name="bigEndian">True if the bits in the original image data are stored as big-endian.</param>
        /// <returns>The image data in a 1-byte-per-pixel format, with a stride exactly the same as the width.</returns>
        public static Byte[] ConvertTo8Bit(Byte[] fileData, Int32 width, Int32 height, Int32 start, Int32 bitsLength, Boolean bigEndian)
        {
            Int32 stride = GetMinStride(width, bitsLength);
            return ConvertTo8Bit(fileData, width, height, start, bitsLength, bigEndian, ref stride);
        }

        /// <summary>
        /// Converts given raw image data for a paletted image to 8-bit, so we have a simple one-byte-per-pixel format to work with.
        /// </summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="start">Start offset of the image data in the fileData parameter.</param>
        /// <param name="bitsLength">Amount of bits used by one pixel.</param>
        /// <param name="bigEndian">True if the bits in the original image data are stored as big-endian.</param>
        /// <param name="stride">Stride used in the original image data. Will be adjusted to the new stride value.</param>
        /// <returns>The image data in a 1-byte-per-pixel format, with a stride exactly the same as the width.</returns>
        public static Byte[] ConvertTo8Bit(Byte[] fileData, Int32 width, Int32 height, Int32 start, Int32 bitsLength, Boolean bigEndian, ref Int32 stride)
        {
            if (bitsLength != 1 && bitsLength != 2 && bitsLength != 4 && bitsLength != 8)
                throw new ArgumentOutOfRangeException("Cannot handle image data with " + bitsLength + "bits per pixel.");
            // Full array
            Byte[] data8bit = new Byte[width * height];
            // Amount of runs that end up on the same pixel
            Int32 parts = 8 / bitsLength;
            // Amount of bytes to read per width
            Int32 newStride = width;
            // Bit mask for reducing read and shifted data to actual bits length
            Int32 bitmask = (1 << bitsLength) - 1;
            Int32 size = stride * height;
            // File check, and getting actual data.
            if (start + size > fileData.Length)
                throw new IndexOutOfRangeException("Data exceeds array bounds!");
            // Actual conversion porcess.
            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexXbit = start + y * stride + x / parts;
                    // This will always get a new index
                    Int32 index8bit = y * newStride + x;
                    // Amount of bits to shift the data to get to the current pixel data
                    Int32 shift = (x % parts) * bitsLength;
                    // Reversed for big-endian
                    if (bigEndian)
                        shift = 8 - shift - bitsLength;
                    // Get data and store it.
                    data8bit[index8bit] = (Byte)((fileData[indexXbit] >> shift) & bitmask);
                }
            }
            stride = newStride;
            return data8bit;
        }

        /// <summary>
        /// Converts given raw image data for a paletted 8-bit image to lower amount of bits per pixel.
        /// Stride is assumed to be the same as the width. Output stride is the minimum needed to contain the data.
        /// </summary>
        /// <param name="data8bit">The eight bit per pixel image data</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="bitsLength">The new amount of bits per pixel</param>
        /// <param name="bigEndian">True if the bits in the new image data are to be stored as big-endian.</param>
        /// <returns>The image data converted to the requested amount of bits per pixel.</returns>
        public static Byte[] ConvertFrom8Bit(Byte[] data8bit, Int32 width, Int32 height, Int32 bitsLength, Boolean bigEndian)
        {
            Int32 stride = width;
            return ConvertFrom8Bit(data8bit, width, height, bitsLength, bigEndian, ref stride);
        }

        /// <summary>
        /// Converts given raw image data for a paletted 8-bit image to lower amount of bits per pixel.
        /// </summary>
        /// <param name="data8bit">The eight bit per pixel image data</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="bitsLength">The new amount of bits per pixel</param>
        /// <param name="bigEndian">True if the bits in the new image data are to be stored as big-endian.</param>
        /// <param name="stride">Stride used in the original image data. Will be adjusted to the new stride value.</param>
        /// <returns>The image data converted to the requested amount of bits per pixel.</returns>
        public static Byte[] ConvertFrom8Bit(Byte[] data8bit, Int32 width, Int32 height, Int32 bitsLength, Boolean bigEndian, ref Int32 stride)
        {
            Int32 parts = 8 / bitsLength;
            // Amount of bytes to write per width
            Int32 newStride = GetMinStride(width, bitsLength);
            // Bit mask for reducing original data to actual bits maximum.
            // Should not be needed if data is correct, but eh.
            Int32 bitmask = (1 << bitsLength) - 1;
            Byte[] dataXbit = new Byte[newStride * height];
            // Actual conversion porcess.
            for (Int32 y = 0; y < height; y++)
            {
                for (Int32 x = 0; x < width; x++)
                {
                    // This will hit the same byte multiple times
                    Int32 indexXbit = y * newStride + x / parts;
                    // This will always get a new index
                    Int32 index8bit = y * stride + x;
                    // Amount of bits to shift the data to get to the current pixel data
                    Int32 shift = (x % parts) * bitsLength;
                    // Reversed for big-endian
                    if (bigEndian)
                        shift = 8 - shift - bitsLength;
                    // Get data, reduce to bit rate, shift it and store it.
                    dataXbit[indexXbit] |= (Byte)((data8bit[index8bit] & bitmask) << shift);
                }
            }
            stride = newStride;
            return dataXbit;
        }

    }
}
