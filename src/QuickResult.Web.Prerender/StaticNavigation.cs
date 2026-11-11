using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace QuickResult.Web.Prerender;

/// <summary>A navigation manager fixed at one address; a static render never navigates.</summary>
internal sealed class StaticNavigationManager : NavigationManager
{
    public StaticNavigationManager(Uri origin, Uri address) =>
        Initialize(origin.AbsoluteUri, address.AbsoluteUri);

    protected override void NavigateToCore(string uri, NavigationOptions options) =>
        throw new NotSupportedException($"A page tried to navigate to '{uri}' while being prerendered.");
}

/// <summary>Browser-only routing services, which have nothing to do in a static render.</summary>
internal sealed class NoBrowserRouting : INavigationInterception, IScrollToLocationHash
{
    public Task EnableNavigationInterceptionAsync() => Task.CompletedTask;

    public Task RefreshScrollPositionForHash(string locationAbsolute) => Task.CompletedTask;
}

/// <summary>There is no browser during a static render, so JavaScript calls are refused.</summary>
internal sealed class NoJavaScriptRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
        throw new NotSupportedException($"A page called JavaScript ('{identifier}') while being prerendered.");

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
        InvokeAsync<TValue>(identifier, args);
}
