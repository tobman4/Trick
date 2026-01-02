using System.Text.Json.Serialization;

namespace Trick.LOL;

class Account {

  [JsonPropertyName("puuid")]
  public string PUUID { get; init; } = null!;

  [JsonPropertyName("gameName")]
  public string GameName { get; init; } = null!;

  [JsonPropertyName("tagLine")]
  public string Tag { get; init; } = null!;

  public string RiotID => $"{GameName}#{Tag}";

}
