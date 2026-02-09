using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;
using ManagmentDS.Editor;
using ManagmentDS.Scripting;

namespace ManagmentDS
{
    public partial class Form1 : Form
    {
        private TreeNode _originalTree;

        private SqlTemplateLoader _templates;
        private SqlAutoCompleteEngine _autoComplete;
        private SqlSyntaxHighlighterV2 _editorHighlighter;
        private SqlSyntaxHighlighterV2 _scriptHighlighter;

        private readonly List<string> _sqlMessages = new();
        private readonly List<DbObjectInfo> _dbObjectCache = new();

        private const int HOTKEY_ID = 1;
        private const uint MOD_WIN = 0x0008;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public Form1()
        {
            InitializeComponent();

            KeyPreview = true;
            KeyDown += Form1_KeyDown;

            treeDbObjects.BeforeExpand += treeDbObjects_BeforeExpand;
            txtFindObjSQL.TextChanged += txtFindObjSQL_TextChanged;

            txtSqlEditor.TextChanged += (_, __) => panelLineNumbers.Invalidate();
            txtSqlEditor.VScroll += (_, __) => panelLineNumbers.Invalidate();
            panelLineNumbers.Paint += panelLineNumbers_Paint;

            InitializeSqlIde();
            RegisterHotkeyFromJson();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }

        private void RegisterHotkeyFromJson()
        {
            try
            {
                string path = System.IO.Path.Combine(AppContext.BaseDirectory, "templates.json");
                if (!System.IO.File.Exists(path)) return;

                using JsonDocument doc = JsonDocument.Parse(System.IO.File.ReadAllText(path));

                if (!doc.RootElement.TryGetProperty("config", out var cfg)) return;
                if (!cfg.TryGetProperty("ui", out var ui)) return;
                if (!ui.TryGetProperty("hotkey", out var hk)) return;

                if (hk.GetString()?.Equals("Win+Y", StringComparison.OrdinalIgnoreCase) == true)
                {
                    RegisterHotKey(Handle, HOTKEY_ID, MOD_WIN, (uint)Keys.Y);
                }
            }
            catch { }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
                ToggleWindow();

            base.WndProc(ref m);
        }

        private void ToggleWindow()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
                tabControl1.SelectedTab = tabPage2;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void InitializeSqlIde()
        {
            _templates = SqlTemplateLoader.Load(
                System.IO.Path.Combine(AppContext.BaseDirectory, "templates.json"));

            if (!string.IsNullOrWhiteSpace(_templates.DefaultServer))
                txtSQLServer.Text = _templates.DefaultServer;

            _editorHighlighter = new SqlSyntaxHighlighterV2(txtSqlEditor);
            _scriptHighlighter = new SqlSyntaxHighlighterV2(txtGeneratedScript);

            _autoComplete = new SqlAutoCompleteEngine(
                txtSqlEditor,
                this,
                new[]
                {
                    "SELECT","FROM","WHERE","JOIN","INSERT","UPDATE","DELETE",
                    "CREATE","ALTER","DROP","BEGIN","END","PRINT"
                },
                _templates
            );
        }

        private void btnExecuteSql_Click(object sender, EventArgs e)
        {
            if (cBoxDatabaseList.SelectedValue == null) return;

            tabControlResults.TabPages.Clear();
            txtGeneratedScript.Clear();
            _sqlMessages.Clear();

            UpdateStatus("Executing SQL...");
            try
            {
                using SqlConnection conn = new(GetDbConnectionString());
                conn.FireInfoMessageEventOnUserErrors = true;
                conn.InfoMessage += Connection_InfoMessage;
                conn.Open();

                using SqlCommand cmd = new(txtSqlEditor.Text, conn)
                {
                    CommandTimeout = 60
                };
                using SqlDataReader r = cmd.ExecuteReader();

                var selectStatements = SqlBatchSplitter.SplitSelects(txtSqlEditor.Text);

                int idx = 1;
                while (r.HasRows)
                {
                    DataTable dt = ReadResultSet(r);
                    AddResultTab(dt, idx);

                    string select = idx - 1 < selectStatements.Count
                        ? selectStatements[idx - 1]
                        : string.Empty;

                    string script = GenerateScriptForResult(conn, select, dt, idx);
                    if (!string.IsNullOrWhiteSpace(script))
                        txtGeneratedScript.AppendText(script);

                    idx++;
                    r.NextResult();
                }

                AddMessagesTab();
                UpdateStatus("SQL executed successfully.");
            }
            catch (SqlException ex)
            {
                HandleException("SQL execution failed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                HandleException("SQL execution failed due to invalid state.", ex);
            }
            catch (Exception ex)
            {
                HandleException("Unexpected error during SQL execution.", ex);
            }
        }

        private static DataTable ReadResultSet(SqlDataReader r)
        {
            DataTable t = new();
            for (int i = 0; i < r.FieldCount; i++)
                t.Columns.Add(r.GetName(i));

            while (r.Read())
            {
                object[] v = new object[r.FieldCount];
                r.GetValues(v);
                t.Rows.Add(v);
            }
            return t;
        }

        private void AddResultTab(DataTable dt, int idx)
        {
            TabPage p = new($"Result {idx}");
            DataGridView g = new()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                DataSource = dt
            };
            p.Controls.Add(g);
            tabControlResults.TabPages.Add(p);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnExecuteSql_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        private void btnConnectionSQL_Click(object sender, EventArgs e)
        {
            UpdateStatus("Connecting to SQL Server...");
            try
            {
                using SqlConnection conn =
                    new($"Server={txtSQLServer.Text};Database=master;Trusted_Connection=True;");
                conn.Open();

                DataTable dt = new();
                new SqlDataAdapter("SELECT name FROM sys.databases", conn).Fill(dt);

                cBoxDatabaseList.DataSource = dt;
                cBoxDatabaseList.DisplayMember = "name";
                cBoxDatabaseList.ValueMember = "name";
                UpdateStatus("Connection successful.");
            }
            catch (SqlException ex)
            {
                HandleException("SQL connection failed.", ex);
            }
            catch (Exception ex)
            {
                HandleException("Unexpected error while connecting.", ex);
            }
        }

        private void cBoxDatabaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBoxDatabaseList.SelectedValue != null)
                LoadDatabaseObjects();
        }

        private string GetDbConnectionString() =>
            $"Server={txtSQLServer.Text};Database={cBoxDatabaseList.SelectedValue};Trusted_Connection=True;";

        private void LoadDatabaseObjects()
        {
            UpdateStatus("Loading database objects...");
            _dbObjectCache.Clear();
            try
            {
                using SqlConnection conn = new(GetDbConnectionString());
                conn.Open();

                string sql = @"
SELECT s.name AS SchemaName, o.name, o.type
FROM sys.objects o
JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE o.type IN ('U','V','P')
ORDER BY s.name, o.name";

                using SqlCommand cmd = new(sql, conn);
                using SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    _dbObjectCache.Add(new DbObjectInfo
                    {
                        Schema = r.GetString(0),
                        Name = r.GetString(1),
                        Type = r.GetString(2)
                    });
                }

                UpdateAutoCompleteObjects();
                BuildObjectTree(txtFindObjSQL.Text);
                UpdateStatus("Database objects loaded.");
            }
            catch (SqlException ex)
            {
                HandleException("Failed to load database objects.", ex);
            }
            catch (Exception ex)
            {
                HandleException("Unexpected error while loading objects.", ex);
            }
        }

        private void treeDbObjects_BeforeExpand(object sender, TreeViewCancelEventArgs e) { }

        private void txtFindObjSQL_TextChanged(object sender, EventArgs e)
        {
            BuildObjectTree(txtFindObjSQL.Text);
        }

        private void panelLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(SystemColors.Control);
            int firstLine = txtSqlEditor.GetLineFromCharIndex(
                txtSqlEditor.GetCharIndexFromPosition(new Point(0, 0)));
            int lastLine = txtSqlEditor.GetLineFromCharIndex(
                txtSqlEditor.GetCharIndexFromPosition(new Point(0, txtSqlEditor.Height)));

            using var brush = new SolidBrush(Color.DimGray);
            using var font = new Font("Consolas", 9F);

            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                int charIndex = txtSqlEditor.GetFirstCharIndexFromLine(i);
                if (charIndex < 0) continue;

                Point pos = txtSqlEditor.GetPositionFromCharIndex(charIndex);
                e.Graphics.DrawString(
                    (i + 1).ToString(),
                    font,
                    brush,
                    new PointF(5, pos.Y));
            }
        }

        private void BuildObjectTree(string filter)
        {
            treeDbObjects.BeginUpdate();
            treeDbObjects.Nodes.Clear();

            string match = filter?.Trim() ?? string.Empty;

            var groups = _dbObjectCache
                .Where(o =>
                    string.IsNullOrWhiteSpace(match) ||
                    o.Name.Contains(match, StringComparison.OrdinalIgnoreCase) ||
                    o.Schema.Contains(match, StringComparison.OrdinalIgnoreCase))
                .GroupBy(o => o.Type)
                .OrderBy(g => g.Key);

            foreach (var g in groups)
            {
                string header = g.Key switch
                {
                    "U" => "Tables",
                    "V" => "Views",
                    "P" => "Procedures",
                    _ => "Objects"
                };

                TreeNode parent = new(header);
                foreach (var obj in g)
                {
                    parent.Nodes.Add($"{obj.Schema}.{obj.Name}");
                }

                treeDbObjects.Nodes.Add(parent);
            }

            treeDbObjects.EndUpdate();
        }

        private void UpdateAutoCompleteObjects()
        {
            var objects = _dbObjectCache
                .Where(o => o.Type is "U" or "V")
                .Select(o => $"{o.Schema}.{o.Name}")
                .ToList();

            _autoComplete.UpdateDatabaseObjects(objects);
        }

        private string GenerateScriptForResult(
            SqlConnection conn,
            string sql,
            DataTable data,
            int idx)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return string.Empty;

            SelectAnalysis analysis = SelectAnalyzer.Analyze(sql);
            if (!analysis.HasFrom || string.IsNullOrWhiteSpace(analysis.Schema))
                return string.Empty;

            List<ColumnSchema> columns = TableSchemaLoader.Load(
                conn,
                analysis.Schema,
                analysis.Table,
                analysis.IsSelectStar ? null : analysis.Columns);

            if (columns.Count == 0)
                return string.Empty;

            HashSet<string> allowedColumns = new(
                columns.Select(c => c.Name),
                StringComparer.OrdinalIgnoreCase);

            List<string> indexScripts = IndexScriptGenerator.Generate(
                conn,
                analysis.Schema,
                analysis.Table,
                allowedColumns);

            return ResultScriptGenerator.Generate(idx, columns, indexScripts, data);
        }

        private void HandleException(string message, Exception ex)
        {
            _sqlMessages.Add($"{message} {ex.Message}");
            UpdateStatus(message);
            MessageBox.Show($"{message}\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateStatus(string message)
        {
            toolStripStatusLabel1.Text = message;
        }

        private void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            foreach (SqlError error in e.Errors)
            {
                if (!string.IsNullOrWhiteSpace(error.Message))
                    _sqlMessages.Add(error.Message);
            }
        }

        private void AddMessagesTab()
        {
            if (_sqlMessages.Count == 0)
                return;

            TabPage p = new("Messages");
            RichTextBox box = new()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10F),
                WordWrap = false,
                Text = string.Join(Environment.NewLine, _sqlMessages)
            };

            p.Controls.Add(box);
            tabControlResults.TabPages.Add(p);
        }

        private sealed class DbObjectInfo
        {
            public string Schema { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }
    }
}
