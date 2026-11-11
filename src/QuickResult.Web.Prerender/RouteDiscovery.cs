using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace QuickResult.Web.Prerender;

/// <summary>
/// Finds the static pages of the site from their <c>@page</c> routes, so a new page is
/// prerendered and listed in the sitemap without being registered anywhere.
/// </summary>
public static class RouteDiscovery
{
    /// <summary>
    /// Returns every parameterless route in <paramref name="assembly"/>, root first and then
    /// alphabetically, leaving out the routes of <paramref name="notFoundPage"/>.
    /// </summary>
    public static IReadOnlyList<RoutePath> FindPages(Assembly assembly, Type notFoundPage)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(notFoundPage);

        return assembly.GetExportedTypes()
            .Where(type => type != notFoundPage && typeof(IComponent).IsAssignableFrom(type))
            .SelectMany(type => type.GetCustomAttributes<RouteAttribute>(inherit: false))
            .Select(route => RoutePath.TryCreate(route.Template, out var path) ? path : null)
            .OfType<RoutePath>()
            .Distinct()
            .OrderBy(path => path.Value, StringComparer.Ordinal)
            .ToList();
    }
}
