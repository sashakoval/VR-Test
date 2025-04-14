using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for data services.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Saves the list of boxes to the database in batches.
        /// </summary>
        /// <param name="boxes">The list of boxes to be saved.</param>
        /// <param name="batchSize">The size of each batch to be saved.</param>
        Task SaveBoxesAsync(List<Box> boxes);
    }
}
