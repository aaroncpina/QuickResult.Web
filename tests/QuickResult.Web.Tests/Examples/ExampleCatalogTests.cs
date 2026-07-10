using System.Text.RegularExpressions;
using QuickResult.Web.Examples;

namespace QuickResult.Web.Tests.Examples;

/// <summary>
/// Invariants that must hold for every example shipped in the catalog,
/// no matter how many examples are added later (Open/Closed: new examples
/// are added as new classes and are automatically covered here).
/// </summary>
public class ExampleCatalogTests
{
    private static IReadOnlyList<IExample> All => ExampleCatalog.CreateAll();

    public static TheoryData<string> AllExampleIds()
    {
        var data = new TheoryData<string>();
        foreach (var example in ExampleCatalog.CreateAll())
        {
            data.Add(example.Id);
        }

        return data;
    }

    private static IExample Get(string id) => All.Single(e => e.Id == id);

    [Fact]
    public void Catalog_ContainsAReasonableNumberOfExamples()
    {
        Assert.True(All.Count >= 10, $"Expected at least 10 examples but found {All.Count}");
    }

    [Fact]
    public void Ids_AreUnique()
    {
        var ids = All.Select(e => e.Id).ToList();

        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public void Id_IsKebabCase(string id)
    {
        Assert.Matches(new Regex("^[a-z0-9]+(-[a-z0-9]+)*$"), id);
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public void Metadata_IsPopulated(string id)
    {
        var example = Get(id);

        Assert.False(string.IsNullOrWhiteSpace(example.Title));
        Assert.False(string.IsNullOrWhiteSpace(example.Category));
        Assert.False(string.IsNullOrWhiteSpace(example.Description));
        Assert.False(string.IsNullOrWhiteSpace(example.SourceCode));
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public void Inputs_HaveNamesLabelsAndDefaults(string id)
    {
        var example = Get(id);

        foreach (var input in example.Inputs)
        {
            Assert.False(string.IsNullOrWhiteSpace(input.Name));
            Assert.False(string.IsNullOrWhiteSpace(input.Label));
            Assert.NotNull(input.DefaultValue);
        }
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public async Task RunAsync_WithDefaultInputs_ProducesOutput(string id)
    {
        var example = Get(id);
        var args = ExampleArguments.ForDefaults(example.Inputs);

        var output = await example.RunAsync(args);

        Assert.NotEmpty(output);
        Assert.All(output, line => Assert.NotNull(line));
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public async Task RunAsync_WithHostileInputs_DoesNotThrow(string id)
    {
        var example = Get(id);
        var hostile = new ExampleArguments(
            example.Inputs.ToDictionary(i => i.Name, _ => "  !@# not-a-number \"quotes\" "));

        var output = await example.RunAsync(hostile);

        Assert.NotEmpty(output);
    }

    [Theory]
    [MemberData(nameof(AllExampleIds))]
    public async Task RunAsync_WithEmptyInputs_DoesNotThrow(string id)
    {
        var example = Get(id);
        var empty = new ExampleArguments(
            example.Inputs.ToDictionary(i => i.Name, _ => string.Empty));

        var output = await example.RunAsync(empty);

        Assert.NotEmpty(output);
    }
}
