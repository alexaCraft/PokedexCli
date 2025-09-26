namespace PokedexCli.Presentation.Console;

public interface IConsoleService
{
    public void Banner();
    public void Help();
    public void Clear();
    public void Prompt();

    public void Separator();
    public void Divider(char ch = '-');
    public void WriteTitle(string text);
    public int GetWidth();

    public void PrintInfo(string message);
    public void PrintWarn(string message);
    public void PrintError(string message);
}