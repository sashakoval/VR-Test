﻿using VR.Models;

namespace VR.Interfaces
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
        Task SaveBoxesAsync(List<Box> boxes, int batchSize);

        /// <summary>
        /// Checks if a box with the specified identifier exists in the database.
        /// </summary>
        /// <param name="identifier">The identifier of the box to check.</param>
        /// <returns>True if the box exists, otherwise false.</returns>
        Task<bool> BoxExistsAsync(string identifier);
    }
}
