using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Utils;

internal struct Token
{
    public TokenType Type;
    public int Row;
    public int Column;
    public string Value;

    public string GetStringValue()
    {
        if (Type == TokenType.String)
        {
            return StringSerializer.Deserialize(Value);
        }
        else if (Type == TokenType.Section)
        {
            return Value.Substring(1, Value.Length - 2).Trim();
        }
        return Value;
    }

    public override string ToString()
    {
        return $"{Type} {Value}";
    }

    public static bool operator ==(Token left, TokenType type) => left.Type == type;

    public static bool operator !=(Token left, TokenType type) => left.Type != type;

    public static bool operator ==(Token left, (TokenType Type, string Value) right) => left.Type == right.Type && left.Value == right.Value;

    public static bool operator !=(Token left, (TokenType Type, string Value) right) => !(left == right);
}
