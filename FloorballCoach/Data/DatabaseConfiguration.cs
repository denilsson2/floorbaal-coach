using System;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Enum for supported database providers
    /// </summary>
    public enum DatabaseProvider
    {
        SQLite,
        SqlServer,
        PostgreSQL,
        MySQL
    }

    /// <summary>
    /// Configuration class for database settings
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// Database provider to use
        /// </summary>
        public DatabaseProvider Provider { get; set; } = DatabaseProvider.SQLite;

        /// <summary>
        /// Connection string for the database
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets default SQLite configuration for local database
        /// </summary>
        public static DatabaseConfiguration GetLocalSQLiteConfig()
        {
            var appDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FloorballCoach");

            System.IO.Directory.CreateDirectory(appDataPath);
            var dbPath = System.IO.Path.Combine(appDataPath, "floorball.db");

            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SQLite,
                ConnectionString = $"Data Source={dbPath}"
            };
        }

        /// <summary>
        /// Creates configuration for Azure SQL Database
        /// </summary>
        public static DatabaseConfiguration GetAzureSqlConfig(string server, string database, string username, string password)
        {
            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SqlServer,
                ConnectionString = $"Server=tcp:{server},1433;Initial Catalog={database};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            };
        }

        /// <summary>
        /// Creates configuration for SQL Server (local or remote)
        /// </summary>
        public static DatabaseConfiguration GetSqlServerConfig(string server, string database, bool integratedSecurity = true, string? username = null, string? password = null)
        {
            string connectionString;
            if (integratedSecurity)
            {
                connectionString = $"Server={server};Database={database};Integrated Security=True;TrustServerCertificate=True;";
            }
            else
            {
                connectionString = $"Server={server};Database={database};User ID={username};Password={password};TrustServerCertificate=True;";
            }

            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SqlServer,
                ConnectionString = connectionString
            };
        }

        /// <summary>
        /// Creates configuration for PostgreSQL (e.g., Supabase, Railway, etc.)
        /// </summary>
        public static DatabaseConfiguration GetPostgreSqlConfig(string host, string database, string username, string password, int port = 5432)
        {
            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.PostgreSQL,
                ConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;"
            };
        }

        /// <summary>
        /// Creates configuration for MySQL
        /// </summary>
        public static DatabaseConfiguration GetMySqlConfig(string server, string database, string username, string password, int port = 3306)
        {
            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.MySQL,
                ConnectionString = $"Server={server};Port={port};Database={database};User={username};Password={password};SslMode=Required;"
            };
        }
    }
}
