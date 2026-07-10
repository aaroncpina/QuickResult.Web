namespace QuickResult.Web.Examples.Snippets;

public sealed class LinqMixedAsyncExample : ExampleBase
{
    public override string Id => "linq-mixed-async";
    public override string Title => "Mixed sync + async in one query";
    public override string Category => "LINQ query syntax";

    public override string Description =>
        "QuickResult's signature feature: synchronous and asynchronous steps mix freely in a single " +
        "LINQ query. No Task.Result, no ceremony — await the query once at the end.";

    public override string SourceCode =>
        """
        static Task<Result<int>> FetchAsync(string text) =>
            Task.FromResult(Result.Try(() => int.Parse(text)));

        var query =
            from a in Result.Try(() => int.Parse(first))   // sync
            from b in FetchAsync(second)                   // async
            from c in Result.Try(() => int.Parse(third))   // sync again
            select a + b + c;

        Console.WriteLine(await query);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("a", "First (sync)", "4"),
        new("b", "Second (async)", "6"),
        new("c", "Third (sync)", "2"),
    ];

    public override async Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var first = args.Get("a");
        var second = args.Get("b");
        var third = args.Get("c");

        static Task<Result<int>> FetchAsync(string text) =>
            Task.FromResult(Result.Try(() => int.Parse(text)));

        var query =
            from a in Result.Try(() => int.Parse(first))
            from b in FetchAsync(second)
            from c in Result.Try(() => int.Parse(third))
            select a + b + c;

        var result = await query;

        return [result.ToString()];
    }
}
