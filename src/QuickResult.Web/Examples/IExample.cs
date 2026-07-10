namespace QuickResult.Web.Examples;

/// <summary>
/// A runnable, self-describing code example (Strategy pattern).
/// New examples are added as new implementations and registered in
/// <see cref="ExampleCatalog"/> — existing code never changes (Open/Closed).
/// </summary>
public interface IExample
{
    /// <summary>Unique kebab-case identifier, used in URLs and anchors.</summary>
    string Id { get; }

    string Title { get; }

    /// <summary>Display category used to group examples in navigation.</summary>
    string Category { get; }

    string Description { get; }

    /// <summary>The C# source shown to the user; mirrors what <see cref="RunAsync"/> executes.</summary>
    string SourceCode { get; }

    /// <summary>Editable inputs surfaced in the UI; may be empty.</summary>
    IReadOnlyList<ExampleInput> Inputs { get; }

    /// <summary>Executes the example against the real QuickResult package and returns output lines.</summary>
    Task<IReadOnlyList<string>> RunAsync(ExampleArguments args);
}
