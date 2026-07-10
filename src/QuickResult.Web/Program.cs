using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickResult.Web;
using QuickResult.Web.Examples;
using QuickResult.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<ISyntaxHighlighter, CSharpSyntaxHighlighter>();
builder.Services.AddSingleton<IExampleRegistry>(new ExampleRegistry(ExampleCatalog.CreateAll()));

await builder.Build().RunAsync();
