using QuickResult.Web.Pages;

namespace QuickResult.Web.Prerender;

/// <summary>
/// Turns the published page shell into the full set of static files for the site: one HTML
/// file per page, a <c>404.html</c>, and a <c>sitemap.xml</c>.
/// </summary>
public sealed class SitePrerenderer
{
    private const string NotFoundFile = "404.html";
    private const string SitemapFile = "sitemap.xml";

    private readonly StaticPageRenderer _renderer = new();

    /// <summary>Returns each output file's content, keyed by its path relative to the site root.</summary>
    public async Task<IReadOnlyDictionary<string, string>> BuildAsync(string templateHtml)
    {
        var template = PageTemplate.Parse(templateHtml);
        var routes = RouteDiscovery.FindPages(typeof(App).Assembly, typeof(NotFound));
        var files = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var route in routes)
        {
            files[route.OutputFile] = template.Fill(await _renderer.RenderAsync(route));
        }

        files[NotFoundFile] = template.Fill(await _renderer.RenderNotFoundAsync());
        files[SitemapFile] = Sitemap.Build(SiteInfo.Origin, routes);

        return files;
    }
}
