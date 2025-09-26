using PokedexCli.Extensions;

namespace PokedexCli.Test.Extensions;

public class StringExtensionTest
{
    [Fact]
    public void Capitalize_ReturnsEmpty_WhenNull()
    {
        string? input = null;
        Assert.Equal(string.Empty, input.Capitalize());
    }

    [Fact]
    public void Capitalize_ReturnsEmpty_WhenEmpty()
    {
        Assert.Equal(string.Empty, "".Capitalize());
    }

    [Fact]
    public void Capitalize_ReturnsEmpty_WhenWhitespace()
    {
        Assert.Equal(string.Empty, "   ".Capitalize());
    }

    [Fact]
    public void Capitalize_UppercasesFirstLetter()
    {
        Assert.Equal("Hello", "hello".Capitalize());
        Assert.Equal("Hello", "Hello".Capitalize());
        Assert.Equal("A", "a".Capitalize());
        Assert.Equal("Abc", "abc".Capitalize());
    }
}