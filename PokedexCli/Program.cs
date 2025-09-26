using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokedexCli.App;
using PokedexCli.Services;
using PokedexCli.Presentation.Console;
using PokedexCli.Presentation.PokemonPrinter;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddHttpClient<IPokemonService, PokemonService>(client =>
    {
        client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        client.Timeout = TimeSpan.FromSeconds(15);
    });

builder.Services.AddSingleton<IConsoleService, ConsoleService>();
builder.Services.AddSingleton<IPokemonPrinter, PokemonPrinter>();
builder.Services.AddSingleton<PokedexCliApp>();

var host = builder.Build();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var ct = lifetime.ApplicationStopping;

var app = host.Services.GetRequiredService<PokedexCliApp>();

int exitCode;
if (args.Length > 0)
{
    exitCode = await app.RunOnceAsync(args[0], ct);
}
else
{
    exitCode = await app.RunInteractiveAsync(ct);
}

Environment.Exit(exitCode);