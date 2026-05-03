using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WWFontEditor.Domain

{
    public static class ArrayUtils
    {

        public static T[][] SwapDimensions<T>(T[][] original)
        {
            Int32 origHeight = original.Length;
            if (origHeight == 0)
                return new T[0][];
            // Since this is for images, it is assumed that the array is a perfectly rectangular matrix
            Int32 origWidth = original[0].Length;

            T[][] swapped = new T[origWidth][];
            for (Int32 newHeight = 0; newHeight < origWidth; newHeight++)
            {
                swapped[newHeight] = new T[origHeight];
                for (Int32 newWidth = 0; newWidth < origHeight; newWidth++)
                {
                    swapped[newHeight][newWidth] = original[newWidth][newHeight];
                }
            }
            return swapped;
        }

        public static Int32 GetBEIntFromByteArray(Byte[] data, Int32 startIndex)
        {
            return (data[startIndex] << 24)
                 | (data[startIndex + 1] << 16)
                 | (data[startIndex + 2] << 8)
                 | data[startIndex + 3];
        }

        public static Int16 GetBEShortFromByteArray(Byte[] data, Int32 startIndex)
        {
            return (Int16)Convert.ToUInt16((data[startIndex] << 8) | data[startIndex + 1]);
        }

        public static Int32 GetLEIntFromByteArray(Byte[] data, Int32 startIndex)
        {
            return (data[startIndex + 3] << 24)
                 | (data[startIndex + 2] << 16)
                 | (data[startIndex + 1] << 8)
                 | data[startIndex];
        }


        public static void SetLEIntInByteArray(Byte[] data, Int32 startIndex, Int32 value)
        {
            data[startIndex + 0x00] = (Byte)(value & 0xFF);
            data[startIndex + 0x01] = (Byte)((value >> 0x08) & 0xFF);
            data[startIndex + 0x02] = (Byte)((value >> 0x10) & 0xFF);
            data[startIndex + 0x03] = (Byte)((value >> 0x18) & 0xFF);
        }
        public static Int16 GetLEShortFromByteArray(Byte[] data, Int32 startIndex)
        {
            return (Int16)Convert.ToUInt16((data[startIndex + 1] << 8) | data[startIndex]);
        }

        public static void SetLEShortInByteArray(Byte[] data, Int32 startIndex, Int16 value)
        {
            data[startIndex + 0x00] = (Byte)(value & 0xFF);
            data[startIndex + 0x01] = (Byte)((value >> 8) & 0xFF);
        }
    }
}
