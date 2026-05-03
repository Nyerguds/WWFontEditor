using System;
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

    }
}
