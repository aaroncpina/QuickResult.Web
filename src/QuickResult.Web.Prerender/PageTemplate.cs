namespace QuickResult.Web.Prerender;

/// <summary>
/// The published <c>index.html</c> shell, split around its two marked regions so each
/// page's head tags and body markup can be dropped in.
/// </summary>
public sealed class PageTemplate
{
    private const string HeadStart = "<!--prerender:head-->";
    private const string HeadEnd = "<!--/prerender:head-->";
    private const string AppStart = "<!--prerender:app-->";
    private const string AppEnd = "<!--/prerender:app-->";

    private readonly string _beforeHead;
    private readonly string _betweenHeadAndApp;
    private readonly string _afterApp;

    private PageTemplate(string beforeHead, string betweenHeadAndApp, string afterApp)
    {
        _beforeHead = beforeHead;
        _betweenHeadAndApp = betweenHeadAndApp;
        _afterApp = afterApp;
    }

    public static PageTemplate Parse(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var headStart = IndexAfter(html, HeadStart, 0);
        var headEnd = IndexOf(html, HeadEnd, headStart);
        var appStart = IndexAfter(html, AppStart, headEnd);
        var appEnd = IndexOf(html, AppEnd, appStart);

        return new PageTemplate(html[..headStart], html[headEnd..appStart], html[appEnd..]);
    }

    public string Fill(PrerenderedPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        return string.Concat(_beforeHead, page.Head, _betweenHeadAndApp, page.Body, _afterApp);
    }

    private static int IndexAfter(string html, string marker, int from) =>
        IndexOf(html, marker, from) + marker.Length;

    private static int IndexOf(string html, string marker, int from)
    {
        var index = html.IndexOf(marker, from, StringComparison.Ordinal);
        if (index < 0)
        {
            throw new FormatException(
                $"The page shell is missing the marker {marker}, or its markers are out of order.");
        }

        return index;
    }
}
