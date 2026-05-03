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
            get { return IdNum.ToString("X4"); }
            set { IdNum = Int32.Parse(value, NumberStyles.HexNumber); }
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
            get { return UpperCaseNum.ToString("X4"); }
            set { UpperCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 UpperCaseNum { get; private set; }
        [CsvColumn("LowerCase")]
        public String LowerCase
        {
            get { return LowerCaseNum.ToString("X4"); }
            set { LowerCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 LowerCaseNum { get; private set; }
        [CsvColumn("TitleCase")]
        public String TitleCase
        {
            get { return TitleCaseNum.ToString("X4"); }
            set { TitleCaseNum = String.IsNullOrEmpty(value) ? -1 : Int32.Parse(value, NumberStyles.HexNumber); }
        }
        public Int32 TitleCaseNum { get; private set; }
        
        private static List<UnicodeInfo> allUnicodeInfo;
        public static List<UnicodeInfo> AllUnicodeInfo
        {
            get
            {
                if (allUnicodeInfo == null)
                {
                    List<String[]> split = CsvConverter.SplitCsvFile(global::WWFontEditor.Properties.Resources.UnicodeDescriptions, ';', true, true);
                    allUnicodeInfo = CsvParser.ParseCsvInfo<UnicodeInfo>(split);
                }
                return allUnicodeInfo;
            }
        }

        public static UnicodeInfo GetForId(Int32 id)
        {
            return AllUnicodeInfo.FirstOrDefault(x => x.IdNum == id);
        }
    }
}