using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ManagmentDS.Editor
{
    public class SqlTemplateLoader
    {
        public string DefaultServer { get; private set; }
        public List<SqlSnippet> Snippets { get; private set; } = new();

        public static SqlTemplateLoader Load(string path)
        {
            var json = File.ReadAllText(path);
            var doc = JsonDocument.Parse(json);

            var loader = new SqlTemplateLoader();

            loader.DefaultServer =
                doc.RootElement.GetProperty("config")
                               .GetProperty("sql")
                               .GetProperty("server")
                               .GetString();

            foreach (var sn in doc.RootElement.GetProperty("sqlSnippets").EnumerateArray())
            {
                loader.Snippets.Add(new SqlSnippet
                {
                    Shortcut = sn.GetProperty("shortcut").GetString(),
                    PasteText = sn.GetProperty("paste_text").GetString()
                });
            }

            return loader;
        }
    }

    public class SqlSnippet
    {
        public string Shortcut { get; set; }
        public string PasteText { get; set; }
    }
}
