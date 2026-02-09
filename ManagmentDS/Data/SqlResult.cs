using System.Data;

namespace ManagmentDS.Data
{
    public class SqlResult
    {
        public DataTable Data { get; set; }
        public int RowsAffected { get; set; }
        public string Error { get; set; }

        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
