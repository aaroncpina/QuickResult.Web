using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class RoutePathTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/reference")]
    [InlineData("/getting-started")]
    [InlineData("/docs/api-2")]
    public void Create_Accepts_RootAndKebabCasePaths(string input)
    {
        Assert.Equal(input, RoutePath.Create(input).Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("reference")]
    [InlineData("/reference/")]
    [InlineData("/Reference")]
    [InlineData("/reference?x=1")]
    [InlineData("/reference#top")]
    [InlineData("/users/{id}")]
    [InlineData("//reference")]
    [InlineData("/a b")]
    public void Create_Rejects_PathsThatAreNotStaticRoutes(string? input)
    {
        Assert.Throws<ArgumentException>(() => RoutePath.Create(input));
    }

    [Fact]
    public void Root_IsTheSiteRoot()
    {
        Assert.Equal("/", RoutePath.Root.Value);
    }

    [Fact]
    public void RoutePaths_AreEqual_WhenTheirValuesMatch()
    {
        Assert.Equal(RoutePath.Create("/reference"), RoutePath.Create("/reference"));
        Assert.NotEqual(RoutePath.Create("/reference"), RoutePath.Create("/examples"));
    }

    [Fact]
    public void OutputFile_IsIndexHtml_ForTheRoot()
    {
        Assert.Equal("index.html", RoutePath.Root.OutputFile);
    }

    [Theory]
    [InlineData("/reference", "reference.html")]
    [InlineData("/getting-started", "getting-started.html")]
    [InlineData("/docs/api", "docs/api.html")]
    public void OutputFile_IsNamedAfterThePath_SoTheHostServesItWithoutATrailingSlash(string path, string file)
    {
        Assert.Equal(file, RoutePath.Create(path).OutputFile);
    }

    [Theory]
    [InlineData("/", "https://quickresult.net/")]
    [InlineData("/reference", "https://quickresult.net/reference")]
    public void ToAbsoluteUrl_JoinsThePathToTheOrigin(string path, string expected)
    {
        var url = RoutePath.Create(path).ToAbsoluteUrl(new Uri("https://quickresult.net"));

        Assert.Equal(expected, url.AbsoluteUri);
    }

    [Fact]
    public void ToString_ReturnsThePath()
    {
        Assert.Equal("/reference", RoutePath.Create("/reference").ToString());
    }
}
