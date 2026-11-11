using QuickResult.Web.Examples;

namespace QuickResult.Web.Tests.Examples;

/// <summary>
/// Pins the observable behavior of the C# union example (QuickResult 2.5.0+)
/// against the real QuickResult package.
/// </summary>
public class UnionSwitchExampleTests
{
    private static async Task<IReadOnlyList<string>> RunAsync(string? quantity = null)
    {
        var example = ExampleCatalog.CreateAll().Single(e => e.Id == "union-switch");
        var values = example.Inputs.ToDictionary(i => i.Name, i => i.DefaultValue);
        if (quantity is not null)
        {
            values["quantity"] = quantity;
        }

        return await example.RunAsync(new ExampleArguments(values));
    }

    [Fact]
    public async Task UnionSwitch_SelectsValueArm_WhenQuantityIsInStock()
    {
        var output = await RunAsync();

        Assert.Equal("Success(3)", output[0]);
        Assert.Equal("Reserved 3 widgets", output[1]);
    }

    [Fact]
    public async Task UnionSwitch_SelectsCustomErrorArm_WhenQuantityExceedsStock()
    {
        var output = await RunAsync("25");

        Assert.Equal("Failure(OutOfStockError { Message = Only 10 in stock, 25 requested })", output[0]);
        Assert.Equal("409 Conflict: Only 10 in stock, 25 requested", output[1]);
    }

    [Fact]
    public async Task UnionSwitch_SelectsGeneralErrorArm_WhenQuantityIsNotANumber()
    {
        var output = await RunAsync("abc");

        Assert.Equal("Failure(Quantity must be a positive whole number)", output[0]);
        Assert.Equal("400 Bad Request: Quantity must be a positive whole number", output[1]);
    }

    [Fact]
    public async Task UnionSwitch_SelectsGeneralErrorArm_WhenQuantityIsNotPositive()
    {
        var output = await RunAsync("0");

        Assert.Equal("400 Bad Request: Quantity must be a positive whole number", output[1]);
    }

    [Fact]
    public async Task UnionSwitch_SelectsValueArm_WhenQuantityEqualsStock()
    {
        var output = await RunAsync("10");

        Assert.Equal("Reserved 10 widgets", output[1]);
    }

    [Fact]
    public async Task UnionSwitch_SelectsCustomErrorArm_WhenQuantityIsOneOverStock()
    {
        var output = await RunAsync("11");

        Assert.Equal("409 Conflict: Only 10 in stock, 11 requested", output[1]);
    }

    [Fact]
    public void UnionSwitch_SourceCode_ShowsTypePatternSwitch()
    {
        var example = ExampleCatalog.CreateAll().Single(e => e.Id == "union-switch");

        Assert.Contains("result switch", example.SourceCode);
        Assert.Contains("IError", example.SourceCode);
    }
}
