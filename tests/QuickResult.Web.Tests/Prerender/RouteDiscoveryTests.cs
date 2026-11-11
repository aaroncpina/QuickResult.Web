using Microsoft.AspNetCore.Components;
using QuickResult.Web.Pages;
using QuickResult.Web.Prerender;

namespace QuickResult.Web.Tests.Prerender;

public class RouteDiscoveryTests
{
    private static IReadOnlyList<string> Discovered() =>
        RouteDiscovery.FindPages(typeof(App).Assembly, typeof(NotFound)).Select(r => r.Value).ToList();

    [Fact]
    public void FindPages_ReturnsEveryRoutablePage()
    {
        // Runs against the stub pages below rather than the site's own pages, so adding a
        // page to the site does not change what this test expects.
        var routes = RouteDiscovery
            .FindPages(typeof(RouteDiscoveryTests).Assembly, typeof(StubPages.Missing))
            .Select(r => r.Value);

        Assert.Equal(["/", "/alpha", "/nested/page", "/zebra"], routes);
    }

    [Fact]
    public void FindPages_LeavesOutTheNotFoundPage()
    {
        Assert.DoesNotContain("/not-found", Discovered());
    }

    [Fact]
    public void FindPages_ListsTheRootFirst()
    {
        Assert.Equal("/", Discovered()[0]);
    }

    /// <summary>Stand-in pages for the discovery test: static routes, a parameterised one, and a not-found page.</summary>
    public static class StubPages
    {
        [Route("/zebra")]
        public sealed class Zebra : ComponentBase;

        [Route("/")]
        public sealed class Home : ComponentBase;

        [Route("/alpha")]
        [Route("/nested/page")]
        public sealed class Alpha : ComponentBase;

        [Route("/users/{id}")]
        public sealed class UserDetails : ComponentBase;

        [Route("/missing")]
        public sealed class Missing : ComponentBase;

        public sealed class NotRoutable : ComponentBase;
    }
}
