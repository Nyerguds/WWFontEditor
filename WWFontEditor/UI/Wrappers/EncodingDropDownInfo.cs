using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nyerguds.Util.UI.Wrappers
{
    public class EncodingDropDownInfo
    {
        protected static readonly Regex regex_replacename = new Regex(@"^(.+)\s*\((.+)\)$");
        protected String labelOverride;
        public Encoding Encoding { get; private set; }

        
        public EncodingDropDownInfo(Encoding enc) : this(enc, null)
        {
        }
        
        public EncodingDropDownInfo(Encoding enc, String labelOverride)
        {
            // Avoid null items that would crash ToString()
            if (enc == null && labelOverride == null)
                labelOverride = "null";
            this.Encoding = enc;
            this.labelOverride = labelOverride;
        }
        
        public override String ToString()
        {
            if (labelOverride != null)
                return labelOverride;
            // why is it called all strange anyway? Should just be DOS 437...
            if (Encoding.CodePage == 437)
                return "DOS-437 - United States";
            // Mostly just done to avoid mangling the Dune 2000 one.
            if (Encoding.CodePage == 0)
                return Encoding.EncodingName;
            Match match = regex_replacename.Match(this.Encoding.EncodingName);
            if (match.Success)
            {
                if (!match.Groups[2].Value.EndsWith(Encoding.CodePage.ToString()))
                {
                    if (match.Groups[2].Value == "ISO")
                        return Encoding.WebName.ToUpper() + " - " + match.Groups[1].Value;
                    return match.Groups[2].Value + "-" + Encoding.CodePage + " - " + match.Groups[1].Value;
                }
                return match.Groups[2].Value + " - " + match.Groups[1].Value;
            }
            return this.Encoding.EncodingName + " (" + Encoding.CodePage + ")";
        }

        public static List<EncodingDropDownInfo> GetAsDropDownItems(IEnumerable<Encoding> encodings)
        {
            return encodings.Select(e => new EncodingDropDownInfo(e)) // Put in wrapper class to add a ToString() for the dropdown
                .OrderBy(n => n.ToString(), new ExplorerFileComparer()) // Order by name as returned by wrapper class (with extra info first)
                .ToList();
        }
    }
}
