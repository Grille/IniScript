using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

using Grille.IO.IniScript.Evaluation;
using Grille.IO.IniScript.Tokenization;

using Utils;

using static Grille.IO.IniScript.Tokenization.TokenType;


internal class Parser                     
{
    private readonly Signature _sectionSignature0;
    private readonly Signature _sectionSignature1;

    public int TabSize { get; set; } = 4;

    internal Parser()
    {
        _sectionSignature0 = Signature.New().OpeningBracket('[').Parameter().ClosingBracket();
        _sectionSignature1 = Signature.New().OpeningBracket('[').Parameter().ClosingBracket().Symbol(':').Parameter();
    }

    public ScriptCreationObject Parse(string text)
    {
        var script = new ScriptCreationObject();
        Parse(text, script);
        return script;
    }

    public ScriptCreationObject Parse(Stream stream)
    {
        var script = new ScriptCreationObject();
        Parse(stream, script);
        return script;
    }

    public ScriptCreationObject Parse(TextReader reader)
    {
        var script = new ScriptCreationObject();
        Parse(reader, script);
        return script;
    }

    public void Parse(TextReader reader, ScriptCreationObject script)
    {
        var text = reader.ReadToEnd();
        Parse(text, script);
    }

    public void Parse(Stream stream, ScriptCreationObject script)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        Parse(reader, script);
    }

    public void Parse(string text, ScriptCreationObject script)
    {
        var lines = ParserLexer.Lexer.Tokenize(text);
        var purged = PurgeTokens(lines);

        for (int i = 0; i < purged.Lines.Length; i++)
        {
            ParseLine(purged, i, script);
        }
    }

    private void ParseLine(PurgedLines purged, int index, ScriptCreationObject script)
    {
        var line = purged.Lines[index];

        string AsIdentifier(Argument arg) => ((Identifier)arg.Value).Literal;

        if (_sectionSignature0.Matches(line))
        {
            var args = _sectionSignature0.ExtractArguments(line);
            var name = AsIdentifier(args[0]);
            script.GetSection(name);
        }
        else if (_sectionSignature1.Matches(line))
        {
            var args = _sectionSignature1.ExtractArguments(line);
            var name = AsIdentifier(args[0]);
            var parentName = AsIdentifier(args[1]);
            script.GetSection(name, parentName);
        }
        else
        {
            script.CurrentSection.Add(new(purged.Lines, index, purged.Locations[index]));
        }
    }

    private record class PurgedLines(RangedArray<Token> Lines, InstructionLocation[] Locations);

    private PurgedLines PurgeTokens(RangedArray<Token> lines)
    {
        int capacity = lines.Items.Length;
        var purgedTokens = new RangedArrayBuilder<Token>(capacity);
        var locations = new List<InstructionLocation>(capacity);

        for (int ix = 0; ix < lines.Length; ix++)
        {
            var tokens = lines[ix];

            int line = tokens[0].Location.Row;
            int indentation = 0;

            for (int iy = 0; iy < tokens.Length; iy++)
            {
                var token = tokens[iy];
                if (token == Whitespace)
                {
                    if (iy == 0)
                    {
                        indentation = GetIndentation(token);
                    }
                    continue;
                }
                else if (token == EndOfLine || token == EndOfFile || token == Comment)
                {
                    continue;
                }
                purgedTokens.Add(token);
            }
            if (purgedTokens.YieldRange() != -1)
            {
                locations.Add(new(line, indentation));
            }
        }

        return new(purgedTokens.ToArray(), locations.ToArray());
    }

    private int GetIndentation(ReadOnlySpan<char> text)
    {
        int count = 0;
        for (int i = 0; i < text.Length; i++)
        {
            count += text[i] switch
            {
                ' ' => 1,
                '\t' => TabSize,
                _ => throw new InvalidDataException(),
            };
        }
        return count;
    }
}
