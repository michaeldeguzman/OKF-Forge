using OutSystems.ExternalLibraries.SDK;

namespace OKFEngine;

[OSInterface(
    Name = "OKFEngine",
    Description = "Stateless parser and validator for OKF (Open Knowledge Format) Markdown documents. OKF is a Markdown-based authoring convention for AI-navigable knowledge graphs, where each document is a node, YAML frontmatter is metadata, and inline Markdown links to other OKF slugs are graph edges. This library takes a raw Markdown string, validates it against OKF Spec v1.0, and returns structured data (frontmatter fields, graph edges, metadata rows) for the calling ODC app to persist into its own entities. It performs no entity reads, no HTTP calls, and holds no state between invocations.",
    IconResourceName = "OKFEngine.resources.okfengine-icon.png"
)]
public interface IOKFEngine
{
    [OSAction(
        Description = "Validates a raw OKF Markdown document against OKF Spec v1.0 and extracts its frontmatter. Checks that the document opens with a closed YAML frontmatter block, that all five required fields (title, slug, type, version, created) are present and well-formed, and that the Markdown body is non-empty. Does not extract graph links or optional frontmatter fields — call ExtractLinks with the returned CleanBody for links, and ExtractMetadata with the original markdownContent for the full set of frontmatter key-value pairs (including optional fields like tags, owner, status).",
        ReturnName = "ParseResult",
        ReturnDescription = "Validation outcome (IsValid, pipe-delimited Errors), the five required frontmatter fields (Title, Slug, Type, Version, Created), and CleanBody — the Markdown body with the frontmatter block removed."
    )]
    OKFParseResult ParseDocument(
        [OSParameter(Description = "The complete, unmodified OKF document as a single string: the '---' delimited YAML frontmatter block followed by the Markdown body. Pass the raw file contents exactly as authored — do not pre-strip the frontmatter.")]
        string markdownContent
    );

    [OSAction(
        Description = "Scans a Markdown body for inline [alias](target) links and returns the valid OKF graph edges among them. A link is excluded from the result (and silently skipped, with no effect on any other action's IsValid) when its target is an external http:// or https:// URL, when its alias is empty, or when its target is a self-link matching documentSlug. When the same target slug appears more than once, only the first occurrence is kept.",
        ReturnName = "Links",
        ReturnDescription = "One row per unique, valid graph edge found in the body. Each row has Alias (the link text) and TargetSlug (the linked document's slug). Empty if no valid links are found."
    )]
    IEnumerable<OKFLinkRaw> ExtractLinks(
        [OSParameter(Description = "The Markdown body to scan for links, with the YAML frontmatter block already removed — pass the CleanBody value returned by ParseDocument for this same document.")]
        string markdownBody,
        [OSParameter(Description = "The slug of the document being scanned (its own 'slug' frontmatter value). Any link whose target matches this slug is treated as an invalid self-link and excluded from the result.")]
        string documentSlug
    );

    [OSAction(
        Description = "Re-parses the full OKF document's YAML frontmatter and flattens it into one key-value row per field, covering both required fields (title, slug, type, version, created) and any optional fields present (e.g. tags, owner, status, supersedes, related). YAML list values are flattened to comma-separated strings; nested YAML mappings are flattened to dot-notation keys (e.g. author.name). Use this action to populate a metadata entity with every frontmatter field found, not just the required ones returned by ParseDocument.",
        ReturnName = "Metadata",
        ReturnDescription = "One row per frontmatter field found in the document. Each row has Key (the lowercased field name, dot-notation for nested fields) and Value (the field's flattened string value)."
    )]
    IEnumerable<OKFMetadataRaw> ExtractMetadata(
        [OSParameter(Description = "The complete, unmodified OKF document as a single string, including its YAML frontmatter block — the same input you would pass to ParseDocument.")]
        string markdownContent
    );
}

public class OKFEngine : IOKFEngine
{
    public OKFEngine() { }

    public OKFParseResult ParseDocument(string markdownContent)
    {
        var (fields, cleanBody, parseErrors) = FrontmatterParser.Parse(markdownContent);
        var allErrors = SyntaxValidator.Validate(fields, cleanBody, parseErrors);
        bool isValid = !allErrors.Any(e => !e.StartsWith("WARN:", StringComparison.Ordinal));

        return new OKFParseResult
        {
            IsValid = isValid,
            Errors = string.Join("|", allErrors),
            Title = fields.GetValueOrDefault("title", ""),
            Slug = fields.GetValueOrDefault("slug", ""),
            Type = fields.GetValueOrDefault("type", ""),
            Version = fields.GetValueOrDefault("version", ""),
            Created = fields.GetValueOrDefault("created", ""),
            CleanBody = cleanBody ?? ""
        };
    }

    public IEnumerable<OKFLinkRaw> ExtractLinks(string markdownBody, string documentSlug)
    {
        var (links, _) = GraphLinkExtractor.Extract(markdownBody, documentSlug);
        return links;
    }

    public IEnumerable<OKFMetadataRaw> ExtractMetadata(string markdownContent)
    {
        var (fields, _, _) = FrontmatterParser.Parse(markdownContent);
        return fields.Select(kvp => new OKFMetadataRaw { Key = kvp.Key, Value = kvp.Value });
    }
}
