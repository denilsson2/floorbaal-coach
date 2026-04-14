using System;
using System.IO;
using System.Text.Json;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Loads database configuration from appsettings.json
    /// </summary>
    public class DatabaseConfigurationLoader
    {
        private const string SettingsFileName = "appsettings.json";

        public class AppSettings
        {
            public DatabaseSettings? Database { get; set; }
        }

        public class DatabaseSettings
        {
            public string Provider { get; set; } = "SQLite";
            public SQLiteSettings? SQLite { get; set; }
            public SqlServerSettings? SqlServer { get; set; }
            public PostgreSQLSettings? PostgreSQL { get; set; }
            public MySQLSettings? MySQL { get; set; }
        }

        public class SQLiteSettings
        {
            public bool UseDefault { get; set; } = true;
            public string? ConnectionString { get; set; }
        }

        public class SqlServerSettings
        {
            public string? Server { get; set; }
            public string? Database { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public bool UseManagedIdentity { get; set; }
        }

        public class PostgreSQLSettings
        {
            public string? Host { get; set; }
            public int Port { get; set; } = 5432;
            public string? Database { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public class MySQLSettings
        {
            public string? Server { get; set; }
            public int Port { get; set; } = 3306;
            public string? Database { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        /// <summary>
        /// Loads database configuration from appsettings.json with environment variable override
        /// </summary>
        public static DatabaseConfiguration LoadConfiguration()
        {
            // Try to read from appsettings.json
            var appSettings = LoadAppSettings();
            
            if (appSettings?.Database == null)
            {
                // Fall back to default SQLite if no configuration found
                return DatabaseConfiguration.GetLocalSQLiteConfig();
            }

            var dbSettings = appSettings.Database;

            // Check environment variable override for provider
            var providerOverride = Environment.GetEnvironmentVariable("DB_PROVIDER");
            if (!string.IsNullOrEmpty(providerOverride))
            {
                dbSettings.Provider = providerOverride;
            }

            return dbSettings.Provider.ToUpper() switch
            {
                "SQLITE" => LoadSQLiteConfig(dbSettings.SQLite),
                "SQLSERVER" => LoadSqlServerConfig(dbSettings.SqlServer),
                "POSTGRESQL" => LoadPostgreSqlConfig(dbSettings.PostgreSQL),
                "MYSQL" => LoadMySqlConfig(dbSettings.MySQL),
                _ => DatabaseConfiguration.GetLocalSQLiteConfig()
            };
        }

        private static AppSettings? LoadAppSettings()
        {
            try
            {
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var settingsPath = Path.Combine(appDirectory, SettingsFileName);

                if (!File.Exists(settingsPath))
                {
                    return null;
                }

                var json = File.ReadAllText(settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
            }
        }

        private static DatabaseConfiguration LoadSQLiteConfig(SQLiteSettings? settings)
        {
            if (settings?.UseDefault == true || string.IsNullOrEmpty(settings?.ConnectionString))
            {
                return DatabaseConfiguration.GetLocalSQLiteConfig();
            }

            return new DatabaseConfiguration
            {
                Provider = DatabaseProvider.SQLite,
                ConnectionString = settings.ConnectionString!
            };
        }

        private static DatabaseConfiguration LoadSqlServerConfig(SqlServerSettings? settings)
        {
            if (settings == null)
            {
                throw new InvalidOperationException("SqlServer settings not configured");
            }

            // Check for environment variables
            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? settings.Server;
            var database = Environment.GetEnvironmentVariable("DB_NAME") ?? settings.Database;
            var username = Environment.GetEnvironmentVariable("DB_USER") ?? settings.Username;
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? settings.Password;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
            {
                throw new InvalidOperationException("SqlServer configuration incomplete");
            }

            // Check if this is Azure SQL
            if (server.Contains(".database.windows.net", StringComparison.OrdinalIgnoreCase))
            {
                return DatabaseConfiguration.GetAzureSqlConfig(server, database, username!, password!);
            }

            return DatabaseConfiguration.GetSqlServerConfig(
                server, 
                database, 
                string.IsNullOrEmpty(username), 
                username, 
                password);
        }

        private static DatabaseConfiguration LoadPostgreSqlConfig(PostgreSQLSettings? settings)
        {
            if (settings == null)
            {
                throw new InvalidOperationException("PostgreSQL settings not configured");
            }

            var host = Environment.GetEnvironmentVariable("PG_HOST") ?? settings.Host;
            var database = Environment.GetEnvironmentVariable("PG_DATABASE") ?? settings.Database;
            var username = Environment.GetEnvironmentVariable("PG_USER") ?? settings.Username;
            var password = Environment.GetEnvironmentVariable("PG_PASSWORD") ?? settings.Password;

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(database))
            {
                throw new InvalidOperationException("PostgreSQL configuration incomplete");
            }

            return DatabaseConfiguration.GetPostgreSqlConfig(
                host, 
                database, 
                username!, 
                password!, 
                settings.Port);
        }

        private static DatabaseConfiguration LoadMySqlConfig(MySQLSettings? settings)
        {
            if (settings == null)
            {
                throw new InvalidOperationException("MySQL settings not configured");
            }

            var server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? settings.Server;
            var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? settings.Database;
            var username = Environment.GetEnvironmentVariable("MYSQL_USER") ?? settings.Username;
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? settings.Password;

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
            {
                throw new InvalidOperationException("MySQL configuration incomplete");
            }

            return DatabaseConfiguration.GetMySqlConfig(
                server, 
                database, 
                username!, 
                password!, 
                settings.Port);
        }
    }
}
