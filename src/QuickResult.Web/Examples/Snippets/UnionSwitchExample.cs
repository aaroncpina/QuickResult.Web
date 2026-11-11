namespace QuickResult.Web.Examples.Snippets;

public sealed class UnionSwitchExample : ExampleBase
{
    private const string InvalidQuantityMessage = "Quantity must be a positive whole number";
    private const int Stock = 10;

    public override string Id => "union-switch";
    public override string Title => "Switch on a Result as a C# union";
    public override string Category => "C# unions";

    public override string Description =>
        "On .NET 10 and later, Result<T> is a C# union of T and IError. With C# 15 you can switch on the " +
        "result itself by type — the compiler checks that both cases are handled — and a value or an " +
        "error converts implicitly to a result, so a method can return either directly.";

    public override string SourceCode =>
        """
        public sealed record OutOfStockError(string Message) : IError;

        Result<int> Reserve(string text)
        {
            if (!int.TryParse(text, out var requested) || requested <= 0)
                return new Error("Quantity must be a positive whole number");

            if (requested > 10)
                return new OutOfStockError($"Only 10 in stock, {requested} requested");

            return requested;
        }

        var result = Reserve(quantity);

        var line = result switch
        {
            int reserved => $"Reserved {reserved} widgets",
            OutOfStockError e => $"409 Conflict: {e.Message}",
            IError e => $"400 Bad Request: {e.Message}",
        };

        Console.WriteLine(result);
        Console.WriteLine(line);
        """;

    public override IReadOnlyList<ExampleInput> Inputs =>
    [
        new("quantity", "Quantity (10 in stock)", "3"),
    ];

    public override Task<IReadOnlyList<string>> RunAsync(ExampleArguments args)
    {
        var result = Reserve(args.Get("quantity"));

        var line = result switch
        {
            int reserved => $"Reserved {reserved} widgets",
            OutOfStockError e => $"409 Conflict: {e.Message}",
            IError e => $"400 Bad Request: {e.Message}",
        };

        return Task.FromResult<IReadOnlyList<string>>([result.ToString(), line]);
    }

    private static Result<int> Reserve(string text)
    {
        if (!int.TryParse(text, out var requested) || requested <= 0)
            return new Error(InvalidQuantityMessage);

        if (requested > Stock)
            return new OutOfStockError($"Only {Stock} in stock, {requested} requested");

        return requested;
    }

    private sealed record OutOfStockError(string Message) : IError;
}
