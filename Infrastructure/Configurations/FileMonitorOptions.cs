namespace Infrastructure.Configurations
{
    /// <summary>
    /// Options for file monitoring.
    /// </summary>
    public class FileMonitorOptions
    {
        /// <summary>
        /// Gets or sets the folder to watch for new or changed files.
        /// </summary>
        public required string WatchFolder { get; set; }
    }
}
