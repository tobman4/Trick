namespace Trick.Options;

class PlayerExportOptions {
  public string PUUID { get; init; } = null!;
  
  public bool ExportMastery { get; init; } = true;
  public bool ExportMatch { get; init; } = false;
}
