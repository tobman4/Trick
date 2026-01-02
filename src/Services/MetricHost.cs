using Microsoft.Extensions.Hosting;
using Prometheus;

class MetricHost(
) : IHostedService {
	
	private KestrelMetricServer _server = new KestrelMetricServer(port: 8088);

	
	public Task StartAsync(CancellationToken ct) {
		_server.Start();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken ct) {
		_server.Stop();
		return Task.CompletedTask;
	}


}
