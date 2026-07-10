namespace QuickResult.Web.Examples.Snippets;

public sealed class MapFailureExample : ExampleBase
{
    public override string Id => "map-failure";
    public override string Title => "MapFailure — enrich errors on the way up";
    public override string Category => "Custom errors";

    public override string Description =>
        "MapFailure transforms the error while leaving successes untouched — ideal for adding context " +
        "(service name, correlation id) as a failure bubbles up through layers.";

    public override string SourceCode =>
        """
        var result = Result.Failure<int>(message)
            .MapFailure(e => new Error($"[orders-service] {e.Message}"));

        Console.WriteLine(result);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("error", "Inner error message", "Timeout after 30s"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var message = OrDefault(args.Get("error"), "Timeout after 30s");

        var result = Result.Failure<int>(message)
            .MapFailure(e => new Error($"[orders-service] {e.Message}"));

        return Task.FromResult<IReadOnlyList<string>>([result.ToString()]);
    }
}
