using Microsoft.Extensions.Options;
using Infrastructure.Configurations;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service to monitor a specified folder for new or changed text files and process them.
    /// </summary>
    public class FileMonitorService : IFileMonitorService
    {
        private readonly ILogger<FileMonitorService> _logger;
        private readonly IFileParserService _fileParserService;
        private readonly IDataService _dataService;
        private readonly FileMonitorOptions _options;
        private PollingFileWatcher? _pollingFileWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMonitorService"/> class.
        /// </summary>
        /// <param name="options">The options for file monitoring.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="fileParserService">The file parser service.</param>
        /// <param name="dataService">The data service.</param>
        public FileMonitorService(IOptions<FileMonitorOptions> options, ILogger<FileMonitorService> logger, IFileParserService fileParserService, IDataService dataService)
        {
            _logger = logger;
            _fileParserService = fileParserService;
            _options = options.Value;
            _dataService = dataService;
        }

        /// <summary>
        /// Starts watching the specified folder for new or changed text files.
        /// </summary>
        public void StartWatching()
        {
            ProcessExistingFiles();

            _pollingFileWatcher = new PollingFileWatcher(_options.WatchFolder, "*.txt", TimeSpan.FromSeconds(10));
            _pollingFileWatcher.Created += OnCreated;
            _pollingFileWatcher.Changed += OnChanged;
            _pollingFileWatcher.Start();

            _logger.LogInformation("Started watching folder: {folder}", _options.WatchFolder);
        }

        /// <summary>
        /// Processes a specified file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to process.</param>
        public async Task ProcessFileAsync(string filePath)
        {
            _logger.LogInformation("Processing file: {filePath}", filePath);

            await _fileParserService.ParseFileAsync(filePath, async (boxes) =>
            {
                _logger.LogInformation("Parsed {count} boxes from file: {filePath}", boxes.Count, filePath);
                await _dataService.SaveBoxesAsync(boxes);
            });

            _logger.LogInformation("Finished processing file: {filePath}", filePath);
        }

        private async void ProcessExistingFiles()
        {
            var existingFiles = Directory.GetFiles(_options.WatchFolder, "*.txt");
            foreach (var file in existingFiles)
            {
                await ProcessFileAsync(file);
            }
        }

        private async void OnCreated(object? sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File created: {filePath}", e.FullPath);
            await ProcessFileAsync(e.FullPath);
        }

        private async void OnChanged(object? sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File changed: {filePath}", e.FullPath);
            await ProcessFileAsync(e.FullPath);
        }
    }
}
