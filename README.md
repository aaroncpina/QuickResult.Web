# QuickResult.Web

The website for the [QuickResult](https://www.nuget.org/packages/QuickResult/)
NuGet package — a lightweight Result monad for C# with full LINQ query syntax —
live at **[quickresult.net](https://quickresult.net)**.

Built with **Blazor WebAssembly** (.NET 10): the interactive examples on the
site execute the real QuickResult package, compiled to WebAssembly, in the
visitor's browser.

## Structure

```
src/QuickResult.Web/          Blazor WASM site
  Examples/                   Example engine: IExample, registry, catalog
  Examples/Snippets/          One class per interactive example (Open/Closed)
  Services/                   Dependency-free C# syntax highlighter
  Components/                 CodeBlock, ExampleRunner
  Pages/                      Home, Getting Started, Examples, Reference,
                              C# Unions, Philosophy, Contact, 404
src/QuickResult.Web.Prerender/ Build-time tool: renders each page to static HTML,
                              plus 404.html and sitemap.xml
tests/QuickResult.Web.Tests/  xUnit + bUnit suite (TDD; tests are immutable)
.github/workflows/deploy.yml  Test + deploy to Cloudflare Pages
DEPLOYMENT.md                 Manual go-live checksheet
```

## Development

```bash
dotnet test                                 # run the full suite
dotnet run --project src/QuickResult.Web    # local dev server
dotnet publish src/QuickResult.Web -c Release -o publish   # static output in publish/wwwroot
dotnet run --project src/QuickResult.Web.Prerender -c Release -- publish/wwwroot   # prerender it
```

## Prerendering

The site is a WebAssembly app, but crawlers and link previews mostly don't run
WebAssembly. After publishing, the prerender tool runs the real app router for
every `@page` route and writes the result into the published output:

- `index.html`, `reference.html`, … — each page's full markup and its own
  `<title>`, description, canonical URL and Open Graph tags (from the
  `PageMeta` component at the top of each page);
- `404.html` — so unknown URLs return a real 404 instead of the app shell;
- `sitemap.xml` — every page's public address.

The app still boots in the browser and takes over, so the examples stay
interactive. New pages are picked up automatically: add a `.razor` page with a
`PageMeta` and it is prerendered and listed in the sitemap.

## Adding an example

1. Add a class in `Examples/Snippets/` implementing `ExampleBase`.
2. Register it in `ExampleCatalog.CreateAll()`.

Nothing else changes — navigation, rendering, and the catalog invariant tests
pick it up automatically.

## Deployment

Pushes to `main` run the tests and deploy to Cloudflare Pages via GitHub
Actions. After each deploy the workflow pings IndexNow with the sitemap's
URLs so Bing and other participating search engines re-crawl promptly. See [DEPLOYMENT.md](DEPLOYMENT.md) for the one-time setup
(domain, DNS, email routing for support@quickresult.net).
