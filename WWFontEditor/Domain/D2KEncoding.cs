using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WWFontEditor.Domain
{
    public class D2KEncoding : Encoding
    {

        /// <summary>
        /// The default Dune 2000 character remap table, copied from FONT.BIN
        /// </summary>
        protected static Byte[] OriginalRemapTable = new Byte[]
        {
            0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 
            0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 
            0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 
            0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 
            0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 
            0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 
            0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 
            0x20, 0xBA, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xB7, 0x20, 0x20, 0x20, 0x20, 0xB6, 0x20, 
            0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xB9, 
            0x9D, 0x9E, 0x9F, 0xB3, 0x83, 0xB5, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 
            0x20, 0xBB, 0xAA, 0xAB, 0xAC, 0xAD, 0x84, 0x20, 0x20, 0xAE, 0xAF, 0xB0, 0x85, 0xB1, 0x20, 0x86, 
            0x87, 0x88, 0x89, 0xB2, 0x80, 0xB4, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 
            0x20, 0xB8, 0x94, 0x95, 0x96, 0x97, 0x81, 0x20, 0x20, 0x98, 0x99, 0x9A, 0x82, 0x9B, 0x20, 0x9C
        };

        protected Byte[] m_RemapTable;
        protected String m_EncodingName = "Dune 2000 text encoding";
        protected Encoding m_BaseEncoding = GetEncoding("Windows-1252");

        public D2KEncoding()
        {
            this.m_RemapTable = new Byte[0x100];
            Array.Copy(OriginalRemapTable, this.m_RemapTable, 0x100);
        }

        public D2KEncoding(String encName)
            :this()
        {
            if (encName != null)
                this.m_EncodingName = encName;
        }

        public D2KEncoding(Byte[] remapTable, String encName)
        {
            if (remapTable == null)
                throw new ArgumentNullException("remapTable");
            if (remapTable.Length != 0x100)
                throw new ArgumentException("Array size does not match! Needs to be exactly 256 bytes!", "remapTable");
            this.m_RemapTable = new Byte[0x100];
            Array.Copy(remapTable, this.m_RemapTable, 0x100);
            if (encName != null)
                this.m_EncodingName = encName;
        }
        
        public D2KEncoding(Byte[] remapTable, String encName, Encoding baseEncoding)
            : this(remapTable, encName)
        {
            if (baseEncoding != null)
            {
                if (!baseEncoding.IsSingleByte)
                    throw new ArgumentException("The base needs to be a single byte encoding!", "baseEncoding");
                this.m_BaseEncoding = baseEncoding;
            }
        }

        public override String EncodingName { get { return this.m_EncodingName; } }
        public override String WebName { get { return this.m_EncodingName; } }
        public override String HeaderName { get { return "Dune-2000-enc"; } }
        public override Boolean IsSingleByte { get { return true; } }

        public override Int32 GetBytes(Char[] chars, Int32 charIndex, Int32 charCount, Byte[] bytes, Int32 byteIndex)
        {
            // Not really necessary; input for d2k is Windows-1252.
            // But, it gives the symbol index to use for a character, I guess. Could be useful if I implement previews.
            Int32 retval = this.m_BaseEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
            for (Int32 i = byteIndex; i < byteIndex + charCount; i++)
                bytes[i] = this.m_RemapTable[bytes[i]];
            return retval;
        }

        public override Int32 GetChars(Byte[] bytes, Int32 byteIndex, Int32 byteCount, Char[] chars, Int32 charIndex)
        {
            // make copy of array
            Byte[] bytesCopy = new Byte[bytes.Length];
            Array.Copy(bytes, 0, bytesCopy, 0, bytes.Length);
            for (Int32 i = byteIndex; i < byteIndex + byteCount; i++)
                bytesCopy[i] = this.FindIndexInList(bytesCopy[i]);
            // call parent method with adapted copy
            Int32 retval = this.m_BaseEncoding.GetChars(bytesCopy, byteIndex, byteCount, chars, charIndex);
            // transform here?
            return retval;
        }

        protected Byte FindIndexInList(Byte value)
        {
            if (value == 0x20)
                return 0x20;
            for (Int32 i = 0; i < this.m_RemapTable.Length; i++)
                if (this.m_RemapTable[i] == value)
                    return (Byte)i;
            return 0x20;
        }

        public override Int32 GetByteCount(Char[] chars, Int32 index, Int32 count)
        {
            return this.m_BaseEncoding.GetByteCount(chars, index, count);
        }

        public override Int32 GetCharCount(Byte[] bytes, Int32 index, Int32 count)
        {
            return this.m_BaseEncoding.GetCharCount(bytes, index, count);
        }

        public override Int32 GetMaxByteCount(Int32 charCount)
        {
            return this.m_BaseEncoding.GetMaxByteCount(charCount);
        }

        public override Int32 GetMaxCharCount(Int32 byteCount)
        {
            return this.m_BaseEncoding.GetMaxCharCount(byteCount);
        }
    }
}
