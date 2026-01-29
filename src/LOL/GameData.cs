using System.Text.Json.Nodes;

namespace Trick.LOL;

class GameData(JsonObject raw) {

	public readonly JsonObject Raw = raw;

  public string GameID => Raw["metaData"]!.AsObject().Get<string>("matchId");

  public static implicit operator GameData(JsonNode rawObj) => new GameData(rawObj.AsObject());

}
