using Moq;
using PokedexCli.PokeApi.Models;
using PokedexCli.Presentation.Console;
using PokedexCli.Presentation.PokemonPrinter;

namespace PokedexCli.Test.Presentation;

public class PokemonPrinterTest
{
private readonly Mock<IConsoleService> _consoleMock;
    private readonly PokemonPrinter _printer;

    public PokemonPrinterTest()
    {
        _consoleMock = new Mock<IConsoleService>();
        _printer = new PokemonPrinter(_consoleMock.Object);
    }

    [Fact]
    public void PrintPokemon_NullPokemon_PrintsError()
    {
        _printer.PrintPokemon(null!);

        _consoleMock.Verify(c => c.PrintError(It.Is<string>(s => s.Contains("missing"))), Times.Once);
    }

    [Fact]
    public void PrintPokemon_ValidPokemon_PrintsInfo()
    {
        var pokemon = new Pokemon
        {
            Id = 25,
            Name = "pikachu",
            Types = [new PokemonTypeEntry { Slot = 1, Type = new NamedApiResource { Name = "electric" } }],
            Stats = [new PokemonStat { BaseStat = 55, Stat = new NamedApiResource { Name = "speed" } }],
            Height = 4,
            Weight = 60,
            Sprites = new Sprites { FrontDefault = "sprite-url" }
        };

        _printer.PrintPokemon(pokemon);

        _consoleMock.Verify(c => c.PrintInfo(It.Is<string>(s => s.Contains("pikachu", StringComparison.OrdinalIgnoreCase))), Times.AtLeastOnce);
        _consoleMock.Verify(c => c.PrintInfo(It.Is<string>(s => s.Contains("Types"))), Times.Once);
        _consoleMock.Verify(c => c.PrintInfo(It.Is<string>(s => s.Contains("Height"))), Times.Once);
        _consoleMock.Verify(c => c.PrintInfo(It.Is<string>(s => s.Contains("sprite-url"))), Times.Once);
    }
}