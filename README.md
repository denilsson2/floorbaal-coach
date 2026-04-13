# Floorball Coach

En WPF-applikation för innebandytränare som hjälper till med laguttagning och spelarhantering.

## Funktioner

### Implementerade funktioner
- **Spelardatabas**: Hantera spelare med detaljerad information
  - Namn, tröjnummer, position, ålder
  - Statistik: matcher, mål, assist, poäng, utvisningar
  - Sökfunktion för att snabbt hitta spelare
  
- **Laguttagning med Drag-and-Drop**: 
  - Dra spelare från databasen till specifika positioner
  - 3 kedjor med 5 spelare vardera (1 center, 2 forwards, 2 backar)
  - 2 målvakter (starting och backup)
  - Bänk för extra utespelare (max 3)
  - Visuell representation av hela laget

### Framtida expansionsmöjligheter
- **Matchstatistik**: Registrera skott, mål, assist under match
- **Matchhistorik**: Spara och granska tidigare uppställningar
- **Rapporter**: Generera statistikrapporter för spelare och matcher
- **Export**: Exportera uppställningar och statistik
- **Mobilapp**: Konvertera till mobilapp för användning under matcher

## Teknologi

- **Framework**: .NET 8 / WPF (Windows Presentation Foundation)
- **Språk**: C#
- **Databas**: SQLite (lokal databas)
- **Arkitektur**: MVVM (Model-View-ViewModel)
- **ORM**: Entity Framework Core
- **UI Pattern**: Drag-and-Drop för intuitiv laguttagning

## Projektstruktur

```
FloorballCoach/
├── Models/              # Datamodeller (Player, Position, Line, MatchSetup)
├── ViewModels/          # ViewModels för MVVM-mönstret
├── Views/               # UI-komponenter (XAML)
├── Data/                # Databaskontxt och repositories
├── Services/            # Tjänster (DatabaseSeeder)
├── Helpers/             # Hjälpklasser (RelayCommand, ViewModelBase)
└── App.xaml             # Applikationskonfiguration
```

## Kom igång

### Krav
- Windows 10/11
- .NET 8 SDK

### Installation och körning

1. **Klona eller öppna projektet**:
   ```powershell
   cd C:\git\coach
   ```

2. **Bygg projektet**:
   ```powershell
   cd FloorballCoach
   dotnet build
   ```

3. **Kör applikationen**:
   ```powershell
   dotnet run
   ```

### Första gången
När du startar applikationen första gången:
- En SQLite-databas skapas automatiskt i `%LOCALAPPDATA%\FloorballCoach\floorball.db`
- Databasen fylls med exempelspelare (16 spelare med olika positioner)

## Användning

### Spelardatabas
1. Klicka på **"Player Database"** i navigeringen
2. Se alla spelare med deras statistik
3. Använd sökfältet för att filtrera spelare
4. Använd knapparna för att:
   - **Add Player**: Lägg till ny spelare
   - **Edit Player**: Redigera vald spelare
   - **Delete Player**: Ta bort vald spelare

### Laguttagning
1. Klicka på **"Lineup"** i navigeringen
2. Dra spelare från listan till vänster till önskade positioner
3. Bygg upp ditt lag:
   - Välj starting och backup målvakt
   - Fyll i alla tre kedjor (5 spelare per kedja)
   - Lägg till bänkspelare (max 3 utespelare)
4. Klicka **"Save Lineup"** för att spara uppställningen
5. Klicka **"Clear Lineup"** för att börja om

### Positioner
- **Goalkeeper**: Målvakt
- **Center**: Centerforward
- **Forward**: Ytterforward
- **Back**: Back

## Databasstruktur

### Players (Spelare)
- Personliga uppgifter: Namn, ålder, tröjnummer
- Position och status
- Statistik: Matcher, mål, assist, utvisningar
- Valfri bildväg för spelarbild

### Lines (Kedjor)
- Namn på kedjan
- 5 positioner: Center, Left Forward, Right Forward, Left Back, Right Back

### MatchSetup (Matchuppställning)
- Matchnamn och datum
- Starting och backup målvakt
- 3 kedjor
- Bänkspelare
- Anteckningar

### MatchStatistics (Matchstatistik)
- Detaljerad statistik per spelare och match
- Mål, assist, skott, speltid
- Förberedd för framtida expansion

## Utveckling

### Lägga till nya funktioner
Projektet är byggt för att enkelt expanderas:

1. **Nya datamodeller**: Lägg till i `Models/`
2. **Nya vyer**: Skapa XAML i `Views/` och motsvarande ViewModel i `ViewModels/`
3. **Databasändringar**: Uppdatera `FloorballDbContext` och kör migrations

### Kod-språk
- **Kod**: All kod är skriven på engelska (variabler, klasser, metoder)
- **UI**: Användargrä

nssnitt kan anpassas till svenskt eller engelskt språk
- **Kommentarer**: Kan vara på svenska eller engelska

## Licens

Detta är ett öppet projekt för tränare och lag att använda fritt.

## Framtida utveckling

### Kortsiktigt
- Dialog för att lägga till/redigera spelare
- Validering av positioner (t.ex. varna om forward läggs som målvakt)
- Spara och ladda matchuppställningar
- Utskriftsfunktion för laguppställning

### Långsiktigt
- Matchstatistik med realtidsregistrering
- Grafiska rapporter och analyser
- Import/export av spelardatabas
- Integration med föreningsregister
- Mobilapp (Xamarin/MAUI)
- Cloud-synkning mellan enheter

## Support

För frågor eller problem, skapa ett issue i projektet.
