namespace PokedexCli.Presentation.Console;

public static class ConsoleMessage
{
    public static string CanceledByUser => "👋 Cancelled by user. Exiting Pokédex.";
    public static string Exiting => "👋 Exiting Pokédex. Bye!";
    public static string FailedRandomPokemon => "Failed to fetch random Pokémon.";
    public static string InvalidCommand(string commandString) => $"Invalid command {commandString}. Type 'help' for usage.";
    public static string PokemonNotFound(string nameOrId) => $"Pokémon {nameOrId} not found.";
}