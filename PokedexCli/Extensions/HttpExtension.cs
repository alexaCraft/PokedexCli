namespace PokedexCli.Extensions;

public static class HttpExtension
{
    public static HttpRequestMessage Clone(this HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);

        foreach (var header in req.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (req.Content is not null)
        {
            var ms = new MemoryStream();
            req.Content.CopyTo(ms, null, CancellationToken.None);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var header in req.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }

}