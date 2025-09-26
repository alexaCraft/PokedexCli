using PokedexCli.Extensions;
using PokedexCli.PokeApi.Models;
using PokedexCli.Presentation.Console;

namespace PokedexCli.Presentation.PokemonPrinter;

public class PokemonPrinter : IPokemonPrinter
{
    private const int Columns = 4;
    private const int Width = 16;
    
    private readonly IConsoleService _consoleService;

    public PokemonPrinter(IConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    public void PrintPokemon(Pokemon pokemon)
    {
        if (pokemon == null)
        {
            _consoleService.PrintError("Pokémon data is missing.");
            return;
        }

        _consoleService.PrintInfo(string.Empty);
        _consoleService.PrintInfo($"#{pokemon.Id} — {pokemon.Name?.Capitalize() ?? "Unknown"}");
        _consoleService.Separator();

        var types = pokemon.Types?
            .OrderBy(t => t.Slot)
            .Select(t => (t.Type?.Name ?? "").Capitalize())
            .ToArray() ?? [];
        _consoleService.PrintInfo($"Types: {string.Join(", ", types)}");

        if (pokemon.Stats is { Length: > 0 })
        {
            _consoleService.PrintInfo("Stats base:");
            foreach (var s in pokemon.Stats.OrderBy(s => s.Stat?.Name))
            {
                _consoleService.PrintInfo($"  {(s.Stat?.Name ?? "?").Capitalize(),-10}: {s.BaseStat,3}");
            }
        }

        _consoleService.PrintInfo($"Height: {pokemon.Height / 10.0:0.0} m");
        _consoleService.PrintInfo($"Weight:   {pokemon.Weight / 10.0:0.0} kg");

        var sprite = pokemon.Sprites?.Other?["official-artwork"]
                         .GetPropertyOrNull("front_default")?.GetString()
                     ?? pokemon.Sprites?.FrontDefault;
        if (!string.IsNullOrWhiteSpace(sprite))
        {
            _consoleService.PrintInfo($"Sprite: {sprite}");
        }

        _consoleService.PrintInfo("");
    }

    public void PrintPokemonList(int total, int limit, int offset, IReadOnlyList<NamedApiResource> items)
    {
        _consoleService.PrintInfo("");
        _consoleService.PrintInfo($"Pokédex — showing {items.Count} of ~{total} (limit={limit}, offset={offset})");
        _consoleService.Divider();

        if (items.Count == 0)
        {
            _consoleService.PrintInfo("No results.");
            _consoleService.PrintInfo("");
            return;
        }

        for (var i = 0; i < items.Count; i++)
        {
            var name = items[i].Name.Capitalize();
            _consoleService.PrintInfo($"{i + 1 + offset,4}. {name}");
        }

        _consoleService.PrintInfo("");
        
        var hasNext = offset + limit < total;
        var hasPrev = offset > 0;
        if (hasPrev || hasNext)
        {
            var prev = hasPrev ? $"list --limit {limit} --offset {Math.Max(0, offset - limit)}" : null;
            var next = hasNext ? $"list --limit {limit} --offset {offset + limit}" : null;
            _consoleService.PrintInfo("Tip:");
            if (prev is not null) _consoleService.PrintInfo($"  prev: {prev}");
            if (next is not null) _consoleService.PrintInfo($"  next: {next}");
            _consoleService.PrintInfo("");
        }
    }

    public void PrintTypes(IReadOnlyList<string> types)
    {
        _consoleService.PrintInfo("");
        _consoleService.WriteTitle("Types");
        _consoleService.Divider();

        if (types.Count == 0)
        {
            _consoleService.PrintInfo("No types found.");
            _consoleService.PrintInfo("");
            return;
        }
        
        for (var i = 0; i < types.Count; i++)
        {
            var cell = types[i].Capitalize().PadRight(Width);
            _consoleService.PrintInfo(cell);
            if ((i + 1) % Columns == 0) _consoleService.PrintInfo("");
        }
        if (types.Count % Columns != 0) _consoleService.PrintInfo("");
        _consoleService.PrintInfo("");
    }
}