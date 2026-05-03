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
        public Encoding Encoding { get; private set; }

        public EncodingDropDownInfo(Encoding enc)
        {
            this.Encoding = enc;
        }

        public override String ToString()
        {
            // why is it called all strange anyway? Should just be DOS 437...
            if (Encoding.CodePage == 437)
                return "DOS 437 - United States";
            // Mostly just done to avoid mangling the Dune 2000 one.
            if (Encoding.CodePage == 0)
                return Encoding.EncodingName;
            String name = this.Encoding.EncodingName;
            Match match = regex_replacename.Match(this.Encoding.EncodingName);
            if (match.Success)
            {
                if (!match.Groups[2].Value.EndsWith(Encoding.CodePage.ToString()))
                    return match.Groups[2].Value + " " + Encoding.CodePage + " - " + match.Groups[1].Value;
                else
                    return match.Groups[2].Value + " - " + match.Groups[1].Value;
            }
            else
                return this.Encoding.EncodingName + " (" + Encoding.CodePage + ")";
        }
    }
}
