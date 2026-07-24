namespace OKFEngine.Tests;

public class ExtractMetadataTests
{
    private readonly global::OKFEngine.OKFEngine _engine = new();

    [Fact]
    public void TagsParsedAsCommaList()
    {
        var doc = TestHelpers.ValidOKFDoc(extraFrontmatter: "tags: [rag, retrieval]");

        var metadata = _engine.ExtractMetadata(doc).ToList();

        var tags = metadata.Single(m => m.Key == "tags");
        Assert.Equal("rag,retrieval", tags.Value);
    }

    [Fact]
    public void NestedYamlFlattened()
    {
        var doc = TestHelpers.ValidOKFDoc(extraFrontmatter: "author:\n  name: Mike");

        var metadata = _engine.ExtractMetadata(doc).ToList();

        var authorName = metadata.Single(m => m.Key == "author.name");
        Assert.Equal("Mike", authorName.Value);
    }

    [Fact]
    public void MetadataRowsIncludeRequired()
    {
        var doc = TestHelpers.ValidOKFDoc();

        var metadata = _engine.ExtractMetadata(doc).ToList();
        var keys = metadata.Select(m => m.Key).ToList();

        Assert.Contains("title", keys);
        Assert.Contains("slug", keys);
        Assert.Contains("type", keys);
        Assert.Contains("version", keys);
        Assert.Contains("created", keys);
    }
}
