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
                              Philosophy, Contact, 404
tests/QuickResult.Web.Tests/  xUnit + bUnit suite (TDD; tests are immutable)
.github/workflows/deploy.yml  Test + deploy to Cloudflare Pages
DEPLOYMENT.md                 Manual go-live checksheet
```

## Development

```bash
dotnet test                                 # run the full suite
dotnet run --project src/QuickResult.Web    # local dev server
dotnet publish src/QuickResult.Web -c Release -o publish   # static output in publish/wwwroot
```

## Adding an example

1. Add a class in `Examples/Snippets/` implementing `ExampleBase`.
2. Register it in `ExampleCatalog.CreateAll()`.

Nothing else changes — navigation, rendering, and the catalog invariant tests
pick it up automatically.

## Deployment

Pushes to `main` run the tests and deploy to Cloudflare Pages via GitHub
Actions. See [DEPLOYMENT.md](DEPLOYMENT.md) for the one-time setup
(domain, DNS, email routing for support@quickresult.net).
