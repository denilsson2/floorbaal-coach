using System.Configuration;
using System.Data;
using System.Windows;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FloorballCoach.Data;
using FloorballCoach.ViewModels;
using FloorballCoach.Services;

namespace FloorballCoach;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        // Initialize database
        InitializeDatabase();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configure database - Easy to switch between providers!
        var dbConfig = GetDatabaseConfiguration();
        
        // Register database factory and configuration
        services.AddSingleton(dbConfig);
        services.AddSingleton<IDbContextFactory>(sp => new DbContextFactory(dbConfig));
        services.AddSingleton<DatabaseManager>(sp => 
            new DatabaseManager(sp.GetRequiredService<IDbContextFactory>()));

        // Register DbContext factory
        services.AddScoped<FloorballDbContext>(sp =>
        {
            var factory = sp.GetRequiredService<IDbContextFactory>();
            return factory.CreateDbContext();
        });

        // Register repositories
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();

        // Register services
        services.AddTransient<DatabaseSeeder>();

        // Register ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<TeamManagementViewModel>();
        services.AddTransient<PlayerDatabaseViewModel>();
        services.AddTransient<RosterViewModel>();
        services.AddTransient<LineupViewModel>();

        // Register MainWindow
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Configure database provider here - Switch between SQLite, SQL Server, PostgreSQL, etc.
    /// </summary>
    private DatabaseConfiguration GetDatabaseConfiguration()
    {
        // Load configuration from appsettings.json
        // Easy to switch providers by editing appsettings.json or setting environment variables!
        try
        {
            return DatabaseConfigurationLoader.LoadConfiguration();
        }
        catch (Exception ex)
        {
            // Fall back to local SQLite if configuration fails
            System.Diagnostics.Debug.WriteLine($"Failed to load database configuration: {ex.Message}");
            return DatabaseConfiguration.GetLocalSQLiteConfig();
        }

        // ALTERNATIVE: Configure database directly in code (uncomment if preferred)
        /*
        // OPTION 1: Local SQLite (Default - good for development)
        return DatabaseConfiguration.GetLocalSQLiteConfig();

        // OPTION 2: Azure SQL Database
        return DatabaseConfiguration.GetAzureSqlConfig(
            server: Environment.GetEnvironmentVariable("AZURE_SQL_SERVER") ?? "your-server.database.windows.net",
            database: Environment.GetEnvironmentVariable("AZURE_SQL_DATABASE") ?? "floorballcoach",
            username: Environment.GetEnvironmentVariable("AZURE_SQL_USER") ?? "adminuser",
            password: Environment.GetEnvironmentVariable("AZURE_SQL_PASSWORD") ?? "YourPassword123!"
        );

        // OPTION 3: PostgreSQL / Supabase
        return DatabaseConfiguration.GetPostgreSqlConfig(
            host: Environment.GetEnvironmentVariable("PG_HOST") ?? "db.supabase.co",
            database: Environment.GetEnvironmentVariable("PG_DATABASE") ?? "postgres",
            username: Environment.GetEnvironmentVariable("PG_USER") ?? "postgres",
            password: Environment.GetEnvironmentVariable("PG_PASSWORD") ?? "your-password"
        );

        // OPTION 4: Local SQL Server
        return DatabaseConfiguration.GetSqlServerConfig(
            server: "localhost\\SQLEXPRESS",
            database: "FloorballCoach",
            integratedSecurity: true
        );
        */
    }

    private void InitializeDatabase()
    {
        using var scope = _serviceProvider!.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FloorballDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        
        // Create database if it doesn't exist
        context.Database.EnsureCreated();
        
        // Seed with sample data
        seeder.SeedAsync().Wait();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}



