using Trick.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trick.Options;


namespace Trick.Services;

class ExportController(
  ILogger<ExportController> logger,
  IServiceProvider services,
  IOptions<ExportOptions> options
) : BackgroundService {

  private readonly ILogger _logger = logger;
  private readonly IServiceProvider _services = services;
  private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.Interval));

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    
    do {
      using(var scope = _services.CreateAsyncScope()) {
        var exporters = scope.ServiceProvider.GetServices<IAsyncExporter>();
        Task.WaitAll(exporters.Select(e =>ExportAsync(e)), stoppingToken);
      }


    }while(await _timer.WaitForNextTickAsync(stoppingToken));
  

  }

  private async Task ExportAsync(IAsyncExporter exporter) {
    try {
      var st = Stopwatch.StartNew();
      await exporter.ExportAsync();
      st.Stop();

      _logger.LogDebug("Export({time}ms) {name}", st.ElapsedMilliseconds, exporter.GetType().Name);
    } catch(Exception err) {
      _logger.LogError(err, $"Failed to export {exporter.GetType().Name}");
    }

  }

}
