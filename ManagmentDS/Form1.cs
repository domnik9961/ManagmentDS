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

            using SqlConnection conn = new(GetDbConnectionString());
            conn.Open();

            using SqlCommand cmd = new(txtSqlEditor.Text, conn);
            using SqlDataReader r = cmd.ExecuteReader();

            int idx = 1;
            while (r.HasRows)
            {
                DataTable dt = ReadResultSet(r);
                AddResultTab(dt, idx++);
                r.NextResult();
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
            using SqlConnection conn =
                new($"Server={txtSQLServer.Text};Database=master;Trusted_Connection=True;");
            conn.Open();

            DataTable dt = new();
            new SqlDataAdapter("SELECT name FROM sys.databases", conn).Fill(dt);

            cBoxDatabaseList.DataSource = dt;
            cBoxDatabaseList.DisplayMember = "name";
            cBoxDatabaseList.ValueMember = "name";
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
            treeDbObjects.Nodes.Clear();
            treeDbObjects.Nodes.Add("Tables");
        }

        private void treeDbObjects_BeforeExpand(object sender, TreeViewCancelEventArgs e) { }

        private void txtFindObjSQL_TextChanged(object sender, EventArgs e) { }

        private void panelLineNumbers_Paint(object sender, PaintEventArgs e) { }
    }
}
