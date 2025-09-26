using PokedexCli.PokeApi.Models;

namespace PokedexCli.Services;

public interface IPokemonService
{
    /// <summary>
    /// Get Pokémon by name or id
    /// </summary>
    public Task<Pokemon?> GetPokemonAsync(string nameOrId, CancellationToken ct = default);
    
    /// <summary>
    /// Get random Pokemon
    /// </summary>
    public Task<Pokemon?> GetRandomPokemonAsync(CancellationToken ct = default);
    public Task<(int total, IReadOnlyList<NamedApiResource> items)> PokemonListAsync(int limit = 20, int offset = 0, CancellationToken ct = default);

    public Task<IReadOnlyList<string>> GetPokemonTypesAsync(CancellationToken ct = default);
}