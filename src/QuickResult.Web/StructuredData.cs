namespace QuickResult.Web;

/// <summary>
/// schema.org descriptions (JSON-LD) that tell search engines what a page is about.
/// Kept as literal JSON so nothing depends on reflection-based serialization in the
/// trimmed WebAssembly build.
/// </summary>
public static class StructuredData
{
    /// <summary>The home page: the site itself and the library it documents.</summary>
    public const string Home =
        """
        {
          "@context": "https://schema.org",
          "@graph": [
            {
              "@type": "WebSite",
              "name": "QuickResult",
              "url": "https://quickresult.net/",
              "description": "Documentation and live examples for QuickResult, a Result type for C# with LINQ query syntax."
            },
            {
              "@type": "SoftwareSourceCode",
              "name": "QuickResult",
              "description": "Lightweight, zero-dependency Result<T> for C# and .NET with full LINQ query syntax for sync, async and mixed flows, and C# union support.",
              "url": "https://quickresult.net/",
              "codeRepository": "https://github.com/aaroncpina/QuickResult",
              "programmingLanguage": "C#",
              "runtimePlatform": ".NET",
              "license": "https://opensource.org/licenses/MIT",
              "downloadUrl": "https://www.nuget.org/packages/QuickResult",
              "author": { "@type": "Person", "name": "Aaron Pina" }
            }
          ]
        }
        """;

    /// <summary>The C# unions article.</summary>
    public const string UnionsArticle =
        """
        {
          "@context": "https://schema.org",
          "@type": "TechArticle",
          "headline": "C# union types, and how Result<T> became one",
          "description": "What C# 15 union types are, why a Result is the textbook union, and how QuickResult makes Result<T> a union of T and IError without changing its API.",
          "mainEntityOfPage": "https://quickresult.net/csharp-unions",
          "image": "https://quickresult.net/og-image.png",
          "datePublished": "2026-11-11",
          "dateModified": "2026-11-11",
          "proficiencyLevel": "Intermediate",
          "about": ["C#", "Union types", "Result pattern", "Error handling"],
          "author": { "@type": "Person", "name": "Aaron Pina" },
          "publisher": { "@type": "Organization", "name": "QuickResult", "url": "https://quickresult.net/" }
        }
        """;
}
