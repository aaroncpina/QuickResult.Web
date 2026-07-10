namespace QuickResult.Web.Examples.Snippets;

public sealed class CustomErrorsExample : ExampleBase
{
    public override string Id => "custom-errors";
    public override string Title => "Custom IError types";
    public override string Category => "Custom errors";

    public override string Description =>
        "Any type implementing IError can ride in a failure, so domain errors carry structured data. " +
        "Pattern match on the concrete type to recover it — no string parsing, no exception hierarchies.";

    public override string SourceCode =>
        """
        public sealed record HttpResponseError(int StatusCode, string Message) : IError;

        var result = Result.Failure<string>(
            new HttpResponseError(statusCode, "Upstream call failed"));

        var line = result.Match(
            onSuccess: v => $"Fetched: {v}",
            onFailure: e => e is HttpResponseError http
                ? $"HTTP {http.StatusCode}: {http.Message}"
                : $"Unknown error: {e.Message}");

        Console.WriteLine(result);
        Console.WriteLine(line);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("status", "HTTP status code", "503"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var statusCode = int.TryParse(args.Get("status"), out var parsed) ? parsed : 503;

        var result = Result.Failure<string>(
            new HttpResponseError(statusCode, "Upstream call failed"));

        var line = result.Match(
            onSuccess: v => $"Fetched: {v}",
            onFailure: e => e is HttpResponseError http
                ? $"HTTP {http.StatusCode}: {http.Message}"
                : $"Unknown error: {e.Message}");

        return Task.FromResult<IReadOnlyList<string>>([result.ToString(), line]);
    }

    private sealed record HttpResponseError(int StatusCode, string Message) : IError;
}
