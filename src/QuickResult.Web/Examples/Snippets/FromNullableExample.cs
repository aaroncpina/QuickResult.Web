namespace QuickResult.Web.Examples.Snippets;

public sealed class FromNullableExample : ExampleBase
{
    public override string Id => "from-nullable";
    public override string Title => "Result.FromNullable — banish null checks";
    public override string Category => "Creating results";

    public override string Description =>
        "FromNullable converts a possibly-null value into an explicit Result: " +
        "success when a value is present, a named failure when it is null. Clear the input to see the failure path.";

    public override string SourceCode =>
        """
        string? name = LookupName(); // may be null

        var result = Result.FromNullable(name, "No name provided");

        Console.WriteLine(result);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("name", "Name (clear to simulate null)", "Ada"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var raw = args.Get("name");
        string? name = string.IsNullOrWhiteSpace(raw) ? null : raw;

        var result = Result.FromNullable(name, "No name provided");

        return Task.FromResult<IReadOnlyList<string>>([result.ToString()]);
    }
}
