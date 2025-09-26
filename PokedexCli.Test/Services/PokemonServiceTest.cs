using System.Net;
using Moq;
using Moq.Protected;
using PokedexCli.Services;

namespace PokedexCli.Test.Services;

public class PokemonServiceTest
{
    [Fact]
    public async Task GetPokemonAsync_ReturnsPokemon_WhenFound()
    {
        // Arrange
        var handlerMock = CreateHandlerMock(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":25,\"name\":\"pikachu\"}")
        });
        var service = CreatePokemonService(handlerMock);
        
        // Act
        var result = await service.GetPokemonAsync("pikachu", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.Id);
        Assert.Equal("pikachu", result.Name);
    }

    [Fact]
    public async Task GetPokemonAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var handlerMock = CreateHandlerMock(new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = CreatePokemonService(handlerMock);
        // Act
        var result = await service.GetPokemonAsync("unknown", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task PokemonListAsync_ReturnsItems_WhenApiReturnsResults()
    {
        // Arrange
        var responseContent = "{\"count\":2,\"results\":[{\"name\":\"bulbasaur\"},{\"name\":\"ivysaur\"}]}";
        var handlerMock = CreateHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        });
        var service = CreatePokemonService(handlerMock);

        // Act
        var (total, items) = await service.PokemonListAsync(2, 0);

        // Assert
        Assert.Equal(2, total);
        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.Name == "bulbasaur");
        Assert.Contains(items, i => i.Name == "ivysaur");
    }

    [Fact]
    public async Task PokemonListAsync_ReturnsEmpty_WhenApiReturnsEmptyResults()
    {
        // Arrange
        var responseContent = "{\"count\":0,\"results\":[]}";
        var handlerMock = CreateHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        });
        var service = CreatePokemonService(handlerMock);

        // Act
        var (total, items) = await service.PokemonListAsync(2, 0);

        // Assert
        Assert.Equal(0, total);
        Assert.Empty(items);
    }

    [Fact]
    public async Task GetPokemonTypesAsync_ReturnsTypes_WhenApiReturnsResults()
    {
        // Arrange
        var responseContent = "{\"count\":2,\"results\":[{\"name\":\"fire\"},{\"name\":\"water\"}]}";
        var handlerMock = CreateHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        });
        var service = CreatePokemonService(handlerMock);
        
        // Act
        var types = await service.GetPokemonTypesAsync();

        // Assert
        Assert.Equal(2, types.Count);
        Assert.Contains("fire", types);
        Assert.Contains("water", types);
    }

    [Fact]
    public async Task GetPokemonTypesAsync_ReturnsEmpty_WhenApiReturnsEmptyResults()
    {
        // Arrange
        var responseContent = "{\"count\":0,\"results\":[]}";
        var handlerMock = CreateHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        });
        var service = CreatePokemonService(handlerMock);
        
        // Act
        var types = await service.GetPokemonTypesAsync();

        // Assert
        Assert.Empty(types);
    }
    
    private Mock<HttpMessageHandler> CreateHandlerMock(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return handlerMock;
    }
    
    private PokemonService CreatePokemonService(Mock<HttpMessageHandler> handlerMock)
    {
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2/")
        };
        
        return new PokemonService(httpClient);
    }
}