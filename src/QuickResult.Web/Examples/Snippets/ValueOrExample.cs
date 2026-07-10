namespace QuickResult.Web.Examples.Snippets;

public sealed class ValueOrExample : ExampleBase
{
    public override string Id => "value-or";
    public override string Title => "ValueOr & the | operator — fallbacks";
    public override string Category => "Transforming";

    public override string Description =>
        "ValueOr unwraps a success or substitutes a fallback value. " +
        "The | operator does the same at the Result level: keep the left result if it succeeded, otherwise use the right.";

    public override string SourceCode =>
        """
        var parsed = Result.Try(() => int.Parse(text));

        Console.WriteLine($"ValueOr(-1): {parsed.ValueOr(-1)}");

        var withFallback = parsed | Result.Success(0);
        Console.WriteLine($"parsed | Success(0): {withFallback}");
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("text", "Text to parse", "abc"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var text = args.Get("text");

        var parsed = Result.Try(() => int.Parse(text));
        var withFallback = parsed | Result.Success(0);

        return Task.FromResult<IReadOnlyList<string>>(
        [
            $"ValueOr(-1): {parsed.ValueOr(-1)}",
            $"parsed | Success(0): {withFallback}",
        ]);
    }
}
