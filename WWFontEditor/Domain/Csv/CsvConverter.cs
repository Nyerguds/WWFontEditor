using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace Nyerguds.Util.Csv
{
    public class CsvConverter
    {
        public static String CreateCSVRow(String[] values, Char separator)
        {
            if (separator == '"')
                throw new ArgumentException("Separator can't be a double quote character!", "separator");

            StringBuilder sb = new StringBuilder();
            Int32 len = values.Length;
            Int32 last = len - 1;
            for (Int32 i = 0; i < len; i++)
            {
                String col = values[i] ?? String.Empty;
                Boolean needsEscaping = col.Contains(separator) || col.Contains("\"") || col.Contains("\r") || col.Contains("\n");
                if (needsEscaping)
                    col = "\"" + col.Replace("\"", "\"\"") + "\"";
                sb.Append(col);
                if (i < last)
                    sb.Append(separator);
            }
            return sb.ToString();
        }

        public static String[] CreateCsvFile(IEnumerable<String[]> fileLines, Char separator)
        {
            if (separator == '"')
                throw new ArgumentException("Separator can't be a double quote character!", "separator");
            List<String> csvLines = new List<String>();
            foreach (String[] lineParts in fileLines)
                csvLines.Add(CreateCSVRow(lineParts, separator));
            return csvLines.ToArray();
        }

        /// <summary>
        /// Parses a CSV file into a list of String arrays.
        /// This override has support for multi-line values.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="separator">Separator character</param>
        /// <param name="ignoreIllegalLines">Do not abort on lines that can't be parsed.</param>
        /// <param name="ignoreEmptyLines">Do not add empty lines to the resulting list.</param>
        /// <exception cref="System.FormatException">Thrown when the file could not be parsed. Never thrown if ignoreIllegalLines is set to true.</exception>
        /// <returns>The parsed file, as list of string arrays.</returns>
        public static List<String[]> SplitCsvFilePath(String filePath, Char separator, Boolean ignoreIllegalLines, Boolean ignoreEmptyLines)
        {
            Encoding enc = null;
            String fileContents = EncodingSupport.ReadFileAndGetEncoding(filePath, ref enc);
            try
            {
                using (StringReader sr = new StringReader(fileContents))
                    return SplitCsvFile(sr, separator, ignoreIllegalLines, ignoreEmptyLines);
            }
            catch (MalformedLineException mfle)
            {
                throw new FormatException(String.Format("Could not parse line {0} in file {1}!", mfle.LineNumber, filePath));
            }
        }
        /// <summary>
        /// Parses CSV data into a list of String arrays.
        /// This override has support for multi-line values.
        /// </summary>
        /// <param name="fileContents">File contents</param>
        /// <param name="separator">Separator character</param>
        /// <param name="ignoreIllegalLines">Do not abort on lines that can't be parsed.</param>
        /// <param name="ignoreEmptyLines">Do not add empty lines to the resulting list.</param>
        /// <exception cref="System.FormatException">Thrown when the file could not be parsed. Never thrown if ignoreIllegalLines is set to true.</exception>
        /// <returns>The parsed data, as list of string arrays.</returns>
        public static List<String[]> SplitCsvFile(String fileContents, Char separator, Boolean ignoreIllegalLines, Boolean ignoreEmptyLines)
        {
            try
            {
                using (StringReader sr = new StringReader(fileContents))
                    return SplitCsvFile(sr, separator, ignoreIllegalLines, ignoreEmptyLines);
            }
            catch (MalformedLineException mfle)
            {
                throw new FormatException(String.Format("Could not parse line {0} in the given data!", mfle.LineNumber));
            }
        }

        /// <summary>
        /// Parses CSV data into a list of String arrays.
        /// This override has support for multi-line values.
        /// </summary>
        /// <param name="fileContents">File contents</param>
        /// <param name="separator">Separator character</param>
        /// <param name="ignoreIllegalLines">Do not abort on lines that can't be parsed.</param>
        /// <param name="ignoreEmptyLines">Do not add empty lines to the resulting list.</param>
        /// <exception cref="Microsoft.VisualBasic.FileIO.MalformedLineException">Thrown when the file could not be parsed. Never thrown if ignoreIllegalLines is set to true.</exception>
        /// <returns>The parsed data, as list of string arrays.</returns>
        public static List<String[]> SplitCsvFile(TextReader fileContents, Char separator, Boolean ignoreIllegalLines, Boolean ignoreEmptyLines)
        {
            List<String[]> splitLines = new List<String[]>();
            using (TextFieldParser tfp = new TextFieldParser(fileContents))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.Delimiters = new String[] {separator.ToString()};
                while (true)
                {
                    try
                    {
                        String[] curLine = tfp.ReadFields();
                        if (curLine == null)
                            break;
                        if (ignoreEmptyLines && (curLine.Length == 0 || curLine.All(x => String.IsNullOrEmpty(x) || String.IsNullOrEmpty(x.Trim()))))
                            continue;
                        splitLines.Add(curLine);
                    }
                    catch (MalformedLineException)
                    {
                        if (!ignoreIllegalLines)
                            throw;
                    }
                }
            }
            return splitLines;
        }

        /// <summary>
        /// Parses a CSV file into a list of String arrays.
        /// This function needs the pre-split lines of a file, meaning it will most likely not handle multi-line content correctly.
        /// </summary>
        /// <param name="fileLines">The lines read from a CSV file.</param>
        /// <param name="separator">Separator character</param>
        /// <param name="ignoreIllegalLines">Do not abort on lines that can't be parsed.</param>
        /// <param name="ignoreEmptyLines">Do not add empty lines to the resulting list.</param>
        /// <exception cref="Microsoft.VisualBasic.FileIO.MalformedLineException">Thrown when the file could not be parsed. Never thrown if ignoreIllegalLines is set to true.</exception>
        /// <returns>The parsed data, as list of string arrays.</returns>
        public static List<String[]> SplitCsvFile(String[] fileLines, Char separator, Boolean ignoreIllegalLines, Boolean ignoreEmptyLines)
        {
            List<String[]> splitLines = new List<String[]>();
            for (Int32 i = 0; i < fileLines.Length; i++)
            {
                try
                {
                    String[] curLine = SplitCsvRow(fileLines[i], separator);
                    if (ignoreEmptyLines && (curLine == null || curLine.Length == 0 || curLine.All(x => String.IsNullOrEmpty(x) || String.IsNullOrEmpty(x.Trim()))))
                        continue;
                    splitLines.Add(curLine);
                }
                catch (MalformedLineException)
                {
                    if (!ignoreIllegalLines)
                        throw new FormatException(String.Format("Could not parse line {0} in the given data!", i));
                }
            }
            return splitLines;
        }

        public static String[] SplitCsvRow(String line, Char separator)
        {
            using (StringReader sr = new StringReader(line))
            using (TextFieldParser tfp = new TextFieldParser(sr))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.Delimiters = new String[] { separator.ToString() };
                return tfp.ReadFields();
            }
        }

    }
}