using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class PagedList<T>
{
    [JsonPropertyName("count")] 
    public int Count { get; set; }
    
    [JsonPropertyName("results")] 
    public List<T> Results { get; set; }
}