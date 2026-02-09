using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ManagmentDS.Scripting
{
    public static class IndexScriptGenerator
    {
        public static List<string> Generate(
            SqlConnection conn,
            string schema,
            string table,
            HashSet<string> allowedColumns)
        {
            List<string> scripts = new();

            var indexes = new List<(string Name, bool Unique, string Type, string Filter)>();

            string sql = @"
SELECT name, is_unique, type_desc, filter_definition
FROM sys.indexes
WHERE object_id = OBJECT_ID(@obj)
AND name IS NOT NULL
AND is_primary_key = 0";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@obj", $"{schema}.{table}");

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        indexes.Add((
                            r.GetString(0),
                            r.GetBoolean(1),
                            r.GetString(2),
                            r.IsDBNull(3) ? null : r.GetString(3)
                        ));
                    }
                }
            }

            foreach (var idx in indexes)
            {
                List<string> cols = LoadIndexColumns(conn, idx.Name, schema, table);

                if (!cols.All(c => allowedColumns.Contains(c)))
                    continue;

                StringBuilder sb = new();
                sb.Append("CREATE ");
                if (idx.Unique) sb.Append("UNIQUE ");
                sb.Append($"{idx.Type} INDEX {idx.Name} ON #temp (");
                sb.Append(string.Join(",", cols));
                sb.Append(")");

                if (!string.IsNullOrWhiteSpace(idx.Filter))
                    sb.Append($" WHERE {idx.Filter}");

                sb.Append(";");

                scripts.Add(sb.ToString());
            }

            return scripts;
        }

        private static List<string> LoadIndexColumns(
            SqlConnection conn,
            string index,
            string schema,
            string table)
        {
            List<string> cols = new();

            string sql = @"
SELECT c.name
FROM sys.index_columns ic
JOIN sys.columns c ON ic.column_id=c.column_id AND ic.object_id=c.object_id
JOIN sys.indexes i ON i.index_id=ic.index_id AND i.object_id=ic.object_id
WHERE i.name=@idx AND i.object_id=OBJECT_ID(@obj)
ORDER BY ic.key_ordinal";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@idx", index);
            cmd.Parameters.AddWithValue("@obj", $"{schema}.{table}");

            using SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                cols.Add(r.GetString(0));

            return cols;
        }
    }
}
