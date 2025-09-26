using PokedexCli.Constants;
using PokedexCli.Presentation.Console;
using PokedexCli.Presentation.PokemonPrinter;
using PokedexCli.Services;

namespace PokedexCli.App;

/// <summary>
/// Main application class for the Pokédex CLI.
/// Handles command parsing and user interaction.
/// </summary>
public class PokedexCliApp
{
    private const int DefaultLimitList = 20;
    private const int DefaultOffsetList = 0;

    private readonly IPokemonService _pokemonService;
    private readonly IPokemonPrinter _pokemonPrinter;
    private readonly IConsoleService _consoleService;

    public PokedexCliApp(IPokemonService pokemonService, IPokemonPrinter pokemonPrinter, IConsoleService consoleService)
    {
        _pokemonService = pokemonService;
        _pokemonPrinter = pokemonPrinter;
        _consoleService = consoleService;
    }
    
    /// <summary>
    /// Runs the application once with the specified Pokémon name or ID.
    /// </summary>
    /// <param name="nameOrId">The Pokémon name or ID.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>0 if successful, 2 if Pokémon not found.</returns>
    public async Task<int> RunOnceAsync(string nameOrId, CancellationToken ct)
    {
        var pokemon = await _pokemonService.GetPokemonAsync(nameOrId, ct);
        if (pokemon is null)
        {
            _consoleService.PrintError(ConsoleMessage.PokemonNotFound(nameOrId));
            return 2;
        }

        _pokemonPrinter.PrintPokemon(pokemon);
        return 0;
    }

    /// <summary>
    /// Runs the application in interactive mode.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>0 when exiting.</returns>
    public async Task<int> RunInteractiveAsync(CancellationToken ct)
    {
        _consoleService.Banner();

        while (!ct.IsCancellationRequested)
        {
            _consoleService.Prompt();
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var commandStr = parts[0];
            if (!Enum.TryParse(commandStr, ignoreCase: true, out Command command))
            {
                _consoleService.PrintWarn(ConsoleMessage.InvalidCommand(commandStr));
                continue;
            }

            switch (command)
            {
                case Command.exit:
                case Command.quit:
                    _consoleService.PrintInfo(ConsoleMessage.Exiting);
                    return 0;
                
                case Command.clear:
                    _consoleService.Clear();
                    break;

                case Command.help:
                    _consoleService.Help();
                    break;

                case Command.find when parts.Length == 2:
                {
                    await HandleFindAsync(parts[1], ct);
                    break;
                }

                case Command.list:
                {
                    await HandleListAsync(parts.Length == 2 ? parts[1] : null, ct);
                    break;
                }

                case Command.random:
                {
                    await HandleRandomAsync(ct);
                    break;
                }
                
                case Command.types:
                {
                    await HandleTypesAsync(ct);
                    break;
                }

                default:
                    _consoleService.PrintWarn(ConsoleMessage.InvalidCommand(commandStr));
                    break;
            }
        }

        _consoleService.PrintInfo(ConsoleMessage.CanceledByUser);
        return 0;
    }
    
    private async Task HandleFindAsync(string nameOrId, CancellationToken ct)
    {
        var pokemon = await _pokemonService.GetPokemonAsync(nameOrId, ct);
        if (pokemon is null)
        {
            _consoleService.PrintError(ConsoleMessage.PokemonNotFound(nameOrId));
        }
        else
        {
            _pokemonPrinter.PrintPokemon(pokemon);
        }
    }

    private async Task HandleRandomAsync(CancellationToken ct)
    {
        var pokemon = await _pokemonService.GetRandomPokemonAsync(ct);
        if (pokemon is null)
            _consoleService.PrintError(ConsoleMessage.FailedRandomPokemon);
        else
            _pokemonPrinter.PrintPokemon(pokemon);
    }
    
    private async Task HandleListAsync(string? args, CancellationToken ct)
    {
        var limit = DefaultLimitList;
        var offset = DefaultOffsetList;

        if (!string.IsNullOrWhiteSpace(args))
        {
            var argParts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < argParts.Length; i++)
            {
                var part = argParts[i].ToLowerInvariant();
                if (part == Argument.Limit && i + 1 < argParts.Length && int.TryParse(argParts[i + 1], out var l) && l > 0)
                {
                    limit = l;
                    i++;
                }
                else if (part == Argument.Offset && i + 1 < argParts.Length && int.TryParse(argParts[i + 1], out var o) && o >= 0)
                {
                    offset = o;
                    i++;
                }
            }
        }

        var (total, items) = await _pokemonService.PokemonListAsync(limit, offset, ct);
        _pokemonPrinter.PrintPokemonList(total, limit, offset, items);
    }
    
    private async Task HandleTypesAsync(CancellationToken ct)
    {
        var types = await _pokemonService.GetPokemonTypesAsync(ct);
        _pokemonPrinter.PrintTypes(types);
    }
}
