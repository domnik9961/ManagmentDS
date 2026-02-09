using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace ManagmentDS.Scripting
{
    public sealed class SelectAnalysis
    {
        public bool HasFrom;
        public bool IsSelectStar;
        public string Schema;
        public string Table;
        public List<string> Columns = new();
    }

    public static class SelectAnalyzer
    {
        public static SelectAnalysis Analyze(string sql)
        {
            SelectAnalysis a = new();

            // tylko realna tabela schema.table
            var from = Regex.Match(
                sql,
                @"from\s+([\[\]\w]+)\.([\[\]\w]+)\b",
                RegexOptions.IgnoreCase);

            if (from.Success)
            {
                a.HasFrom = true;
                a.Schema = from.Groups[1].Value;
                a.Table = from.Groups[2].Value;
            }

            // kolumny SELECT
            var select = Regex.Match(
                sql,
                @"select\s+(.*?)\s+from",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!select.Success)
                select = Regex.Match(
                    sql,
                    @"select\s+(.*)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

            string cols = select.Groups[1].Value.Trim();

            if (cols == "*")
            {
                a.IsSelectStar = true;
            }
            else
            {
                a.Columns = cols
                    .Split(',')
                    .Select(c => c.Split(new[] { " as " }, System.StringSplitOptions.RemoveEmptyEntries)[0])
                    .Select(c => c.Trim().Split('.').Last())
                    .ToList();
            }

            return a;
        }
    }
}
