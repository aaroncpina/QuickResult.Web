using QuickResult.Web.Examples;

namespace QuickResult.Web.Tests.Examples;

/// <summary>
/// Pins the observable behavior of each curated example against the real
/// QuickResult package, for default inputs and representative custom inputs.
/// </summary>
public class ExampleBehaviorTests
{
    private static async Task<IReadOnlyList<string>> RunAsync(
        string id, IReadOnlyDictionary<string, string>? overrides = null)
    {
        var example = ExampleCatalog.CreateAll().Single(e => e.Id == id);
        var values = example.Inputs.ToDictionary(i => i.Name, i => i.DefaultValue);
        if (overrides is not null)
        {
            foreach (var (key, value) in overrides)
            {
                values[key] = value;
            }
        }

        return await example.RunAsync(new ExampleArguments(values));
    }

    private static string Joined(IEnumerable<string> lines) => string.Join("\n", lines);

    // -- success-and-failure -------------------------------------------------

    [Fact]
    public async Task SuccessAndFailure_Defaults()
    {
        var output = Joined(await RunAsync("success-and-failure"));

        Assert.Contains("Success(42)", output);
        Assert.Contains("Failure(Something went wrong)", output);
        Assert.Contains("ok.IsSuccess: True", output);
        Assert.Contains("fail.IsFailure: True", output);
    }

    [Fact]
    public async Task SuccessAndFailure_CustomValue()
    {
        var output = Joined(await RunAsync(
            "success-and-failure",
            new Dictionary<string, string> { ["value"] = "hello" }));

        Assert.Contains("Success(hello)", output);
    }

    // -- try ------------------------------------------------------------------

    [Fact]
    public async Task Try_ParsesValidNumber()
    {
        var output = await RunAsync("try");

        Assert.Equal("Success(123)", output[0]);
    }

    [Fact]
    public async Task Try_CapturesExceptionAsFailure()
    {
        var output = await RunAsync("try", new Dictionary<string, string> { ["text"] = "abc" });

        Assert.StartsWith("Failure(", output[0]);
    }

    // -- from-nullable ----------------------------------------------------------

    [Fact]
    public async Task FromNullable_NullBecomesFailure()
    {
        var output = await RunAsync(
            "from-nullable", new Dictionary<string, string> { ["name"] = "" });

        Assert.Equal("Failure(No name provided)", output[0]);
    }

    [Fact]
    public async Task FromNullable_ValueBecomesSuccess()
    {
        var output = await RunAsync(
            "from-nullable", new Dictionary<string, string> { ["name"] = "Ada" });

        Assert.Equal("Success(Ada)", output[0]);
    }

    // -- match ------------------------------------------------------------------

    [Fact]
    public async Task Match_SuccessBranch()
    {
        var output = await RunAsync("match", new Dictionary<string, string> { ["quantity"] = "3" });

        Assert.Equal("Ordered 3 widgets", output[0]);
    }

    [Fact]
    public async Task Match_FailureBranch()
    {
        var output = await RunAsync("match", new Dictionary<string, string> { ["quantity"] = "abc" });

        Assert.StartsWith("Could not order:", output[0]);
    }

    // -- map-and-bind -------------------------------------------------------------

    [Fact]
    public async Task MapAndBind_DoublesParsedPositiveNumber()
    {
        var output = await RunAsync("map-and-bind", new Dictionary<string, string> { ["text"] = "21" });

        Assert.Equal("Success(42)", output[0]);
    }

    [Fact]
    public async Task MapAndBind_RejectsNonPositiveNumber()
    {
        var output = await RunAsync("map-and-bind", new Dictionary<string, string> { ["text"] = "-5" });

        Assert.Equal("Failure(Not a positive integer)", output[0]);
    }

    // -- value-or ---------------------------------------------------------------

    [Fact]
    public async Task ValueOr_FallsBackOnFailure()
    {
        var output = Joined(await RunAsync("value-or", new Dictionary<string, string> { ["text"] = "abc" }));

        Assert.Contains("ValueOr(-1): -1", output);
        Assert.Contains("Success(0)", output);
    }

    [Fact]
    public async Task ValueOr_UsesValueOnSuccess()
    {
        var output = Joined(await RunAsync("value-or", new Dictionary<string, string> { ["text"] = "7" }));

        Assert.Contains("ValueOr(-1): 7", output);
        Assert.Contains("Success(7)", output);
    }

    // -- linq-sync ----------------------------------------------------------------

    [Fact]
    public async Task LinqSync_AddsTwoNumbers()
    {
        var output = await RunAsync(
            "linq-sync", new Dictionary<string, string> { ["a"] = "10", ["b"] = "5" });

        Assert.Equal("Success(15)", output[0]);
    }

    [Fact]
    public async Task LinqSync_ShortCircuitsOnFirstFailure()
    {
        var output = await RunAsync(
            "linq-sync", new Dictionary<string, string> { ["a"] = "abc", ["b"] = "5" });

        Assert.StartsWith("Failure(", output[0]);
    }

    // -- linq-mixed-async ------------------------------------------------------------

    [Fact]
    public async Task LinqMixedAsync_CombinesSyncAndAsyncSteps()
    {
        var output = await RunAsync(
            "linq-mixed-async",
            new Dictionary<string, string> { ["a"] = "4", ["b"] = "6", ["c"] = "2" });

        Assert.Equal("Success(12)", output[0]);
    }

    // -- guards -----------------------------------------------------------------------

    [Fact]
    public async Task Guards_FailWhenUnderage()
    {
        var output = await RunAsync("guards", new Dictionary<string, string> { ["age"] = "16" });

        Assert.Equal("Failure(Must be 18 or older)", output[0]);
    }

    [Fact]
    public async Task Guards_PassWhenOldEnough()
    {
        var output = await RunAsync("guards", new Dictionary<string, string> { ["age"] = "21" });

        Assert.Equal("Success(Age 21: ballot issued)", output[0]);
    }

    // -- async-pipeline ------------------------------------------------------------------

    [Fact]
    public async Task AsyncPipeline_SuccessFlowsThrough()
    {
        var output = await RunAsync(
            "async-pipeline", new Dictionary<string, string> { ["value"] = "data" });

        Assert.Equal("Success(data)", output[0]);
    }

    [Fact]
    public async Task AsyncPipeline_NullBecomesFailure()
    {
        var output = await RunAsync(
            "async-pipeline", new Dictionary<string, string> { ["value"] = "" });

        Assert.Equal("Failure(Response was null)", output[0]);
    }

    [Fact]
    public async Task AsyncPipeline_ExceptionBecomesFailure()
    {
        var output = await RunAsync(
            "async-pipeline", new Dictionary<string, string> { ["value"] = "throw" });

        Assert.Equal("Failure(Simulated network failure)", output[0]);
    }

    // -- custom-errors ----------------------------------------------------------------------

    [Fact]
    public async Task CustomErrors_PatternMatchesOnErrorType()
    {
        var output = Joined(await RunAsync(
            "custom-errors", new Dictionary<string, string> { ["status"] = "503" }));

        Assert.Contains("HTTP 503: Upstream call failed", output);
    }

    [Fact]
    public async Task CustomErrors_UsesProvidedStatusCode()
    {
        var output = Joined(await RunAsync(
            "custom-errors", new Dictionary<string, string> { ["status"] = "404" }));

        Assert.Contains("HTTP 404: Upstream call failed", output);
    }

    // -- map-failure -------------------------------------------------------------------------

    [Fact]
    public async Task MapFailure_EnrichesErrorMessage()
    {
        var output = await RunAsync(
            "map-failure", new Dictionary<string, string> { ["error"] = "raw" });

        Assert.Equal("Failure([orders-service] raw)", output[0]);
    }
}
