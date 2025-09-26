using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class Sprites
{
    [JsonPropertyName("front_default")] 
    public string? FrontDefault { get; set; }
    
    [JsonPropertyName("other")] 
    public Dictionary<string, JsonElement>? Other { get; set; }
}
