using PokedexCli.PokeApi.Models;

namespace PokedexCli.Presentation.PokemonPrinter;

public interface IPokemonPrinter
{
    public void PrintPokemon(Pokemon pokemon);
    public void PrintPokemonList(int total, int limit, int offset, IReadOnlyList<NamedApiResource> items);
    public void PrintTypes(IReadOnlyList<string> types);
}