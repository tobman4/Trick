using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Prometheus;

class MetricHost : IHostedService {
	
  private readonly int _port;
  private readonly ILogger _logger;
	private readonly KestrelMetricServer _server;

  public MetricHost(ILogger<MetricHost> logger, IConfiguration conf) {
    _port = conf.GetValue<int>("ExportPort", 8088);
    _server = new(port: _port);
    _logger = logger;
  }

	
	public Task StartAsync(CancellationToken ct) {
    _logger.LogInformation("Export metrcs on {port}", _port);
		_server.Start();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken ct) {
		_server.Stop();
		return Task.CompletedTask;
	}


}
