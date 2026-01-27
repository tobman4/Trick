global using Microsoft.Extensions.Logging;
global using Trick.Interfaces;

using Trick;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Trick.Services;
using Trick.Interfaces;
using Trick.Exportets;

var builder = Host.CreateApplicationBuilder();
builder.Logging.Setup();
builder.Services.AddHostedService<MetricHost>();
builder.AddExporters();

builder.Services.AddHttpClient<RiotClient>(e => {
	var token = builder.Configuration["RiotToken"];
	if(string.IsNullOrWhiteSpace(token))
		throw new Exception("Missing \"RiotToken\"");
	
	e.BaseAddress = new("https://europe.api.riotgames.com");
	e.DefaultRequestHeaders.Add("X-Riot-Token", token);	
});

builder.Services.AddHttpClient<DataDragon>(e => {
	e.BaseAddress =  new("https://ddragon.leagueoflegends.com");
});


var app = builder.Build();

await app.RunAsync();
