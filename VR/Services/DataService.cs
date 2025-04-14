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

            var existingBoxIdentifiers = await _dbContext.Boxes
                .Select(b => b.Identifier)
                .ToDictionaryAsync(id => id);

            int savedCount = 0;

            for (int i = 0; i < boxes.Count; i += batchSize)
            {
                var batch = boxes.GetRange(i, Math.Min(batchSize, boxes.Count - i));
                foreach (var box in batch)
                {
                    if (!existingBoxIdentifiers.ContainsKey(box.Identifier))
                    {
                        await _dbContext.Boxes.AddAsync(box);
                        savedCount++;
                    }
                }

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Saved {count} boxes to database", savedCount);
            }
        }
    }
}
