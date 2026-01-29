using System.Text.Json.Nodes;
using Trick.LOL;
using Prometheus;

namespace Trick.Exportets;

class PingExport : IPlayerExporter {

  
  private readonly Counter _bluePing = Metrics.CreateCounter("player_blue_ping", "How many blue pings used", "riotID");
  private readonly Counter _cautionPing = Metrics.CreateCounter("player_caution_ping", "How many blue pings used", "riotID");

  private readonly Counter _targetPing = Metrics.CreateCounter("player_target_ping", "How many blue pings used", "riotID");
  private readonly Counter _defendPing = Metrics.CreateCounter("player_defend_ping", "How many blue pings used", "riotID");

  private readonly Counter _dangerPing = Metrics.CreateCounter("player_danger_ping", "How many blue pings used", "riotID");
  private readonly Counter _pushPing = Metrics.CreateCounter("player_push_ping", "How many blue pings used", "riotID");
  private readonly Counter _missingPing = Metrics.CreateCounter("player_missing_ping", "How many blue pings used", "riotID");
  private readonly Counter _omwPing = Metrics.CreateCounter("player_omw_ping", "How many blue pings used", "riotID");
  private readonly Counter _allInPing = Metrics.CreateCounter("player_allIn_ping", "How many blue pings used", "riotID");
  private readonly Counter _helpPing = Metrics.CreateCounter("player_help_ping", "How many blue pings used", "riotID");
  private readonly Counter _visionPing = Metrics.CreateCounter("player_vision_ping", "How many blue pings used", "riotID");
  private readonly Counter _enemyVisionPing = Metrics.CreateCounter("player_enemyVision_ping", "How many blue pings used", "riotID");


  public Task ExportAsync(Account account, GameData data, JsonObject playerData) {


    _bluePing.WithLabels(account.RiotID).Inc(playerData.Get<int>("commandPings"));
    _cautionPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("getBackPings"));
                    
    // _targetPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("")); // Not in data????
    // _defendPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("")); // Not in data????
                    
    // _dangerPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("")); // ???????????????
    _pushPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("pushPings"));
    _missingPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("enemyMissingPings"));
    _omwPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("onMyWayPings"));
    _allInPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("allInPings"));
    _helpPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("assistMePings"));
    _visionPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("needVisionPings"));
    _enemyVisionPing.WithLabels(account.RiotID).Inc(playerData.Get<int>("enemyVisionPings"));

    return Task.CompletedTask;
  }

}
