using System.Text;
using PokedexCli.Extensions;

namespace PokedexCli.Test.Extensions;

public class HttpExtensionTest
{
    private const string Uri = "https://pokeapi.co/api/v2/pokemon";
    [Fact]
    public void Clone_CopiesMethodAndUri()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Uri);
        var clone = request.Clone();

        Assert.Equal(request.Method, clone.Method);
        Assert.Equal(request.RequestUri, clone.RequestUri);
    }

    [Fact]
    public void Clone_CopiesHeaders()
    {
        var header = "X-Test-Header";
        var request = new HttpRequestMessage(HttpMethod.Get, Uri);
        request.Headers.Add(header, "value");
        var clone = request.Clone();

        Assert.True(clone.Headers.Contains(header));
        Assert.Equal("value", clone.Headers.GetValues(header).First());
    }

    [Fact]
    public void Clone_CopiesContentAndContentHeaders()
    {
        var header = "X-Content-Header";
        var content = new StringContent("pikachu", Encoding.UTF8, "text/plain");
        content.Headers.Add(header, "abc");
        var request = new HttpRequestMessage(HttpMethod.Put, Uri) { Content = content };

        var clone = request.Clone();

        Assert.NotNull(clone.Content);
        Assert.Equal("pikachu", clone.Content.ReadAsStringAsync().Result);
        Assert.True(clone.Content.Headers.Contains(header));
        Assert.Equal("abc", clone.Content.Headers.GetValues(header).First());
    }

    [Fact]
    public void Clone_ReturnsNewInstance()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Uri);
        var clone = request.Clone();

        Assert.NotSame(request, clone);
    }
}