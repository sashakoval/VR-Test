using VR.Models;

namespace VR.Interfaces
{
    /// <summary>
    /// Interface for file parser services.
    /// </summary>
    public interface IFileParserService
    {
        /// <summary>
        /// Parses the file and processes valid boxes in batches.
        /// </summary>
        /// <param name="filePath">The path to the file to be parsed.</param>
        /// <param name="processBatchAsync">The function to process a batch of valid boxes.</param>
        /// <param name="batchSize">The size of each batch to be processed.</param>
        Task ParseFileAsync(string filePath, Func<List<Box>, Task> processBatchAsync, int batchSize = 1000);
    }
}
