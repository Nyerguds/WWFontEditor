using System;

namespace Compression
{
    public class DynamixCompression
    {
        private Byte[] codeCur = new Byte[256];
        private Int32 codeSize;
        private Int32 codeLen;
        private Int32 cacheBits;

        private Byte[][] dictTableStr;
        private Byte[] dictTableLen;

        private Int32 dictSize;
        private Int32 dictMax;
        private Boolean dictFull;
        
	    private Int32 bitsData = 0;
        private Int32 bitsSize = 0;

        private void Dynamix_LZW_reset()
        {
            dictTableStr = new Byte[0x4000][];
            dictTableLen = new Byte[0x4000];
            for (int i = 0; i < this.dictTableStr.Length; i++)
                dictTableStr[i] = new Byte[100];
            for (Int32 lcv = 0; lcv < 256; lcv++)
            {
                this.dictTableLen[lcv] = 1;
                this.dictTableStr[lcv][0] = (Byte)lcv;
            }
            // 00-FF = ASCII
            // 100 = reset
            this.dictSize = 0x101;
            this.dictMax = 0x200;
            this.dictFull = false;
            // start = 9 bit codes
            this.codeSize = 9;
            this.codeLen = 0;
            // 9-12 byte cache chunks
            this.cacheBits = 0;
            // Bits
	        this.bitsData = 0;
            this.bitsSize = 0;
        }

        public static Byte[] LzwDecode(Byte[] buffer, Int32 decompressedSize)
        {
            DynamixCompression decompr = new DynamixCompression();
            Byte[] outputBuffer = new Byte[decompressedSize];
            decompr.DynamixLzwDecode(buffer, outputBuffer);
            return outputBuffer;
        }
        
        public static Byte[] RleDecode(Byte[] buffer, Int32 decompressedSize)
        {
            DynamixCompression decompr = new DynamixCompression();
            Byte[] outputBuffer = new Byte[decompressedSize];
            decompr.DynamixRleDecode(buffer, outputBuffer);
            return outputBuffer;
        }

        public void DynamixLzwDecode(Byte[] buffer, Byte[] bufferOut)
        {
            Int32 inPtr = 0;
            Int32 outPtr = 0;
            Dynamix_LZW_reset();
            this.cacheBits = 0;
            while (outPtr < bufferOut.Length)
            {
                Int32 lcv;
                // get next code
                Int32 code = GetBitsRight(this.codeSize, buffer, ref inPtr);
                if (code == -1)
                    return;
                // refresh data cache
                this.cacheBits += this.codeSize;
                if (this.cacheBits >= this.codeSize * 8)
                    this.cacheBits -= this.codeSize * 8;
                // reset
                if (code == 0x100)
                {
                    // Dynamix: dump data cache
                    if (this.cacheBits > 0)
                        GetBitsRight(this.codeSize * 8 - this.cacheBits, buffer, ref inPtr);
                    Dynamix_LZW_reset();
                    continue;
                }
                // special case: expand for new entry
                if (code >= this.dictSize && this.dictFull == false)
                {
                    this.codeCur[this.codeLen++] = this.codeCur[0];
                    // write output - future expanded string
                    for (lcv = 0; lcv < this.codeLen; lcv++)
                        bufferOut[outPtr++] = this.codeCur[lcv];
                }
                else
                {
                    // write output
                    for (lcv = 0; lcv < this.dictTableLen[code]; lcv++)
                        bufferOut[outPtr++] = this.dictTableStr[code][lcv];
                    // expand string
                    this.codeCur[this.codeLen++] = this.dictTableStr[code][0];
                }
                // add to dictionary (2+ bytes only)
                Boolean hit = this.codeLen < 2;
                if (hit)
                    continue;
                // add to dictionary
                if (!this.dictFull)
                {
                    // check full condition
                    if (this.dictSize == this.dictMax && this.codeSize == 12)
                    {
                        this.dictFull = true;
                        lcv = this.dictSize;
                    }
                    else
                    {
                        lcv = this.dictSize++;
                        this.cacheBits = 0;
                    }
                    // expand dictionary (adaptive LZW)
                    if (this.dictSize == this.dictMax && this.codeSize < 12)
                    {
                        this.dictMax *= 2;
                        this.codeSize++;
                    }
                    // add new entry
                    for (UInt32 lcv2 = 0; lcv2 < this.codeLen; lcv2++)
                    {
                        this.dictTableStr[lcv][lcv2] = this.codeCur[lcv2];
                        this.dictTableLen[lcv]++;
                    }
                }
                // reset running code!
                for (lcv = 0; lcv < this.dictTableLen[code]; lcv++)
                    this.codeCur[lcv] = this.dictTableStr[code][lcv];

                this.codeLen = this.dictTableLen[code];
            }
        }

        public void DynamixRleDecode(Byte[] buffer, Byte[] bufferOut)
        {
            Int32 inPtr = 0;
            Int32 outPtr = 0;

            // RLE implementation:
            // highest bit set = followed by range of repeating bytes
            // highest bit not set = followed by range of non-repeating bytes
            // In both cases, the "code" specifies the amount of bytes; either to write, or to skip.

            while (outPtr < bufferOut.Length)
            {
                // get next code
                Int32 code = buffer[inPtr++];
                if (code == -1)
                    return;
                // RLE run
                if ((code & 0x80) != 0)
                {
                    Int32 run = code & 0x7f;
                    Int32 rle = buffer[inPtr++];

                    for (UInt32 lcv = 0; lcv < run; lcv++)
                        bufferOut[outPtr++] = (Byte)rle;
                }
                // raw run
                else
                {
                    Int32 run = code & 0x7f;
                    for (UInt32 lcv = 0; lcv < run; lcv++)
                    {
                        Int32 data = buffer[inPtr++];
                        bufferOut[outPtr++] = (Byte)data;
                    }
                }
            }
        }

        /// <summary>
        /// Applies Run-length encoding (RLE) to the given data.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <param name="minimumRepeating">Minimum amount of repeating bytes before compression is applied.</param>
        /// <returns>The run-length encoded data</returns>
        public static Byte[] RleEncode(Byte[] buffer, Int32 minimumRepeating)
        {
            if (minimumRepeating < 2)
                minimumRepeating = 2;
            Int32 inPtr = 0;
            Int32 outPtr = 0;
            // Ensure big enough buffer. Sanity check will be done afterwards.
            Byte[] bufferOut = new Byte[(buffer.Length * 3) / 2];

            // RLE implementation:
            // highest bit set = followed by range of repeating bytes
            // highest bit not set = followed by range of non-repeating bytes
            // In both cases, the "code" specifies the amount of bytes; either to write, or to skip.
            Int32 len = buffer.Length;
            Boolean repeatDetected = false;
            while (inPtr < len)
            {
                if (repeatDetected || HasRepeatingAhead(buffer, len, inPtr, minimumRepeating))
                {
                    repeatDetected = false;
                    // Found more than 2 bytes. Worth compressing. Apply run-length encoding.
                    Int32 start = inPtr;
                    Int32 end = Math.Min(inPtr + 0x7F, len);
                    inPtr += 2;
                    Byte cur = buffer[inPtr];
                    for (; inPtr < end && buffer[inPtr] == cur; inPtr++) {}
                    bufferOut[outPtr++] = (Byte)((inPtr - start) | 0x80);
                    bufferOut[outPtr++] = cur;
                }
                else
                {
                    while (!repeatDetected)
                    {
                        Int32 start = inPtr;
                        Int32 end = Math.Min(inPtr + 0x7F, len);
                        for (; inPtr < end; inPtr++)
                        {
                            // detected bytes to compress after this one: abort.
                            if (!HasRepeatingAhead(buffer, len, inPtr, minimumRepeating))
                                continue;
                            repeatDetected = true;
                            break;

                        }
                        bufferOut[outPtr++] = (Byte)(inPtr - start);
                        for (Int32 i = start; i < inPtr; i++)
                            bufferOut[outPtr++] = buffer[i];
                    }
                }
            }
            Byte[] finalOut = new Byte[outPtr];
            Array.Copy(bufferOut, 0, finalOut, 0, outPtr);
            return finalOut;
        }

        public static Boolean HasRepeatingAhead(Byte[] buffer, Int32 max, Int32 ptr, Int32 minAmount)
        {
            if (ptr + minAmount - 1 >= max)
                return false;
            Byte cur = buffer[ptr];
            for (Int32 i = 1; i < minAmount; i++)
                if (buffer[ptr + i] != cur)
                    return false;
            return true;
        }

        private Int32 GetBitsRight(Int32 total_bits, Byte[] bufferIn, ref Int32 bufferPtr)
        {
            Byte[] bits_mask = {
		        0x00, 0x01, 0x03, 0x07, 0x0f,
		        0x1f, 0x3f, 0x7f, 0xff };
	        Int32 numBits = total_bits;
	        Int32 data = 0;

	        while( numBits > 0 ) {
	            // ERROR!
                if (bufferPtr >= bufferIn.Length)
			        return -1;
		        // 8-bit buffer
		        if( this.bitsSize == 0 ) {
			        this.bitsSize = 8;
			        this.bitsData = bufferIn[ bufferPtr++ ];
		        }
		        /*/
		        //#if 0
		        // consume cached bits
		        data <<= 1;
		        data |= ( bitsData & 1 );


		        bitsSize--;
		        numBits--;
		        //#else
		        //*/
		        Int32 useBits = numBits;
		        if( useBits > 8 )
			        useBits = 8;
		        if( useBits > this.bitsSize )
			        useBits = this.bitsSize;
		        /*
		        ex.
		        45678 || 123 = full 8
		        >> 3 needed

		        45 || 123 || xxx = remain 5
		        >> 3 needed
		        */

		        // add on bits
		        data |= ( this.bitsData & bits_mask[useBits] ) << ( total_bits - numBits );


		        // update cache buffer
		        numBits -= useBits;
		        this.bitsSize -= useBits;
		        this.bitsData >>= useBits;
		        //#endif
	        }

	        return data;
        }
    }
}