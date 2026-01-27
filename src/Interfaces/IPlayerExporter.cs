using System.Text.Json.Nodes;
using Trick.LOL;

namespace Trick.Interfaces;

interface IPlayerExporter {

  public Task ExportAsync(Account account, GameData gameData, JsonObject playerData);

}
