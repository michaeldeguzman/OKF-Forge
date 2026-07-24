using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OKFEngine;

internal static class FrontmatterParser
{
    internal static (Dictionary<string, string> Fields, string CleanBody, List<string> Errors)
        Parse(string markdownContent)
    {
        var errors = new List<string>();
        var content = markdownContent.TrimStart('﻿');

        if (!content.StartsWith("---", StringComparison.Ordinal))
        {
            errors.Add("ERR001: Document does not begin with frontmatter block (---)");
            return (new Dictionary<string, string>(), content, errors);
        }

        int firstDelimiterEnd = content.IndexOf('\n', 0);
        if (firstDelimiterEnd < 0)
        {
            errors.Add("ERR002: Frontmatter block is not closed");
            return (new Dictionary<string, string>(), content, errors);
        }

        int closingIndex = FindClosingDelimiter(content, firstDelimiterEnd + 1);
        if (closingIndex < 0)
        {
            errors.Add("ERR002: Frontmatter block is not closed");
            return (new Dictionary<string, string>(), content, errors);
        }

        string yamlString = content.Substring(firstDelimiterEnd + 1, closingIndex - (firstDelimiterEnd + 1));

        int bodyStart = content.IndexOf('\n', closingIndex);
        string cleanBody = bodyStart < 0 ? string.Empty : content.Substring(bodyStart + 1).TrimStart('\r', '\n', ' ', '\t');

        Dictionary<string, string> fields;
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build();
            var raw = deserializer.Deserialize<Dictionary<string, object>>(yamlString) ?? new Dictionary<string, object>();
            fields = Flatten(raw);
        }
        catch (Exception ex)
        {
            errors.Add($"ERR003: Frontmatter YAML is malformed — {ex.Message}");
            fields = new Dictionary<string, string>();
        }

        return (fields, cleanBody, errors);
    }

    private static int FindClosingDelimiter(string content, int searchStart)
    {
        int pos = searchStart;
        while (pos < content.Length)
        {
            int lineEnd = content.IndexOf('\n', pos);
            string line = lineEnd < 0 ? content.Substring(pos) : content.Substring(pos, lineEnd - pos);
            if (line.TrimEnd('\r') == "---")
            {
                return pos;
            }
            if (lineEnd < 0)
            {
                break;
            }
            pos = lineEnd + 1;
        }
        return -1;
    }

    private static Dictionary<string, string> Flatten(Dictionary<string, object> raw)
    {
        var result = new Dictionary<string, string>();
        foreach (var kvp in raw)
        {
            FlattenValue(kvp.Key.ToLowerInvariant(), kvp.Value, result);
        }
        return result;
    }

    private static void FlattenValue(string key, object? value, Dictionary<string, string> result)
    {
        switch (value)
        {
            case null:
                result[key] = string.Empty;
                break;
            case Dictionary<object, object> nested:
                foreach (var kvp in nested)
                {
                    FlattenValue($"{key}.{kvp.Key.ToString()?.ToLowerInvariant()}", kvp.Value, result);
                }
                break;
            case List<object> list:
                result[key] = string.Join(",", list.Select(item => item?.ToString() ?? string.Empty));
                break;
            default:
                result[key] = value.ToString() ?? string.Empty;
                break;
        }
    }
}
