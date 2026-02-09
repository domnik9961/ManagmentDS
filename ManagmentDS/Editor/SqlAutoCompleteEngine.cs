using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ManagmentDS.Editor
{
    public sealed class SqlAutoCompleteEngine
    {
        private readonly RichTextBox _editor;
        private readonly ListBox _listBox;

        private readonly List<string> _keywords = new();
        private readonly List<string> _dbObjects = new();
        private readonly List<SqlSnippet> _snippets;

        public SqlAutoCompleteEngine(
            RichTextBox editor,
            Control parent,
            IEnumerable<string> keywords,
            SqlTemplateLoader templates)
        {
            _editor = editor;
            _keywords.AddRange(keywords.Select(k => k.ToUpperInvariant()));
            _snippets = templates.Snippets;

            _listBox = new ListBox
            {
                Visible = false,
                Font = editor.Font,
                IntegralHeight = true,
                Height = 140
            };

            parent.Controls.Add(_listBox);
            _listBox.BringToFront();

            _editor.TextChanged += (_, __) => ShowSuggestions();
            _editor.KeyDown += Editor_KeyDown;
            _listBox.DoubleClick += (_, __) => InsertSelected();
        }

        // =====================================================
        // ===================== PUBLIC API ====================
        // =====================================================

        /// <summary>
        /// Aktualizuje listę obiektów DB (tabele, widoki)
        /// </summary>
        public void UpdateDatabaseObjects(IEnumerable<string> objects)
        {
            _dbObjects.Clear();
            _dbObjects.AddRange(objects.Select(o => o.ToUpperInvariant()));
        }

        // =====================================================
        // ===================== CORE LOGIC ====================
        // =====================================================

        private void ShowSuggestions()
        {
            string word = GetCurrentWord();
            if (word.Length < 1)
            {
                _listBox.Visible = false;
                return;
            }

            string prevToken = GetPreviousToken();

            IEnumerable<string> source =
                prevToken is "FROM" or "JOIN"
                    ? _dbObjects
                    : _keywords
                        .Concat(_dbObjects)
                        .Concat(_snippets.Select(s => s.Shortcut));

            var matches = source
                .Where(s => s.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            if (!matches.Any())
            {
                _listBox.Visible = false;
                return;
            }

            _listBox.BeginUpdate();
            _listBox.Items.Clear();
            _listBox.Items.AddRange(matches.ToArray());
            _listBox.EndUpdate();

            // <<< KLUCZOWE: zaznacz pierwszy element
            _listBox.SelectedIndex = 0;

            PositionListBox();
            _listBox.Visible = true;
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_listBox.Visible)
                return;

            // ↓ dół
            if (e.KeyCode == Keys.Down)
            {
                if (_listBox.SelectedIndex < _listBox.Items.Count - 1)
                    _listBox.SelectedIndex++;

                e.SuppressKeyPress = true;
            }
            // ↑ góra
            else if (e.KeyCode == Keys.Up)
            {
                if (_listBox.SelectedIndex > 0)
                    _listBox.SelectedIndex--;

                e.SuppressKeyPress = true;
            }
            // ENTER / TAB → wklejenie
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                InsertSelected();
                e.SuppressKeyPress = true;
            }
            // ESC → zamknięcie
            else if (e.KeyCode == Keys.Escape)
            {
                _listBox.Visible = false;
                e.SuppressKeyPress = true;
            }
        }

        private void InsertSelected()
        {
            if (_listBox.SelectedItem == null)
                return;

            string text = _listBox.SelectedItem.ToString();

            var snippet = _snippets.FirstOrDefault(s =>
                s.Shortcut.Equals(text, StringComparison.OrdinalIgnoreCase));

            ReplaceCurrentWord(snippet?.PasteText ?? text);
            _listBox.Visible = false;
        }

        // =====================================================
        // ===================== TEXT HELPERS ==================
        // =====================================================

        private string GetCurrentWord()
        {
            int pos = _editor.SelectionStart;
            int start = pos;

            while (start > 0 &&
                   (char.IsLetterOrDigit(_editor.Text[start - 1]) ||
                    _editor.Text[start - 1] == '_'))
                start--;

            return _editor.Text.Substring(start, pos - start);
        }

        private string GetPreviousToken()
        {
            string text = _editor.Text[.._editor.SelectionStart].TrimEnd();
            var parts = text.Split(
                new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= 2
                ? parts[^2].ToUpperInvariant()
                : string.Empty;
        }

        private void ReplaceCurrentWord(string replacement)
        {
            int pos = _editor.SelectionStart;
            int start = pos;

            while (start > 0 &&
                   (char.IsLetterOrDigit(_editor.Text[start - 1]) ||
                    _editor.Text[start - 1] == '_'))
                start--;

            _editor.Select(start, pos - start);
            _editor.SelectedText = replacement;
        }

        private void PositionListBox()
        {
            Point p = _editor.GetPositionFromCharIndex(_editor.SelectionStart);
            _listBox.Location = new Point(p.X + 50, p.Y + 25);
            _listBox.Width = 260;
        }
    }
}
