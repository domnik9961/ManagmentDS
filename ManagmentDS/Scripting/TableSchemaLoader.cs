using System.Collections.Generic;
using System.Data.SqlClient;

namespace ManagmentDS.Scripting
{
    public sealed class ColumnSchema
    {
        public string Name;
        public string SqlType;
        public bool IsNullable;
    }

    public static class TableSchemaLoader
    {
        public static List<ColumnSchema> Load(
            SqlConnection conn,
            string schema,
            string table,
            IEnumerable<string> onlyColumns = null)
        {
            List<ColumnSchema> cols = new();

            string filter = onlyColumns != null
                ? "AND c.name IN (" + string.Join(",", onlyColumns.Select(c => $"'{c}'")) + ")"
                : "";

            string sql = $@"
SELECT c.name,
       t.name +
       CASE 
         WHEN t.name IN ('nvarchar','varchar','char','nchar') 
           THEN '(' + CASE WHEN c.max_length=-1 THEN 'MAX' ELSE CAST(c.max_length AS VARCHAR) END + ')'
         ELSE '' 
       END,
       c.is_nullable
FROM sys.columns c
JOIN sys.types t ON c.user_type_id=t.user_type_id
WHERE c.object_id = OBJECT_ID('{schema}.{table}')
{filter}
ORDER BY c.column_id";

            using SqlCommand cmd = new(sql, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                cols.Add(new ColumnSchema
                {
                    Name = r.GetString(0),
                    SqlType = r.GetString(1),
                    IsNullable = r.GetBoolean(2)
                });
            }

            return cols;
        }
    }
}
