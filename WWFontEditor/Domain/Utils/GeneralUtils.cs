using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Nyerguds.Util
{
    public class GeneralUtils
    {
        public static Boolean IsNumeric(String str)
        {
            Int32 strLen = str.Length;
            for (Int32 i = 0; i < strLen; ++i)
            {
                Char c = str[i];
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given value starts with T, J, Y, O (TRUE, JA, YES, OUI) or is 1
        /// If the value is null or the parse fails, the default is False.
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <returns>True if the string's first letter matches J, Y, O, 1 or T</returns>
        public static Boolean IsTrueValue(String value)
        {
            return IsTrueValue(value, false);
        }

        /// <summary>
        /// Checks if the given value starts with T, J, Y, O (TRUE, JA, YES, OUI) or is a non-zero number.
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <param name="defaultVal">Default value to return in case parse fails</param>
        /// <returns>True if the string's first letter matches J, Y, O, 1 or T</returns>
        public static Boolean IsTrueValue(String value, Boolean defaultVal)
        {
            if (String.IsNullOrEmpty(value))
                return defaultVal;
            // Either it starts with the start letter of "True", "Ja", "Yes", "Oui", or the whole thing is a non-zero number.
            // Number is checked as: any amount of leading zeroes, then one non-zero number, then any amount of trailing numbers.
            return Regex.IsMatch(value.Trim(), "^(([TJYO].*)|(0*[1-9]\\d*))$", RegexOptions.IgnoreCase);
        }

        public static Boolean IsHexadecimal(String str)
        {
            return Regex.IsMatch(str, "^[0-9A-F]*$", RegexOptions.IgnoreCase);
        }

        public static String GetApplicationPath()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        public static TEnum TryParseEnum<TEnum>(String value, TEnum defaultValue, Boolean ignoreCase) where TEnum : struct
        {
            if (String.IsNullOrEmpty(value))
                return defaultValue;
            try { return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase); }
            catch (ArgumentException) { return defaultValue; }
        }

        public static String GetAbsolutePath(String relativePath, String basePath)
        {
            if (relativePath == null)
                return null;
            if (basePath == null)
                basePath = Path.GetFullPath("."); // quick way of getting current working directory
            else
                basePath = GetAbsolutePath(basePath, null); // to be REALLY sure ;)
            String path;
            // specific for windows paths starting on \ - they need the drive added to them.
            // I constructed this piece like this for possible Mono support.
            if (!Path.IsPathRooted(relativePath) || "\\".Equals(Path.GetPathRoot(relativePath)))
            {
                if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    path = Path.Combine(Path.GetPathRoot(basePath), relativePath.TrimStart(Path.DirectorySeparatorChar));
                else
                    path = Path.Combine(basePath, relativePath);
            }
            else
                path = relativePath;
            // resolves any internal "..\" to get the true full path.
            Int32 filenameStart = path.LastIndexOf(Path.DirectorySeparatorChar);
            String dirPart = path.Substring(0, filenameStart + 1);
            String filePart = path.Substring(filenameStart + 1);
            if (filePart.Contains("*") || filePart.Contains("?"))
            {
                dirPart = Path.GetFullPath(dirPart);
                return Path.Combine(dirPart, filePart);
            }
            return Path.GetFullPath(path);
        }

        public static String ProgramVersion()
        {
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            //Version v = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version;
            String version = String.Format("v{0}.{1}", ver.FileMajorPart, ver.FileMinorPart);
            if (ver.FileBuildPart > 0)
                version += "." + ver.FileBuildPart;
            if (ver.FilePrivatePart > 0)
                version += "." + ver.FilePrivatePart;
            return version;
        }

        public static String DoubleFirstAmpersand(String input)
        {
            if (input == null)
                return null;
            Int32 index = input.IndexOf('&');
            if (index == -1)
                return input;
            return input.Substring(0, index) + '&' + input.Substring(index);
        }

        /// <summary>
        /// ArgumentException messes with the Message property, and dumps its own extra (localised) bit of
        /// text with the argument onto the end of the message. This uses serialisation to retrieve the 
        /// original internal message of the exception without added junk.
        /// </summary>
        /// <param name="argex">The ArgumentException to retrieve the message from</param>
        /// <param name="fallback">True to construct a fallback message if the error message is empty.</param>
        /// <returns>The actual message given when the ArgumentException was created.</returns>
        public static String RecoverArgExceptionMessage(ArgumentException argex, Boolean fallback)
        {
            if (argex == null)
                return null;
            SerializationInfo info = new SerializationInfo(typeof(ArgumentException), new FormatterConverter());
            argex.GetObjectData(info, new StreamingContext(StreamingContextStates.Clone));
            String message = info.GetString("Message");
            if (!String.IsNullOrEmpty(message))
                return message;
            if (!fallback)
                return String.Empty;
            // Fallback: if no message, provide basic info.
            if (String.IsNullOrEmpty(argex.ParamName))
                return String.Empty;
            if (argex is ArgumentNullException)
                return String.Format("\"{0}\" is null.", argex.ParamName);
            if (argex is ArgumentOutOfRangeException)
                return String.Format("\"{0}\" out of range.", argex.ParamName);
            return argex.ParamName;
        }
    }


    /// <summary>
    /// Sorts files using the same method used by the Windows Explorer, sorting numbers with a lower amount of digits before numbers with a higher amount of digits.
    /// See http://stackoverflow.com/a/3099659/395685
    /// </summary>
    public class ExplorerFileComparer : IComparer<String>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern Int32 StrCmpLogicalW(String x, String y);

        /// <summary>
        /// Sorts files using the same method used by the Windows Explorer, sorting numbers with a lower amount of digits before numbers with a higher amount of digits.
        /// See http://stackoverflow.com/a/3099659/395685
        /// </summary>
        public ExplorerFileComparer() { }

        public Int32 Compare(String x, String y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
