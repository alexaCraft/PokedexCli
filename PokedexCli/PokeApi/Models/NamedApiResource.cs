using System.Text.Json.Serialization;

namespace PokedexCli.PokeApi.Models;

public class NamedApiResource
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("url")] 
    public string Url { get; set; }
}