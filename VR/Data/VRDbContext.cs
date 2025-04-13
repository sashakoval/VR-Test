using Microsoft.EntityFrameworkCore;
using VR.Models;

namespace VR.Data
{
    public class VRDbContext : DbContext
    {
        public VRDbContext(DbContextOptions<VRDbContext> options) : base(options) { }

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Box.Content> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Box>().HasKey(b => b.Identifier);
            modelBuilder.Entity<Box.Content>().HasKey(c => new { c.PoNumber, c.Isbn });
        }
    }
}
