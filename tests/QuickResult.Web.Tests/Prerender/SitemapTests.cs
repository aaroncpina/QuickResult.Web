using System.Xml.Linq;
using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class SitemapTests
{
    private static readonly Uri Origin = new("https://quickresult.net");
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    [Fact]
    public void Build_ListsTheAbsoluteUrlOfEveryRoute_InOrder()
    {
        var xml = Sitemap.Build(Origin, [RoutePath.Root, RoutePath.Create("/reference")]);

        var locations = XDocument.Parse(xml).Descendants(Ns + "loc").Select(l => l.Value);
        Assert.Equal(["https://quickresult.net/", "https://quickresult.net/reference"], locations);
    }

    [Fact]
    public void Build_ProducesAUrlsetInTheSitemapNamespace()
    {
        var xml = Sitemap.Build(Origin, [RoutePath.Root]);

        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>", xml);
        Assert.Equal(Ns + "urlset", XDocument.Parse(xml).Root!.Name);
    }
}
