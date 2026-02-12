using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;

namespace Trick.Services;

class DataDragon(
	ILogger<DataDragon> logger,
	HttpClient client,
	IMemoryCache cache
) {
	private readonly ILogger _logger = logger;
	private readonly HttpClient _client = client;
	private readonly IMemoryCache _cache = cache;

	public async Task<String> GetLastVersionAsync() =>
		(await GetVersionsAsync()).First();

	public async Task<IEnumerable<string>> GetVersionsAsync() =>
		await _cache.GetOrCreateAsync("dd_versions", async entry => {
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
			return (await _client.GetFromJsonAsync<IEnumerable<string>>("api/versions.json")) ?? new string[0];
		}) ?? new string[0];
	

	//public async Task<string> GetNameFromKeyAsync(string key) {
	public async Task<string> GetNameFromKeyAsync(int key) {
		var version = await GetLastVersionAsync();
		var champions = await _cache.GetOrCreateAsync($"dd_champions_{version}", async entry => {
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
			var json = await _client.GetFromJsonAsync<JsonObject>($"cdn/{version}/data/en_US/champion.json");
		
			if(json is null)
				throw new Exception();

			var x = json["data"]!.AsObject();
			var dict = new Dictionary<int, string>();

			foreach(var champ in x) {
				var champKey = Int32.Parse(champ.Value!["key"]!.GetValue<string>());
				dict[champKey] = champ.Value!["name"]!.GetValue<string>();
			}
			return dict;
		});

		if(champions != null && champions.TryGetValue(key, out var name))
			return name;

		throw new Exception($"No champ with key \"{key}\"");
	}
}
