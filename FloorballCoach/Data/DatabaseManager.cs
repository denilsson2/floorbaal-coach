using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Manager class for handling database operations and migrations
    /// </summary>
    public class DatabaseManager
    {
        private readonly IDbContextFactory _contextFactory;

        public DatabaseManager(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        /// <summary>
        /// Ensures the database is created and all migrations are applied
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync();
        }

        /// <summary>
        /// Applies pending migrations to the database
        /// </summary>
        public async Task MigrateDatabaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.MigrateAsync();
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets information about the current database
        /// </summary>
        public string GetDatabaseInfo()
        {
            var config = _contextFactory.GetConfiguration();
            return $"Provider: {config.Provider}, Connection: {MaskConnectionString(config.ConnectionString)}";
        }

        private string MaskConnectionString(string connectionString)
        {
            // Mask passwords in connection strings
            if (connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase))
            {
                var parts = connectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "Password=***";
                    }
                }
                return string.Join(";", parts);
            }
            return connectionString;
        }
    }
}
