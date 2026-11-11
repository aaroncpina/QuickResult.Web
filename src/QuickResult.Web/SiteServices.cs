using Microsoft.Extensions.DependencyInjection;
using QuickResult.Web.Examples;
using QuickResult.Web.Services;

namespace QuickResult.Web;

public static class SiteServices
{
    /// <summary>
    /// Registers the services the pages depend on. Shared by the browser app and the
    /// build-time prerenderer so both render the same content.
    /// </summary>
    public static IServiceCollection AddSiteServices(this IServiceCollection services)
    {
        services.AddSingleton<ISyntaxHighlighter, CSharpSyntaxHighlighter>();
        services.AddSingleton<IExampleRegistry>(new ExampleRegistry(ExampleCatalog.CreateAll()));
        return services;
    }
}
