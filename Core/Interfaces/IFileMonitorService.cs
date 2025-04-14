namespace Core.Interfaces
{
    /// <summary>
    /// Interface for file monitor services.
    /// </summary>
    public interface IFileMonitorService
    {
        /// <summary>
        /// Starts watching the specified folder for new or changed text files.
        /// </summary>
        void StartWatching();

        /// <summary>
        /// Processes a specified file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to process.</param>
        Task ProcessFileAsync(string filePath);
    }
}
