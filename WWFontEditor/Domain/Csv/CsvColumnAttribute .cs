using System;
using System.Text.RegularExpressions;

namespace Nyerguds.Util.Csv
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CsvColumnAttribute : System.Attribute
    {
        public String Name { get; private set; }
        public Regex ValidationRegex { get; private set; }

        public CsvColumnAttribute(String name) : this(name, null) { }

        public CsvColumnAttribute(String name, String validationRegex)
        {
            this.Name = name;
            this.ValidationRegex = new Regex(validationRegex ?? "^.*$");
        }
    }
}