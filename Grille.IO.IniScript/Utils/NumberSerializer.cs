using System;

using Grille.IO.IniScript.Tokenization;

using Grille.IO.IniScript.Compilation;

namespace Grille.IO.IniScript.Utils;

internal static class NumberSerializer
{
    public readonly record struct Result(long Integer, double Decimal, bool IsDecimal)
    {
        public static Result operator -(Result value) => new Result(-value.Integer, -value.Decimal, value.IsDecimal);

        public object ToObject() => IsDecimal ? Decimal : Integer;
    }

    public const uint MaxBase = 10 + 'Z' - 'A';

    public const uint ExponentLimit = 10 + 'E' - 'A';

    public static int Deserialize(char c)
    {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'a' && c <= 'z') return c - 'a' + 10;
        if (c >= 'A' && c <= 'Z') return c - 'A' + 10;
        throw new ArgumentException();
    }

    public static ulong GetBase(char c) => char.ToLower(c) switch
    {
        'b' => 2,
        'o' => 8,
        'd' => 10,
        'x' => 16,
        _ => throw new ArgumentException(),
    };

    public static Result Deserialize(ReadOnlySpan<char> text)
    {
        char sign = '\0';
        bool signSet = false;
        ulong baseFactor = 10;
        bool baseSet = false;

        int offset = 0;

        for (int i = 0; i < text.Length; i++)
        {
            var ctx = new LexerRuleContext(text, i);
            if (ctx.Is(" _")) continue;
            else if (ctx.Is("+-") && !signSet)
            {
                sign = ctx[0];
                signSet = true;
            }
            else if(ctx.Is('0') && CharSets.IsLetter(ctx[1]) && !baseSet)
            {
                baseFactor = GetBase(ctx[1]);
                baseSet = true;
                i += 1;
            }
            else if (CharSets.IsLetterOrDigit(ctx[0]) || ctx.Is('.'))
            {
                offset = i;
                break;
            }
            else throw new UnexpectedTokenException(ctx[0], new(0, i));
        }

        var result = Deserialize(text.Slice(offset), baseFactor);
        bool invert = sign == '-';
        return invert ? -result : result;
    }

    public static Result Deserialize(ReadOnlySpan<char> text, ulong baseFactor)
    {
        bool exponentEnabled = baseFactor < ExponentLimit;
        char exponentSign = '\0';
        ulong number = 0;
        ulong exponent = 0;
        ulong currentBase = 1;
        ulong periodBase = 1;

        for (int i = text.Length - 1; i >= 0; i--)
        {
            var ctx = new LexerRuleContext(text, i);
            char c = ctx[i];
            void Throw() => throw new UnexpectedTokenException(c, new(0, i));

            if (ctx.Is(" _")) continue;
            else if (CharSets.IsLetterOrDigit(ctx[0]))
            {
                if ((exponentEnabled || ctx.Is("+-", 1)) && ctx.Is("eE") && (exponent == 0 || periodBase == 1))
                {
                    currentBase = 1;
                    exponent = number;
                    number = 0;
                }
                else
                {
                    ulong digit = (ulong)Deserialize(ctx[0]);
                    if (digit >= baseFactor) Throw();
                    checked
                    {
                        number += digit * currentBase;
                        currentBase *= baseFactor;
                    }
                }
            }
            else if (ctx.Is('.'))
            {
                if (periodBase > 1) Throw();
                periodBase = currentBase;
            }
            else if (ctx.Is("+-") && ctx.Is("eE", -1))
            {
                exponentSign = ctx[0];
            }
            else
            {
                Throw();
            }
        }
        checked
        {
            ulong efactor = 1;
            for (ulong i = 0; i < exponent; i++) efactor *= baseFactor;

            if (exponentSign == '-' || periodBase > 1)
            {
                double result = number / (double)periodBase;
                if (exponentSign == '-') result /= efactor;
                else result *= efactor;
                long resultInt = (long)result;
                return new Result(resultInt, result, resultInt != result);
            }
            else
            {
                long result = (long)(number * efactor);
                return new Result(result, result, false);
            }
        }
    }
}
