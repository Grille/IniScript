using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Grille.IO.IniScript.Utils;

namespace Grille.IO.IniScript;

public class Parser
{
    internal Lexer _lexer;

    public bool ParseEmpty { get; set; }

    public Parser()
    {
        var rules = new Lexer.Rule[] {
            new Lexer.Rule(TokenType.String, (a) => a.IsStringBegin(), (a) => !a.IsStringEnd()),
            new Lexer.Rule(TokenType.InterpolatedString, (a) => a.GetChar() == '$', (a) => !a.IsStringEnd()),
            new Lexer.Rule(TokenType.Section, (a) => a.GetChar() == '[', (a) => a.GetChar(-1) != ']'),
            new Lexer.Rule(TokenType.Comment, (a) => a.BeginComment(), (a) => true),
            new Lexer.Rule(TokenType.Symbol, (a) => a.IsSymbol(), (a) => a.IsSymbol()),
            new Lexer.Rule(TokenType.Word, (a) => a.IsWord(), (a) => a.IsWordOrNumber()),
            new Lexer.Rule(TokenType.Number, (a) => a.IsNumber(), (a) => a.IsWordOrNumber()),
        };

        _lexer = new Lexer(rules);
        ParseEmpty = true;
    }

    public Script Parse(string text)
    {
        var script = new Script();
        Parse(text, script);
        return script;
    }

    public Script Parse(Stream stream)
    {
        var script = new Script();
        Parse(stream, script);
        return script;
    }

    public Script Parse(TextReader reader)
    {
        var script = new Script();
        Parse(reader, script);
        return script;
    }

    public void Parse(string text, Script script)
    {
        using var reader = new StringReader(text);
        Parse(reader, script);
    }

    public void Parse(Stream stream, Script script)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        Parse(reader, script);
    }

    public void Parse(TextReader reader, Script script)
    {
        var lines = _lexer.Tokenize(reader);

        var args = new List<Argument>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            int indentation = 0;
            string? key = null;
            string? comment = null;
            bool isSection = false;

            args.Clear();

            foreach (var token in line)
            {
                if (token.Type == TokenType.Word || token.Type == TokenType.String)
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
                else if (token.Type == TokenType.Section && key == null)
                {
                    key = token.GetStringValue();
                    isSection = true;
                }
                else if (token.Type == TokenType.Symbol)
                {
                    continue;
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

            var instruction = new Instruction(key, args.ToArray(), indentation, comment);

            if (isSection)
            {
                var section = new Function(instruction);
                script.Add(section);
            }
            else if (key != null)
            {
                script.ActiveSection.Add(instruction);
            }
            else if (ParseEmpty)
            {
                script.ActiveSection.Add(instruction);
            }
        }
    }
}
