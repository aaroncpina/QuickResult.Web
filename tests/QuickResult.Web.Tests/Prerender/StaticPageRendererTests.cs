using QuickResult.Web.Pages;
using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class StaticPageRendererTests
{
    private static readonly StaticPageRenderer Renderer = new();

    public static TheoryData<string> AllRoutes()
    {
        var data = new TheoryData<string>();
        foreach (var route in RouteDiscovery.FindPages(typeof(App).Assembly, typeof(NotFound)))
        {
            data.Add(route.Value);
        }

        return data;
    }

    // The renderer writes non-ASCII text as character references (— becomes &#x2014;), which
    // browsers and crawlers read as the same text. Decode the head so tests can state it plainly.
    private static async Task<PrerenderedPage> RenderAsync(string path)
    {
        var page = await Renderer.RenderAsync(RoutePath.Create(path));
        return page with { Head = System.Net.WebUtility.HtmlDecode(page.Head) };
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Body_ContainsThePageInsideTheSiteLayout(string path)
    {
        var page = await RenderAsync(path);

        Assert.Contains("<h1", page.Body);
        Assert.Contains("class=\"site-header\"", page.Body);
        Assert.Contains("class=\"site-footer\"", page.Body);
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_HasExactlyOneTitleDescriptionAndCanonical(string path)
    {
        var page = await RenderAsync(path);

        Assert.Single(System.Text.RegularExpressions.Regex.Matches(page.Head, "<title>[^<]+</title>"));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(page.Head, "<meta name=\"description\" content=\"[^\"]{50,}\""));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(page.Head, "<link rel=\"canonical\""));
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_CanonicalUrl_IsThePagesPublicAddress(string path)
    {
        var page = await RenderAsync(path);

        var expected = RoutePath.Create(path).ToAbsoluteUrl(SiteInfo.Origin).AbsoluteUri;
        Assert.Contains($"<link rel=\"canonical\" href=\"{expected}\"", page.Head);
        Assert.Contains($"<meta property=\"og:url\" content=\"{expected}\"", page.Head);
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_HasSocialPreviewTags(string path)
    {
        var page = await RenderAsync(path);

        Assert.Contains("<meta property=\"og:title\" content=\"", page.Head);
        Assert.Contains("<meta property=\"og:description\" content=\"", page.Head);
        Assert.Contains("<meta property=\"og:type\" content=\"website\"", page.Head);
        Assert.Contains("<meta property=\"og:image\" content=\"https://quickresult.net/", page.Head);
        Assert.Contains("<meta name=\"twitter:card\" content=\"summary\"", page.Head);
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_DoesNotAskSearchEnginesToSkipThePage(string path)
    {
        var page = await RenderAsync(path);

        Assert.DoesNotContain("noindex", page.Head);
    }

    [Fact]
    public async Task Titles_AreDifferentOnEveryPage()
    {
        var titles = new List<string>();
        foreach (var path in AllRoutes())
        {
            var page = await RenderAsync(path);
            titles.Add(System.Text.RegularExpressions.Regex.Match(page.Head, "<title>([^<]+)</title>").Groups[1].Value);
        }

        Assert.Equal(titles.Count, titles.Distinct().Count());
    }

    [Fact]
    public async Task Descriptions_AreDifferentOnEveryPage()
    {
        var descriptions = new List<string>();
        foreach (var path in AllRoutes())
        {
            var page = await RenderAsync(path);
            descriptions.Add(System.Text.RegularExpressions.Regex
                .Match(page.Head, "<meta name=\"description\" content=\"([^\"]+)\"").Groups[1].Value);
        }

        Assert.Equal(descriptions.Count, descriptions.Distinct().Count());
    }

    [Theory]
    [InlineData("/contact", "Contact & Support — QuickResult")]
    [InlineData("/philosophy", "Philosophy & History — QuickResult")]
    public async Task Head_Title_ShowsAnAmpersandAsAnAmpersand(string path, string title)
    {
        var page = await RenderAsync(path);

        Assert.Contains($"<title>{title}</title>", page.Head);
        Assert.Contains($"<meta property=\"og:title\" content=\"{title}\"", page.Head);
    }

    [Fact]
    public async Task Reference_Body_ContainsItsDocumentation()
    {
        var page = await RenderAsync("/reference");

        Assert.Contains("API Reference", page.Body);
        Assert.Contains("C# union support", page.Body);
        Assert.Contains("<title>API Reference — QuickResult</title>", page.Head);
    }

    [Fact]
    public async Task Examples_Body_ContainsEveryExample()
    {
        var page = await RenderAsync("/examples");

        Assert.Contains("id=\"union-switch\"", page.Body);
        Assert.Contains("id=\"linq-mixed-async\"", page.Body);
    }

    [Fact]
    public async Task Home_Body_ContainsTheHeroAndTheInstallCommand()
    {
        var page = await RenderAsync("/");

        Assert.Contains("dotnet add package QuickResult", page.Body);
    }

    [Fact]
    public async Task Body_MarksTheCurrentPageInTheNavigation()
    {
        var page = await RenderAsync("/reference");

        Assert.Contains("<a href=\"reference\" class=\"active\"", page.Body);
        Assert.DoesNotContain("<a href=\"examples\" class=\"active\"", page.Body);
    }

    [Fact]
    public async Task NotFound_RendersTheNotFoundPage_AndAsksSearchEnginesToSkipIt()
    {
        var page = await Renderer.RenderNotFoundAsync();

        Assert.Contains("Failure(Page not found)", page.Body);
        Assert.Contains("class=\"site-header\"", page.Body);
        Assert.Contains("<meta name=\"robots\" content=\"noindex\"", page.Head);
        Assert.DoesNotContain("rel=\"canonical\"", page.Head);
    }
}
