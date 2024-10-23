using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Grille.IO.IniScript.Utils;

using static Grille.IO.IniScript.Utils.TokenType;

namespace Grille.IO.IniScript;

public class Parser
{
    internal Lexer _lexer;

    public int TabSize { get; set; } = 4;

    public bool ParseEmpty { get; set; }

    public Parser()
    {
        var rules = new Lexer.Rule[] {
            new Lexer.Rule(TokenType.Whitespace, (a) => a.IsWhitespace()),
            new Lexer.Rule(TokenType.String, (a) => a.IsStringBegin(), (a) => !a.IsStringEnd()),
            new Lexer.Rule(TokenType.InterpolatedString, (a) => a.IsIStringBegin(), (a) => !a.IsIStringEnd()),
            new Lexer.Rule(TokenType.Section, (a) => a.GetChar() == '[', (a) => a.GetChar(-1) != ']'),
            new Lexer.Rule(TokenType.Comment, (a) => a.BeginComment(), (a) => true),
            new Lexer.Rule(TokenType.Symbol, (a) => a.IsSymbol(), (a) => false),
            new Lexer.Rule(TokenType.Word, (a) => a.IsLetter(), (a) => a.IsWord()),
            new Lexer.Rule(TokenType.Number, (a) => a.IsDigit(), (a) => a.IsWord()),
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

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.Length == 0)
            {
                continue;
            }

            ParseLine(lines[i], script);
        }
    }

    void ParseLine(Token[] tokens, Script script)
    {
        int index = 0;
        int line = tokens[0].Row;
        int indentation = tokens[0].Column;

        string? comment = null;
        var purgedList = new List<Token>();

        bool isFunctionDefinition = false;
        bool isAssignment = false;
        bool isAssignmentCall = false;

        Instruction Instruction(string key, Argument[] args) => new Instruction(key, args, line, indentation, comment);

        while (index < tokens.Length)
        {
            var token = tokens[index++];
            if (token == Whitespace || token == (Symbol, ")"))
            {
                if (index == 1)
                {
                    indentation = GetIndentation(token.Value);
                }
                continue;
            }
            else if (token == Comment)
            {
                comment = token.Value;
                continue;
            }

            int position = purgedList.Count;
            if (position == 0)
            {
                if (token == Section)
                {
                    isFunctionDefinition = true;
                }
            }
            else if (position == 1)
            {
                if (token == (Symbol, "="))
                {
                    isAssignment = true;
                    continue;
                }
            }
            else if (position == 2)
            {
                if (isAssignment && token == (Symbol, "("))
                {
                    isAssignmentCall = true;
                    continue;
                }
            }
            else if (token == (Symbol, "("))
            {
                continue;
            }

            purgedList.Add(token);
        }

        if (isFunctionDefinition)
        {
            script.Section(purgedList[0].GetStringValue());
            for (int i = 1; i < purgedList.Count; i++)
            {
                var instruction0 = Instruction("Pop", [purgedList[i].Value]);
                script.ActiveSection.Add(instruction0);
            }
        }
        else if (isAssignmentCall)
        {
            var args = new Argument[purgedList.Count - 1];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = purgedList[i + 1].Value;
            }

            var instruction0 = Instruction("Call", args);
            var instruction1 = Instruction("Pop", [purgedList[0].Value]);
            script.ActiveSection.Add(instruction0);
            script.ActiveSection.Add(instruction1);
        }
        else if (isAssignment)
        {
            var instruction = Instruction("Set", [purgedList[0].Value, purgedList[1].Value]);
            script.ActiveSection.Add(instruction);
        }
        else
        {
            if (purgedList.Count == 0)
            {
                return;
            }

            string key = purgedList[0].Value;
            var args = new Argument[purgedList.Count - 1];
            for (int i = 0; i < purgedList.Count - 1; i++)
            {
                args[i] = purgedList[i + 1].Value;
            }

            var instruction = Instruction(key, args);
            script.ActiveSection.Add(instruction);
        }
    }

    void ParseFunctionDefinition(Token[] tokens, int indentation, Script script)
    {

    }

    void ParseInstruction(Token[] tokens, int indentation, Script script)
    {

    }

    private int GetIndentation(string text)
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
