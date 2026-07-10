namespace QuickResult.Web.Examples.Snippets;

public sealed class MapAndBindExample : ExampleBase
{
    public override string Id => "map-and-bind";
    public override string Title => "Map & Bind — chain without unwrapping";
    public override string Category => "Transforming";

    public override string Description =>
        "Map transforms a success value; Bind chains an operation that itself returns a Result. " +
        "Failures skip every later step automatically — the railway stays on the failure track.";

    public override string SourceCode =>
        """
        Result<int> ParsePositive(string s) =>
            int.TryParse(s, out var n) && n > 0
                ? Result.Success(n)
                : Result.Failure<int>("Not a positive integer");

        var result = Result.Success(text)
            .Bind(ParsePositive)
            .Map(n => n * 2);

        Console.WriteLine(result);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("text", "Number to double", "21"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var text = args.Get("text");

        static Result<int> ParsePositive(string s) =>
            int.TryParse(s, out var n) && n > 0
                ? Result.Success(n)
                : Result.Failure<int>("Not a positive integer");

        var result = Result.Success(text)
            .Bind(ParsePositive)
            .Map(n => n * 2);

        return Task.FromResult<IReadOnlyList<string>>([result.ToString()]);
    }
}
