using Moq;
using PokedexCli.App;
using PokedexCli.PokeApi.Models;
using PokedexCli.Presentation;
using PokedexCli.Presentation.Console;
using PokedexCli.Presentation.PokemonPrinter;
using PokedexCli.Services;

namespace PokedexCli.Test.App;

public class PokedexCliAppTest
{
    private readonly Mock<IPokemonService> _mockPokemonService;
    private readonly Mock<IPokemonPrinter> _mockPokemonPrinter;
    private readonly Mock<IConsoleService> _mockConsoleService;

    public PokedexCliAppTest()
    {
        _mockPokemonService = new Mock<IPokemonService>();
        _mockPokemonPrinter = new Mock<IPokemonPrinter>();
        _mockConsoleService = new Mock<IConsoleService>();
    }
    
    [Fact]
    public async Task RunOnceAsync_PrintsPokemon_WhenFound()
    {
        // Arrange
        var pokemonName = "bulbasaur";
        var pokemon = new Pokemon 
        {
            Id = 1, 
            Name= pokemonName 
        };
        
        _mockPokemonService.Setup(s => s.GetPokemonAsync(pokemonName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pokemon);

        var app = new PokedexCliApp(_mockPokemonService.Object, _mockPokemonPrinter.Object, _mockConsoleService.Object);

        // Act
        var result = await app.RunOnceAsync(pokemonName, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
        _mockPokemonPrinter.Verify(p => p.PrintPokemon(pokemon), Times.Once);
    }

    [Fact]
    public async Task RunOnceAsync_PrintsError_WhenNotFound()
    {
        // Arrange
        var pokemonName = "missingno";
        _mockPokemonService.Setup(s => s.GetPokemonAsync(pokemonName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pokemon)null);

        var app = new PokedexCliApp(_mockPokemonService.Object, _mockPokemonPrinter.Object, _mockConsoleService.Object);

        // Act
        var result = await app.RunOnceAsync(pokemonName, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
        _mockConsoleService.Verify(p => p.PrintError(It.IsAny<string>()), Times.Once);
    }
}