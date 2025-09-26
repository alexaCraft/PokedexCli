using Moq;
using PokedexCli.Presentation.Console;

namespace PokedexCli.Test.Presentation;

public class ConsoleServiceTest
{
    private readonly Mock<TextWriter> _writerMock;
    private readonly ConsoleService _consoleService;

    public ConsoleServiceTest()
    {
        _writerMock = new Mock<TextWriter>();
        _consoleService = new ConsoleService(_writerMock.Object, enableAnsi: false);
    }

    [Fact]
    public void Banner_WritesBannerText()
    {
        _consoleService.Banner();

        _writerMock.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Pokédex CLI"))), Times.Once);
        _writerMock.Verify(w => w.WriteLine(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void Help_WritesHelpText()
    {
        _consoleService.Help();

        _writerMock.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Usage"))), Times.Once);
        _writerMock.Verify(w => w.WriteLine(It.IsAny<string>()), Times.AtLeast(5));
    }

    [Fact]
    public void Prompt_WritesPromptSymbol()
    {
        _consoleService.Prompt();

        _writerMock.Verify(w => w.Write("> "), Times.Once);
    }

    [Fact]
    public void Separator_WritesSeparator()
    {
        _consoleService.Separator();

        _writerMock.Verify(w => w.WriteLine(It.Is<string>(s => s.All(c => c == '-'))), Times.Once);
    }

    [Fact]
    public void WriteTitle_WritesTitle()
    {
        _consoleService.WriteTitle("Test Title");

        _writerMock.Verify(w => w.WriteLine("Test Title"), Times.Once);
    }

    [Fact]
    public void PrintInfo_WritesCyanText()
    {
        _consoleService.PrintInfo("info message");

        _writerMock.Verify(w => w.WriteLine("info message"), Times.Once);
    }

    [Fact]
    public void PrintWarn_WritesWarnText()
    {
        _consoleService.PrintWarn("warn message");

        _writerMock.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("⚠️"))), Times.Once);
    }

    [Fact]
    public void PrintError_WritesErrorText()
    {
        _consoleService.PrintError("error message");

        _writerMock.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("❌"))), Times.Once);
    }
}