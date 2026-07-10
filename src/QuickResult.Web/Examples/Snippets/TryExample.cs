namespace QuickResult.Web.Examples.Snippets;

public sealed class TryExample : ExampleBase
{
    public override string Id => "try";
    public override string Title => "Result.Try — exceptions become failures";
    public override string Category => "Creating results";

    public override string Description =>
        "Result.Try runs code that might throw and converts any exception into a failure, " +
        "so exception-throwing APIs plug straight into a Result flow.";

    public override string SourceCode =>
        """
        var parsed = Result.Try(() => int.Parse(text));

        Console.WriteLine(parsed);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("text", "Text to parse", "123"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var text = args.Get("text");

        var parsed = Result.Try(() => int.Parse(text));

        return Task.FromResult<IReadOnlyList<string>>([parsed.ToString()]);
    }
}
