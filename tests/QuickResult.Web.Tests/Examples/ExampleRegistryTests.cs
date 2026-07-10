using QuickResult.Web.Examples;

namespace QuickResult.Web.Tests.Examples;

public class ExampleRegistryTests
{
    private static ExampleRegistry CreateRegistry(params IExample[] examples) => new(examples);

    private sealed class FakeExample(string id, string category) : IExample
    {
        public string Id => id;
        public string Title => $"Title of {id}";
        public string Category => category;
        public string Description => $"Description of {id}";
        public string SourceCode => "// code";
        public IReadOnlyList<ExampleInput> Inputs => [];
        public Task<IReadOnlyList<string>> RunAsync(ExampleArguments args) =>
            Task.FromResult<IReadOnlyList<string>>([id]);
    }

    [Fact]
    public void All_PreservesRegistrationOrder()
    {
        var registry = CreateRegistry(
            new FakeExample("one", "Cat A"),
            new FakeExample("two", "Cat B"),
            new FakeExample("three", "Cat A"));

        Assert.Equal(["one", "two", "three"], registry.All.Select(e => e.Id));
    }

    [Fact]
    public void Categories_AreDistinct_InFirstAppearanceOrder()
    {
        var registry = CreateRegistry(
            new FakeExample("one", "Cat A"),
            new FakeExample("two", "Cat B"),
            new FakeExample("three", "Cat A"));

        Assert.Equal(["Cat A", "Cat B"], registry.Categories);
    }

    [Fact]
    public void GetByCategory_ReturnsOnlyMatchingExamples()
    {
        var registry = CreateRegistry(
            new FakeExample("one", "Cat A"),
            new FakeExample("two", "Cat B"),
            new FakeExample("three", "Cat A"));

        Assert.Equal(["one", "three"], registry.GetByCategory("Cat A").Select(e => e.Id));
    }

    [Fact]
    public void GetByCategory_ReturnsEmpty_ForUnknownCategory()
    {
        var registry = CreateRegistry(new FakeExample("one", "Cat A"));

        Assert.Empty(registry.GetByCategory("Nope"));
    }

    [Fact]
    public void Find_ReturnsSuccess_ForKnownId()
    {
        var registry = CreateRegistry(new FakeExample("one", "Cat A"));

        var result = registry.Find("one");

        Assert.True(result.IsSuccess);
        Assert.Equal("one", result.Value.Id);
    }

    [Fact]
    public void Find_ReturnsFailure_ForUnknownId()
    {
        var registry = CreateRegistry(new FakeExample("one", "Cat A"));

        var result = registry.Find("does-not-exist");

        Assert.True(result.IsFailure);
        Assert.Contains("does-not-exist", result.Error.Message);
    }
}
