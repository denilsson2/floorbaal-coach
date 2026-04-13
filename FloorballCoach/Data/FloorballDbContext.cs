using Microsoft.EntityFrameworkCore;
using FloorballCoach.Models;
using System.IO;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Entity Framework DbContext for the floorball database
    /// </summary>
    public class FloorballDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<MatchSetup> MatchSetups { get; set; }
        public DbSet<MatchStatistics> MatchStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Creates the database in the AppData/Local folder
            var appDataPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "FloorballCoach");

            Directory.CreateDirectory(appDataPath);
            var dbPath = Path.Combine(appDataPath, "floorball.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Line relationships
            modelBuilder.Entity<Line>()
                .HasOne(l => l.Center)
                .WithMany()
                .HasForeignKey(l => l.CenterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Line>()
                .HasOne(l => l.LeftForward)
                .WithMany()
                .HasForeignKey(l => l.LeftForwardId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Line>()
                .HasOne(l => l.RightForward)
                .WithMany()
                .HasForeignKey(l => l.RightForwardId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Line>()
                .HasOne(l => l.LeftBack)
                .WithMany()
                .HasForeignKey(l => l.LeftBackId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Line>()
                .HasOne(l => l.RightBack)
                .WithMany()
                .HasForeignKey(l => l.RightBackId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure MatchSetup relationships
            modelBuilder.Entity<MatchSetup>()
                .HasOne(ms => ms.StartingGoalkeeper)
                .WithMany()
                .HasForeignKey(ms => ms.StartingGoalkeeperId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MatchSetup>()
                .HasOne(ms => ms.BackupGoalkeeper)
                .WithMany()
                .HasForeignKey(ms => ms.BackupGoalkeeperId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
