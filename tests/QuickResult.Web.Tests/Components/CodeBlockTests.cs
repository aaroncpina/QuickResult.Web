using Bunit;
using Microsoft.Extensions.DependencyInjection;
using QuickResult.Web.Components;
using QuickResult.Web.Services;

namespace QuickResult.Web.Tests.Components;

public class CodeBlockTests : BunitContext
{
    public CodeBlockTests()
    {
        Services.AddSingleton<ISyntaxHighlighter, CSharpSyntaxHighlighter>();
    }

    [Fact]
    public void RendersAllSourceText()
    {
        const string code = "var x = Result.Success(42);";

        var cut = Render<CodeBlock>(ps => ps.Add(p => p.Code, code));

        Assert.Equal(code, cut.Find("code").TextContent);
    }

    [Fact]
    public void WrapsKeywordsInClassifiedSpans()
    {
        var cut = Render<CodeBlock>(ps => ps.Add(p => p.Code, "var x = 42;"));

        var keywordSpans = cut.FindAll("span.tok-keyword");

        Assert.Contains(keywordSpans, s => s.TextContent == "var");
    }

    [Fact]
    public void WrapsNumbersInClassifiedSpans()
    {
        var cut = Render<CodeBlock>(ps => ps.Add(p => p.Code, "var x = 42;"));

        var numberSpans = cut.FindAll("span.tok-number");

        Assert.Contains(numberSpans, s => s.TextContent == "42");
    }
}
