using System.Runtime.InteropServices;

namespace PokedexCli.Presentation.Console;

public class ConsoleService : IConsoleService
{
    private const int MinDividerWidth = 16;
    private const int DefaultSeparatorLength = 32;
    private readonly string _separator = new('-', DefaultSeparatorLength);
    
    private readonly TextWriter _out;
    private readonly bool _enableAnsi;

    public ConsoleService(TextWriter? @out = null, bool? enableAnsi = true)
    {
        _out = @out ?? System.Console.Out;
        _enableAnsi = enableAnsi ?? (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || !System.Console.IsOutputRedirected);
    }

    public void Banner()
    {
        _out.WriteLine("🕹️ Pokédex CLI — PokéAPI (.NET)");
        _out.WriteLine("Type 'help' for usage, 'exit' to quit.");
        _out.WriteLine();
    }

    public void Help()
    {
        _out.WriteLine("Usage:");
        _out.WriteLine("  find <name|id>               → Find a Pokémon by name or id");
        _out.WriteLine("  random                       → Fetch a random Pokémon");
        _out.WriteLine("  list [--limit N] [--offset N]→ List Pokémon with pagination");
        _out.WriteLine("  types                        → List all Pokémon types");
        _out.WriteLine("  help                         → Show this help");
        _out.WriteLine("  clear                        → Clear the screen");
        _out.WriteLine("  exit                         → Quit the app");
        _out.WriteLine();
    }

    public void Clear()
    {
        try
        {
            System.Console.Clear();
            Banner();
        }
        catch
        {
            _out.WriteLine(new string('-', 32));
        }
    }

    public void Prompt() => _out.Write("> ");
    
    public void Separator() => _out.WriteLine(_separator);
    public void Divider(char ch = '-')
    {
        var width = GetWidth();
        _out.WriteLine(new string(ch, Math.Max(MinDividerWidth, width)));
    }

    public void WriteTitle(string text)
    {
        if (_enableAnsi)
            _out.WriteLine($"\u001b[1m{text}\u001b[0m"); // bold
        else
            _out.WriteLine(text);
    }

    public int GetWidth()
    {
        try
        {
            return System.Console.WindowWidth > 0 ? System.Console.WindowWidth - 1 : DefaultSeparatorLength;
        }
        catch
        {
            return DefaultSeparatorLength;
        }
    }
    
    public void PrintInfo(string message) => WriteLine(message, ConsoleColor.Cyan, isError: false);
    public void PrintWarn(string message) => WriteLine("⚠️ " + message, ConsoleColor.Yellow, isError: false);
    public void PrintError(string message) => WriteLine("❌ " + message, ConsoleColor.Red, isError: true);
    
    private void WriteLine(string text, ConsoleColor color, bool isError)
    {
        if (_enableAnsi && !System.Console.IsOutputRedirected)
        {
            var ansi = color switch
            {
                ConsoleColor.Red => "\u001b[31m",
                ConsoleColor.Yellow => "\u001b[33m",
                ConsoleColor.Cyan => "\u001b[36m",
                _ => ""
            };
            var reset = string.IsNullOrEmpty(ansi) ? "" : "\u001b[0m";
            _out.WriteLine(string.IsNullOrEmpty(ansi) ? text : $"{ansi}{text}{reset}");
        }
        else
        {
            _out.WriteLine(text);
        }
    }
}