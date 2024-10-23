using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal static class StringSerializer
{
    static readonly KeyValuePair<char, string>[] _pairs;

    static StringSerializer()
    {
        _pairs =
        [
            new( '\\', "\\\\" ),
            new( '\"', "\\\"" ),
            new( '\n', "\\n" ),
            new( '\r', "\\r" ),
            new( '\t', "\\t" ),
        ];
    }

    public static bool IsStringifyNecessary(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (!CharSets.IsWordOrNumber(text[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static string Serialize(string text)
    {
        var sb = new StringBuilder(text.Length * 2);
        sb.Append('\0');
        sb.Append(text);
        foreach (var pair in _pairs)
        {
            sb.Replace(pair.Key.ToString(), pair.Value);
        }
        sb[0] = '\"';
        sb.Append('\"');
        return sb.ToString();
    }

    public static string Deserialize(string text)
    {
        var sb = new StringBuilder(text, 1, text.Length - 2, text.Length);
        foreach (var pair in _pairs)
        {
            sb.Replace(pair.Value, pair.Key.ToString());
        }
        return sb.ToString();
    }
}
