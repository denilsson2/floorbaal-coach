# Snabbguide: Byta Databas

## ✅ Vad har skapats?

Din FloorballCoach app har nu en flexibel databasarkitektur:

### 📁 Nya filer:
- `Data/DatabaseConfiguration.cs` - Konfigurationsklasser för olika databaser
- `Data/IDbContextFactory.cs` - Interface för factory pattern
- `Data/DbContextFactory.cs` - Factory för att skapa DbContext
- `Data/DatabaseManager.cs` - Hanterar databas operation
- `Data/DatabaseConfigurationLoader.cs` - Läser från appsettings.json
- `Data/README.md` - Detaljerad dokumentation
- `appsettings.json` - Konfigurationsfil

### 🔄 Uppdaterade filer:
- `Data/FloorballDbContext.cs` - Stöder nu dependency injection
- `App.xaml.cs` - Använder factory pattern
- `FloorballCoach.csproj` - Klar för olika databaser

## 🚀 Kom igång

### Steg 1: Bygg projektet
```powershell
dotnet build
```

### Steg 2: Testa att det fungerar (SQLite som standard)
```powershell
dotnet run
```

Appen använder nu automatiskt lokal SQLite som innan - ingen förändring i beteende!

## 📦 Byta till Azure SQL Database

### Steg 1: Installera NuGet-paket
```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Steg 2: Redigera appsettings.json
```json
{
  "Database": {
    "Provider": "SqlServer",
    "SqlServer": {
      "Server": "din-server.database.windows.net",
      "Database": "floorballcoach",
      "Username": "adminuser",
      "Password": "dittLösenord123!"
    }
  }
}
```

### Steg 3: Kör applikationen
```powershell
dotnet run
```

## 🔐 Säker konfiguration med miljövariabler

Istället för att lagra lösenord i appsettings.json, använd miljövariabler:

### Windows PowerShell:
```powershell
$env:DB_PROVIDER = "SqlServer"
$env:DB_SERVER = "your-server.database.windows.net"
$env:DB_NAME = "floorballcoach"
$env:DB_USER = "adminuser"
$env:DB_PASSWORD = "YourSecurePassword123!"

dotnet run
```

### .env fil (för utveckling):
Skapa en `.env` fil i projektroten:
```
DB_PROVIDER=SqlServer
DB_SERVER=your-server.database.windows.net
DB_NAME=floorballcoach
DB_USER=adminuser
DB_PASSWORD=YourSecurePassword123!
```

## 🧪 Testa anslutningen

Lägg till denna kod i `MainWindow.xaml.cs` för att testa:

```csharp
protected override async void OnContentRendered(EventArgs e)
{
    base.OnContentRendered(e);
    
    // Test database connection on startup
    var dbManager = App.Current.Services.GetService<DatabaseManager>();
    if (dbManager != null)
    {
        bool isConnected = await dbManager.TestConnectionAsync();
        string info = dbManager.GetDatabaseInfo();
        
        if (isConnected)
        {
            MessageBox.Show(
                $"✓ Databas ansluten!\n\n{info}", 
                "Databasanslutning", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                $"✗ Kunde inte ansluta till databasen\n\n{info}", 
                "Databasfel", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }
    }
}
```

## 📋 Providers som stöds

| Provider | NuGet Paket | Användningsområde |
|----------|-------------|-------------------|
| SQLite | ✅ Redan installerat | Lokal utveckling |
| SQL Server / Azure SQL | Microsoft.EntityFrameworkCore.SqlServer | Azure molndatabas |
| PostgreSQL | Npgsql.EntityFrameworkCore.PostgreSQL | Supabase, Railway |
| MySQL | Pomelo.EntityFrameworkCore.MySql | MySQL molntjänster |

## 🔄 Migrera data mellan databaser

Om du vill flytta data från SQLite till en molndatabas:

1. **Exportera från SQLite**
```powershell
# Använd SQLite browser eller kommando för att exportera data
```

2. **Byt till ny databas** (redigera appsettings.json)

3. **Kör applikationen** - skapar automatiskt tabeller

4. **Importera data** till den nya databasen

## 🆘 Felsökning

### Problem: "Unable to connect"
- ✅ Kontrollera connection string
- ✅ Kolla brandvägg/säkerhetsgrupper i molnet
- ✅ Verifiera användarnamn och lösenord
- ✅ Testa med DatabaseManager.TestConnectionAsync()

### Problem: "Provider not supported"
- ✅ Installera rätt NuGet-paket
- ✅ Kontrollera att Provider-namnet är korrekt i appsettings.json

### Problem: "Tables don't exist"
- ✅ Databasen skapas automatiskt vid första körningen
- ✅ Om problem, använd DatabaseManager.InitializeDatabaseAsync()

## 📚 Mer information

Se [Data/README.md](./Data/README.md) för fullständig dokumentation.
