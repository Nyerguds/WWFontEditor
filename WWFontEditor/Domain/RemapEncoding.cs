using System;
using System.Text;

namespace WWFontEditor.Domain
{
    public abstract class RemapEncoding : Encoding
    {

        protected readonly Byte[] m_RemapTable;
        protected readonly String m_EncodingName;
        protected readonly String m_HeaderName;
        protected readonly Encoding m_BaseEncoding;
        
        protected RemapEncoding(Byte[] remapTable, String encName, String headerName, Encoding baseEncoding)
        {
            if (remapTable == null)
                throw new ArgumentNullException("remapTable");
            if (remapTable.Length != 0x100)
                throw new ArgumentException("Array size does not match! Needs to be exactly 256 bytes!", "remapTable");
            this.m_RemapTable = new Byte[0x100];
            Array.Copy(remapTable, this.m_RemapTable, 0x100);
            if (encName == null)
                throw new ArgumentNullException("encName");
            this.m_EncodingName = encName;
            if (headerName == null)
                throw new ArgumentNullException("headerName");
            this.m_HeaderName = headerName;
            if (baseEncoding == null)
                throw new ArgumentNullException("baseEncoding");
            if (!baseEncoding.IsSingleByte)
                throw new ArgumentException("The base needs to be a single byte encoding!", "baseEncoding");
            this.m_BaseEncoding = baseEncoding;
        }

        public override String EncodingName { get { return this.m_EncodingName; } }
        public override String WebName { get { return this.m_EncodingName; } }
        public override String HeaderName { get { return this.m_HeaderName; } }
        public override Boolean IsSingleByte { get { return true; } }

        public override Int32 GetBytes(Char[] chars, Int32 charIndex, Int32 charCount, Byte[] bytes, Int32 byteIndex)
        {
            Int32 retval = this.m_BaseEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
            for (Int32 i = byteIndex; i < byteIndex + charCount; ++i)
                bytes[i] = this.m_RemapTable[bytes[i]];
            return retval;
        }

        public override Int32 GetChars(Byte[] bytes, Int32 byteIndex, Int32 byteCount, Char[] chars, Int32 charIndex)
        {
            // make copy of array
            Byte[] bytesCopy = new Byte[bytes.Length];
            Array.Copy(bytes, 0, bytesCopy, 0, bytes.Length);
            for (Int32 i = byteIndex; i < byteIndex + byteCount; ++i)
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
            for (Int32 i = 0; i < this.m_RemapTable.Length; ++i)
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