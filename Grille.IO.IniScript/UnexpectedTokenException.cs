using Grille.IO.IniScript.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript;

public class UnexpectedTokenException : Exception
{
    internal Token Token { get; }

    public string TokenText => Token.Substring();

    public string TokenType => Token.Type.ToString();

    public int Line => Token.Location.Row;

    internal UnexpectedTokenException(char c, TokenLocation location) : this(new Token(c.ToString(),0,1,Utils.TokenType.None, location)) { }

    internal UnexpectedTokenException(Token token) : base(ToMessage(token))
    {
        Token = token;
    }

    private static string ToMessage(Token token)
    {
        var sb = new StringBuilder();
        sb.Append("Unexpected Token: '");
        sb.Append(token.Substring());
        sb.Append("'");
        if (token.Location != TokenLocation.Empty)
        {
            sb.Append('@');
            sb.Append(token.Location);
        }
        if (token.Type != Utils.TokenType.None)
        {
            sb.Append(' ');
            sb.Append(token.Type);
        }
        return sb.ToString();
    }
}
