using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class PokemonTypeEntry
{
    [JsonPropertyName("slot")] 
    public int Slot { get; set; }
    
    [JsonPropertyName("type")] 
    public NamedApiResource? Type { get; set; }
}
