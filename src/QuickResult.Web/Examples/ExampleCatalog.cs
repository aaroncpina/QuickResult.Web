using QuickResult.Web.Examples.Snippets;

namespace QuickResult.Web.Examples;

/// <summary>
/// Composition root for the curated examples (Factory). Adding an example
/// means adding a class and one line here — nothing else changes.
/// </summary>
public static class ExampleCatalog
{
    public static IReadOnlyList<IExample> CreateAll() =>
    [
        new SuccessAndFailureExample(),
        new TryExample(),
        new FromNullableExample(),
        new MatchExample(),
        new MapAndBindExample(),
        new ValueOrExample(),
        new LinqSyncExample(),
        new LinqMixedAsyncExample(),
        new GuardsExample(),
        new AsyncPipelineExample(),
        new CustomErrorsExample(),
        new MapFailureExample(),
    ];
}
