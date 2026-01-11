using Prometheus;
using Trick.Services;
using System.Text.Json.Nodes;
using Trick.Options;
using Microsoft.Extensions.Configuration;

namespace Trick.Exportets;

class GameExporter(
  IConfiguration conf,
  ILogger<GameExporter> logger,
  RiotClient riot
) : IAsyncExporter {

  private static readonly int LOG_SIZE = 10;
  private static readonly List<string> EXPORT_LOG = new();
  private static readonly List<string> EXPORT_QUEUE = new();


  private readonly IConfiguration _conf = conf;
  private readonly ILogger _logger = logger;
  private readonly RiotClient _riot = riot;

  private readonly Counter _totalGamesCounter = Metrics.CreateCounter("games_count", "Total games observed", "riotID");
  private readonly Counter _wonGamesCounter = Metrics.CreateCounter("games_won", "Total games won", "riotID");
  private readonly Counter _lostGamesCounter = Metrics.CreateCounter("games_lost", "Total games lost", "riotID");
  private readonly Counter _remageGamesCounter = Metrics.CreateCounter("games_remake", "Total games remade", "riotID");
  
  

  public async Task ExportAsync() {
    // 0. Prep
    // TODO: Make not shit
    var playersOps = _conf
      .GetSection("Players")
      .Get<IEnumerable<PlayerExportOptions>>() ?? new PlayerExportOptions[0];

    var playerIDs = playersOps.Select(e => e.PUUID);

    // 1. Look for new games
    foreach(var puuid in playerIDs)
      await LookForNewGames(puuid);

    
    // 2. Export data from games
    var nextGameID = EXPORT_QUEUE.FirstOrDefault();
    if(nextGameID is not null) {
      try {
        _logger.LogDebug("Exporting game {id}", nextGameID);
        await ExportGame(nextGameID, playerIDs);
      } catch(Exception err) {
        _logger.LogError(err, "Failed to handle game {id}", nextGameID);
      } finally {
        EXPORT_QUEUE.Remove(nextGameID);
        EXPORT_LOG.Insert(0,nextGameID);
      }
    }
    
    // 3. Save game push gameID
    // ^^^^^^^^^^^^^^^^^^^^^^^^

    // 4. Clean
    if(EXPORT_LOG.Count() > LOG_SIZE) {
      var toRemove = EXPORT_LOG.Count() - LOG_SIZE;
      EXPORT_LOG.RemoveRange(LOG_SIZE, toRemove);
      _logger.LogDebug("Removed {count} games from log", toRemove);
    }
  }


  private async Task LookForNewGames(string puuid) {
    var gameIDs = await _riot.GetGameIDsAsync(puuid, 3);

    var newGameIDs = gameIDs
      .Where(e => !EXPORT_LOG.Contains(e) && !EXPORT_QUEUE.Contains(e));

    if(newGameIDs.Count() == 0)
      return;

    _logger.LogDebug("Adding {count} games to export", newGameIDs.Count());
    EXPORT_QUEUE.AddRange(newGameIDs);
  }

  private async Task ExportGame(string gameID, IEnumerable<string> playerIDs) {
    _logger.LogInformation("beep boop");
    var gameData = await _riot.GetGameAsync(gameID);

    var players = gameData?["info"]?["participants"]?.AsArray()
      ?? throw new Exception("Cant get players");

    foreach(var player in players) {
      var puuid = player?["puuid"]?.GetValue<string>();
      if(!playerIDs.Contains(puuid))
          continue;

      await ExportPlayer(player?.AsObject()!);
    }


    // Doing work
    await Task.Delay(1);
  }

  private async Task ExportPlayer(JsonObject playerData) {
    var acc = await _riot.GetAccountAsync(playerData["puuid"]?.GetValue<string>() ?? "");

    _logger.LogDebug("Found: {riotID}", acc.RiotID);

    var didWin = playerData["win"]?.GetValue<bool>() ?? false;
    if(didWin)
      _wonGamesCounter.WithLabels(acc.RiotID).Inc();
    else
      _lostGamesCounter.WithLabels(acc.RiotID).Inc();
    _totalGamesCounter.WithLabels(acc.RiotID).Inc();

    

  }


}
