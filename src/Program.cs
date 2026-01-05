global using Microsoft.Extensions.Logging;
global using Trick.Interfaces;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Trick.Services;
using Trick.Interfaces;
using Trick.Exportets;

var builder = Host.CreateApplicationBuilder();
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



// builder.Services.AddSingleton<IAsyncExporter, GameExporter>(e => 
//   new GameExporter(
//     "gWuTiRR8VdReLlRPHbGQXX3gVxoGNYW_3Rs7L10KBuiZb0jCx4AvkDRdSCvD_Czp8HLnFbBDr3mYnA",
//     e.GetRequiredService<ILogger<GameExporter>>(),
//     e.GetRequiredService<RiotClient>()
//   )
// );
//
// builder.Services.AddSingleton<IAsyncExporter, GameExporter>(e => 
//   new GameExporter(
//     "I5SSQpHLl88Wne8eNA8gMco97juJ7E1kqIDycnyv1CZxn9x_cdDweTd6Mb8ozjN6OMX3ZBlDbh3s1Q",
//     e.GetRequiredService<ILogger<GameExporter>>(),
//     e.GetRequiredService<RiotClient>()
//   )
// );
//




var app = builder.Build();

await app.RunAsync();
