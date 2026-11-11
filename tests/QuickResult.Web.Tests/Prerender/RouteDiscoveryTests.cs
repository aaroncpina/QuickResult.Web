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
        Assert.Equal(
            ["/", "/contact", "/examples", "/getting-started", "/philosophy", "/reference"],
            Discovered());
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
}
