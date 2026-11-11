using System.Xml.Linq;

namespace QuickResult.Web.Prerender;

/// <summary>Builds the <c>sitemap.xml</c> that tells crawlers which pages exist.</summary>
public static class Sitemap
{
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static string Build(Uri origin, IEnumerable<RoutePath> routes)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(routes);

        var urlset = new XElement(
            Ns + "urlset",
            routes.Select(route =>
                new XElement(Ns + "url", new XElement(Ns + "loc", route.ToAbsoluteUrl(origin).AbsoluteUri))));

        return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n{urlset}\n";
    }
}
