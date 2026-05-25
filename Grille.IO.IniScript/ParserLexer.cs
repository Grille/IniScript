using Grille.IO.IniScript.Utils;

namespace Grille.IO.IniScript;

using static TokenType;
using CS = CharSets;
using Rule = LexerRule;

file static class ParserRules
{
    extension(LexerRuleContext ctx)
    {
        public char Char0 => ctx.GetChar(0);

        public char Char1 => ctx.GetChar(1);

        public bool IsWhitespace() => ctx.Is(CS.Whitespaces);

        public bool IsSymbol() => ctx.Is(CS.Symbols) && ctx.TokenLength == 0;

        public bool IsBracket() => ctx.Is(CS.Brackets) && ctx.TokenLength == 0;

        public bool IsStringBegin() => ctx.Is('"');

        public bool IsStringContinue(int minLength) => !(!ctx.Is('\\', -2) && ctx.Is('"', -1) && ctx.TokenLength > minLength);

        public bool IsStringContinue() => ctx.IsStringContinue(1);

        public bool IsIStringBegin() => ctx.Is('$') && ctx.Is('"', 1);

        public bool IsIStringContinue() => ctx.IsStringContinue(2);

        public bool IsLineCommentBegin() => ctx.Is(";#") || (ctx.Is('/') && ctx.Is('/', 1));

        public bool IsNotEndOfLine() => !ctx.IsEndOfLine();

        public bool IsMLCommentBegin() => ctx.Is('/') && ctx.Is('*', 1);

        public bool IsMLCommentContinue() => !(ctx.Is('*', -2) && ctx.Is('/', -1) && ctx.TokenLength > 2);

        public bool IsNumber()
        {
            if (ctx.TokenLength == 0)
            {
                return CS.IsDigit(ctx[0]) || (ctx.Is('.') && CS.IsDigit(ctx[1]));
            }
            return CS.IsLetterOrDigit(ctx[0]) || ctx.Is("._") || (ctx.Is("+-") && ctx.Is("eE", -1));
        }

        public bool IsWord()
        {
            if (ctx.TokenLength == 0)
            {
                return CS.IsLetter(ctx[0]) || ctx.Is('_');
            }
            return CS.IsLetterOrDigit(ctx[0]) || ctx.Is("._");
        }
    }

    public static readonly Rule[] Rules = [
        Rule.EndOfLine,
        new Rule(Whitespace, IsWhitespace),
        new Rule(LiteralString, IsStringBegin, IsStringContinue),
        new Rule(InterpolatedString, IsIStringBegin, IsIStringContinue),
        new Rule(Comment, IsLineCommentBegin, IsNotEndOfLine),
        new Rule(Comment, IsMLCommentBegin, IsMLCommentContinue),
        new Rule(Symbol, IsSymbol),
        new Rule(Bracket, IsBracket),
        new Rule(Word, IsWord),
        new Rule(Number, IsNumber),
    ];
}

internal static class ParserLexer
{
    public static readonly Lexer Lexer = new(ParserRules.Rules);
}
