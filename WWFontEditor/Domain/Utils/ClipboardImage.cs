using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Nyerguds.ImageManipulation;
using Nyerguds.Util;

namespace WWFontEditor.Domain.Utils
{
    public class ClipboardImage
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
                    using (Bitmap bm = BitmapHandler.LoadBitmap(png_stream.ToArray()))
                        clipboardimage = ImageUtils.CloneImage(bm, null);
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Dib))
            {
                Byte[] dibdata = ClipboardImage.TryGetDibDataClipboard(retrievedData);
                clipboardimage = ClipboardImage.ImageFromClipboardDib(dibdata);
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Bitmap))
                clipboardimage = new Bitmap(retrievedData.GetData(DataFormats.Bitmap) as Image);
            if (clipboardimage == null && retrievedData.GetDataPresent(typeof (Image)))
                clipboardimage = new Bitmap(retrievedData.GetData(typeof (Image)) as Image);
            return clipboardimage;
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
            using (MemoryStream dibMemStream = new MemoryStream())
            {
                // As standard bitmap, without transparency support
                data.SetData(DataFormats.Bitmap, true, imageNoTr);
                // As PNG. Gimp will prefer this over the other two.
                Byte[] pngData = BitmapHandler.GetPngImageData(image, 0);
                pngMemStream.Write(pngData, 0, pngData.Length);
                data.SetData("PNG", false, pngMemStream);
                // As DIB. This is (wrongly) accepted as ARGB by many applications.
                Byte[] dibData = ClipboardImage.ConvertToDib(image);
                dibMemStream.Write(dibData, 0, dibData.Length);
                data.SetData(DataFormats.Dib, false, dibMemStream);
                // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
                Clipboard.SetDataObject(data, true);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BITMAPINFOHEADER
        {
            public Int32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public BITMAPCOMPRESSION biCompression;
            public Int32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public Int32 biClrUsed;
            public Int32 biClrImportant;
        }
        
        public enum BITMAPCOMPRESSION : int
        {
            BI_RGB = 0x0000,
            BI_RLE8 = 0x0001,
            BI_RLE4 = 0x0002,
            BI_BITFIELDS = 0x0003,
            BI_JPEG = 0x0004,
            BI_PNG = 0x0005,
            BI_CMYK = 0x000B,
            BI_CMYKRLE8 = 0x000C,
            BI_CMYKRLE4 = 0x000D
        }

        public static Byte[] TryGetDibDataClipboard(DataObject retrievedData)
        {
            if (!retrievedData.GetDataPresent(DataFormats.Dib))
                return null;
            // Get the dib header
            MemoryStream dib = retrievedData.GetData(DataFormats.Dib) as MemoryStream;
            if (dib == null)
                return null;
            Byte[] dibBytes = dib.ToArray();
            if (dibBytes.Length < 40)
                return null;
            return dibBytes;
        }

        /// <summary>
        /// Converts the image to Device Independent Bitmap format of type BI_BITFIELDS.
        /// This is (wrongly) accepted by many applications as containing transparency,
        /// so I'm abusing it for that.
        /// </summary>
        /// <param name="image">Image to convert to DIB</param>
        /// <returns>The image converted to DIB, in bytes.</returns>
        public static Byte[] ConvertToDib(Image image)
        {
            Bitmap bm32b = ImageUtils.PaintOn32bpp(image, null);
            // Bitmap format has its lines reversed.
            bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
            Int32 stride;
            Byte[] bm32bData = ImageUtils.GetImageData(bm32b, out stride);

            BITMAPINFOHEADER hdr = new BITMAPINFOHEADER();
            Int32 hdrSize = Marshal.SizeOf(typeof (BITMAPINFOHEADER));
            hdr.biSize = hdrSize;
            hdr.biWidth = bm32b.Width;
            hdr.biHeight = bm32b.Height;
            hdr.biPlanes = 1;
            hdr.biBitCount = 32;
            hdr.biCompression = BITMAPCOMPRESSION.BI_BITFIELDS;
            hdr.biSizeImage = bm32bData.Length;
            hdr.biXPelsPerMeter = 0;
            hdr.biYPelsPerMeter=0;
            hdr.biClrUsed=0;
            hdr.biClrImportant=0;

            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            Byte[] pibHeaderBytes = StructToByteArray(hdr);
            Array.Copy(pibHeaderBytes, 0, fullImage, 0, hdrSize);
            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }
        
        public static Bitmap ImageFromClipboardDib(Byte[] dibBytes)
        {
            if (dibBytes == null || dibBytes.Length < 4)
                return null;
            try
            {
                Int32 headerSize = (Int32)ArrayUtils.ReadIntFromByteArray(dibBytes, 0, 4, true);
                // Only supporting 40-byte DIB from clipboard... sadly.
                if (headerSize != 40)
                    return null;
                Byte[] header = new Byte[40];
                Array.Copy(dibBytes, header, 40);
                BITMAPINFOHEADER dibHdr = StructFromByteArray<BITMAPINFOHEADER>(header);
                // Not dealing with non-standard formats
                if (dibHdr.biPlanes != 1 || (dibHdr.biCompression != BITMAPCOMPRESSION.BI_RGB && dibHdr.biCompression != BITMAPCOMPRESSION.BI_BITFIELDS))
                    return null;
                Int32 imageIndex = headerSize;
                Int32 width = dibHdr.biWidth;
                Int32 height = dibHdr.biHeight;
                Int32 bitCount = dibHdr.biBitCount;
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
                if (dibHdr.biCompression == BITMAPCOMPRESSION.BI_BITFIELDS)
                    imageIndex += 12;
                if (dibBytes.Length < imageIndex)
                    return null;
                Byte[] image = new Byte[dibBytes.Length - imageIndex];
                Array.Copy(dibBytes, imageIndex, image, 0, image.Length);
                Int32 stride = ImageUtils.GetClassicStride(width, bitCount);
                if (dibHdr.biCompression == BITMAPCOMPRESSION.BI_BITFIELDS)
                {
                    UInt32 redMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 0, 4, true);
                    UInt32 greenMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 4, 4, true);
                    UInt32 blueMask = ArrayUtils.ReadIntFromByteArray(dibBytes, headerSize + 8, 4, true);
                    // Fix for the undocumented use of 32bppARGB disguised as BI_BITFIELDS. Despite lacking an alpha bit field,
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
                    if (fmt != PixelFormat.Format32bppPArgb)
                    {
                        // Reformat bytes.
                        PixelFormatter pf = new PixelFormatter((Byte)(bitCount / 8), redMask, greenMask, blueMask, 0);
                        PixelFormatter pf32Argb = PixelFormatter.Format32BitArgb;
                        Int32 strideArgb = ImageUtils.GetClassicStride(width, 32);
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
        
        public static T StructFromByteArray<T>(Byte[] bytes) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                Int32 size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, ptr, size);
                Object obj = Marshal.PtrToStructure(ptr, typeof(T));
                return (T)obj;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public static Byte[] StructToByteArray<T>(T obj) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                Int32 size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, true);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(ptr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }
    }

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
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Dib))
            {
                MemoryStream dib = retrievedData.GetData(DataFormats.Dib) as MemoryStream;
                if (dib != null)
                    clipboardimage = ImageFromClipboardDib(dib.ToArray());
            }
            if (clipboardimage == null && retrievedData.GetDataPresent(DataFormats.Bitmap))
                clipboardimage = new Bitmap(retrievedData.GetData(DataFormats.Bitmap) as Image);
            if (clipboardimage == null && retrievedData.GetDataPresent(typeof(Image)))
                clipboardimage = new Bitmap(retrievedData.GetData(typeof(Image)) as Image);
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
                    // Fix for the undocumented use of 32bppARGB disguised as BI_BITFIELDS. Despite lacking an alpha bit field,
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
                    else
                        if (fmt != PixelFormat.Format32bppPArgb)
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
            using (MemoryStream dibMemStream = new MemoryStream())
            {
                // As standard bitmap, without transparency support
                data.SetData(DataFormats.Bitmap, true, imageNoTr);
                // As PNG. Gimp will prefer this over the other two.
                image.Save(pngMemStream, ImageFormat.Png);
                data.SetData("PNG", false, pngMemStream);
                // As DIB. This is (wrongly) accepted as ARGB by many applications.
                Byte[] dibData = ConvertToDib(image);
                dibMemStream.Write(dibData, 0, dibData.Length);
                data.SetData(DataFormats.Dib, false, dibMemStream);
                // The 'copy=true' argument means the MemoryStreams can be safely disposed after the operation.
                Clipboard.SetDataObject(data, true);
            }
        }

        /// <summary>
        /// Converts the image to Device Independent Bitmap format of type BI_BITFIELDS.
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
            Byte[] headerBytes = new Byte[hdrSize];
            //Int32 biSize;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 biWidth;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x04, 4, true, (UInt32)width);
            //Int32 biHeight;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x08, 4, true, (UInt32)height);
            //Int16 biPlanes;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x0C, 2, true, 1);
            //Int16 biBitCount;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BI_BITFIELDS;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x10, 4, true, 3);
            //Int32 biSizeImage;
            ArrayUtils.WriteIntToByteArray(headerBytes, 0x14, 4, true, (UInt32)bm32bData.Length);
            //Int32 biXPelsPerMeter = 0;
            //Int32 biYPelsPerMeter = 0;
            //Int32 biClrUsed = 0;
            //Int32 biClrImportant = 0;

            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            Array.Copy(headerBytes, 0, fullImage, 0, hdrSize);
            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }
    }
}
