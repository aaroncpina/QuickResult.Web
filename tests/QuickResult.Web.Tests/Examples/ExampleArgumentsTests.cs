using QuickResult.Web.Examples;

namespace QuickResult.Web.Tests.Examples;

public class ExampleArgumentsTests
{
    [Fact]
    public void Get_ReturnsValue_WhenKeyPresent()
    {
        var args = new ExampleArguments(new Dictionary<string, string> { ["text"] = "42" });

        Assert.Equal("42", args.Get("text"));
    }

    [Fact]
    public void Get_ReturnsEmptyString_WhenKeyMissing()
    {
        var args = new ExampleArguments(new Dictionary<string, string>());

        Assert.Equal(string.Empty, args.Get("missing"));
    }

    [Fact]
    public void Empty_ReturnsEmptyStringForAnyKey()
    {
        Assert.Equal(string.Empty, ExampleArguments.Empty.Get("anything"));
    }

    [Fact]
    public void ForDefaults_UsesInputDefaultValues()
    {
        var inputs = new List<ExampleInput>
        {
            new("a", "First", "10"),
            new("b", "Second", "5"),
        };

        var args = ExampleArguments.ForDefaults(inputs);

        Assert.Equal("10", args.Get("a"));
        Assert.Equal("5", args.Get("b"));
    }
}
