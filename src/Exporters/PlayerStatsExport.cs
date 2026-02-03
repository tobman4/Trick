using System.Text.Json.Nodes;
using Trick.LOL;
using Prometheus;

namespace Trick.Exportets;

class PlayerStatsExporter(
  ILogger<PlayerStatsExporter> logger
) : IPlayerExporter {


  private readonly ILogger _logger = logger;


  private static readonly Counter _totalGamesCounter = Metrics.CreateCounter("games_count", "Total games observed", "riotID");
  private static readonly Counter _wonGamesCounter = Metrics.CreateCounter("games_won", "Total games won", "riotID");
  // private static readonly Counter _lostGamesCounter = Metrics.CreateCounter("games_lost", "Total games lost", "riotID");
  // private static readonly Counter _remageGamesCounter = Metrics.CreateCounter("games_remake", "Total games remade", "riotID");

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
    "riotID"
  );

  private static readonly Histogram _visionScore = Metrics.CreateHistogram("player_vision", "Player vision score", new string[] {"riotId"}, new HistogramConfiguration {
    Buckets = Histogram.LinearBuckets(start: 0, width: 10, count: 15)
  });

  public Task ExportAsync(Account account, GameData data, JsonObject playerData) {
    _logger.LogDebug("Export: {game}-{champ}", data.GameID, account.RiotID);

    // Win
    var didWin = playerData["win"]?.GetValue<bool>() ?? false;
    if(didWin)
      _wonGamesCounter.WithLabels(account.RiotID).Inc();

    _totalGamesCounter.WithLabels(account.RiotID).Inc();
    
    // Champ
    var champion = playerData.Get<string>("championName");
    _champsGamesPlayed.WithLabels(account.RiotID, champion).Inc();
    if(didWin)
      _champsGamesWon.WithLabels(account.RiotID, champion).Inc();

    // Small stats
    var gold = playerData.Get<int>("goldEarned");
    _goldEarned.WithLabels(account.RiotID).Inc(gold);

    var vision = playerData["visionScore"]?.GetValue<int>() ?? 0;
    _visionScore.WithLabels(account.RiotID).Observe(vision);

    return Task.CompletedTask;
  }

}
