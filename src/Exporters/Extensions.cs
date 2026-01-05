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

    var players = builder.Configuration.GetSection("Players")
      .Get<IEnumerable<PlayerExportOptions>>() ?? new PlayerExportOptions[0];

    foreach(var player in players) {
      Console.WriteLine(player.PUUID);
      if(player.ExportMastery)
        builder.Services.AddSingleton<IAsyncExporter, MasteryExporter>(e => 
          new MasteryExporter(
            player.PUUID,
            e.GetRequiredService<ILogger<MasteryExporter>>(),
            e.GetRequiredService<RiotClient>(),
            e.GetRequiredService<DataDragon>()
          )
        );




    }
  }
}
