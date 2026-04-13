using System.Configuration;
using System.Data;
using System.Windows;
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
        // Register DbContext
        services.AddDbContext<FloorballDbContext>();

        // Register repositories
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        // Register services
        services.AddTransient<DatabaseSeeder>();

        // Register ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<PlayerDatabaseViewModel>();
        services.AddTransient<RosterViewModel>();
        services.AddTransient<LineupViewModel>();

        // Register MainWindow
        services.AddSingleton<MainWindow>();
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



