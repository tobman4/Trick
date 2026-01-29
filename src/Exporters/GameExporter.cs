using Prometheus;
using Trick.Services;
using System.Text.Json.Nodes;
using Trick.Options;
using Trick.LOL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Trick.Exportets;

class GameExporter(
  IConfiguration conf,
  ILogger<GameExporter> logger,
  IServiceProvider services,
  RiotClient riot
) : IAsyncExporter {

  private static readonly int LOG_SIZE = 30;
  private static readonly List<string> EXPORT_LOG = new();
  private static readonly List<string> EXPORT_QUEUE = new();

  private readonly IConfiguration _conf = conf;
  private readonly ILogger _logger = logger;
  private readonly IServiceProvider _services = services;
  private readonly RiotClient _riot = riot;


  private static readonly Histogram _gameLength = Metrics.CreateHistogram("game_length", "Game length in seconds", new string[] { "mapId", "gameMode" }, new HistogramConfiguration {
    Buckets = Histogram.LinearBuckets(start: 60, width: 60, count: 45)
  });

  private async Task<IEnumerable<Account>> GetTargetAccounts() {
    var o = new List<Account>();
    var playerNames = _conf.GetSection("Players").Get<IEnumerable<string>>() ??
      throw new Exception();



    foreach(var riotID in playerNames) {
      var split = riotID.Split("#");
      if(split.Count() != 2)
        throw new Exception($"Got bad riotID \"{riotID}\"");

      var acc = await _riot.GetAccountAsync(split[0], split[1]);
      o.Add(acc);
    }
    
    return o.ToArray();
  }

  public async Task ExportAsync() {
    // 0. Prep
    // TODO: Make not shit
    var targets = await GetTargetAccounts();
    var playerIDs = targets.Select(e => e.PUUID);

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
    var gameIDs = await _riot.GetGameIDsAsync(puuid, 5);

    var newGameIDs = gameIDs
      .Where(e => !EXPORT_LOG.Contains(e) && !EXPORT_QUEUE.Contains(e));

    if(newGameIDs.Count() == 0)
      return;

    _logger.LogDebug("Adding {count} games to export", newGameIDs.Count());
    EXPORT_QUEUE.AddRange(newGameIDs);
  }

  private async Task ExportGame(string gameID, IEnumerable<string> playerIDs) {
    var gameData = await _riot.GetGameAsync(gameID);

    var mapID = gameData["info"]!["mapId"]!.GetValue<int>();
    var gameMode = gameData["info"]!["gameMode"]!.GetValue<string>();
    var gameLength = gameData["info"]!["gameDuration"]!.GetValue<int>();

    _gameLength.WithLabels(mapID.ToString(),gameMode).Observe(gameLength);

    ////////////
    // Player //
    ////////////
    var playerExporters = _services.GetServices<IPlayerExporter>();
    var players = gameData?["info"]?["participants"]?.AsArray()
      ?? throw new Exception("Cant get players");

    foreach(var player in players) {
      var puuid = player?["puuid"]?.GetValue<string>();
      if(!playerIDs.Contains(puuid))
          continue;

      // await ExportPlayer(player?.AsObject()!); // TODO: Dont use this shit
      Account acc = await _riot.GetAccountAsync(puuid!);
      Task.WaitAll(playerExporters.Select(e => e.ExportAsync(acc, gameData, player!.AsObject())));
    }
  }

  private async Task ExportPlayer(JsonObject playerData) {
    var acc = await _riot.GetAccountAsync(playerData["puuid"]?.GetValue<string>() ?? "");

    _logger.LogDebug("Found: {riotID}", acc.RiotID);

    throw new NotImplementedException();
  }
}
