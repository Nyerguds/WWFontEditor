/*
 * EncodingSupport.cs 
 * 
 * Author: Maarten Meuris. No copyrights apply; this class was written as hobby project.
 * Released under WTFPL v2.
 */
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Nyerguds.Util
{
    public static class EncodingSupport
    {
        /// <summary>
        /// This function parses a text encoding string and returns the corresponding encoding.
        /// If the encoding fails, Windows-1252 encoding is returned instead.
        /// This function adds the extra possibility "utf-8-nbom", which will return an UTF-8 encoding without byte order marks.
        /// </summary>
        /// <param name="name">Encoding name</param>
        /// <returns>The found encoding, or Windows-1252 in case the detection fails.</returns>
        public static Encoding GetEncoding(String name)
        {
            Encoding enc = GetEncoding(name, null);
            return enc ?? Encoding.GetEncoding(1252);
        }

        /// <summary>
        /// This function parses a text encoding string and returns the corresponding encoding.
        /// It adds the extra possibility "utf-8-nbom", which will return an UTF-8 encoding without byte order marks.
        /// </summary>
        /// <param name="name">Encoding name</param>
        /// <param name="fallback">The value that is returned in case the detection fails.</param>
        /// <returns>The found encoding, or the given fallback in case the detection fails.</returns>
        public static Encoding GetEncoding(String name, Encoding fallback)
        {
            if (String.IsNullOrEmpty(name))
                return fallback;
            if (name.Equals("utf-8-nbom", StringComparison.InvariantCultureIgnoreCase))
                return new UTF8Encoding(false, false);
            try { return Encoding.GetEncoding(name); }
            catch { return fallback; }
        }

        public static String GetEncodingName(Encoding encoding)
        {
            if (encoding == null)
                return null;
            UTF8Encoding utf8Encoding = encoding as UTF8Encoding;
            if (utf8Encoding != null && utf8Encoding.GetPreamble().Length == 0)
                return "utf-8-nbom";
            return encoding.WebName;
        }

        /// <summary>
        /// Supported preamble-exposing encodings. The order is important: the UTF-16LE preamble
        /// is identical to the start of the UTF-32LE one, so the latter needs to be tested first.
        /// All of these are set to throw an exception on failure, to ensure smooth and accurate detection.
        /// </summary>
        public static Encoding[] SupportedBomEncodings =
        {
            new UTF32Encoding(false, true, true), // UTF-32-LE
            new UTF32Encoding(true, true, true), // UTF-32-BE
            new UnicodeEncoding(false, true, true), // UTF-16-LE
            new UnicodeEncoding(true, true, true), // UTF-16-BE
            new UTF8Encoding(true, true), // UTF-8
        };

        /// <summary>
        ///     Reads a text file, and detects whether it is a known preamble-exposing encoding, or if its encoding is valid UTF-8 or ascii.
        ///     If not, decodes the text using the given fallback encoding.
        /// </summary>
        /// <param name="filename">The file to read</param>
        /// <param name="encoding">The default encoding to use as fallback if the text is detected not to be pure ascii or UTF-8 compliant. This ref parameter is changed to the detected encoding, or Windows-1252 if the source object is null and the text is not valid UTF-8.</param>
        /// <returns>The contents of the read file</returns>
        public static String ReadFileAndGetEncoding(String filename, ref Encoding encoding)
        {
            Byte[] binFile = File.ReadAllBytes(filename);
            return ReadFileAndGetEncoding(binFile, ref encoding);
        }

        /// <summary>
        ///     Analyses the contents of a file, and detects whether it is text in a known preamble-exposing encoding, or if its encoding is valid UTF-8 or ascii.
        ///     If not, decodes the text using the given fallback encoding.
        /// </summary>
        /// <param name="docBytes">The bytes of the text document.</param>
        /// <param name="encoding">The default encoding to use as fallback if the text is detected not to be pure ascii or UTF-8 compliant. This ref parameter is changed to the detected encoding, or Windows-1252 if the source object is null and the text is not valid UTF-8.</param>
        /// <returns>The contents of the read file</returns>
        public static String ReadFileAndGetEncoding(Byte[] docBytes, ref Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.GetEncoding(1252);

            foreach (Encoding enc in SupportedBomEncodings)
            {
                Byte[] preamble = enc.GetPreamble();
                Int32 prLen = preamble.Length;
                if (docBytes.Length < prLen || !docBytes.Take(prLen).SequenceEqual(preamble))
                    continue;
                try
                {
                    // Seems that despite being an encoding with preamble, it doesn't actually skip said preamble when decoding...
                    String parsed = enc.GetString(docBytes, prLen, docBytes.Length - prLen);
                    encoding = enc;
                    return parsed;
                }
                catch (ArgumentException)
                {
                    /* Ignore and move on */
                }
            }
            if (docBytes.All(x => (x < 0x80 && (x >= 0x20 || x == 0x09 || x == 0x0D || x == 0x0A))))
            {
                // pure ASCII
                encoding = new ASCIIEncoding();
                return encoding.GetString(docBytes);
            }
            try
            {
                Encoding utf8NoBom = new UTF8Encoding(false, true);
                String parsed = utf8NoBom.GetString(docBytes);
                encoding = utf8NoBom;
                return parsed;
            }
            catch (ArgumentException)
            {
                /* Ignore and move on */
            }
            return encoding.GetString(docBytes);
        }
        
        private static readonly Byte[] AsciiValues = Enumerable.Range(0, 128).Select(b => (Byte)b).ToArray();
        private static readonly String AsciiChars = new String(AsciiValues.Select(b => (Char)b).ToArray());

        /// <summary>
        /// Checks if the given encoding is compatible with the ASCII encoding by testing if
        /// bytes 0-127 decode to the 0-127 unicode code points, and the string containing
        /// the 0-127 code point chars encode to bytes 0-127.
        /// </summary>
        /// <param name="encoding">The encoding to test</param>
        /// <returns>True if the first 127 code points in the given encoding are compatible with the ASCII encoding.</returns>
        public static Boolean IsAsciiCompatible(Encoding encoding)
        {
            try
            {
                return encoding.GetString(AsciiValues).Equals(AsciiChars, StringComparison.Ordinal)
                       && encoding.GetBytes(AsciiChars).SequenceEqual(AsciiValues);
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
