using System.Text;
using QuickResult.Web.Prerender;

// Usage: QuickResult.Web.Prerender <published wwwroot directory>
// Rewrites the published site in place so every page is real HTML before the app boots.
if (args is not [var siteRoot] || !File.Exists(Path.Combine(siteRoot, "index.html")))
{
    Console.Error.WriteLine("Usage: QuickResult.Web.Prerender <published wwwroot directory containing index.html>");
    return 1;
}

var shell = await File.ReadAllTextAsync(Path.Combine(siteRoot, "index.html"));
var files = await new SitePrerenderer().BuildAsync(shell);

foreach (var (relativePath, content) in files)
{
    var path = Path.Combine(siteRoot, relativePath);
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    await File.WriteAllTextAsync(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

    // Publish pre-compresses index.html; those copies are now stale.
    File.Delete(path + ".br");
    File.Delete(path + ".gz");

    Console.WriteLine($"prerendered {relativePath}");
}

return 0;
