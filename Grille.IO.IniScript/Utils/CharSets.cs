using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal static class CharSets
{
    public const string WordExtensions = "._";
    public const string Whitespaces = " \t";
    public const string Symbols = "*/+-:=!%&?,";
    public const string OpeningBrackets = "({[<";
    public const string ClosingBrackets = ")}]>";
    public const string Brackets = OpeningBrackets + ClosingBrackets;

    public static bool IsWord(char c) => IsLetterOrDigit(c) || WordExtensions.Contains(c);

    public static bool IsLetterOrDigit(char c) => IsDigit(c) || IsLetter(c);

    public static bool IsLetter(char c) => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';

    public static bool IsDigit(char c) => c >= '0' && c <= '9';
}
