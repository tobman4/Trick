using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Trick.LOL;

namespace Trick.Services;

class RiotClient(
	ILogger<RiotClient> logger,
	HttpClient client,
	IMemoryCache cache
) {

	private readonly ILogger _logger = logger;
	private readonly HttpClient _client = client;
	private readonly IMemoryCache _cache = cache;

	// public async Task<IEnumerable<string>> GetGameIDsAsync(string puuid) =>
	// 	(await _client.GetFromJsonAsync<IEnumerable<string>>($"/lol/match/v5/matches/by-puuid/{puuid}/ids")) ?? new string[0];

  public async Task<IEnumerable<JsonObject>> GetTopMasteryAsync(string puuid) =>
    await _cache.GetOrCreateAsync($"riot_topmastery_{puuid}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
      return (await _client.GetFromJsonAsync<IEnumerable<JsonObject>>($"https://euw1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-puuid/{puuid}/top")) ?? new JsonObject[0];
    }) ?? new JsonObject[0];

  public async Task<IEnumerable<JsonObject>> GetMasteryAsync(string puuid) =>
    await _cache.GetOrCreateAsync($"riot_mastery_{puuid}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
      return (await _client.GetFromJsonAsync<IEnumerable<JsonObject>>($"https://euw1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-puuid/{puuid}")) ?? new JsonObject[0];
    }) ?? new JsonObject[0];


  public async Task<Account> GetAccountAsync(string puuid) =>
    await _cache.GetOrCreateAsync($"riot_account_puuid_{puuid}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
      return (await _client.GetFromJsonAsync<Account>($"/riot/account/v1/accounts/by-puuid/{puuid}")) ?? throw new Exception("Cant get account info");
    }) ?? throw new Exception("Cant get account info");

  public async Task<Account> GetAccountAsync(string gameName, string tag) =>
    await _cache.GetOrCreateAsync($"riot_account_riotid_{gameName}_{tag}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
      return (await _client.GetFromJsonAsync<Account>($"/riot/account/v1/accounts/by-riot-id/{gameName}/{tag}")) ?? throw new Exception("Cant get account info");
    }) ?? throw new Exception("Cant get account info");



  public async Task<IEnumerable<string>> GetGameIDsAsync(string puuid, int count = 10) => 
    await _cache.GetOrCreateAsync($"riot_gameids_{puuid}_{count}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
      return (await _client.GetFromJsonAsync<IEnumerable<string>>($"/lol/match/v5/matches/by-puuid/{puuid}/ids?count={count}")) ?? new String[0];
    }) ?? new String[0];

  public async Task<JsonObject> GetGameAsync(string gameID) =>
    await _cache.GetOrCreateAsync($"riot_game_{gameID}", async entry => {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
      return (await _client.GetFromJsonAsync<JsonObject>($"/lol/match/v5/matches/{gameID}")) ?? throw new Exception($"Cant get game: {gameID}");
    }) ?? throw new Exception($"Cant get game: {gameID}");

}
