using QuickResult.Web.Services;

namespace QuickResult.Web.Tests.Services;

public class CSharpSyntaxHighlighterTests
{
    private readonly CSharpSyntaxHighlighter _highlighter = new();

    [Theory]
    [InlineData("var x = 42;")]
    [InlineData("// a comment\nvar s = \"text\";")]
    [InlineData("var q = from a in Result.Success(4) select a; /* block */")]
    [InlineData("Console.WriteLine($\"got {value}\");")]
    [InlineData("")]
    public void Tokenize_RoundTrips_ExactSourceText(string code)
    {
        var tokens = _highlighter.Tokenize(code);

        Assert.Equal(code, string.Concat(tokens.Select(t => t.Text)));
    }

    [Fact]
    public void Tokenize_MarksKeywords()
    {
        var tokens = _highlighter.Tokenize("var x = await GetAsync();");

        Assert.Contains(tokens, t => t.Kind == TokenKind.Keyword && t.Text == "var");
        Assert.Contains(tokens, t => t.Kind == TokenKind.Keyword && t.Text == "await");
    }

    [Fact]
    public void Tokenize_MarksNumbers()
    {
        var tokens = _highlighter.Tokenize("var x = 42;");

        Assert.Contains(tokens, t => t.Kind == TokenKind.Number && t.Text == "42");
    }

    [Fact]
    public void Tokenize_MarksLineComments()
    {
        var tokens = _highlighter.Tokenize("// hello\nvar x = 1;");

        Assert.Contains(tokens, t => t.Kind == TokenKind.Comment && t.Text == "// hello");
    }

    [Fact]
    public void Tokenize_MarksStringLiterals()
    {
        var tokens = _highlighter.Tokenize("var s = \"hello\";");

        Assert.Contains(tokens, t => t.Kind == TokenKind.String && t.Text == "\"hello\"");
    }

    [Fact]
    public void Tokenize_MarksTypeNames()
    {
        var tokens = _highlighter.Tokenize("var r = Result.Success(1);");

        Assert.Contains(tokens, t => t.Kind == TokenKind.TypeName && t.Text == "Result");
    }

    [Fact]
    public void Tokenize_DoesNotMarkKeywordInsideIdentifier()
    {
        var tokens = _highlighter.Tokenize("var variable = 1;");

        Assert.Contains(tokens, t => t.Kind != TokenKind.Keyword && t.Text == "variable");
        Assert.DoesNotContain(tokens, t => t.Kind == TokenKind.Keyword && t.Text == "variable");
    }
}
