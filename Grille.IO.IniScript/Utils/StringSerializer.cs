using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.Utils;

internal static class StringSerializer
{
    static readonly KeyValuePair<char, string>[] _pairs;

    static readonly HashSet<char> _set;

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
        _set =
        [
            ' ',
            ',',
            ';',
            '(',
            ')',
            ':',
            '=',
        ];
        foreach (var pair in _pairs)
        {
            _set.Add(pair.Key);
        }
    }

    public static bool IsStringifyNecessary(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (_set.Contains(text[i]))
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
