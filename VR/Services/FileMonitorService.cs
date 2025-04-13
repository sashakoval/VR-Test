using VR.Interfaces;
using Microsoft.Extensions.Options;
using VR.Configurations;

namespace VR.Services
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
        private FileSystemWatcher? _fileSystemWatcher;

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

            _fileSystemWatcher = new FileSystemWatcher(_options.WatchFolder, "*.txt")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _fileSystemWatcher.Created += OnCreated;
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;

            _logger.LogInformation("Started watching folder: {folder}", _options.WatchFolder);
        }

        /// <summary>
        /// Processes a specified file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to process.</param>
        public async Task ProcessFileAsync(string filePath)
        {
            _logger.LogInformation("Processing file: {filePath}", filePath);

            try
            {
                await _fileParserService.ParseFileAsync(filePath, async (boxes) =>
                {
                    _logger.LogInformation("Parsed {count} boxes from file: {filePath}", boxes.Count, filePath);
                    await _dataService.SaveBoxesAsync(boxes, 1000);
                });

                _logger.LogInformation("Finished processing file: {filePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file: {filePath}", filePath);
            }
        }

        /// <summary>
        /// Processes existing files in the watch folder.
        /// </summary>
        private async void ProcessExistingFiles()
        {
            var existingFiles = Directory.GetFiles(_options.WatchFolder, "*.txt");
            foreach (var file in existingFiles)
            {
                _logger.LogInformation("Processing existing file: {filePath}", file);
                await ProcessFileAsync(file);
            }
        }

        /// <summary>
        /// Event handler for when a new file is created in the watch folder.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File created: {filePath}", e.FullPath);
            await ProcessFileAsync(e.FullPath);
        }

        /// <summary>
        /// Event handler for when a file is changed in the watch folder.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File changed: {filePath}", e.FullPath);
            await ProcessFileAsync(e.FullPath);
        }
    }
}
