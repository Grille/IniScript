using Grille.IO.IniScript;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

using Grille.IO.IniScript.Evaluation;

using Utils;

using static Utils.TokenType;


internal class Parser                     
{
    private readonly Signature _sectionSignature0;
    private readonly Signature _sectionSignature1;
    private readonly Signature _assignmentSignature;

    public int TabSize { get; set; } = 4;

    public bool ParseEmpty { get; set; } = true;

    internal Parser()
    {
        _sectionSignature0 = Signature.New().OpeningBracket('[').Parameter().ClosingBracket();
        _sectionSignature1 = Signature.New().OpeningBracket('[').Parameter().ClosingBracket().Symbol(':').Parameter();
        _assignmentSignature = Signature.New().Parameter().Symbol('=');
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
        var instructions = ParserLexer.Lexer.Tokenize(text);

        for (int i = 0; i < instructions.Length; i++)
        {
            var line = instructions[i];
            if (line.Length > 1)
            {
                ParseLine(line, script);
            }
        }
    }

    void ParseLine(ReadOnlySpan<Token> tokens, ScriptCreationObject script)
    {
        int line = tokens[0].Location.Row;
        int indentation = 0;

        var comments = new List<string>();
        var purgedList = new List<Token>();

        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            if (token == Whitespace)
            {
                if (i == 0)
                {
                    indentation = GetIndentation(token);
                }
                continue;
            }
            else if (token == Comment)
            {
                comments.Add(token);
                continue;
            }
            else if (token == EndOfLine || token == EndOfFile)
            {
                continue;
            }
            purgedList.Add(token);
        }

        var purgedArray = purgedList.ToArray();

        string AsIdentifier(Argument arg) => ((Identifier)arg.Value).Literal;

        if (_sectionSignature0.Matches(purgedArray))
        {
            var args = _sectionSignature0.ExtractArguments(purgedArray);
            var name = AsIdentifier(args[0]);
            script.GetSection(name);
        }
        else if (_sectionSignature1.Matches(purgedArray))
        {
            var args = _sectionSignature1.ExtractArguments(purgedArray);
            var name = AsIdentifier(args[0]);
            var parentName = AsIdentifier(args[1]);
            script.GetSection(name, parentName);
        }
        else
        {
            bool isAssignment = purgedArray.Length > 2 && _assignmentSignature.Matches(purgedArray.AsSpan(0, 2));
            var location = new InstructionLocation(line, indentation);
            script.CurrentSection.Add(new InstructionInfo(purgedArray, location, comments.ToArray()));
        }
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
