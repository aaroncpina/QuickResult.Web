namespace QuickResult.Web.Examples.Snippets;

public sealed class GuardsExample : ExampleBase
{
    public override string Id => "guards";
    public override string Title => "FailIfFalse / FailIfTrue — boolean guards";
    public override string Category => "Guards & validation";

    public override string Description =>
        "Guards turn boolean checks into failures inside a query, keeping validation rules on the " +
        "same railway as the rest of the flow. Change the age to cross the threshold.";

    public override string SourceCode =>
        """
        var canVote =
            from age in Result.Try(() => int.Parse(ageText))
            from ok in Result.Success(age >= 18)
                             .FailIfFalse("Must be 18 or older")
            select $"Age {age}: ballot issued";

        Console.WriteLine(canVote);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("age", "Age", "16"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var ageText = args.Get("age");

        var canVote =
            from age in Result.Try(() => int.Parse(ageText))
            from ok in Result.Success(age >= 18)
                             .FailIfFalse("Must be 18 or older")
            select $"Age {age}: ballot issued";

        return Task.FromResult<IReadOnlyList<string>>([canVote.ToString()]);
    }
}
