using System.Globalization;
using System.Text.RegularExpressions;

namespace OKFEngine;

internal static class SyntaxValidator
{
    private static readonly HashSet<string> AllowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "concept", "process", "reference", "decision", "guide", "index"
    };

    private static readonly HashSet<string> AllowedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "draft", "review", "published", "deprecated"
    };

    private static readonly HashSet<string> ReservedSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "index", "log"
    };

    private static readonly Regex SlugPattern = new Regex(
        @"^([a-z0-9]|[a-z0-9][a-z0-9\-]*[a-z0-9])$",
        RegexOptions.Compiled
    );

    private static readonly Regex VersionPattern = new Regex(
        @"^\d+\.\d+$",
        RegexOptions.Compiled
    );

    private static readonly string[] RequiredFields = { "title", "slug", "type", "version", "created" };

    internal static List<string> Validate(
        Dictionary<string, string> fields,
        string cleanBody,
        List<string> existingErrors)
    {
        var errors = existingErrors;
        var passed = new HashSet<string>();

        foreach (var field in RequiredFields)
        {
            if (!fields.TryGetValue(field, out var value))
            {
                errors.Add($"ERR003: Required field '{field}' is missing");
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"ERR004: Required field '{field}' is present but empty");
            }
            else
            {
                passed.Add(field);
            }
        }

        if (passed.Contains("slug") && !SlugPattern.IsMatch(fields["slug"]))
        {
            errors.Add($"ERR005: Slug '{fields["slug"]}' is invalid — use lowercase alphanumeric and hyphens only");
        }

        if (passed.Contains("slug") && ReservedSlugs.Contains(fields["slug"].Trim()))
        {
            errors.Add($"ERR012: Slug '{fields["slug"]}' is reserved for system-generated content (index, log) and cannot be used by an authored document");
        }

        if (passed.Contains("type") && !AllowedTypes.Contains(fields["type"]))
        {
            errors.Add($"ERR006: Type '{fields["type"]}' is not in the allowed vocabulary (concept, process, reference, decision, guide, index)");
        }

        if (passed.Contains("version") && !VersionPattern.IsMatch(fields["version"]))
        {
            errors.Add($"ERR007: Version '{fields["version"]}' must be in MAJOR.MINOR format (e.g. 1.0)");
        }

        if (passed.Contains("created") &&
            !DateTime.TryParseExact(fields["created"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            errors.Add($"ERR008: Created date '{fields["created"]}' must be in YYYY-MM-DD format");
        }

        if (string.IsNullOrWhiteSpace(cleanBody))
        {
            errors.Add("ERR011: Document body is empty — a valid OKF document must have content after the frontmatter block");
        }

        if (fields.TryGetValue("status", out var status) &&
            !string.IsNullOrWhiteSpace(status) &&
            !AllowedStatuses.Contains(status))
        {
            errors.Add($"WARN: Status value '{status}' is not recognised — expected draft, review, published, or deprecated");
        }

        return errors;
    }
}
