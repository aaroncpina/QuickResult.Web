using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuickResult.Web.Pages;
using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

/// <summary>
/// What search engines and link previews read from the prerendered pages: structured data,
/// preview images, and the C# unions article.
/// </summary>
public partial class SearchVisibilityTests
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

    private static Task<PrerenderedPage> RenderAsync(string path) =>
        Renderer.RenderAsync(RoutePath.Create(path));

    private static List<JsonElement> StructuredData(PrerenderedPage page) =>
        JsonLdScript().Matches(page.Head)
            .Select(match => JsonDocument.Parse(match.Groups[1].Value).RootElement)
            .ToList();

    [GeneratedRegex("<script type=\"application/ld\\+json\">(.*?)</script>", RegexOptions.Singleline)]
    private static partial Regex JsonLdScript();

    // -- preview images -------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_LargePreviewImage_IsDeclaredWithItsSize(string path)
    {
        var page = await RenderAsync(path);

        Assert.Contains("<meta property=\"og:image\" content=\"https://quickresult.net/og-image.png\"", page.Head);
        Assert.Contains("<meta property=\"og:image:width\" content=\"1200\"", page.Head);
        Assert.Contains("<meta property=\"og:image:height\" content=\"630\"", page.Head);
        Assert.Contains("<meta property=\"og:image:alt\" content=\"", page.Head);
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_SummaryCard_UsesTheSquareIcon(string path)
    {
        var page = await RenderAsync(path);

        Assert.Contains("<meta name=\"twitter:image\" content=\"https://quickresult.net/icon-192.png\"", page.Head);
    }

    // -- structured data ------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Head_StructuredData_IsValidJsonForSchemaOrg(string path)
    {
        var page = await RenderAsync(path);

        Assert.All(
            StructuredData(page),
            data => Assert.Equal("https://schema.org", data.GetProperty("@context").GetString()));
    }

    [Fact]
    public async Task Home_StructuredData_DescribesTheLibraryAsSourceCode()
    {
        var data = Assert.Single(StructuredData(await RenderAsync("/")));

        var library = data.GetProperty("@graph").EnumerateArray()
            .Single(node => node.GetProperty("@type").GetString() == "SoftwareSourceCode");

        Assert.Equal("QuickResult", library.GetProperty("name").GetString());
        Assert.Equal("https://github.com/aaroncpina/QuickResult", library.GetProperty("codeRepository").GetString());
        Assert.Equal("C#", library.GetProperty("programmingLanguage").GetString());
        Assert.Equal("https://opensource.org/licenses/MIT", library.GetProperty("license").GetString());
    }

    [Fact]
    public async Task Home_StructuredData_NamesTheWebSite()
    {
        var data = Assert.Single(StructuredData(await RenderAsync("/")));

        var site = data.GetProperty("@graph").EnumerateArray()
            .Single(node => node.GetProperty("@type").GetString() == "WebSite");

        Assert.Equal("QuickResult", site.GetProperty("name").GetString());
        Assert.Equal("https://quickresult.net/", site.GetProperty("url").GetString());
    }

    [Fact]
    public async Task UnionsArticle_StructuredData_DescribesATechArticle()
    {
        var data = Assert.Single(StructuredData(await RenderAsync("/csharp-unions")));

        Assert.Equal("TechArticle", data.GetProperty("@type").GetString());
        Assert.Equal("https://quickresult.net/csharp-unions", data.GetProperty("mainEntityOfPage").GetString());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("headline").GetString()));
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", data.GetProperty("datePublished").GetString());
    }

    // -- the C# unions article ------------------------------------------------

    [Fact]
    public async Task UnionsArticle_Head_TargetsCSharpUnionSearches()
    {
        var page = await RenderAsync("/csharp-unions");
        var head = WebUtility.HtmlDecode(page.Head);

        Assert.Matches("<title>[^<]*C# union types[^<]*</title>", head);
        Assert.Contains("Result<T>", head);
    }

    [Fact]
    public async Task UnionsArticle_Body_ExplainsUnionsAndHowResultImplementsThem()
    {
        var body = WebUtility.HtmlDecode((await RenderAsync("/csharp-unions")).Body);

        Assert.Contains("<h1", body);
        Assert.Contains("C# 15", body);
        Assert.Contains(".NET 11", body);
        Assert.Contains("IUnionMembers", body);
        Assert.Contains("IError", body);
    }

    [Fact]
    public async Task UnionsArticle_Body_EmbedsTheLiveUnionExample()
    {
        var page = await RenderAsync("/csharp-unions");

        Assert.Contains("id=\"union-switch\"", page.Body);
        Assert.Contains("class=\"run-button\"", page.Body);
    }

    [Fact]
    public async Task UnionsArticle_Body_LinksToTheOfficialDocumentation()
    {
        var page = await RenderAsync("/csharp-unions");

        Assert.Contains("href=\"https://learn.microsoft.com/en-us/dotnet/csharp/", page.Body);
    }

    [Theory]
    [MemberData(nameof(AllRoutes))]
    public async Task Navigation_LinksToTheUnionsArticle(string path)
    {
        var page = await RenderAsync(path);

        Assert.Contains("<a href=\"csharp-unions\"", page.Body);
    }

    [Fact]
    public async Task Home_Body_PointsVisitorsToTheUnionsArticle()
    {
        var page = await RenderAsync("/");

        Assert.Matches("<main[^>]*>.*href=\"csharp-unions\".*</main>", Regex.Replace(page.Body, @"\s+", " "));
    }
}
