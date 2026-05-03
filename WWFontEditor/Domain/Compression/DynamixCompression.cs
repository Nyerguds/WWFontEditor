using System;
using Nyerguds.GameData.Dynamix;
using Nyerguds.Util;
using Nyerguds.Util.GameData;

namespace Compression
{
    public static class DynamixCompression
    {

        public static Byte[] LzwDecode(Byte[] buffer, Int32? startOffset, Int32? endOffset, Int32 decompressedSize)
        {
            DynamixLzwDecoder lzwDec = new DynamixLzwDecoder();
            Byte[] outputBuffer = new Byte[decompressedSize];
            lzwDec.LzwDecode(buffer, startOffset, endOffset, outputBuffer);
            return outputBuffer;
        }

        public static Byte[] RleDecode(Byte[] buffer, UInt32? startOffset, UInt32? endOffset, Int32 decompressedSize, Boolean abortOnError)
        {
            Byte[] outputBuffer = new Byte[decompressedSize];
            // Uses standard RLE implementation.
            RleCompressionHighBitRepeat rle = new RleCompressionHighBitRepeat();
            rle.RleDecodeData(buffer, startOffset, endOffset, ref outputBuffer, abortOnError);
            return outputBuffer;
        }

        /// <summary>
        /// Applies LZW Encoding to the given data.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <returns>The run-length encoded data</returns>
        public static Byte[] LzwEncode(Byte[] buffer)
        {
            //DynamixLzwEncoder enc = new DynamixLzwEncoder();
            //return enc.Compress(buffer);
            throw new NotImplementedException("Not implemented.");
        }

        /// <summary>
        /// Applies Run-Length Encoding (RLE) to the given data.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <returns>The run-length encoded data</returns>
        public static Byte[] RleEncode(Byte[] buffer)
        {
            // Uses standard RLE implementation.
            RleCompressionHighBitRepeat rle = new RleCompressionHighBitRepeat();
            return rle.RleEncodeData(buffer);
        }
        
    }
}