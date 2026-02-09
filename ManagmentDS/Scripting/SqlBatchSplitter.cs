using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ManagmentDS.Scripting
{
    public static class SqlBatchSplitter
    {
        public static List<string> SplitSelects(string sql)
        {
            List<string> selects = new();

            var matches = Regex.Matches(
                sql,
                @"select\s+.*?(?=select\s+|$)",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match m in matches)
                selects.Add(m.Value.Trim());

            return selects;
        }
    }
}
