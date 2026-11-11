namespace QuickResult.Web;

/// <summary>Facts about the deployed site that pages and build tooling share.</summary>
public static class SiteInfo
{
    /// <summary>The public origin the site is served from; canonical URLs are built on it.</summary>
    public static Uri Origin { get; } = new("https://quickresult.net");
}
