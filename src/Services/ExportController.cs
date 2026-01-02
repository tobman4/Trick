using Trick.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Trick.Services;

class ExportController(
  ILogger<ExportController> logger,
  IServiceProvider services
) : BackgroundService {

  private readonly ILogger _logger = logger;
  private readonly IServiceProvider _services = services;
  private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(120));

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
      await exporter.ExportAsync();
    } catch(Exception err) {
      _logger.LogError(err, $"Failed to export {exporter.GetType().Name}");
    }

  }

}
