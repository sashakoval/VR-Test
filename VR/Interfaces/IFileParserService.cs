using VR.Models;

namespace VR.Interfaces
{
    public interface IFileParserService
    {
        Task ParseFileAsync(string filePath, Func<List<Box>, Task> processBatchAsync, int batchSize = 1000);
    }
}
