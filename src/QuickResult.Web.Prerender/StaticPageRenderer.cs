using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace QuickResult.Web.Prerender;

/// <summary>
/// Renders the site's pages to static HTML by running the real <see cref="App"/> router at a
/// given address, with the same services the browser app registers.
/// </summary>
public sealed class StaticPageRenderer
{
    // A path no page can claim: route paths are lower-case kebab segments, so the router
    // falls through to the app's not-found page.
    private const string UnroutablePath = "/__not-found__";

    public Task<PrerenderedPage> RenderAsync(RoutePath route)
    {
        ArgumentNullException.ThrowIfNull(route);
        return RenderAtAsync(route.ToAbsoluteUrl(SiteInfo.Origin));
    }

    public Task<PrerenderedPage> RenderNotFoundAsync() =>
        RenderAtAsync(new Uri(SiteInfo.Origin, UnroutablePath));

    private static async Task<PrerenderedPage> RenderAtAsync(Uri address)
    {
        var services = new ServiceCollection();
        services.AddSiteServices();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton<NavigationManager>(new StaticNavigationManager(SiteInfo.Origin, address));
        services.AddSingleton<NoBrowserRouting>();
        services.AddSingleton<INavigationInterception>(s => s.GetRequiredService<NoBrowserRouting>());
        services.AddSingleton<IScrollToLocationHash>(s => s.GetRequiredService<NoBrowserRouting>());
        services.AddSingleton<IJSRuntime, NoJavaScriptRuntime>();

        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            // The page renders first so its title and head content are registered by the
            // time the head outlet renders.
            var body = await renderer.RenderComponentAsync<App>();
            var head = await renderer.RenderComponentAsync<HeadOutlet>();

            return new PrerenderedPage(head.ToHtmlString(), body.ToHtmlString());
        });
    }
}
