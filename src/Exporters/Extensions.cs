using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Trick.Options;
using Trick.Services;

namespace Trick.Exportets;

static class Extensions {

  public static void AddExporters(this HostApplicationBuilder builder) {
    builder.Services.AddHostedService<ExportController>();
    builder.Services.AddScoped<IAsyncExporter, GameExporter>();

    builder.Services.AddScoped<IPlayerExporter, PlayerTest>();

    var players = builder.Configuration.GetSection("Players")
      .Get<IEnumerable<string>>() ?? new string[0];

    foreach(var player in players) {
      builder.Services.AddSingleton<IAsyncExporter, MasteryExporter>(e => 
        new MasteryExporter(
          player,
          e.GetRequiredService<ILogger<MasteryExporter>>(),
          e.GetRequiredService<RiotClient>(),
          e.GetRequiredService<DataDragon>()
        )
      );




    }
  }
}
