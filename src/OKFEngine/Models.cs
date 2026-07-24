using OutSystems.ExternalLibraries.SDK;

namespace OKFEngine;

[OSStructure(Description = "Result of parsing and validating a raw OKF Markdown document against OKF Spec v1.0.")]
public struct OKFParseResult
{
    [OSStructureField(Description = "True when the document has no validation errors. Remains true if only WARN: entries are present in Errors.", IsMandatory = true)]
    public bool IsValid;

    [OSStructureField(Description = "Pipe-delimited (|) list of validation errors and warnings. Entries prefixed WARN: are warnings and do not affect IsValid; all other entries are hard errors. Empty string when the document is fully clean.", IsMandatory = true)]
    public string Errors;

    [OSStructureField(Description = "Value of the required 'title' frontmatter field. Empty string if missing.", IsMandatory = true)]
    public string Title;

    [OSStructureField(Description = "Value of the required 'slug' frontmatter field (lowercase, alphanumeric and hyphens only). Empty string if missing.", IsMandatory = true)]
    public string Slug;

    [OSStructureField(Description = "Value of the required 'type' frontmatter field. Must be one of: concept, process, reference, decision, guide, index. Empty string if missing.", IsMandatory = true)]
    public string Type;

    [OSStructureField(Description = "Value of the required 'version' frontmatter field in MAJOR.MINOR format (e.g. 1.0). Empty string if missing.", IsMandatory = true)]
    public string Version;

    [OSStructureField(Description = "Value of the required 'created' frontmatter field as an ISO 8601 date (YYYY-MM-DD). Empty string if missing.", IsMandatory = true)]
    public string Created;

    [OSStructureField(Description = "The Markdown body with the frontmatter block stripped and leading whitespace trimmed. Pass this into ExtractLinks.", IsMandatory = true)]
    public string CleanBody;
}

[OSStructure(Description = "A single OKF graph edge — an inline Markdown link from the current document to another OKF slug.")]
public struct OKFLinkRaw
{
    [OSStructureField(Description = "The visible link text, e.g. the 'alias' in [alias](slug).", IsMandatory = true)]
    public string Alias;

    [OSStructureField(Description = "The slug this link points to, e.g. the 'slug' in [alias](slug). External http(s) links are never returned here.", IsMandatory = true)]
    public string TargetSlug;
}

[OSStructure(Description = "A single frontmatter key-value pair extracted from an OKF document, used to populate the OKFMetadata entity.")]
public struct OKFMetadataRaw
{
    [OSStructureField(Description = "Frontmatter field name, lowercased. Nested YAML mappings are flattened to dot-notation (e.g. author.name).", IsMandatory = true)]
    public string Key;

    [OSStructureField(Description = "Frontmatter field value as a string. YAML lists are flattened to comma-separated values.", IsMandatory = true)]
    public string Value;
}
