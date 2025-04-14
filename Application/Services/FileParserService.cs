using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Service to parse files and process valid boxes in batches.
    /// </summary>
    public class FileParserService : IFileParserService
    {
        private readonly ILogger<FileParserService> _logger;
        private readonly IValidationService _validationService;

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
        /// Parses the file and processes valid boxes in batches.
        /// </summary>
        /// <param name="filePath">The path to the file to be parsed.</param>
        /// <param name="processBatchAsync">The function to process a batch of valid boxes.</param>
        /// <param name="batchSize">The size of each batch to be processed.</param>
        public async Task ParseFileAsync(string filePath, Func<List<Box>, Task> processBatchAsync, int batchSize = 1000)
        {
            var validBoxes = new List<Box>();
            var invalidBoxes = new Dictionary<string, List<string>>();
            Box? currentBox = null;

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
                        currentBox = await HandleHeaderAsync(parts, currentBox, validBoxes, invalidBoxes, processBatchAsync, batchSize);
                    }
                    else if (parts[0] == "LINE")
                    {
                        HandleLine(parts, currentBox, invalidBoxes);
                    }
                }

                AddCurrentBoxToAppropriateList(currentBox, validBoxes, invalidBoxes);

                if (validBoxes.Count > 0)
                {
                    await processBatchAsync(validBoxes);
                }

                LogInvalidBoxes(invalidBoxes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing file: {filePath}", filePath);
                throw;
            }
        }

        private async Task<Box?> HandleHeaderAsync(string[] parts, Box? currentBox, List<Box> validBoxes, Dictionary<string, List<string>> invalidBoxes, Func<List<Box>, Task> processBatchAsync, int batchSize)
        {
            if (currentBox != null)
            {
                AddCurrentBoxToAppropriateList(currentBox, validBoxes, invalidBoxes);

                if (validBoxes.Count >= batchSize)
                {
                    await processBatchAsync(validBoxes);
                    validBoxes.Clear();
                }
            }

            try
            {
                _validationService.ValidateHeader(parts);
                return ProcessHeader(parts);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Validation error in header: {header}", string.Join(' ', parts));
                if (currentBox != null)
                {
                    if (!invalidBoxes.ContainsKey(currentBox.Identifier))
                    {
                        invalidBoxes[currentBox.Identifier] = new List<string>();
                    }
                }
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

        private void AddCurrentBoxToAppropriateList(Box? currentBox, List<Box> validBoxes, Dictionary<string, List<string>> invalidBoxes)
        {
            if (currentBox != null)
            {
                if (!invalidBoxes.ContainsKey(currentBox.Identifier))
                {
                    validBoxes.Add(currentBox);
                }
                else
                {
                    if (!invalidBoxes.ContainsKey(currentBox.Identifier))
                    {
                        invalidBoxes[currentBox.Identifier] = new List<string>();
                    }
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
