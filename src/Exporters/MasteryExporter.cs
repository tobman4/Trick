using Prometheus;
using Trick.Interfaces;
using Trick.Services;

namespace Trick.Exportets;

class MasteryExporter(
  string puuid,
  ILogger<MasteryExporter> logger,
  RiotClient riot,
  DataDragon dd
) : IAsyncExporter {

	private static readonly Gauge _levelGauge = Metrics.CreateGauge("champion_level", "Mastery level on a champion", "riotID", "champion");
	private static readonly Gauge _scoreGauge = Metrics.CreateGauge("champion_score", "Mastery score on a champion", "riotID", "champion");

  private readonly ILogger _logger = logger;
	private readonly string _puuid = puuid;
  private readonly RiotClient _riot = riot;
  private readonly DataDragon _dd = dd;


  public async Task ExportAsync() {
    var account = _riot.GetAccountAsync(_puuid);
    var champs = await _riot.GetTopMasteryAsync(_puuid);

    foreach(var champ in champs) {
      var name = await _dd.GetNameFromKeyAsync(champ["championId"]!.GetValue<int>());
      
      var level = champ["championLevel"]!.GetValue<int>();
      var score = champ["championPoints"]!.GetValue<int>();


      _levelGauge.WithLabels((await account).RiotID, name).Set(level);
      _scoreGauge.WithLabels((await account).RiotID, name).Set(score);
      
    }
  }
}
