using System.Text.RegularExpressions;

namespace QuickResult.Web.Prerender;

/// <summary>
/// The path of a static page on the site: <c>/</c>, or lower-case kebab segments such as
/// <c>/getting-started</c>. Routes with parameters, query strings or fragments are not static
/// pages and are rejected.
/// </summary>
public sealed partial record RoutePath
{
    private const string RootValue = "/";

    public string Value { get; }

    private RoutePath(string value) => Value = value;

    public static RoutePath Root { get; } = new(RootValue);

    public static RoutePath Create(string? input)
    {
        if (input is null || !StaticPath().IsMatch(input))
        {
            throw new ArgumentException(
                $"'{input}' is not a static route path. Expected '/' or lower-case segments such as '/getting-started'.",
                nameof(input));
        }

        return new RoutePath(input);
    }

    public static bool TryCreate(string? input, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out RoutePath? route)
    {
        route = input is not null && StaticPath().IsMatch(input) ? new RoutePath(input) : null;
        return route is not null;
    }

    /// <summary>
    /// The file the page is published as, relative to the site root. Pages are written as
    /// <c>name.html</c> rather than <c>name/index.html</c> so the host serves them at the
    /// path itself, with no trailing-slash redirect.
    /// </summary>
    public string OutputFile => Value == RootValue ? "index.html" : $"{Value[1..]}.html";

    public Uri ToAbsoluteUrl(Uri origin)
    {
        ArgumentNullException.ThrowIfNull(origin);
        return new Uri(origin, Value);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"\A/(?:[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*)?\z")]
    private static partial Regex StaticPath();
}
