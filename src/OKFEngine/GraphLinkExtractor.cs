using System.Text.RegularExpressions;

namespace OKFEngine;

internal static class GraphLinkExtractor
{
    private static readonly Regex LinkPattern = new Regex(
        @"\[([^\]]*)\]\(([^)]+)\)",
        RegexOptions.Compiled
    );

    internal static (List<OKFLinkRaw> Links, List<string> Errors)
        Extract(string markdownBody, string documentSlug)
    {
        var links = new List<OKFLinkRaw>();
        var errors = new List<string>();
        var seenTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string trimmedSlug = documentSlug?.Trim() ?? string.Empty;

        foreach (Match match in LinkPattern.Matches(markdownBody ?? string.Empty))
        {
            string alias = match.Groups[1].Value;
            string target = match.Groups[2].Value;
            string trimmedTarget = target.Trim();

            if (trimmedTarget.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                trimmedTarget.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(alias))
            {
                errors.Add($"ERR009: Link with empty alias found targeting '{target}'");
                continue;
            }

            if (trimmedTarget.Equals(trimmedSlug, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"ERR010: Self-link detected — document slug '{documentSlug}' cannot link to itself");
                continue;
            }

            if (!seenTargets.Add(trimmedTarget))
            {
                errors.Add($"WARN: Duplicate link target '{target}' — collapsed to first occurrence");
                continue;
            }

            links.Add(new OKFLinkRaw { Alias = alias.Trim(), TargetSlug = trimmedTarget });
        }

        return (links, errors);
    }
}
