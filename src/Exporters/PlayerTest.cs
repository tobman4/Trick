using Trick.LOL;
using System.Text.Json.Nodes;

namespace Trick.Exportets;

class PlayerTest(ILogger<PlayerTest> log) : IPlayerExporter {

  public Task ExportAsync(Account account, GameData gameData, JsonObject playerData) {
    log.LogInformation("Export: {riotID}", account.RiotID);
    return Task.CompletedTask;
  }
  
}
