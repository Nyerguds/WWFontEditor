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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public BITMAPCOMPRESSION biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }
        

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct BITMAPFILEHEADER
        {
            public static readonly short BM = 0x4d42; // BM

            public short bfType;
            public int bfSize;
            public short bfReserved1;
            public short bfReserved2;
            public int bfOffBits;
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
        /// <param name="image"></param>
        /// <returns></returns>
        public static Byte[] ConvertToDib(Image image)
        {
            Bitmap bm32b = ImageUtils.PaintOn32bpp(image, null);
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

            Byte[] fullImage = new Byte[hdrSize+12+bm32bData.Length];
            Byte[] pibHeaderBytes = ToByteArray(hdr);
            Array.Copy(pibHeaderBytes, 0, fullImage, 0, hdrSize);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            ArrayUtils.WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }

        public static Bitmap ImageFromClipboardDib(Byte[] dibBytes)
        {
            if (dibBytes == null || dibBytes.Length < 0)
                return null;
            try
            {
                Byte[] header = new Byte[40];
                Array.Copy(dibBytes, header, 40);
                BITMAPINFOHEADER dibHdr = FromByteArray<BITMAPINFOHEADER>(header);
                // Not dealing with non-standard formats
                if (dibHdr.biPlanes != 1 || (dibHdr.biCompression != BITMAPCOMPRESSION.BI_RGB && dibHdr.biCompression != BITMAPCOMPRESSION.BI_BITFIELDS))
                    return null;
                Int32 imageIndex = dibHdr.biSize;
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
                    UInt32 redMask = ArrayUtils.ReadIntFromByteArray(dibBytes, 40, 4, true);
                    UInt32 greenMask = ArrayUtils.ReadIntFromByteArray(dibBytes, 44, 4, true);
                    UInt32 blueMask = ArrayUtils.ReadIntFromByteArray(dibBytes, 48, 4, true);
                    // Fix for the undocumented use of 32bppARGB disguised as BI_BITFIELDS. Despite lacking an alpha bit field,
                    // the alpha bytes are still filled in, without any header indication of alpha usage.
                    // Pure 32-bit RGB: check if a switch to ARGB can be made by checking for non-zero alpha.
                    // Admitted, this may give a mess if the alpha bits simply aren't cleared, but why the hell wouldn't it use 24bpp then?
                    if (bitCount == 32 && redMask == 0xFF0000 && greenMask == 0x00FF00 && blueMask == 0x0000FF)
                    {
                        if (image.Length < 4)
                            return null;
                        // Stride is always multiple of 4; no need to take it into account for 32bpp.
                        for (Int32 pix = 4; pix < image.Length; pix += 4)
                        {
                            // 0 can mean transparent, but can also mean the alpha isn't filled in, so only check for non-zero alpha,
                            // which would indicate there is actual data in the alpha bytes.
                            if (image[pix + 3] == 0)
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
                        Byte[] imageArgb = new Byte[dibHdr.biHeight * strideArgb];
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
                // This is bmp; reverse image lines.
                Byte[] finalImage = new Byte[image.Length];
                for (Int32 y = 0; y < height; y++)
                    Array.Copy(image, (height - y - 1) * stride, finalImage, y * stride, stride);
                return ImageUtils.BuildImage(finalImage, width, height, stride, fmt, null, null);
            }
            catch
            {
                return null;
            }
        }
        
        public static T FromByteArray<T>(Byte[] bytes) where T : struct
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

        public static Byte[] ToByteArray<T>(T obj) where T : struct
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
}
