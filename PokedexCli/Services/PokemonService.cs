using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using PokedexCli.Extensions;
using PokedexCli.PokeApi;
using PokedexCli.PokeApi.Models;

namespace PokedexCli.Services;

public class PokemonService : IPokemonService
{
    private const int MaxId = 1025;
    private const int MaxAttempts = 3;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    
    public PokemonService(HttpClient httpClient)
    { 
        _httpClient = httpClient;
    }
    
    public async Task<Pokemon?> GetPokemonAsync(string nameOrId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(nameOrId))
        {
            return null;
        }
        
        var key = nameOrId.Trim().ToLowerInvariant();
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, PokemonEndpoint.PokemonByIdOrName(key));

        using var responseMessage = await SendWithRetriesAsync(requestMessage, ct);
        if (responseMessage is null)
        {
            return null;
        }

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        responseMessage.EnsureSuccessStatusCode();

        await using var stream = await responseMessage.Content.ReadAsStreamAsync(ct);
        var pokemon = await JsonSerializer.DeserializeAsync<Pokemon>(stream, _jsonOptions, ct);
        return pokemon;
    }
    
    public async Task<Pokemon?> GetRandomPokemonAsync(CancellationToken ct = default)
    {
        var id = Random.Shared.Next(1, MaxId + 1).ToString();
        return await GetPokemonAsync(id, ct);
    }

    public async Task<(int total, IReadOnlyList<NamedApiResource> items)> PokemonListAsync(int limit = 20, int offset = 0, CancellationToken ct = default)
    {
        limit = Math.Clamp(limit, 1, 200); 
        offset = Math.Max(0, offset);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, PokemonEndpoint.PokemonList(limit, offset));
        using var responseMessage = await SendWithRetriesAsync(requestMessage, ct);
        if (responseMessage is null) return (0, []);

        responseMessage.EnsureSuccessStatusCode();
        await using var stream = await responseMessage.Content.ReadAsStreamAsync(ct);
        var page = await JsonSerializer.DeserializeAsync<PagedList<NamedApiResource>>(stream, _jsonOptions, ct);
        
        return page is null 
            ? (0, []) 
            : (page.Count, page.Results);
    }
    
    public async Task<IReadOnlyList<string>> GetPokemonTypesAsync(CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, PokemonEndpoint.PokemonType);
        using var resp = await SendWithRetriesAsync(req, ct);
        if (resp is null) return [];

        resp.EnsureSuccessStatusCode();
        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var page = await JsonSerializer.DeserializeAsync<PagedList<NamedApiResource>>(stream, _jsonOptions, ct);
        if (page is null) return [];

        return page.Results
            .Select(r => r.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .OrderBy(n => n)
            .ToList();
    }

    private async Task<HttpResponseMessage?> SendWithRetriesAsync(HttpRequestMessage req, CancellationToken ct)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                var resp = await _httpClient.SendAsync(req.Clone(), HttpCompletionOption.ResponseHeadersRead, ct);

                if (resp.StatusCode == (HttpStatusCode)429 || ((int)resp.StatusCode >= 500))
                {
                    if (attempt < MaxAttempts)
                    {
                        var delay = ComputeBackoff(attempt, resp);
                        resp.Dispose();
                        await Task.Delay(delay, ct);
                        continue;
                    }
                }

                return resp;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return null;
            }
            catch (TaskCanceledException)
            {
                if (attempt >= MaxAttempts)
                {
                    return null;
                }
                var delay = ComputeBackoff(attempt, retryAfterSeconds: null);
                await Task.Delay(delay, ct);
            }
            catch (HttpRequestException)
            {
                if (attempt >= MaxAttempts) return null;
                var delay = ComputeBackoff(attempt, retryAfterSeconds: null);
                await Task.Delay(delay, ct);
            }
        }

        return null;
    }
    
    private static TimeSpan ComputeBackoff(int attempt, HttpResponseMessage? resp = null, int? retryAfterSeconds = null)
    {
        var ra = retryAfterSeconds;

        if (ra is null && resp is not null && resp.Headers.TryGetValues("Retry-After", out var vals))
        {
            if (int.TryParse(vals.FirstOrDefault(), out var secs))
                ra = secs;
        }

        if (ra is not null)
            return TimeSpan.FromSeconds(Math.Clamp(ra.Value, 1, 30));

        var baseMs = 500 * Math.Pow(2, attempt - 1); // 0.5s, 1s, 2s
        var jitter = Random.Shared.Next(0, 250);     // +0..250ms
        var totalMs = Math.Min(baseMs + jitter, 4000); // limite 4s
        return TimeSpan.FromMilliseconds(totalMs);
    }
}