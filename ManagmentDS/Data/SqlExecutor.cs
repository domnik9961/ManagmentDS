using System;
using System.Data;
using System.Data.SqlClient;

namespace ManagmentDS.Data
{
    public class SqlExecutor
    {
        private readonly string _connectionString;

        public SqlExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlResult Execute(string sql)
        {
            var result = new SqlResult();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(sql, conn);

                    if (sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        var adapter = new SqlDataAdapter(cmd);
                        var table = new DataTable();
                        adapter.Fill(table);
                        result.Data = table;
                    }
                    else
                    {
                        result.RowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }
    }
}
