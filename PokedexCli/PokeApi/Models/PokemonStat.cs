using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class PokemonStat
{
    [JsonPropertyName("base_stat")] 
    public int BaseStat { get; set; }
    
    [JsonPropertyName("effort")] 
    public int Effort { get; set; }
    
    [JsonPropertyName("stat")] 
    public NamedApiResource? Stat { get; set; }
}