using System.Text.Json.Nodes;

namespace Trick.LOL;

class GameData(JsonObject raw) {

	public readonly JsonObject Raw = raw;

  public long GameID => Raw.Get<long>("gameId");

  public static implicit operator GameData(JsonNode rawObj) => new GameData(rawObj.AsObject());

}
