using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ManagmentDS.Editor
{
    public sealed class SqlSyntaxHighlighterV2
    {
        private readonly RichTextBox _box;
        private bool _updating;

        // ===== REGEX =====
        private static readonly Regex Keywords = new(
            @"\b(SELECT|FROM|WHERE|JOIN|INNER|LEFT|RIGHT|FULL|ON|INSERT|INTO|UPDATE|DELETE|CREATE|ALTER|DROP|TABLE|VIEW|PROCEDURE|FUNCTION|BEGIN|END|GROUP|BY|ORDER|HAVING|DISTINCT|SET|NOCOUNT)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Functions = new(
            @"\b(COUNT|SUM|AVG|MIN|MAX|COALESCE|ISNULL|CAST|CONVERT|LEN|SUBSTRING|GETDATE)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Strings = new(
            @"'([^']|'')*'",
            RegexOptions.Compiled);

        private static readonly Regex Print = new(
            @"\b(PRINT|RAISERROR|THROW)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Comments = new(
            @"(--.*?$|/\*.*?\*/)",
            RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        // ===== KOLORY (JASNY MOTYW) =====
        private static readonly Color DefaultText = Color.FromArgb(30, 30, 30);
        private static readonly Color KeywordColor = Color.FromArgb(0, 0, 204);
        private static readonly Color FunctionColor = Color.FromArgb(121, 94, 38);
        private static readonly Color StringColor = Color.FromArgb(163, 21, 21);
        private static readonly Color PrintColor = Color.FromArgb(111, 66, 193);
        private static readonly Color CommentColor = Color.FromArgb(0, 128, 0);

        public SqlSyntaxHighlighterV2(RichTextBox box)
        {
            _box = box;

            // tło + bazowy kolor tekstu
            _box.BackColor = Color.White;
            _box.ForeColor = DefaultText;

            _box.TextChanged += (_, _) => Highlight();
        }

        private void Highlight()
        {
            if (_updating) return;
            _updating = true;

            int selStart = _box.SelectionStart;
            int selLength = _box.SelectionLength;

            _box.SuspendLayout();

            // reset do koloru bazowego
            _box.SelectAll();
            _box.SelectionColor = DefaultText;

            Apply(Comments, CommentColor);
            Apply(Strings, StringColor);
            Apply(Print, PrintColor);
            Apply(Functions, FunctionColor);
            Apply(Keywords, KeywordColor);

            _box.SelectionStart = selStart;
            _box.SelectionLength = selLength;
            _box.SelectionColor = DefaultText;

            _box.ResumeLayout();
            _updating = false;
        }

        private void Apply(Regex regex, Color color)
        {
            foreach (Match m in regex.Matches(_box.Text))
            {
                _box.Select(m.Index, m.Length);
                _box.SelectionColor = color;
            }
        }
    }
}
