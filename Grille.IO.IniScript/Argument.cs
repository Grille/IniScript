using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public struct Argument
{
    public static CultureInfo Culture = CultureInfo.InvariantCulture;

    public object Value;

    public Argument(object value)
    {
        Value = value;
    }

    public string Text
    {
        get
        {
            var text = Value.ToString();
            return text == null ? string.Empty : text;
        }
        set
        {
            Value = value;
        }
    }

    public bool IsBoolean => true;

    public bool Boolean
    {
        get
        {
            if (Value is bool b) return b;
            var text = Text.Trim().ToLower();
            if (text == "1" || text == "true") return true;
            return false;
        }
        set
        {
            Value = value;
        }
    }


    public bool IsSingle => float.TryParse(Text, NumberStyles.Any, Culture, out _);

    public float Single
    {
        set => Text = value.ToString(Culture);
        get => float.Parse(Text, Culture);
    }


    public bool IsDouble => double.TryParse(Text, NumberStyles.Any, Culture, out _);

    public double Double
    {
        set => Text = value.ToString(Culture);
        get => double.Parse(Text, Culture);
    }


    public bool IsDecimal => decimal.TryParse(Text, NumberStyles.Any, Culture, out _);

    public decimal Decimal
    {
        set => Text = value.ToString(Culture);
        get => decimal.Parse(Text, Culture);
    }


    public bool IsInt64 => long.TryParse(Text, NumberStyles.Integer, Culture, out _);

    public long Int64
    {
        set => Text = value.ToString(Culture);
        get => long.Parse(Text, Culture);
    }


    public bool IsHex64 => TryParseHex64(Text, out _);

    public long Hex64
    {
        set => Text = value.ToString(Culture);
        get => ParseHex64(Text);
    }

    public bool IsInt32 => int.TryParse(Text, NumberStyles.Integer, Culture, out _);

    public int Int32
    {
        set => Text = value.ToString(Culture);
        get => int.Parse(Text, Culture);
    }


    public bool IsHex32 => TryParseHex64(Text, out _);

    public int Hex32
    {
        set => Text = value.ToString(Culture);
        get => (int)ParseHex64(Text);
    }

    static bool TryParseHex64(string value, out long number)
    {
        if (value.StartsWith("0x", true, Culture))
        {
            value = value.Substring(2);
        }
        return long.TryParse(value, NumberStyles.HexNumber, Culture, out number);
    }

    static long ParseHex64(string value)
    {
        TryParseHex64(value, out long number);
        return number;
    }

    public T As<T>()
    {
        return (T)Value;
    }


    public static implicit operator string(Argument value) => value.Text;
    public static implicit operator Argument(string value) => new Argument() { Text = value };
}
