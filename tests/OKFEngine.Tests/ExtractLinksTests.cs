namespace OKFEngine.Tests;

public class ExtractLinksTests
{
    private readonly global::OKFEngine.OKFEngine _engine = new();

    [Fact]
    public void ExternalLinkIgnored()
    {
        var body = "See [external](https://example.com) for more.";

        var links = _engine.ExtractLinks(body, "my-doc").ToList();

        Assert.Empty(links);
    }

    [Fact]
    public void SelfLinkBlocked()
    {
        var body = "See [self](my-doc) for more.";

        var (links, errors) = GraphLinkExtractor.Extract(body, "my-doc");

        Assert.Empty(links);
        Assert.Contains(errors, e => e.StartsWith("ERR010"));
    }

    [Fact]
    public void EmptyAliasBlocked()
    {
        var body = "See [](some-slug) for more.";

        var (links, errors) = GraphLinkExtractor.Extract(body, "my-doc");

        Assert.Empty(links);
        Assert.Contains(errors, e => e.StartsWith("ERR009"));
    }

    [Fact]
    public void DuplicateLinkCollapsed()
    {
        var body = "First [alpha](same-slug) then [beta](same-slug) again.";

        var links = _engine.ExtractLinks(body, "my-doc").ToList();

        Assert.Single(links);
        Assert.Equal("alpha", links[0].Alias);
        Assert.Equal("same-slug", links[0].TargetSlug);
    }

    [Fact]
    public void MultipleLinksExtracted()
    {
        var body = "[one](slug-one) and [two](slug-two) and [three](slug-three).";

        var links = _engine.ExtractLinks(body, "my-doc").ToList();

        Assert.Equal(3, links.Count);
    }
}
