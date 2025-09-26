using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class Pokemon
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }
    
    [JsonPropertyName("name")] 
    public string Name { get; set; }
    
    [JsonPropertyName("height")] 
    public int Height { get; set; }
    
    [JsonPropertyName("weight")] 
    public int Weight { get; set; }
    
    [JsonPropertyName("sprites")]
    public Sprites? Sprites { get; set; }
        
    [JsonPropertyName("types")] 
    public PokemonTypeEntry[]? Types { get; set; }
        
    [JsonPropertyName("stats")] 
    public PokemonStat[]? Stats { get; set; }
}
