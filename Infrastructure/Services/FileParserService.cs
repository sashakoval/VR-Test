using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service to parse files and process valid boxes in batches.
    /// </summary>
    public class FileParserService : IFileParserService
    {
        private readonly ILogger<FileParserService> _logger;
        private readonly IValidationService _validationService;
        private readonly int _batchSize = 5000;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileParserService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="validationService">The validation service.</param>
        public FileParserService(ILogger<FileParserService> logger, IValidationService validationService)
        {
            _logger = logger;
            _validationService = validationService;
        }

        /// <summary>
        /// Parses the file and processes valid boxes in batches as they are encountered.
        /// </summary>
        /// <param name="filePath">The path to the file to be parsed.</param>
        /// <param name="processBatchAsync">The function to process a batch of valid boxes.</param>
        /// <param name="batchSize">The size of each batch to be processed.</param>
        public async Task ParseFileAsync(string filePath, Func<List<Box>, Task> processBatchAsync)
        {
            var validBoxesBatch = new List<Box>();
            var invalidBoxes = new Dictionary<string, List<string>>();
            Box? currentBox = null;
            int processedBoxesCount = 0;

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 0)
                        continue;

                    if (parts[0] == "HDR")
                    {
                        if (currentBox != null)
                        {
                            AddCurrentBoxToBatch(currentBox, validBoxesBatch, invalidBoxes);
                            processedBoxesCount++;

                            if (validBoxesBatch.Count >= _batchSize)
                            {
                                await processBatchAsync(validBoxesBatch);
                                validBoxesBatch.Clear();
                            }
                        }
                        currentBox = HandleHeader(parts);
                    }
                    else if (parts[0] == "LINE")
                    {
                        HandleLine(parts, currentBox, invalidBoxes);
                    }
                }

                if (currentBox != null)
                {
                    AddCurrentBoxToBatch(currentBox, validBoxesBatch, invalidBoxes);
                    processedBoxesCount++;
                }

                if (validBoxesBatch.Count > 0)
                {
                    await processBatchAsync(validBoxesBatch);
                }

                LogInvalidBoxes(invalidBoxes);
                _logger.LogInformation($"Processed a total of {processedBoxesCount} boxes from file: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file: {filePath}", filePath);
                throw;
            }
        }

        private void AddCurrentBoxToBatch(Box? currentBox, List<Box> validBoxesBatch, Dictionary<string, List<string>> invalidBoxes)
        {
            if (currentBox != null && !invalidBoxes.ContainsKey(currentBox.Identifier))
            {
                validBoxesBatch.Add(currentBox);
            }
        }

        private Box? HandleHeader(string[] parts)
        {
            try
            {
                _validationService.ValidateHeader(parts);
                return ProcessHeader(parts);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Validation error in header: {header}", string.Join(' ', parts));
                return null;
            }
        }

        private void HandleLine(string[] parts, Box? currentBox, Dictionary<string, List<string>> invalidBoxes)
        {
            try
            {
                _validationService.ValidateLine(parts);
                ProcessLine(parts, currentBox);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Validation error in line: {line}", string.Join(' ', parts));
                if (currentBox != null)
                {
                    if (!invalidBoxes.ContainsKey(currentBox.Identifier))
                    {
                        invalidBoxes[currentBox.Identifier] = new List<string>();
                    }
                    invalidBoxes[currentBox.Identifier].Add(string.Join(' ', parts));
                }
            }
        }

        private void LogInvalidBoxes(Dictionary<string, List<string>> invalidBoxes)
        {
            if (invalidBoxes.Count > 0)
            {
                _logger.LogWarning("The following boxes contained invalid lines and were skipped:");
                foreach (var kvp in invalidBoxes)
                {
                    _logger.LogWarning($"Box: {kvp.Key}");
                    foreach (var invalidLine in kvp.Value)
                    {
                        _logger.LogWarning(invalidLine);
                    }
                }
            }
        }

        private Box ProcessHeader(string[] parts)
        {
            return new Box
            {
                SupplierIdentifier = parts[1],
                Identifier = parts[2],
                Contents = new List<Box.Content>()
            };
        }

        private void ProcessLine(string[] parts, Box? currentBox)
        {
            var content = new Box.Content
            {
                PoNumber = parts[1],
                Isbn = parts[2],
                Quantity = int.Parse(parts[3])
            };

            if (currentBox != null)
            {
                var existingContent = currentBox.Contents.FirstOrDefault(c => c.PoNumber == content.PoNumber && c.Isbn == content.Isbn);
                if (existingContent != null)
                {
                    existingContent.Quantity += content.Quantity;
                }
                else
                {
                    currentBox.Contents.Add(content);
                }
            }
        }
    }
}
