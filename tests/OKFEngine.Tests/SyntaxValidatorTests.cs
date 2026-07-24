namespace OKFEngine.Tests;

public class SyntaxValidatorTests
{
    [Fact]
    public void AllRequiredFieldsMissing_ReturnsOneErrorPerField()
    {
        var fields = new Dictionary<string, string>();

        var errors = SyntaxValidator.Validate(fields, "some body", new List<string>());

        Assert.Contains(errors, e => e.Contains("'title'"));
        Assert.Contains(errors, e => e.Contains("'slug'"));
        Assert.Contains(errors, e => e.Contains("'type'"));
        Assert.Contains(errors, e => e.Contains("'version'"));
        Assert.Contains(errors, e => e.Contains("'created'"));
    }

    [Fact]
    public void RequiredFieldPresentButEmpty_ReturnsERR004()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "  ",
            ["slug"] = "valid-slug",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "body", new List<string>());

        Assert.Contains(errors, e => e.StartsWith("ERR004") && e.Contains("'title'"));
    }

    [Fact]
    public void ExistingErrorsFromCallerArePreserved()
    {
        var existing = new List<string> { "ERR001: Document does not begin with frontmatter block (---)" };

        var errors = SyntaxValidator.Validate(new Dictionary<string, string>(), "body", existing);

        Assert.Contains(errors, e => e.StartsWith("ERR001"));
    }

    [Fact]
    public void ValidFields_ProduceNoErrors()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "test-concept",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.Empty(errors);
    }

    [Fact]
    public void StatusMissing_ProducesNoWarning()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "test-concept",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.DoesNotContain(errors, e => e.StartsWith("WARN:"));
    }

    [Fact]
    public void ReservedSlugIndex_ReturnsERR012()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "index",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.Contains(errors, e => e.StartsWith("ERR012"));
    }

    [Fact]
    public void ReservedSlugLog_ReturnsERR012()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "log",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.Contains(errors, e => e.StartsWith("ERR012"));
    }

    [Fact]
    public void ReservedSlugCaseInsensitive_ReturnsERR012()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "Index",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.Contains(errors, e => e.StartsWith("ERR012"));
    }

    [Fact]
    public void NonReservedSlugSimilarName_DoesNotProduceERR012()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "indexer",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.DoesNotContain(errors, e => e.StartsWith("ERR012"));
    }

    [Fact]
    public void NonReservedSlugSimilarName2_DoesNotProduceERR012()
    {
        var fields = new Dictionary<string, string>
        {
            ["title"] = "Test",
            ["slug"] = "changelog",
            ["type"] = "concept",
            ["version"] = "1.0",
            ["created"] = "2026-07-24"
        };

        var errors = SyntaxValidator.Validate(fields, "some body content", new List<string>());

        Assert.DoesNotContain(errors, e => e.StartsWith("ERR012"));
    }
}
