using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal static class CharSets
{
    public static bool IsWord(char c)
    {
        return IsDigit(c) || IsLetter(c);
    }

    public static bool IsLetter(char c)
    {
        return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
    }

    public static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9' || c == '.';
    }

    public static bool IsWhitespace(char c)
    {
        return c == ' ' || c == '\t' || c == ',';
    }

    public static bool IsSymbol(char c)
    {
        return c == '*' || c == '/' || c == '+' || c == '-' || c == ':' || c == '=' || c == '(' || c == ')';
    }
}
