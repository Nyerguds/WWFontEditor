using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.Utils
{
    
    /// <summary>
    /// Implementation that uses as little external toolsets as possible.
    /// </summary>
    public class ClipboardImageNoTools
    {
        /// <summary>
        /// Retrieves an image from the given clipboard data object, in the order PNG, DIB, Bitmap, Image object.
        /// </summary>
        /// <param name="retrievedData">The clipboard data.</param>
        /// <returns>The extracted image, or null if no supported image type was found.</returns>
        public static Bitmap GetClipboardImage(DataObject retrievedData)
        {
            Bitmap clipboardimage = null;
            // Order: try PNG, move on to try 32-bit ARGB DIB, then try the normal Bitmap and Image types.
            if (retrievedData.GetDataPresent("PNG"))
            {
                MemoryStream png_stream = retrievedData.GetData("PNG") as MemoryStream;
                if (png_stream != null)
                    using (Bitmap bm = new Bitmap(png_stream))
                        clipboardimage = ImageUtils.CloneImage(bm, null);
            }
            if (clipboardimage == null && retrievedData.GetDataPresent("Format17"))
            {
                MemoryStream dib = retrievedData.GetData("Format17") as MemoryStream;
                if (dib != null)
                    clipboardimage = ImageFromClipboardDib5(dib.ToArray());
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Dib))
            {
                MemoryStream dib = retrievedData.GetData(DataFormats.Dib) as MemoryStream;
                if (dib != null)
                    clipboardimage = ImageFromClipboardDib(dib.ToArray());
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Bitmap))
                clipboardimage = new Bitmap(retrievedData.GetData(DataFormats.Bitmap) as Image);
            if (clipboardimage == null && retrievedData.GetDataPresent(typeof (Image)))
                clipboardimage = new Bitmap(retrievedData.GetData(typeof (Image)) as Image);
            return clipboardimage;
        }

        public static Bitmap ImageFromClipboardDib(Byte[] dibBytes)
        {
            if (dibBytes == null || dibBytes.Length < 4)
                return null;
            try
            {
                Int32 headerSize = (Int32)ArrayUtils.ReadIntFromByteArray(dibBytes, 0, 4, true);
                // Only supporting 40-byte DIB from clipboard
                if (headerSize != 40)
                    return null;
                Byte[] header = new Byte[40];
                Array.Copy(dibBytes, header, 40);
                Int32 imageIndex = headerSize;
                Int32 width = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x04, 4, true);
                Int32 height = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x08, 4, true);
                Int16 planes = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0C, 2, true);
                Int16 bitCount = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0E, 2, true);
                Int32 compression = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x10, 4, true);
                // Not dealing with non-standard formats
                if (planes != 1 || (compression != 0 && compression != 3))
                    return null;
                PixelFormat fmt;
                switch (bitCount)
                {
                    case 32:
                        fmt = PixelFormat.Format32bppRgb;
                        break;
                    case 24:
                        fmt = PixelFormat.Format24bppRgb;
                        break;
                    case 16:
                        fmt = PixelFormat.Format16bppRgb555;
                        break;
                    default:
                        return null;
                }
                if (compression == 3)
                    imageIndex += 12;
                if (dibBytes.Length < imageIndex)
                    return null;
                Byte[] image = new Byte[dibBytes.Length - imageIndex];
                Array.Copy(dibBytes, imageIndex, image, 0, image.Length);
                Int32 stride = (((((bitCount * width) + 7) / 8) + 3) / 4) * 4;
                if (compression == 3)
                {
                    UInt32 redMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 0, 4, true);
                    UInt32 greenMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 4, 4, true);
                    UInt32 blueMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 8, 4, true);
                    // Fix for the undocumented use of 32bppARGB disguised as BITFIELDS. Despite lacking an alpha bit field,
                    // the alpha bytes are still filled in, without any header indication of alpha usage.
                    // Pure 32-bit RGB: check if a switch to ARGB can be made by checking for non-zero alpha.
                    // Admitted, this may give a mess if the alpha bits simply aren't cleared, but why the hell wouldn't it use 24bpp then?
                    if (bitCount == 32 && redMask == 0xFF0000 && greenMask == 0x00FF00 && blueMask == 0x0000FF)
                    {
                        // Stride is always a multiple of 4; no need to take it into account for 32bpp.
                        for (Int32 pix = 3; pix < image.Length; pix += 4)
                        {
                            // 0 can mean transparent, but can also mean the alpha isn't filled in, so only check for non-zero alpha,
                            // which would indicate there is actual data in the alpha bytes.
                            if (image[pix] == 0)
                                continue;
                            fmt = PixelFormat.Format32bppPArgb;
                            break;
                        }
                    }
                    else if (fmt != PixelFormat.Format32bppPArgb)
                    {
                        // Reformat bytes.
                        PixelFormatter pf = new PixelFormatter((Byte)(bitCount / 8), redMask, greenMask, blueMask, 0);
                        PixelFormatter pf32Argb = PixelFormatter.Format32BitArgb;
                        Int32 strideArgb = width * 4;
                        Byte[] imageArgb = new Byte[height * strideArgb];
                        Int32 srcbytesPerPixel = bitCount / 8;
                        for (Int32 y = 0; y < height; y++)
                        {
                            Int32 offs = y * stride;
                            Int32 offs32 = y * strideArgb;
                            for (Int32 x = 0; x < width; x++)
                            {
                                Color c = pf.GetColor(image, offs);
                                pf32Argb.WriteColor(imageArgb, offs32, c);
                                offs += srcbytesPerPixel;
                                offs32 += 4;
                            }
                        }
                        image = imageArgb;
                        fmt = PixelFormat.Format32bppArgb;
                        stride = strideArgb;
                    }
                }
                Bitmap bitmap = ImageUtils.BuildImage(image, width, height, stride, fmt, null, null);
                // This is bmp; reverse image lines.
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap ImageFromClipboardDib5(Byte[] dibBytes)
        {
            if (dibBytes == null || dibBytes.Length < 4)
                return null;
            try
            {
                Int32 headerSize = (Int32)ArrayUtils.ReadIntFromByteArray(dibBytes, 0, 4, true);
                // Only supporting 124-byte DIBV5 in this.
                // If it fails, try the other type ;)
                if (headerSize == 0x40)
                    return ImageFromClipboardDib(dibBytes);
                if (headerSize != 0x7C)
                    return null;
                Byte[] header = new Byte[headerSize];
                Array.Copy(dibBytes, header, headerSize);
                Int32 imageIndex = headerSize;
                Int32 width = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x04, 4, true);
                Int32 height = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x08, 4, true);
                Boolean noFlip = height < 0;
                if (noFlip)
                    height = -height;
                Int16 planes = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0C, 2, true);
                Int16 bitCount = (Int16)ArrayUtils.ReadIntFromByteArray(header, 0x0E, 2, true);
                Int32 compression = (Int32)ArrayUtils.ReadIntFromByteArray(header, 0x10, 4, true);
                UInt32 redMask = ArrayUtils.ReadIntFromByteArray(header, 0x28, 4, true);
                UInt32 greenMask = ArrayUtils.ReadIntFromByteArray(header, 0x2C, 4, true);
                UInt32 blueMask = ArrayUtils.ReadIntFromByteArray(header, 0x30, 4, true);
                UInt32 alphaMask = ArrayUtils.ReadIntFromByteArray(header, 0x34, 4, true);

                // Not dealing with non-standard formats
                if (planes != 1 || (compression != 0 && compression != 3))
                    return null;
                PixelFormat fmt = PixelFormat.Undefined;
                switch (bitCount)
                {
                    case 32:
                        if (compression == 3)
                        {
                            if (redMask == 0x00FF0000 && greenMask == 0x0000FF00 && blueMask == 0x000000FF)
                            {
                                if (alphaMask == 0x00000000)
                                    fmt = PixelFormat.Format32bppRgb;
                                else if (alphaMask == 0xFF000000)
                                    fmt = PixelFormat.Format32bppArgb;
                            }
                        }
                        else
                            fmt = PixelFormat.Format32bppRgb;
                        break;
                    case 24:
                        fmt = PixelFormat.Format24bppRgb;
                        break;
                    case 16:
                        if (compression == 3)
                        {
                            if (redMask == 0x7C00 && greenMask == 0x03E0 && blueMask == 0x01F)
                            {
                                if (alphaMask == 0x0000)
                                    fmt = PixelFormat.Format16bppRgb555;
                                else if (alphaMask == 0x8000)
                                    fmt = PixelFormat.Format16bppArgb1555;
                            }
                            else if (redMask == 0xF800 && greenMask == 0x07E0 && blueMask == 0x01F)
                                fmt = PixelFormat.Format16bppRgb565;
                        }
                        else
                            fmt = PixelFormat.Format16bppRgb555;
                        break;
                    default:
                        return null;
                }
                if (fmt == PixelFormat.Undefined)
                    return null;
                Byte[] image = new Byte[dibBytes.Length - imageIndex];
                Array.Copy(dibBytes, imageIndex, image, 0, image.Length);
                Int32 stride = ImageUtils.GetClassicStride(width, bitCount);
                Bitmap bitmap = ImageUtils.BuildImage(image, width, height, stride, fmt, null, null);
                // This is bmp; reverse image lines.
                if (!noFlip)
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Copies the given image to the clipboard as PNG, DIB and standard Bitmap format.
        /// </summary>
        /// <param name="image">Image to put on the clipboard.</param>
        /// <param name="imageNoTr">Optional specifically nontransparent version of the image to put on the clipboard.</param>
        /// <param name="data">Clipboard data object to put the image into. Might already contain other stuff. Leave null to create a new one.</param>
        public static void SetClipboardImage(Bitmap image, Bitmap imageNoTr, DataObject data)
        {
            Clipboard.Clear();
            if (data == null)
                data = new DataObject();
            if (imageNoTr == null)
                imageNoTr = image;
            using (MemoryStream pngMemStream = new MemoryStream())
            using (MemoryStream dib5MemStream = new MemoryStream())
            using (MemoryStream dibMemStream = new MemoryStream())
            {
                // As standard bitmap, without transparency support
                data.SetData(DataFormats.Bitmap, true, imageNoTr);
                // As PNG. Gimp will prefer this over the other two.
                image.Save(pngMemStream, ImageFormat.Png);
                data.SetData("PNG", false, pngMemStream);
                Byte[] dib5Data = ConvertToDib5(image);
                dib5MemStream.Write(dib5Data, 0, dib5Data.Length);
                data.SetData("Format17", false, dib5MemStream);
                // As DIB. This is (wrongly) accepted as ARGB by many applications.
                Byte[] dibData = ConvertToDib(image);
                dibMemStream.Write(dibData, 0, dibData.Length);
                data.SetData(DataFormats.Dib, false, dibMemStream);
                // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
                Clipboard.SetDataObject(data, true);
            }
        }

        /// <summary>
        /// Converts the image to Device Independent Bitmap format of type BITFIELDS.
        /// This is (wrongly) accepted by many applications as containing transparency,
        /// so I'm abusing it for that.
        /// </summary>
        /// <param name="image">Image to convert to DIB</param>
        /// <returns>The image converted to DIB, in bytes.</returns>
        public static Byte[] ConvertToDib(Image image)
        {
            Byte[] bm32bData;
            Int32 width = image.Width;
            Int32 height = image.Height;
            // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
            using (Bitmap bm32b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bm32b))
                    gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
                // Bitmap format has its lines reversed.
                bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
                Int32 stride;
                bm32bData = ImageUtils.GetImageData(bm32b, out stride);
            }
            // BITMAPINFOHEADER struct for DIB.
            Int32 hdrSize = 0x28;
            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            //Int32 biSize;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 biWidth;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
            //Int32 biHeight;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
            //Int16 biPlanes;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
            //Int16 biBitCount;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
            //Int32 biSizeImage;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
            // These are all 0. Since .net clears new arrays, don't bother writing them.
            //Int32 biXPelsPerMeter = 0;
            //Int32 biYPelsPerMeter = 0;
            //Int32 biClrUsed = 0;
            //Int32 biClrImportant = 0;

            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }

        /// <summary>
        /// Converts the image to Device Independent Bitmap format with BITMAPV5HEADER.
        /// </summary>
        /// <param name="image">Image to convert to DIB</param>
        /// <returns>The image converted to DIB, in bytes.</returns>
        public static Byte[] ConvertToDib5(Image image)
        {

            Byte[] bm32bData;
            Int32 width = image.Width;
            Int32 height = image.Height;
            // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
            using (Bitmap bm32b = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bm32b))
                    gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
                // Bitmap format has its lines reversed.
                bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
                Int32 stride;
                bm32bData = ImageUtils.GetImageData(bm32b, out stride);
            }
            // BITMAPINFOHEADER struct for DIB.
            Int32 hdrSize = 0x7C;
            Byte[] fullImage = new Byte[hdrSize + bm32bData.Length];
            //Int32 bV5Size;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 bV5Width;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
            //Int32 bV5Height;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
            //Int16 bV5Planes;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
            //Int16 bV5BitCount;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION bV5Compression = BITMAPCOMPRESSION.BITFIELDS;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
            //Int32 bV5SizeImage;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
            // These are all 0. Since .net clears new arrays, don't bother writing them.
            //Int32 bV5XPelsPerMeter = 0;
            //Int32 bV5YPelsPerMeter = 0;
            //Int32 bV5ClrUsed = 0;
            //Int32 bV5ClrImportant = 0;
            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G, B and A values.
            ArrayUtils.WriteIntToByteArray(fullImage, 0x28, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, 0x2C, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, 0x30, 4, true, 0x000000FF);
            ArrayUtils.WriteIntToByteArray(fullImage, 0x34, 4, true, 0xFF000000);
            // LogicalColorSpace bV5CSType
            ArrayUtils.WriteIntToByteArray(fullImage, 0x38, 4, true, 0x73524742); // litle-endian "sRGB"
            //bV5Endpoints (0x3C-0x60): ignore
            // not sure what to do with these.
            //UInt32 bV5GammaRed = 0; //0x60
            //UInt32 bV5GammaGreen = 0; //0x64
            //UInt32 bV5GammaBlue = 0; //0x68
            
            //GamutMappingIntent bV5Intent;
            ArrayUtils.WriteIntToByteArray(fullImage, 0x6C, 4, true, 0x00000002); // LCS_GM_GRAPHICS

            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }

    }
}
