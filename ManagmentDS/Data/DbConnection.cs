using System.Data.SqlClient;

namespace ManagmentDS.Data
{
    public class DbConnection
    {
        private string _connectionString;

        public void Configure(string server, string database)
        {
            _connectionString =
                $"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public SqlConnection Create()
        {
            return new SqlConnection(_connectionString);
        }

        public bool IsConfigured => !string.IsNullOrEmpty(_connectionString);
    }
}
