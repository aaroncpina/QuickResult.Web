using QuickResult.Web.Pages;
using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class SitePrerendererTests
{
    private const string Shell =
        """
        <html>
        <head>
            <!--prerender:head--><title>Default title</title><!--/prerender:head-->
        </head>
        <body>
            <div id="app"><!--prerender:app-->Loading…<!--/prerender:app--></div>
            <script src="_framework/blazor.webassembly.js"></script>
        </body>
        </html>
        """;

    private static Task<IReadOnlyDictionary<string, string>> BuildAsync() =>
        new SitePrerenderer().BuildAsync(Shell);

    [Fact]
    public async Task Build_ProducesOneHtmlFilePerPage_PlusNotFoundAndSitemap()
    {
        var files = await BuildAsync();

        // Derived from the pages the site has, so adding a page does not change this test.
        var expected = RouteDiscovery
            .FindPages(typeof(App).Assembly, typeof(NotFound))
            .Select(route => route.OutputFile)
            .Append("404.html")
            .Append("sitemap.xml")
            .Order(StringComparer.Ordinal);

        Assert.Equal(expected, files.Keys.Order(StringComparer.Ordinal));
        Assert.Contains("index.html", files.Keys);
        Assert.Contains("reference.html", files.Keys);
    }

    [Fact]
    public async Task Build_FillsTheShell_WithEachPagesContent()
    {
        var files = await BuildAsync();

        Assert.Contains("<title>API Reference &#x2014; QuickResult</title>", files["reference.html"]);
        Assert.Contains("C# union support", files["reference.html"]);
        Assert.Contains("<script src=\"_framework/blazor.webassembly.js\"></script>", files["reference.html"]);
        Assert.DoesNotContain("Loading…", files["reference.html"]);
    }

    [Fact]
    public async Task Build_NotFoundFile_HoldsTheNotFoundPage()
    {
        var files = await BuildAsync();

        Assert.Contains("Failure(Page not found)", files["404.html"]);
        Assert.Contains("noindex", files["404.html"]);
    }

    [Fact]
    public async Task Build_Sitemap_ListsEveryPageButNotTheNotFoundPage()
    {
        var files = await BuildAsync();

        Assert.Contains("<loc>https://quickresult.net/</loc>", files["sitemap.xml"]);
        Assert.Contains("<loc>https://quickresult.net/reference</loc>", files["sitemap.xml"]);
        Assert.DoesNotContain("not-found", files["sitemap.xml"]);
        Assert.DoesNotContain("404", files["sitemap.xml"]);
    }
}
