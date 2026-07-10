using System.Text;

namespace QuickResult.Web.Services;

/// <summary>
/// Small heuristic C# tokenizer — good enough for display, no Roslyn payload.
/// Identifiers starting with an uppercase letter are treated as type names.
/// </summary>
public sealed class CSharpSyntaxHighlighter : ISyntaxHighlighter
{
    private static readonly HashSet<string> Keywords =
    [
        "abstract", "as", "async", "await", "base", "bool", "break", "byte", "case",
        "catch", "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "from", "get", "goto",
        "if", "implicit", "in", "int", "interface", "internal", "is", "let", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "record", "ref",
        "return", "sbyte", "sealed", "select", "set", "short", "sizeof", "stackalloc",
        "static", "string", "struct", "switch", "this", "throw", "true", "try",
        "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "var",
        "virtual", "void", "volatile", "when", "where", "while", "yield",
    ];

    public IReadOnlyList<CodeToken> Tokenize(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        var tokens = new List<CodeToken>();
        var plain = new StringBuilder();
        var i = 0;

        void FlushPlain()
        {
            if (plain.Length > 0)
            {
                tokens.Add(new CodeToken(TokenKind.Plain, plain.ToString()));
                plain.Clear();
            }
        }

        while (i < code.Length)
        {
            var c = code[i];

            if (c == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                FlushPlain();
                var end = code.IndexOf('\n', i);
                if (end < 0)
                {
                    end = code.Length;
                }

                tokens.Add(new CodeToken(TokenKind.Comment, code[i..end]));
                i = end;
                continue;
            }

            if (c == '/' && i + 1 < code.Length && code[i + 1] == '*')
            {
                FlushPlain();
                var close = code.IndexOf("*/", i + 2, StringComparison.Ordinal);
                var end = close < 0 ? code.Length : close + 2;
                tokens.Add(new CodeToken(TokenKind.Comment, code[i..end]));
                i = end;
                continue;
            }

            if (IsStringStart(code, i, out var prefixLength))
            {
                FlushPlain();
                var end = ScanString(code, i, prefixLength);
                tokens.Add(new CodeToken(TokenKind.String, code[i..end]));
                i = end;
                continue;
            }

            if (char.IsDigit(c))
            {
                FlushPlain();
                var end = i;
                while (end < code.Length &&
                       (char.IsLetterOrDigit(code[end]) || code[end] == '.' || code[end] == '_'))
                {
                    end++;
                }

                tokens.Add(new CodeToken(TokenKind.Number, code[i..end]));
                i = end;
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                FlushPlain();
                var end = i;
                while (end < code.Length && (char.IsLetterOrDigit(code[end]) || code[end] == '_'))
                {
                    end++;
                }

                var word = code[i..end];
                var kind = Keywords.Contains(word) ? TokenKind.Keyword
                    : char.IsUpper(word[0]) ? TokenKind.TypeName
                    : TokenKind.Plain;
                tokens.Add(new CodeToken(kind, word));
                i = end;
                continue;
            }

            plain.Append(c);
            i++;
        }

        FlushPlain();
        return tokens;
    }

    private static bool IsStringStart(string code, int i, out int prefixLength)
    {
        prefixLength = 0;
        var j = i;
        while (j < code.Length && (code[j] == '$' || code[j] == '@') && j - i < 2)
        {
            j++;
        }

        if (j < code.Length && code[j] == '"')
        {
            prefixLength = j - i;
            return true;
        }

        prefixLength = 0;
        return code.Length > i && code[i] == '"';
    }

    private static int ScanString(string code, int start, int prefixLength)
    {
        var verbatim = code[start..(start + prefixLength)].Contains('@');
        var i = start + prefixLength + 1; // past the opening quote

        while (i < code.Length)
        {
            if (code[i] == '\\' && !verbatim && i + 1 < code.Length)
            {
                i += 2;
                continue;
            }

            if (code[i] == '"')
            {
                if (verbatim && i + 1 < code.Length && code[i + 1] == '"')
                {
                    i += 2; // escaped "" inside verbatim string
                    continue;
                }

                return i + 1;
            }

            i++;
        }

        return code.Length;
    }
}
