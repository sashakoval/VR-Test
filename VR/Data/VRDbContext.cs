using Microsoft.EntityFrameworkCore;
using VR.Models;

namespace VR.Data
{
    /// <summary>
    /// Database context for the VR application.
    /// </summary>
    public class VRDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VRDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public VRDbContext(DbContextOptions<VRDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the DbSet of boxes.
        /// </summary>
        public DbSet<Box> Boxes { get; set; }

        /// <summary>
        /// Gets or sets the DbSet of box contents.
        /// </summary>
        public DbSet<Box.Content> Contents { get; set; }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the primary key for the Box entity
            modelBuilder.Entity<Box>().HasKey(b => b.Identifier);

            // Configure the composite primary key for the Box.Content entity
            modelBuilder.Entity<Box.Content>().HasKey(c => new { c.PoNumber, c.Isbn });
        }
    }
}
