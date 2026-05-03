using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ColorManipulation
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

            if (saveFormat == ImageFormat.Jpeg)
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
                encparams.Param[0] = new EncoderParameter(qualityEncoder, 100L);
                image.Save(filename, jpegEncoder, encparams);
            }
            else
                image.Save(filename, saveFormat);
        }
        
        /// <summary>
        /// Loads an image without locking the underlying file.
        /// Code taken from http://stackoverflow.com/a/3661892/
        /// </summary>
        /// <param name="path">Path of the image to load</param>
        /// <returns>The image</returns>
        public static Bitmap LoadImageSafe(String path)
        {
            using (Bitmap sourceImage = (Bitmap)Image.FromFile(path))
            {
                return CloneImage(sourceImage);
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

        public static PixelFormat GetPalettedFormat(Int32 bitsPerPixel)
        {
            switch (bitsPerPixel)
            {
                case 1:
                    return PixelFormat.Format1bppIndexed;
                case 4:
                    return PixelFormat.Format4bppIndexed;
                case 8:
                    return PixelFormat.Format8bppIndexed;
            }
            throw new NotSupportedException("No indexed PixelFormat available for " + bitsPerPixel + " bpp.");
        }

        public static ColorPalette MakePalette(Color[] sourcePalette, Int32 bitsPerPixel, Boolean addTransparentZero)
        {
            PixelFormat pixelFormat = GetPalettedFormat(bitsPerPixel);
            return MakePalette(sourcePalette, pixelFormat, addTransparentZero, null);
        }

        public static ColorPalette MakePalette(Color[] sourcePalette, Int32 bitsPerPixel, Boolean addTransparentZero, Color? defaultColor)
        {
            PixelFormat pixelFormat = GetPalettedFormat(bitsPerPixel);
            return MakePalette(sourcePalette, pixelFormat, addTransparentZero, defaultColor);
        }

        public static ColorPalette MakePalette(Color[] sourcePalette, PixelFormat pixelFormat, Boolean addTransparentZero)
        {
            return MakePalette(sourcePalette, pixelFormat, addTransparentZero, null);
        }

        public static ColorPalette MakePalette(Color[] sourcePalette, PixelFormat pixelFormat, Boolean addTransparentZero, Color? defaultColor)
        {
            ColorPalette pal = new Bitmap(10, 10, pixelFormat).Palette;
            for (Int32 i = 0; i < pal.Entries.Length; i++)
            {
                if (sourcePalette != null && i < sourcePalette.Length)
                    pal.Entries[i] = sourcePalette[i];
                else if (defaultColor.HasValue)
                    pal.Entries[i] = defaultColor.Value;
            }
            // make color 0 transparent
            if (addTransparentZero)
                pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;
        }

        public static ColorPalette GenerateGrayPalette(PixelFormat pixelFormat, Boolean addTransparentZero, Boolean reverseGenerated)
        {
            ColorPalette pal = new Bitmap(10, 10, pixelFormat).Palette;
            Int32 palSize = 1 << Image.GetPixelFormatSize(pixelFormat);
            // generate greyscale palette.
            Int32 steps = 255 / (palSize - 1);
            for (Int32 i = 0; i < pal.Entries.Length; i++)
            {
                Double curval = reverseGenerated ? pal.Entries.Length - 1 - i : i;
                Byte grayval = (Byte)Math.Min(255, Math.Round(curval * steps, MidpointRounding.AwayFromZero));
                pal.Entries[i] = Color.FromArgb(255, grayval, grayval, grayval);
            }
            // make color 0 transparent
            if (addTransparentZero)
                pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;
        }

        public static ColorPalette GenerateDefFourBitPalette(Boolean addTransparentZero, Boolean reverseGenerated)
        {
            ColorPalette pal = new Bitmap(10, 10, PixelFormat.Format4bppIndexed).Palette;
            if (reverseGenerated)
            {
                Color[] entries = pal.Entries.Reverse().ToArray();
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                    pal.Entries[i] = entries[i];
            }
            // remove transparency
            for (Int32 i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(0xFF, pal.Entries[i]);
            // make color 0 transparent
            if (addTransparentZero)
                pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;
        }

        public static ColorPalette GenerateDoubleRainbow(Boolean blackOnZero, Boolean addTransparentZero, Boolean reverseGenerated)
        {
            Color[] col = GenerateRainbowPalette(4, false, true, addTransparentZero, false).Entries;
            ColorPalette pal = GenerateRainbowPalette(8, true, blackOnZero, addTransparentZero, false);
            Array.Copy(col,0, pal.Entries, 0, col.Length);
            if (reverseGenerated)
            {
                Color[] entries = pal.Entries.Reverse().ToArray();
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                    pal.Entries[i] = entries[i];
            }
            return pal;
        }

        public static ColorPalette GenerateRainbowPalette(Int32 bpp, Boolean keepWin16Pal, Boolean blackOnZero, Boolean addTransparentZero, Boolean reverseGenerated)
        {
            ColorPalette pal = new Bitmap(10, 10, GetPalettedFormat(bpp)).Palette;
            Int32 colors = 1 << bpp;
            if (keepWin16Pal)
                colors -= 16;
            if (blackOnZero && !keepWin16Pal)
            {
                colors--;
                pal.Entries[0] = Color.Black;
            }
            Double step = 1.0 / colors * ColorHSL.SCALE;
            Double satValue = ColorHSL.SCALE;
            Double lumValue = 0.5 * ColorHSL.SCALE;
            for (Int32 i = 0; i < colors; i++)
            {
                Double curStep = step * i;
                pal.Entries[keepWin16Pal ? 16 + i : blackOnZero ? 1 + i : i] = new ColorHSL(curStep, satValue, lumValue);
            }
            if (reverseGenerated)
            {
                Color[] entries = pal.Entries.Reverse().ToArray();
                for (Int32 i = 0; i < pal.Entries.Length; i++)
                    pal.Entries[i] = entries[i];
            }
            // remove transparency
            for (Int32 i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(0xFF, pal.Entries[i]);
            // make color 0 transparent
            if (addTransparentZero)
                pal.Entries[0] = Color.FromArgb(0, pal.Entries[0]);
            return pal;
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
        public static Bitmap BuildImage(Byte[] sourceData, Int32 width, Int32 height, Int32 stride, PixelFormat pixelFormat, ColorPalette palette)
        {
            if (width == 0 || height == 0)
                return null;
            Bitmap newImage = new Bitmap(width, height, pixelFormat);
            BitmapData targetData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            CopyMemory(targetData.Scan0, sourceData, sourceData.Length, stride, targetData.Stride);
            newImage.UnlockBits(targetData);
            // For 8-bit images, set the palette.
            if ((pixelFormat == PixelFormat.Format8bppIndexed || pixelFormat == PixelFormat.Format4bppIndexed) && palette != null)
                newImage.Palette = palette;
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
                        if (transCols.Contains((Int32)b))
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
        
        private static ColorPalette GeneratePalette(Color[] colors, Color def)
        {
            Bitmap bm = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            ColorPalette pal = bm.Palette;
            for (Int32 i = 0; i < pal.Entries.Length; i++)
                if (i < colors.Length)
                    pal.Entries[i] = colors[i];
                else
                    pal.Entries[i] = def;
            return pal;
        }

        public static Bitmap GenerateBlankImage(Int32 width, Int32 height, Color[] colors, Byte paintColor)
        {
            if (width == 0 || height == 0)
                return null;
            ColorPalette pal = GeneratePalette(colors, Color.Empty);
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
            ColorPalette pal = GeneratePalette(colors, Color.Empty);
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
            ColorPalette pal = GeneratePalette(colors, Color.Empty);
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
            if ((startX < 0 && endX < 0) || (startX >= source.Width && endX >= source.Width)
                || (startY < 0 && endY < 0) || (startY >= source.Height && endX >= source.Height))
                return;
            // Restrict bounds to image.
            Int32 maxw = source.Width - 1;
            Int32 maxh = source.Height - 1;
            startX = Math.Min(maxw, Math.Max(0, startX));
            endX = Math.Min(maxw, Math.Max(0, endX));
            startY = Math.Min(maxh, Math.Max(0, startY));
            endY = Math.Min(maxh, Math.Max(0, endY));
            EditRawImageBytes(source, (arr, stride) => FillRect8BitFunc(startX, startY, endX, endY, colorIndex, arr, stride, fill));
        }

        private static void FillRect8BitFunc(Int32 startX, Int32 startY, Int32 endX, Int32 endY, Byte colorIndex, Byte[] dataArray, Int32 stride, Boolean fill)
        {
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


    }
}
