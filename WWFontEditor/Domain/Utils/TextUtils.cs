using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyerguds.Util
{
    public class TextUtils
    {
        private static readonly Byte[] asciiValues = Enumerable.Range(0, 128).Select(b => (Byte)b).ToArray();
        private static readonly String asciiChars = new String(asciiValues.Select(b => (Char)b).ToArray());
        
        public static Boolean IsAsciiCompatible(Encoding encoding)
        {
            try
            {
                return encoding.GetString(asciiValues).Equals(asciiChars, StringComparison.Ordinal)
                    && encoding.GetBytes(asciiChars).SequenceEqual(asciiValues);
            }
            catch (ArgumentException)
            {
                // Encoding.GetString may throw DecoderFallbackException if a fallback occurred 
                // and DecoderFallback is set to DecoderExceptionFallback.
                // Encoding.GetBytes may throw EncoderFallbackException if a fallback occurred 
                // and EncoderFallback is set to EncoderExceptionFallback.
                // Both of these derive from ArgumentException.
                return false;
            }
        }

        public static List<Encoding> GetAsciiCompatibleEncodings()
        {
            return Encoding.GetEncodings() // Get all known .Net encodings
                .Select(e => e.GetEncoding()) // From EncodingInfo to Encoding
                .Where(e => e.CodePage != 20127 // Exclude actual ASCII
                    && e.IsSingleByte // Single byte encodings only
                    && IsAsciiCompatible(e)).ToList(); // check if 0-127 range matches ASCII.
        }

    }
}
