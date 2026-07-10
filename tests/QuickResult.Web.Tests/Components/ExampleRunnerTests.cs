using Bunit;
using Microsoft.Extensions.DependencyInjection;
using QuickResult.Web.Components;
using QuickResult.Web.Examples;
using QuickResult.Web.Services;

namespace QuickResult.Web.Tests.Components;

public class ExampleRunnerTests : BunitContext
{
    public ExampleRunnerTests()
    {
        Services.AddSingleton<ISyntaxHighlighter, CSharpSyntaxHighlighter>();
    }

    private sealed class EchoExample : IExample
    {
        public string Id => "echo";
        public string Title => "Echo Example";
        public string Category => "Testing";
        public string Description => "Echoes its input.";
        public string SourceCode => "var x = 1;";

        public IReadOnlyList<ExampleInput> Inputs =>
        [
            new("text", "Text to echo", "hello"),
        ];

        public Task<IReadOnlyList<string>> RunAsync(ExampleArguments args) =>
            Task.FromResult<IReadOnlyList<string>>([$"echo: {args.Get("text")}"]);
    }

    private IRenderedComponent<ExampleRunner> RenderRunner() =>
        Render<ExampleRunner>(ps => ps.Add(p => p.Example, new EchoExample()));

    [Fact]
    public void RendersTitleAndDescription()
    {
        var cut = RenderRunner();

        Assert.Contains("Echo Example", cut.Markup);
        Assert.Contains("Echoes its input.", cut.Markup);
    }

    [Fact]
    public void RendersSourceCode()
    {
        var cut = RenderRunner();

        Assert.Equal("var x = 1;", cut.Find("code").TextContent);
    }

    [Fact]
    public void RendersOneInputPerDeclaredInput_WithDefaultValue()
    {
        var cut = RenderRunner();

        var input = cut.Find("input[data-input-name=text]");

        Assert.Equal("hello", input.GetAttribute("value"));
    }

    [Fact]
    public void ClickingRun_ShowsOutputLines()
    {
        var cut = RenderRunner();

        cut.Find("button.run-button").Click();

        var lines = cut.FindAll(".output-line");
        Assert.Single(lines);
        Assert.Equal("echo: hello", lines[0].TextContent);
    }

    [Fact]
    public void ClickingRun_UsesEditedInputValues()
    {
        var cut = RenderRunner();

        cut.Find("input[data-input-name=text]").Change("world");
        cut.Find("button.run-button").Click();

        var lines = cut.FindAll(".output-line");
        Assert.Equal("echo: world", lines[0].TextContent);
    }

    [Fact]
    public void OutputIsHidden_BeforeFirstRun()
    {
        var cut = RenderRunner();

        Assert.Empty(cut.FindAll(".output-line"));
    }
}
