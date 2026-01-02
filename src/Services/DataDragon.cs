using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Trick.Services;

class DataDragon(
	ILogger<DataDragon> logger,
	HttpClient client
) {
	private readonly ILogger _logger = logger;
	private readonly HttpClient _client = client;

	public async Task<String> GetLastVersionAsync() =>
		(await GetVersionsAsync()).First();

	public async Task<IEnumerable<string>> GetVersionsAsync() =>
		(await _client.GetFromJsonAsync<IEnumerable<string>>("api/versions.json")) ?? new string[0];

	

	//public async Task<string> GetNameFromKeyAsync(string key) {
	public async Task<string> GetNameFromKeyAsync(int key) {
		var json = await _client.GetFromJsonAsync<JsonObject>($"cdn/{await GetLastVersionAsync()}/data/en_US/champion.json");
		
		if(json is null)
			throw new Exception();

		var x = json["data"]!.AsObject();

		foreach(var champ in x) {
      var champKey = Int32.Parse(champ.Value!["key"]!.GetValue<string>());  
			if(champKey == key)
				return champ.Value!["name"]!.GetValue<string>();
    }


		throw new Exception($"No champ with key \"{key}\"");
	}
}
