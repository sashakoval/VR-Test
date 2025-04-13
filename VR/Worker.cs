using VR.Interfaces;

namespace VR
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes the background service.
        /// </summary>
        /// <param name="stoppingToken">A token that can be used to stop the background service.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Watching Process has been started");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileMonitorService = scope.ServiceProvider.GetRequiredService<IFileMonitorService>();
                    fileMonitorService.StartWatching();
                    await MonitorFilesAsync(fileMonitorService, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the worker.");
            }
            finally
            {
                _logger.LogInformation("Watching Process has been stopped");
            }
        }

        /// <summary>
        /// Monitors files in the background.
        /// </summary>
        /// <param name="fileMonitorService">The file monitor service.</param>
        /// <param name="stoppingToken">A token that can be used to stop the background service.</param>
        private async Task MonitorFilesAsync(IFileMonitorService fileMonitorService, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

