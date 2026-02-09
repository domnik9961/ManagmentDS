namespace ManagmentDS
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            splitContainer1 = new SplitContainer();
            txtSqlEditor = new RichTextBox();
            panelLineNumbers = new Panel();
            btnExecuteSql = new Button();
            tabControlResults = new TabControl();
            tabPageResult = new TabPage();
            dataGridViewResult = new DataGridView();
            tabPageScript = new TabPage();
            txtGeneratedScript = new RichTextBox();
            tabPage2 = new TabPage();
            panel1 = new Panel();
            label1 = new Label();
            txtSQLServer = new TextBox();
            btnConnectionSQL = new Button();
            label3 = new Label();
            cBoxDatabaseList = new ComboBox();
            label4 = new Label();
            txtFindObjSQL = new TextBox();
            treeDbObjects = new TreeView();
            statusStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControlResults.SuspendLayout();
            tabPageResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewResult).BeginInit();
            tabPageScript.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 578);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1100, 22);
            statusStrip1.TabIndex = 2;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(39, 17);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(225, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(875, 578);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(splitContainer1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(867, 550);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "SQL Lab";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(txtSqlEditor);
            splitContainer1.Panel1.Controls.Add(panelLineNumbers);
            splitContainer1.Panel1.Controls.Add(btnExecuteSql);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControlResults);
            splitContainer1.Size = new Size(867, 550);
            splitContainer1.SplitterDistance = 390;
            splitContainer1.TabIndex = 0;
            // 
            // txtSqlEditor
            // 
            txtSqlEditor.AcceptsTab = true;
            txtSqlEditor.Dock = DockStyle.Fill;
            txtSqlEditor.Font = new Font("Consolas", 11F);
            txtSqlEditor.Location = new Point(45, 0);
            txtSqlEditor.Name = "txtSqlEditor";
            txtSqlEditor.Size = new Size(822, 360);
            txtSqlEditor.TabIndex = 0;
            txtSqlEditor.Text = "";
            txtSqlEditor.WordWrap = false;
            // 
            // panelLineNumbers
            // 
            panelLineNumbers.Dock = DockStyle.Left;
            panelLineNumbers.Location = new Point(0, 0);
            panelLineNumbers.Name = "panelLineNumbers";
            panelLineNumbers.Size = new Size(45, 360);
            panelLineNumbers.TabIndex = 1;
            // 
            // btnExecuteSql
            // 
            btnExecuteSql.Dock = DockStyle.Bottom;
            btnExecuteSql.Location = new Point(0, 360);
            btnExecuteSql.Name = "btnExecuteSql";
            btnExecuteSql.Size = new Size(867, 30);
            btnExecuteSql.TabIndex = 2;
            btnExecuteSql.Text = "Execute";
            btnExecuteSql.Click += btnExecuteSql_Click;
            // 
            // tabControlResults
            // 
            tabControlResults.Controls.Add(tabPageResult);
            tabControlResults.Controls.Add(tabPageScript);
            tabControlResults.Dock = DockStyle.Fill;
            tabControlResults.Location = new Point(0, 0);
            tabControlResults.Name = "tabControlResults";
            tabControlResults.SelectedIndex = 0;
            tabControlResults.Size = new Size(867, 156);
            tabControlResults.TabIndex = 0;
            // 
            // tabPageResult
            // 
            tabPageResult.Controls.Add(dataGridViewResult);
            tabPageResult.Location = new Point(4, 24);
            tabPageResult.Name = "tabPageResult";
            tabPageResult.Size = new Size(859, 128);
            tabPageResult.TabIndex = 0;
            tabPageResult.Text = "Result";
            // 
            // dataGridViewResult
            // 
            dataGridViewResult.AllowUserToAddRows = false;
            dataGridViewResult.Dock = DockStyle.Fill;
            dataGridViewResult.Location = new Point(0, 0);
            dataGridViewResult.Name = "dataGridViewResult";
            dataGridViewResult.ReadOnly = true;
            dataGridViewResult.Size = new Size(859, 128);
            dataGridViewResult.TabIndex = 0;
            // 
            // tabPageScript
            // 
            tabPageScript.Controls.Add(txtGeneratedScript);
            tabPageScript.Location = new Point(4, 24);
            tabPageScript.Name = "tabPageScript";
            tabPageScript.Size = new Size(192, 0);
            tabPageScript.TabIndex = 1;
            tabPageScript.Text = "Script";
            // 
            // txtGeneratedScript
            // 
            txtGeneratedScript.Dock = DockStyle.Fill;
            txtGeneratedScript.Font = new Font("Consolas", 10F);
            txtGeneratedScript.Location = new Point(0, 0);
            txtGeneratedScript.Name = "txtGeneratedScript";
            txtGeneratedScript.ReadOnly = true;
            txtGeneratedScript.Size = new Size(192, 0);
            txtGeneratedScript.TabIndex = 0;
            txtGeneratedScript.Text = "";
            txtGeneratedScript.WordWrap = false;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(192, 72);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Snippet";
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(txtSQLServer);
            panel1.Controls.Add(btnConnectionSQL);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(cBoxDatabaseList);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(txtFindObjSQL);
            panel1.Controls.Add(treeDbObjects);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(225, 578);
            panel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 14F);
            label1.Location = new Point(15, 15);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 0;
            label1.Text = "Połączenie SQL";
            // 
            // txtSQLServer
            // 
            txtSQLServer.Location = new Point(5, 70);
            txtSQLServer.Name = "txtSQLServer";
            txtSQLServer.Size = new Size(215, 23);
            txtSQLServer.TabIndex = 1;
            // 
            // btnConnectionSQL
            // 
            btnConnectionSQL.Location = new Point(5, 100);
            btnConnectionSQL.Name = "btnConnectionSQL";
            btnConnectionSQL.Size = new Size(215, 23);
            btnConnectionSQL.TabIndex = 2;
            btnConnectionSQL.Text = "Połącz!";
            btnConnectionSQL.Click += btnConnectionSQL_Click;
            // 
            // label3
            // 
            label3.Location = new Point(5, 135);
            label3.Name = "label3";
            label3.Size = new Size(100, 23);
            label3.TabIndex = 3;
            label3.Text = "Baza danych";
            // 
            // cBoxDatabaseList
            // 
            cBoxDatabaseList.Location = new Point(5, 155);
            cBoxDatabaseList.Name = "cBoxDatabaseList";
            cBoxDatabaseList.Size = new Size(215, 23);
            cBoxDatabaseList.TabIndex = 4;
            cBoxDatabaseList.SelectedIndexChanged += cBoxDatabaseList_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.Location = new Point(5, 190);
            label4.Name = "label4";
            label4.Size = new Size(100, 23);
            label4.TabIndex = 5;
            label4.Text = "Wyszukaj obiekt:";
            // 
            // txtFindObjSQL
            // 
            txtFindObjSQL.Location = new Point(5, 210);
            txtFindObjSQL.Name = "txtFindObjSQL";
            txtFindObjSQL.Size = new Size(215, 23);
            txtFindObjSQL.TabIndex = 6;
            // 
            // treeDbObjects
            // 
            treeDbObjects.Location = new Point(5, 245);
            treeDbObjects.Name = "treeDbObjects";
            treeDbObjects.Size = new Size(215, 310);
            treeDbObjects.TabIndex = 7;
            // 
            // Form1
            // 
            ClientSize = new Size(1100, 600);
            Controls.Add(tabControl1);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ManagmentDS – SQL IDE";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControlResults.ResumeLayout(false);
            tabPageResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewResult).EndInit();
            tabPageScript.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;

        private Panel panel1;
        private Label label1;
        private Label label3;
        private Label label4;
        private TextBox txtSQLServer;
        private Button btnConnectionSQL;
        private ComboBox cBoxDatabaseList;
        private TextBox txtFindObjSQL;
        private TreeView treeDbObjects;

        private SplitContainer splitContainer1;
        private Panel panelLineNumbers;
        private RichTextBox txtSqlEditor;
        private Button btnExecuteSql;

        private TabControl tabControlResults;
        private TabPage tabPageResult;
        private TabPage tabPageScript;
        private DataGridView dataGridViewResult;
        private RichTextBox txtGeneratedScript;
    }
}
