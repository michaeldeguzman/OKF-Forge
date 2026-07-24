namespace OKFEngine.Tests;

internal static class TestHelpers
{
    internal static string ValidOKFDoc(
        string title = "Test Concept",
        string slug = "test-concept",
        string type = "concept",
        string version = "1.0",
        string created = "2026-07-24",
        string body = "This is the body of the concept.",
        string extraFrontmatter = "")
    {
        return $@"---
title: {title}
slug: {slug}
type: {type}
version: {version}
created: {created}
{extraFrontmatter}
---

{body}";
    }
}
