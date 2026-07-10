namespace QuickResult.Web.Examples;

/// <summary>
/// Default <see cref="IExampleRegistry"/> over an injected example collection.
/// The registry itself never changes when examples are added (Open/Closed).
/// </summary>
public sealed class ExampleRegistry : IExampleRegistry
{
    private readonly List<IExample> _examples;

    public ExampleRegistry(IEnumerable<IExample> examples)
    {
        ArgumentNullException.ThrowIfNull(examples);
        _examples = examples.ToList();
    }

    public IReadOnlyList<IExample> All => _examples;

    public IReadOnlyList<string> Categories =>
        _examples.Select(e => e.Category).Distinct().ToList();

    public IReadOnlyList<IExample> GetByCategory(string category) =>
        _examples.Where(e => e.Category == category).ToList();

    public Result<IExample> Find(string id)
    {
        var match = _examples.FirstOrDefault(e => e.Id == id);
        return match is null
            ? Result.Failure<IExample>($"No example with id '{id}'")
            : Result.Success(match);
    }
}
