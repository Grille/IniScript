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
    public int TabSize { get; set; } = 4;

    public AssemblyCreateInfo Parse(string text)
    {
        var assembly = new AssemblyCreateInfo();
        Parse(text, assembly);
        foreach (var pair in assembly)
        {
            var section = pair.Value;
            section.Array = section.List.ToArray();
            section.List = null!;
        }
        foreach (var pair in assembly)
        {
            var section = pair.Value;
            var array = section.Array;
            for (int i = 0; i < array.Length; i++)
            {
                section.Array[i].BlockSize = GetBlockSize(section, i);
            }
        }
        return assembly;
    }

    public AssemblyCreateInfo Parse(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        return Parse(reader);
    }

    public AssemblyCreateInfo Parse(TextReader reader)
    {
        return Parse(reader.ReadToEnd());
    }

    private void Parse(string text, AssemblyCreateInfo assembly)
    {
        var lines = ParserLexer.Lexer.Tokenize(text);
        var purged = PurgeTokens(lines);

        for (int i = 0; i < purged.Lines.Length; i++)
        {
            ParseLine(purged[i], assembly);
        }
    }

    private void ParseLine(PurgedLines.Line line, AssemblyCreateInfo assembly)
    {
        if (!MatchSectionSignatures(line, assembly))
        {
            assembly.CurrentSection.List.Add(line.ToInstruction());
        }
    }

    private bool MatchSectionSignatures(PurgedLines.Line line, AssemblyCreateInfo assembly)
    {
        var tokens = line.Tokens;

        foreach (var matchInfo in InternalSignatures.SectionSignatures)
        {
            if (!matchInfo.Signature.Matches(tokens)) continue;

            int argsIndex = 0;
            var args = matchInfo.Signature.ExtractArguments(tokens);
            string AsIdentifier() => ((Identifier)args[argsIndex++].Value).Literal;
            Parameter[] AsParameterArray() => (Parameter[])args[argsIndex++].Value;

            var section = assembly.GetSection(AsIdentifier());
            if (matchInfo.HasParameters) section.Parameters = AsParameterArray();
            if (matchInfo.HasParent) section.ParentKey = AsIdentifier();

            return true;
        }

        return false;
    }

    private record class PurgedLines(RangedArray<Token> Lines, InstructionLocation[] Locations)
    {
        public readonly record struct Line(PurgedLines Parent, int Index)
        {
            public ReadOnlySpan<Token> Tokens => Parent.Lines[Index];

            public InstructionCreateInfo ToInstruction() => new(Parent.Lines, Index, Parent.Locations[Index]);
        }

        public Line this[int index] => new Line(this, index);
    }

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

    private int GetBlockSize(AssemblyCreateInfo.Section func, int thisIndex)
    {
        var array = func.Array;

        var thisInstruction = array[thisIndex];
        var thisIndentation = thisInstruction.Location.Indentation;

        int nextIndex = thisIndex + 1;

        while (nextIndex < array.Length)
        {
            var nextInstruction = array[nextIndex];
            var nextIndentation = thisInstruction.Location.Indentation;

            if (nextIndentation <= thisIndentation && !nextInstruction.IsEmpty)
            {
                break;
            }

            nextIndex += 1;
        }

        return nextIndex - thisIndex - 1;
    }
}
