using VR.Data;
using VR.Interfaces;
using VR.Models;
using Microsoft.EntityFrameworkCore;

namespace VR.Services
{
    /// <summary>
    /// Service to handle data operations related to boxes.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly VRDbContext _dbContext;
        private readonly ILogger<DataService> _logger;
        private const int DefaultBatchSize = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DataService(VRDbContext dbContext, ILogger<DataService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Saves the list of boxes to the database in batches.
        /// </summary>
        /// <param name="boxes">The list of boxes to be saved.</param>
        /// <param name="batchSize">The size of each batch to be saved.</param>
        public async Task SaveBoxesAsync(List<Box> boxes, int batchSize = DefaultBatchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentException("Batch size must be greater than zero.", nameof(batchSize));
            }

            int savedCount = 0;

            for (int i = 0; i < boxes.Count; i += batchSize)
            {
                var batch = boxes.GetRange(i, Math.Min(batchSize, boxes.Count - i));
                foreach (var box in batch)
                {
                    if (!await BoxExistsAsync(box.Identifier))
                    {
                        await _dbContext.Boxes.AddAsync(box);
                        savedCount++;
                    }
                    else
                    {
                        _logger.LogWarning("Box with identifier {identifier} already exists in the database", box.Identifier);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Saved {count} boxes to database", savedCount);
            }
        }

        /// <summary>
        /// Checks if a box with the specified identifier exists in the database.
        /// </summary>
        /// <param name="identifier">The identifier of the box to check.</param>
        /// <returns>True if the box exists, otherwise false.</returns>
        public async Task<bool> BoxExistsAsync(string identifier)
        {
            return await _dbContext.Boxes.AnyAsync(b => b.Identifier == identifier);
        }
    }
}
