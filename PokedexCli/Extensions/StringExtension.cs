namespace PokedexCli.Extensions;

public static class StringExtension
{
    public static string Capitalize(this string s) => string.IsNullOrWhiteSpace(s) 
        ? string.Empty 
        : char.ToUpperInvariant(s[0]) + s[1..];
}