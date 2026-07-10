namespace QuickResult.Web.Examples;

/// <summary>Immutable bag of input values passed to <see cref="IExample.RunAsync"/>.</summary>
public sealed class ExampleArguments
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public static ExampleArguments Empty { get; } = new(new Dictionary<string, string>());

    public ExampleArguments(IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = values;
    }

    /// <summary>Builds arguments from each input's declared default value.</summary>
    public static ExampleArguments ForDefaults(IEnumerable<ExampleInput> inputs) =>
        new(inputs.ToDictionary(i => i.Name, i => i.DefaultValue));

    /// <summary>Returns the value for <paramref name="name"/>, or an empty string when absent.</summary>
    public string Get(string name) =>
        _values.TryGetValue(name, out var value) ? value : string.Empty;
}
