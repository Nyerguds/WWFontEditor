using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nyerguds.Util.Csv;

namespace WWFontEditor.Domain
{

    public class UnicodeInfo
    {
        [CsvColumn("Id", "^[0-9A-Z]+$")]
        public String Id
        {
            get { return this.IdNum.ToString("X4"); }
            set { this.IdNum = Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 IdNum { get; private set; }
        [CsvColumn("Name")]
        public String Name { get; set; }
        [CsvColumn("Category")]
        public String Category { get; set; }
        [CsvColumn("Combining")]
        public String Combining { get; set; }
        [CsvColumn("Bidi")]
        public String Bidi { get; set; }
        [CsvColumn("Decomposition")]
        public String Decomposition { get; set; }
        [CsvColumn("DecDig")]
        public String DecDig { get; set; }
        [CsvColumn("NumDig")]
        public String NumDig { get; set; }
        [CsvColumn("Num")]
        public String Num { get; set; }
        [CsvColumn("Mirrored")]
        public String Mirrored { get; set; }
        [CsvColumn("OldName")]
        public String OldName { get; set; }
        [CsvColumn("ISOComment")]
        public String ISOComment { get; set; }
        [CsvColumn("UpperCase")]
        public String UpperCase
        {
            get { return this.UpperCaseNum.ToString("X4"); }
            set { this.UpperCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 UpperCaseNum { get; private set; }
        [CsvColumn("LowerCase")]
        public String LowerCase
        {
            get { return this.LowerCaseNum.ToString("X4"); }
            set { this.LowerCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 LowerCaseNum { get; private set; }
        [CsvColumn("TitleCase")]
        public String TitleCase
        {
            get { return this.TitleCaseNum.ToString("X4"); }
            set { this.TitleCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 TitleCaseNum { get; private set; }

        public UnicodeInfo CloneSymbol()
        {
            return (UnicodeInfo)this.MemberwiseClone();
        }

        private static List<UnicodeInfo> allUnicodeInfo;
        public static List<UnicodeInfo> AllUnicodeInfo
        {
            get
            {
                if (allUnicodeInfo != null)
                    return allUnicodeInfo;
                List<String[]> split = CsvConverter.SplitCsvFile(Properties.Resources.UnicodeDescriptions, ';', true, true);
                allUnicodeInfo = CsvParser.ParseCsvInfo<UnicodeInfo>(split);

                // Renaming this to fix it getting lowercased.
                RenameRange(allUnicodeInfo, "CJK Compatibility Ideograph", 0xF900, 0xFAD9);

                Int32 unicodeInfoCount = allUnicodeInfo.Count;
                for (Int32 i = 0; i < unicodeInfoCount; ++i)
                {
                    UnicodeInfo info = allUnicodeInfo[i];
                    if (info.Name == "<control>")
                        info.Name = info.OldName;
                    String desc = info.Name;
                    if (!String.IsNullOrEmpty(desc) && desc.ToUpperInvariant() == desc)
                        info.Name = desc[0].ToString().ToUpperInvariant() + desc.Substring(1).ToLowerInvariant();
                }
                // Manual fixes. Easier than adding it all to the file.
                    
                // Adding these; the file only contains "begin" and "end" indicators for them.
                // I could also automate detection of such begin and end ranges, I guess... but meh, this works.
                AddRange(allUnicodeInfo, "CJK Ideograph Extension A", 0x3400, 0x4DB5);
                AddRange(allUnicodeInfo, "CJK Ideograph", 0x4E00, 0x9FEF);
                AddRange(allUnicodeInfo, "Hangul Syllable", 0xAC00, 0xD7A3);
                AddRange(allUnicodeInfo, "Non Private Use High Surrogate", 0xD800, 0xDB7F);
                AddRange(allUnicodeInfo, "Private Use High Surrogate", 0xDB80, 0xDBFF);
                AddRange(allUnicodeInfo, "Low Surrogate", 0xDC00, 0xDFFF);
                AddRange(allUnicodeInfo, "Private Use", 0xE000, 0xF8FF);
                allUnicodeInfo = allUnicodeInfo.OrderBy(c => c.IdNum).ToList();
                return allUnicodeInfo;
            }
        }

        private static void RenameRange(List<UnicodeInfo> list, String description, Int32 rangeStart, Int32 rangeEnd)
        {
            Int32 indexStart = list.FindIndex(c => c.IdNum == rangeStart);
            Int32 indexend = list.FindIndex(c => c.IdNum == rangeEnd);
            if (indexStart == -1 || indexend == -1)
                return;
            for (Int32 i = indexStart; i <= indexend; ++i)
            {
                UnicodeInfo cur = list[i];
                cur.Name = description + " " + cur.Id;
            }
        }

        private static void AddRange(List<UnicodeInfo> list, String description, Int32 rangeStart, Int32 rangeEnd)
        {
            UnicodeInfo startSymbol = list.FirstOrDefault(c => c.IdNum == rangeStart);
            if (startSymbol == null)
                startSymbol = new UnicodeInfo();
            list.RemoveAll(c => c.IdNum >= rangeStart && c.IdNum <= rangeEnd);
            for (Int32 i = rangeStart; i <= rangeEnd; ++i)
            {
                UnicodeInfo cur = startSymbol.CloneSymbol();
                cur.IdNum = i;
                cur.Name = description + " " + i.ToString("X");
                list.Add(cur);
            }
        }

        public static UnicodeInfo GetForId(Int32 id)
        {
            return AllUnicodeInfo.FirstOrDefault(x => x.IdNum == id);
        }
    }
}