namespace QuickResult.Web.Services;

public enum TokenKind
{
    Plain,
    Keyword,
    TypeName,
    String,
    Number,
    Comment,
}

public sealed record CodeToken(TokenKind Kind, string Text);

/// <summary>
/// Splits source code into classified tokens for display.
/// Invariant: concatenating the token texts reproduces the input exactly.
/// </summary>
public interface ISyntaxHighlighter
{
    IReadOnlyList<CodeToken> Tokenize(string code);
}
