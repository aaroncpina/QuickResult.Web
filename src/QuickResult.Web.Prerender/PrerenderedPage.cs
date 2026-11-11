namespace QuickResult.Web.Prerender;

/// <summary>The static HTML a page renders to: its head tags and its body markup.</summary>
public sealed record PrerenderedPage(string Head, string Body);
