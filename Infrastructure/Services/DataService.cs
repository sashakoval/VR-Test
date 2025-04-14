using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service to handle data operations related to boxes.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IDbContextFactory<VRDbContext> _dbContextFactory;
        private readonly ILogger<DataService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="logger">The logger instance.</param>
        public DataService(IDbContextFactory<VRDbContext> dbContextFactory, ILogger<DataService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Saves the list of boxes to the database.
        /// </summary>
        /// <param name="boxes">The list of boxes to be saved.</param>
        public async Task SaveBoxesAsync(List<Box> boxes)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var savedCounter = 0;

            var existingBoxIdentifiers = await dbContext.Boxes
                .Select(b => b.Identifier)
                .ToDictionaryAsync(id => id);

            foreach (var box in boxes)
            {
                if (!existingBoxIdentifiers.ContainsKey(box.Identifier))
                {
                    await dbContext.Boxes.AddAsync(box);
                    savedCounter++;
                }
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Saved {savedCounter} boxes to database", savedCounter);
        }
    }
}
