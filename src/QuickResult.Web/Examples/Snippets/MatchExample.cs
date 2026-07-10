namespace QuickResult.Web.Examples.Snippets;

public sealed class MatchExample : ExampleBase
{
    public override string Id => "match";
    public override string Title => "Match — one exit for both branches";
    public override string Category => "Transforming";

    public override string Description =>
        "Match collapses a Result into a single value by handling both branches in one expression — " +
        "the compiler makes it impossible to forget the failure path.";

    public override string SourceCode =>
        """
        var message = Result
            .Try(() => int.Parse(quantity))
            .Match(
                onSuccess: v => $"Ordered {v} widgets",
                onFailure: e => $"Could not order: {e.Message}");

        Console.WriteLine(message);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("quantity", "Quantity", "3"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var quantity = args.Get("quantity");

        var message = Result
            .Try(() => int.Parse(quantity))
            .Match(
                onSuccess: v => $"Ordered {v} widgets",
                onFailure: e => $"Could not order: {e.Message}");

        return Task.FromResult<IReadOnlyList<string>>([message]);
    }
}
