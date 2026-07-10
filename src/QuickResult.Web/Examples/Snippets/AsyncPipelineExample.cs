namespace QuickResult.Web.Examples.Snippets;

public sealed class AsyncPipelineExample : ExampleBase
{
    public override string Id => "async-pipeline";
    public override string Title => "FromAsync → Try → WhenNull pipeline";
    public override string Category => "Async pipelines";

    public override string Description =>
        "A deferred pipeline wraps an async call, captures exceptions as failures, and converts null " +
        "payloads into named failures — three failure modes handled in one fluent chain. " +
        "Try 'throw' to simulate an exception, or clear the input to simulate null.";

    public override string SourceCode =>
        """
        Task<string?> FetchAsync() =>
            value == "throw"
                ? throw new InvalidOperationException("Simulated network failure")
                : Task.FromResult<string?>(value == "" ? null : value);

        var result = await Result
            .FromAsync(FetchAsync)
            .Try()
            .WhenNull("Response was null");

        Console.WriteLine(result);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("value", "Fetched value ('throw' or empty)", "data"),
    ];

    public override async Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var value = args.Get("value");

        Task<string?> FetchAsync() =>
            value == "throw"
                ? throw new InvalidOperationException("Simulated network failure")
                : Task.FromResult<string?>(string.IsNullOrWhiteSpace(value) ? null : value);

        Func<Task<string?>> fetch = FetchAsync;

        var result = await Result
            .FromAsync(fetch)
            .Try()
            .WhenNull("Response was null");

        return [result.ToString()];
    }
}
