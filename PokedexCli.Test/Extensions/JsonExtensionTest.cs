using System.Text.Json;
using PokedexCli.Extensions;

namespace PokedexCli.Test.Extensions;

public class JsonExtensionTest
{
    [Fact]
    public void GetPropertyOrNull_ReturnsProperty_WhenExists()
    {
        var json = "{\"name\":\"pikachu\"}";
        var element = JsonDocument.Parse(json).RootElement;

        var result = element.GetPropertyOrNull("name");

        Assert.NotNull(result);
        Assert.Equal("pikachu", result?.GetString());
    }

    [Fact]
    public void GetPropertyOrNull_ReturnsNull_WhenPropertyDoesNotExist()
    {
        var json = "{\"name\":\"pikachu\"}";
        var element = JsonDocument.Parse(json).RootElement;

        var result = element.GetPropertyOrNull("type");

        Assert.Null(result);
    }

    [Fact]
    public void GetPropertyOrNull_ReturnsNull_WhenNotObject()
    {
        var json = "\"pikachu\"";
        var element = JsonDocument.Parse(json).RootElement;

        var result = element.GetPropertyOrNull("name");

        Assert.Null(result);
    }
}