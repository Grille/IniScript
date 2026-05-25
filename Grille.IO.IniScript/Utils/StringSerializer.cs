using System;
using System.Text;

namespace Grille.IO.IniScript.Utils;

internal static class StringSerializer
{
    public const string Keys = "\\\'\"0nrt";
    public const string Values = "\\\'\"\0\n\r\t";

    public static bool IsStringifyNecessary(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (!CharSets.IsWord(text[i]))
            {
                return true;
            }
        }
        return false; 
    }

    public static void SerializeEscape(ReadOnlySpan<char> src, StringBuilder dst)
    {
        for (int i = 0; i < src.Length; i++)
        {
            char c = src[i];
            int index = Values.IndexOf(c);
            if (index != -1)
            {
                dst.Append('\\');
                dst.Append(Keys[index]);
            }
            else
            {
                dst.Append(c);
            }
        }
    }

    public static void DeserializeEscape(ReadOnlySpan<char> src, StringBuilder dst)
    {
        for (int i = 0; i < src.Length; i++)
        {
            char c = src[i];
            if (c == '\\')
            {
                i += 1;
                int index = Keys.IndexOf((i >= 0 && i < src.Length) ? src[i] : '\0');
                dst.Append(Values[index]);
            }
            else
            {
                dst.Append(c);
            }
        }
    }

    public static string Serialize(ReadOnlySpan<char> text)
    {
        var sb = new StringBuilder(text.Length * 2);
        sb.Append('\"');
        SerializeEscape(text, sb);
        sb.Append('\"');
        return sb.ToString();
    }

    public static string Deserialize(ReadOnlySpan<char> text)
    {
        var sb = new StringBuilder(text.Length);
        DeserializeEscape(text.Slice(1, text.Length -2), sb);
        return sb.ToString();
    }
}
