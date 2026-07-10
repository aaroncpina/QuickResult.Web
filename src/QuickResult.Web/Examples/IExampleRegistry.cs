namespace QuickResult.Web.Examples;

/// <summary>Read-only lookup over the curated example catalog.</summary>
public interface IExampleRegistry
{
    IReadOnlyList<IExample> All { get; }

    /// <summary>Distinct categories in first-appearance order.</summary>
    IReadOnlyList<string> Categories { get; }

    IReadOnlyList<IExample> GetByCategory(string category);

    /// <summary>Finds an example by id — returns a QuickResult failure when unknown.</summary>
    Result<IExample> Find(string id);
}
