using Prometheus;
using Trick.Services;
using System.Text.Json.Nodes;

namespace Trick.Exportets;

class GameExporter(
  string puuid,
  ILogger<GameExporter> logger,
  RiotClient riot
) : IAsyncExporter {

  private static readonly int LOG_SIZE = 20;

  private readonly string _puuid = puuid;
  private readonly ILogger _logger = logger;
  private readonly RiotClient _riot = riot;
  private readonly List<string> _seenGameIDs = new List<string>();

  private readonly Counter _totalGamesCounter = Metrics.CreateCounter("games_count", "Total games observed", "riotID");
  private readonly Counter _wonGamesCounter = Metrics.CreateCounter("games_won", "Total games won", "riotID");
  private readonly Counter _lostGamesCounter = Metrics.CreateCounter("games_lost", "Total games lost", "riotID");
  private readonly Counter _remageGamesCounter = Metrics.CreateCounter("games_remake", "Total games remade", "riotID");


  public async Task ExportAsync() {
    var gameID = await GetFirstNewGame();
    if(string.IsNullOrWhiteSpace(gameID))
      return;

    var data = await _riot.GetGameAsync(gameID);

    try {
      _logger.LogDebug("Looking at game {id}", gameID);
      ObserveGame(data);
    } catch(Exception err) {
      _logger.LogError(err,"Error while looking at game");
    } finally {
      _seenGameIDs.Add(gameID);
    }



  }

  private async Task<string?> GetFirstNewGame() =>
    (await _riot.GetGameIDsAsync(_puuid, LOG_SIZE))
      .FirstOrDefault(e => !_seenGameIDs.Contains(e));

  private void ObserveGame(JsonObject data) {
    var acc = _riot.GetAccountAsync(_puuid).Result;

    var playerData = data["info"]!["participants"]!.AsArray().FirstOrDefault(e => e["puuid"]!.GetValue<string>() == _puuid);
    if(playerData is null)
      throw new Exception("Did not find player in game");

    if(playerData["gameEndedInEarlySurrender"]!.GetValue<bool>()) {
      _wonGamesCounter.WithLabels(acc.RiotID).Inc();
    } else {

      var didWin = playerData["win"]!.GetValue<bool>();
      if(didWin)
        _wonGamesCounter.WithLabels(acc.RiotID).Inc();
      else
        _lostGamesCounter.WithLabels(acc.RiotID).Inc();
    }

    _totalGamesCounter.WithLabels(acc.RiotID).Inc();
  }

}
