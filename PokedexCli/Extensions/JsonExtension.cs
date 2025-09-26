using System.Text.Json;

namespace PokedexCli.Extensions;

public static class JsonExtension
{
    public static JsonElement? GetPropertyOrNull(this JsonElement element, string name)
    {
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out var property) ? property : null;
    }
}