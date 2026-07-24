namespace OKFEngine.Tests;

public class ParseDocumentTests
{
    private readonly global::OKFEngine.OKFEngine _engine = new();

    [Fact]
    public void ValidDocument_AllRequiredFields()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc());

        Assert.True(result.IsValid);
        Assert.Equal("", result.Errors);
    }

    [Fact]
    public void MissingTitle()
    {
        var doc = @"---
slug: test-concept
type: concept
version: 1.0
created: 2026-07-24
---

Body content.";

        var result = _engine.ParseDocument(doc);

        Assert.False(result.IsValid);
        Assert.Contains("title", result.Errors);
    }

    [Fact]
    public void MissingSlug()
    {
        var doc = @"---
title: Test Concept
type: concept
version: 1.0
created: 2026-07-24
---

Body content.";

        var result = _engine.ParseDocument(doc);

        Assert.False(result.IsValid);
        Assert.Contains("slug", result.Errors);
    }

    [Fact]
    public void InvalidSlugUppercase()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(slug: "My-Concept"));

        Assert.False(result.IsValid);
        Assert.Contains("ERR005", result.Errors);
    }

    [Fact]
    public void InvalidSlugSpaces()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(slug: "my concept"));

        Assert.False(result.IsValid);
        Assert.Contains("ERR005", result.Errors);
    }

    [Fact]
    public void InvalidType()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(type: "unknown"));

        Assert.False(result.IsValid);
        Assert.Contains("ERR006", result.Errors);
    }

    [Fact]
    public void InvalidVersionFormat()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(version: "1"));

        Assert.False(result.IsValid);
        Assert.Contains("ERR007", result.Errors);
    }

    [Fact]
    public void InvalidDateFormat()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(created: "24-07-2026"));

        Assert.False(result.IsValid);
        Assert.Contains("ERR008", result.Errors);
    }

    [Fact]
    public void EmptyBody()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(body: ""));

        Assert.False(result.IsValid);
        Assert.Contains("ERR011", result.Errors);
    }

    [Fact]
    public void OptionalFieldsMissing()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UnrecognisedStatus()
    {
        var result = _engine.ParseDocument(TestHelpers.ValidOKFDoc(extraFrontmatter: "status: archived"));

        Assert.True(result.IsValid);
        Assert.Contains("WARN:", result.Errors);
    }
}
