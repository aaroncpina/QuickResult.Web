namespace QuickResult.Web.Examples;

/// <summary>
/// Template for concrete examples: shared input-sanitizing helpers so
/// hostile or empty UI input can never crash an example.
/// </summary>
public abstract class ExampleBase : IExample
{
    public abstract string Id { get; }
    public abstract string Title { get; }
    public abstract string Category { get; }
    public abstract string Description { get; }
    public abstract string SourceCode { get; }
    public virtual IReadOnlyList<ExampleInput> Inputs => [];

    public abstract Task<IReadOnlyList<string>> RunAsync(ExampleArguments args);

    /// <summary>Falls back to <paramref name="fallback"/> when the value is empty or whitespace.</summary>
    protected static string OrDefault(string value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;
}
