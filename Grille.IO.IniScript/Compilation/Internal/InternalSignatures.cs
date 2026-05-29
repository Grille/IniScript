using Grille.IO.IniScript.Tokenization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grille.IO.IniScript.Compilation.Internal;

internal static class InternalSignatures
{
    public static Signature SectionSignature { get; }
    public static Signature SectionSignatureParent { get; }
    public static Signature SectionSignatureArgs { get; }
    public static Signature SectionSignatureArgsParent { get; }
    public static Signature Function { get; }

    [Flags]
    public enum SectionFlags
    {
        None,
        Parameters = 1,
        Parent = 2,
    }

    public readonly record struct SectionInfo(Signature Signature, SectionFlags Flags)
    {
        public bool HasParameters => Flags.HasFlag(SectionFlags.Parameters);
        public bool HasParent => Flags.HasFlag(SectionFlags.Parent);
    }

    private static readonly SectionInfo[] _sectionSignatures;
    public static ReadOnlySpan<SectionInfo> SectionSignatures => _sectionSignatures;

    public static Signature Assignment { get; }

    static InternalSignatures()
    {
        _sectionSignatures =
        [
            new(SectionSignature = Signature.New().OpenBracket('[').Parameter().CloseBracket(), SectionFlags.None),
            new(SectionSignatureParent = SectionSignature.EditCopy().Symbol(':').Parameter(), SectionFlags.Parent),
            new(SectionSignatureArgs = SectionSignature.EditCopy().OpenBracket('(').ParameterList().CloseBracket(), SectionFlags.Parameters),
            new(SectionSignatureArgsParent = SectionSignatureArgs.EditCopy().Symbol(':').Parameter(), SectionFlags.Parameters | SectionFlags.Parent),
            new(Function = Signature.New().Keyword("def").Parameter().OpenBracket('(').ParameterList().CloseBracket().Symbol(':'), SectionFlags.Parameters)
        ];
        Assignment = Signature.New().Parameter().Symbol('=');
    }

    public static bool IsAssignment(ReadOnlySpan<Token> span)
    {
        if (span.Length < 2) return false;
        return Assignment.Matches(span.Slice(0, 2));
    }

}
