namespace QuickResult.Web.Examples.Snippets;

public sealed class LinqSyncExample : ExampleBase
{
    public override string Id => "linq-sync";
    public override string Title => "LINQ query syntax";
    public override string Category => "LINQ query syntax";

    public override string Description =>
        "Results compose with from/select just like sequences. Each from unwraps a success; " +
        "the first failure short-circuits the whole query. Try a non-number to see it stop early.";

    public override string SourceCode =>
        """
        var query =
            from a in Result.Try(() => int.Parse(first))
            from b in Result.Try(() => int.Parse(second))
            select a + b;

        Console.WriteLine(query);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("a", "First number", "10"),
        new("b", "Second number", "5"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var first = args.Get("a");
        var second = args.Get("b");

        var query =
            from a in Result.Try(() => int.Parse(first))
            from b in Result.Try(() => int.Parse(second))
            select a + b;

        return Task.FromResult<IReadOnlyList<string>>([query.ToString()]);
    }
}
