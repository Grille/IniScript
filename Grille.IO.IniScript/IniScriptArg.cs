using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO;

public struct IniScriptArg
{
    public static CultureInfo Culture = CultureInfo.InvariantCulture;

    public string Value;

    public bool IsSingle => float.TryParse(Value, NumberStyles.Any, Culture, out _);

    public float Single
    {
        set => Value = value.ToString(Culture);
        get => float.Parse(Value, Culture);
    }


    public bool IsDouble => double.TryParse(Value, NumberStyles.Any, Culture, out _);

    public double Double
    {
        set => Value = value.ToString(Culture);
        get => double.Parse(Value, Culture);
    }


    public bool IsDecimal => decimal.TryParse(Value, NumberStyles.Any, Culture, out _);

    public decimal Decimal
    {
        set => Value = value.ToString(Culture);
        get => decimal.Parse(Value, Culture);
    }


    public bool IsInt64 => long.TryParse(Value, NumberStyles.Integer, Culture, out _);

    public long Int64
    {
        set => Value = value.ToString(Culture);
        get => long.Parse(Value, Culture);
    }


    public bool IsHex64 => TryParseHex64(Value, out _);

    public long Hex64
    {
        set => Value = value.ToString(Culture);
        get => ParseHex64(Value);
    }

    public bool IsInt32 => int.TryParse(Value, NumberStyles.Integer, Culture, out _);

    public int Int32
    {
        set => Value = value.ToString(Culture);
        get => int.Parse(Value, Culture);
    }


    public bool IsHex32 => TryParseHex64(Value, out _);

    public int Hex32
    {
        set => Value = value.ToString(Culture);
        get => (int)ParseHex64(Value);
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


    public static implicit operator string(IniScriptArg value) => value.Value;
    public static implicit operator IniScriptArg(string value) => new IniScriptArg() { Value = value };
}
