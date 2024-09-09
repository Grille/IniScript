using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Grille.IO.Utils;

namespace Grille.IO;

public class IniScriptParser
{
    Lexer _lexer;

    public bool ParseEmpty { get; set; }

    public IniScriptParser()
    {
        _lexer = new Lexer();
        ParseEmpty = true;
    }

    public IniScript Parse(string text)
    {
        var script = new IniScript();
        Parse(text, script);
        return script;
    }

    public IniScript Parse(Stream stream)
    {
        var script = new IniScript();
        Parse(stream, script);
        return script;
    }

    public IniScript Parse(TextReader reader)
    {
        var script = new IniScript();
        Parse(reader, script);
        return script;
    }

    public void Parse(string text, IniScript script)
    {
        using var reader = new StringReader(text);
        Parse(reader, script);
    }

    public void Parse(Stream stream, IniScript script)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        Parse(reader, script);
    }

    public void Parse(TextReader reader, IniScript script)
    {
        var lines = _lexer.Tokenize(reader);

        var args = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            int indentation = 0;
            string? key = null;
            string? comment = null;

            if (line.Length > 0 && line[0].Type == TokenType.Section)
            {
                script.Section(line[0].GetStringValue());
                continue;
            }

            args.Clear();

            foreach (var token in line)
            {
                if (token.Type == TokenType.Value || token.Type == TokenType.String)
                {
                    var value = token.GetStringValue();
                    if (key == null)
                    {
                        indentation = token.Column;
                        key = value;
                    }
                    else
                    {
                        args.Add(value);
                    }
                }
                else if (token.Type == TokenType.Comment)
                {
                    comment = token.Value;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            if (key != null)
            {
                var entry = new IniScriptEntry(key, args.ToArray(), indentation, comment);
                script.ActiveSection.Entries.Add(entry);
            }
            else if (ParseEmpty)
            {
                var entry = new IniScriptEntry(null, null, 0, comment);
                script.ActiveSection.Entries.Add(entry);
            }
        }
    }
}
