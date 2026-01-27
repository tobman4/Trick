using System.Text.Json.Nodes;
using Trick.LOL;
using Prometheus;

namespace Trick.Exportets;

class PlayerStatsExporter(
  ILogger<PlayerStatsExporter> logger
) : IPlayerExporter {


  private readonly ILogger _logger = logger;

  private static Counter _champsGamesPlayed = Metrics.CreateCounter(
    "player_champ_games", "Total games played on champ",
    "riotID", "champion"
  );
  
  private static Counter _champsGamesWon = Metrics.CreateCounter(
    "player_champ_games_won", "Total games won on champ",
    "riotID", "champion"
  );

  private static Counter _goldEarned = Metrics.CreateCounter(
    "player_gold_earned", "Total games won on champ",
    "riotID", "champion"
  );

  public Task ExportAsync(Account account, GameData data, JsonObject playerData) {
    _logger.LogDebug("Export: {game}-{champ}");

    var win = playerData.Get<bool>("win");
    var champion = playerData.Get<string>("championName");
    
    _champsGamesPlayed.WithLabels(account.RiotID, champion).Inc();
    if(win)
      _champsGamesPlayed.WithLabels(account.RiotID, champion).Inc();


    var gold = playerData.Get<int>("goldEarned");
    _goldEarned.WithLabels(account.RiotID).Inc(gold);

    return Task.CompletedTask;
  }

}
