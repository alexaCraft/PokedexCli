namespace PokedexCli.PokeApi;

public static class PokemonEndpoint
{
    public static string PokemonByIdOrName(string idOrName) => $"pokemon/{idOrName}"; 
    public static string PokemonList(int limit, int offset) => $"pokemon?limit={limit}&offset={offset}"; 
    public static string PokemonType => "type?limit=1000"; 
}