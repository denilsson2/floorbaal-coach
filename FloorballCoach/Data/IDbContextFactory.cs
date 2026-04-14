namespace FloorballCoach.Data
{
    /// <summary>
    /// Factory interface for creating database contexts with different configurations
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Creates a configured FloorballDbContext instance
        /// </summary>
        FloorballDbContext CreateDbContext();

        /// <summary>
        /// Gets the current database configuration
        /// </summary>
        DatabaseConfiguration GetConfiguration();
    }
}
