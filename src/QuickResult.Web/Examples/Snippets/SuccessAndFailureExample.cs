namespace QuickResult.Web.Examples.Snippets;

public sealed class SuccessAndFailureExample : ExampleBase
{
    public override string Id => "success-and-failure";
    public override string Title => "Success & Failure";
    public override string Category => "Creating results";

    public override string Description =>
        "Every operation returns a Result<T>: either a success carrying a value, " +
        "or a failure carrying an IError. No exceptions, no nulls — the outcome is a value you can inspect.";

    public override string SourceCode =>
        """
        var ok = Result.Success("42");
        var fail = Result.Failure<string>("Something went wrong");

        Console.WriteLine(ok);
        Console.WriteLine(fail);
        Console.WriteLine($"ok.IsSuccess: {ok.IsSuccess}");
        Console.WriteLine($"fail.IsFailure: {fail.IsFailure}");
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("value", "Success value", "42"),
        new("error", "Error message", "Something went wrong"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var value = args.Get("value");
        var error = OrDefault(args.Get("error"), "Something went wrong");

        var ok = Result.Success(value);
        var fail = Result.Failure<string>(error);

        return Task.FromResult<IReadOnlyList<string>>(
        [
            ok.ToString(),
            fail.ToString(),
            $"ok.IsSuccess: {ok.IsSuccess}",
            $"fail.IsFailure: {fail.IsFailure}",
        ]);
    }
}
