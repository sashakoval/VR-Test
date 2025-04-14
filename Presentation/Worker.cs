using Core.Interfaces;

namespace Presentation
{
    /// <summary>
    /// Background service that monitors files using the file monitor service.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger instance.</param>
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

