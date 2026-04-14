using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Factory for creating FloorballDbContext with configurable database providers
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        private readonly DatabaseConfiguration _configuration;

        public DbContextFactory(DatabaseConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Creates a FloorballDbContext with the configured database provider
        /// </summary>
        public FloorballDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FloorballDbContext>();

            switch (_configuration.Provider)
            {
                case DatabaseProvider.SQLite:
                    optionsBuilder.UseSqlite(_configuration.ConnectionString);
                    break;

                case DatabaseProvider.SqlServer:
                    // Requires: Microsoft.EntityFrameworkCore.SqlServer NuGet package
                    ConfigureSqlServer(optionsBuilder);
                    break;

                case DatabaseProvider.PostgreSQL:
                    // Requires: Npgsql.EntityFrameworkCore.PostgreSQL NuGet package
                    ConfigurePostgreSQL(optionsBuilder);
                    break;

                case DatabaseProvider.MySQL:
                    // Requires: Pomelo.EntityFrameworkCore.MySql NuGet package
                    ConfigureMySQL(optionsBuilder);
                    break;

                default:
                    throw new NotSupportedException($"Database provider {_configuration.Provider} is not supported");
            }

            return new FloorballDbContext(optionsBuilder.Options);
        }

        private void ConfigureSqlServer(DbContextOptionsBuilder<FloorballDbContext> optionsBuilder)
        {
            try
            {
                // Use reflection to call UseSqlServer if the package is available
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "Microsoft.EntityFrameworkCore.SqlServer");
                
                if (assembly == null)
                {
                    throw new InvalidOperationException(
                        "SQL Server provider not found. Install NuGet package: Microsoft.EntityFrameworkCore.SqlServer");
                }

                var type = assembly.GetType("Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions");
                var method = type?.GetMethods()
                    .FirstOrDefault(m => m.Name == "UseSqlServer" && 
                                        m.GetParameters().Length == 2 &&
                                        m.GetParameters()[1].ParameterType == typeof(string));

                if (method == null)
                {
                    throw new InvalidOperationException("UseSqlServer method not found");
                }

                method.Invoke(null, new object[] { optionsBuilder, _configuration.ConnectionString });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to configure SQL Server. Make sure you have installed: dotnet add package Microsoft.EntityFrameworkCore.SqlServer", 
                    ex);
            }
        }

        private void ConfigurePostgreSQL(DbContextOptionsBuilder<FloorballDbContext> optionsBuilder)
        {
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "Npgsql.EntityFrameworkCore.PostgreSQL");
                
                if (assembly == null)
                {
                    throw new InvalidOperationException(
                        "PostgreSQL provider not found. Install NuGet package: Npgsql.EntityFrameworkCore.PostgreSQL");
                }

                var type = assembly.GetType("Microsoft.EntityFrameworkCore.NpgsqlDbContextOptionsBuilderExtensions");
                var method = type?.GetMethods()
                    .FirstOrDefault(m => m.Name == "UseNpgsql" && 
                                        m.GetParameters().Length == 2 &&
                                        m.GetParameters()[1].ParameterType == typeof(string));

                if (method == null)
                {
                    throw new InvalidOperationException("UseNpgsql method not found");
                }

                method.Invoke(null, new object[] { optionsBuilder, _configuration.ConnectionString });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to configure PostgreSQL. Make sure you have installed: dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL", 
                    ex);
            }
        }

        private void ConfigureMySQL(DbContextOptionsBuilder<FloorballDbContext> optionsBuilder)
        {
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name?.StartsWith("Pomelo.EntityFrameworkCore.MySql") == true);
                
                if (assembly == null)
                {
                    throw new InvalidOperationException(
                        "MySQL provider not found. Install NuGet package: Pomelo.EntityFrameworkCore.MySql");
                }

                var type = assembly.GetType("Microsoft.EntityFrameworkCore.MySqlDbContextOptionsBuilderExtensions");
                if (type == null)
                {
                    type = assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("MySqlDbContextOptionsBuilderExtensions"));
                }

                var method = type?.GetMethods()
                    .FirstOrDefault(m => m.Name == "UseMySql" && m.GetParameters().Length >= 2);

                if (method == null)
                {
                    throw new InvalidOperationException("UseMySql method not found");
                }

                // Get ServerVersion type
                var serverVersionType = assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerVersion");
                var autoDetectMethod = serverVersionType?.GetMethod("AutoDetect");
                
                if (autoDetectMethod == null)
                {
                    throw new InvalidOperationException("ServerVersion.AutoDetect method not found");
                }

                var serverVersion = autoDetectMethod.Invoke(null, new object[] { _configuration.ConnectionString });
                
                if (serverVersion == null)
                {
                    throw new InvalidOperationException("Failed to detect MySQL server version");
                }
                
                method.Invoke(null, new object[] { optionsBuilder, _configuration.ConnectionString, serverVersion });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to configure MySQL. Make sure you have installed: dotnet add package Pomelo.EntityFrameworkCore.MySql", 
                    ex);
            }
        }

        /// <summary>
        /// Gets the current database configuration
        /// </summary>
        public DatabaseConfiguration GetConfiguration()
        {
            return _configuration;
        }
    }
}
