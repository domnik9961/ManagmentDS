using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ManagmentDS.Scripting
{
    public static class ResultScriptGenerator
    {
        public static string Generate(
            int resultIndex,
            List<ColumnSchema> columns,
            List<string> indexScripts,
            DataTable data)
        {
            StringBuilder sb = new();

            sb.AppendLine();
            sb.AppendLine("-- =====================================================");
            sb.AppendLine($"-- RESULT {resultIndex}");
            sb.AppendLine("-- =====================================================");
            sb.AppendLine();

            // CREATE TABLE
            sb.AppendLine("-- CREATE TABLE");
            sb.AppendLine("CREATE TABLE #temp");
            sb.AppendLine("(");

            foreach (var c in columns)
                sb.AppendLine($"    {c.Name} {c.SqlType} {(c.IsNullable ? "NULL" : "NOT NULL")},");

            sb.Length -= 3;
            sb.AppendLine();
            sb.AppendLine(");");
            sb.AppendLine();

            // INDEXES
            if (indexScripts.Count > 0)
            {
                sb.AppendLine("-- INDEXES");
                foreach (string idx in indexScripts)
                    sb.AppendLine(idx);

                sb.AppendLine();
            }

            // INSERT
            sb.AppendLine("-- INSERT");
            sb.AppendLine(
                $"INSERT INTO #temp ({string.Join(",", columns.Select(c => c.Name))})");
            sb.AppendLine("VALUES");

            for (int i = 0; i < data.Rows.Count; i++)
            {
                var vals = data.Rows[i].ItemArray.Select(v =>
                    v == null || v == System.DBNull.Value
                        ? "NULL"
                        : $"'{v.ToString().Replace("'", "''")}'");

                sb.AppendLine(
                    $"({string.Join(",", vals)}){(i < data.Rows.Count - 1 ? "," : ";")}");
            }

            return sb.ToString();
        }
    }
}
