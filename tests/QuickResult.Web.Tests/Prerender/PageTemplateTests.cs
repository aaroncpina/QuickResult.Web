using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class PageTemplateTests
{
    private const string Shell =
        """
        <html>
        <head>
            <meta charset="utf-8" />
            <!--prerender:head-->
            <title>Default title</title>
            <!--/prerender:head-->
            <link rel="stylesheet" href="css/app.css" />
        </head>
        <body>
            <div id="app"><!--prerender:app--><div class="app-loading">Loading…</div><!--/prerender:app--></div>
            <script src="_framework/blazor.webassembly.js"></script>
        </body>
        </html>
        """;

    private static readonly PrerenderedPage Page = new("<title>Reference</title>", "<h1>API Reference</h1>");

    [Fact]
    public void Fill_ReplacesTheHeadRegion_WithThePagesHeadTags()
    {
        var html = PageTemplate.Parse(Shell).Fill(Page);

        Assert.Contains("<title>Reference</title>", html);
        Assert.DoesNotContain("Default title", html);
    }

    [Fact]
    public void Fill_ReplacesTheAppRegion_WithThePagesBody()
    {
        var html = PageTemplate.Parse(Shell).Fill(Page);

        Assert.Contains("<h1>API Reference</h1>", html);
        Assert.DoesNotContain("app-loading", html);
    }

    [Fact]
    public void Fill_KeepsTheRegionMarkers_SoTheRunningAppCanFindThePrerenderedHead()
    {
        var html = PageTemplate.Parse(Shell).Fill(Page);

        Assert.Contains("<!--prerender:head--><title>Reference</title><!--/prerender:head-->", html);
        Assert.Contains("<div id=\"app\"><!--prerender:app--><h1>API Reference</h1><!--/prerender:app--></div>", html);
    }

    [Fact]
    public void Fill_KeepsEverythingOutsideTheRegions()
    {
        var html = PageTemplate.Parse(Shell).Fill(Page);

        Assert.Contains("<meta charset=\"utf-8\" />", html);
        Assert.Contains("<link rel=\"stylesheet\" href=\"css/app.css\" />", html);
        Assert.Contains("<script src=\"_framework/blazor.webassembly.js\"></script>", html);
    }

    [Fact]
    public void Fill_InsertsContentVerbatim_WhenItContainsDollarSigns()
    {
        var html = PageTemplate.Parse(Shell).Fill(new PrerenderedPage("<title>$1</title>", "<p>$0 costs $&</p>"));

        Assert.Contains("<title>$1</title>", html);
        Assert.Contains("<p>$0 costs $&</p>", html);
    }

    [Fact]
    public void Fill_CanBeUsedForSeveralPages()
    {
        var template = PageTemplate.Parse(Shell);

        var first = template.Fill(Page);
        var second = template.Fill(new PrerenderedPage("<title>Examples</title>", "<h1>Examples</h1>"));

        Assert.Contains("<h1>API Reference</h1>", first);
        Assert.Contains("<h1>Examples</h1>", second);
        Assert.DoesNotContain("API Reference", second);
    }

    [Theory]
    [InlineData("<!--prerender:head-->")]
    [InlineData("<!--/prerender:head-->")]
    [InlineData("<!--prerender:app-->")]
    [InlineData("<!--/prerender:app-->")]
    public void Parse_Fails_WhenARegionMarkerIsMissing(string marker)
    {
        var broken = Shell.Replace(marker, string.Empty);

        var error = Assert.Throws<FormatException>(() => PageTemplate.Parse(broken));
        Assert.Contains(marker, error.Message);
    }
}
