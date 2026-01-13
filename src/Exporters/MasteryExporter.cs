using Prometheus;
using Trick.Services;
using Trick.LOL;

namespace Trick.Exportets;

class MasteryExporter(
  string riotID,
  ILogger<MasteryExporter> logger,
  RiotClient riot,
  DataDragon dd
) : IAsyncExporter {

  private static readonly Gauge _totalLevel = Metrics.CreateGauge("player_total_level", "", "riotID");
  private static readonly Gauge _totalScore = Metrics.CreateGauge("player_total_score", "", "riotID");

	private static readonly Gauge _levelGauge = Metrics.CreateGauge("champion_level", "Mastery level on a champion", "riotID", "champion");
	private static readonly Gauge _scoreGauge = Metrics.CreateGauge("champion_score", "Mastery score on a champion", "riotID", "champion");

  private readonly ILogger _logger = logger;
  [Obsolete]
	private readonly string _puuid = "";
  private readonly string _riotID = riotID;
  private readonly RiotClient _riot = riot;
  private readonly DataDragon _dd = dd;

  private async Task<Account> GetAccountAsync() {
    var split = _riotID.Split("#");
    if(split.Count() != 2)
      throw new Exception($"Got bad riot id \"{_riotID}\"");

    return await _riot.GetAccountAsync(split[0], split[1]);
  }

  public async Task ExportAsync() {
    var account = await GetAccountAsync();
    var champs = await _riot.GetMasteryAsync(account.PUUID);

    int totalLevel = 0;
    uint totalScore = 0;

    foreach(var champ in champs) {
      var name = await _dd.GetNameFromKeyAsync(champ["championId"]!.GetValue<int>());
      
      var level = champ["championLevel"]!.GetValue<int>();
      var score = champ["championPoints"]!.GetValue<int>();


      _levelGauge.WithLabels(account.RiotID, name).Set(level);
      _scoreGauge.WithLabels(account.RiotID, name).Set(score);

      totalLevel += level;
      totalScore += (uint)score;
    }

    _totalLevel.WithLabels(account.RiotID).Set(totalScore);
    _totalScore.WithLabels(account.RiotID).Set(totalScore);
  }
}
