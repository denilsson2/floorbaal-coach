# Multi-Team Support Guide

## Översikt

FloorballCoach stöder nu flera lag som kan dela samma spelardatabas. En spelare kan tillhöra flera lag samtidigt.

## Nyckelkoncept

### Arkitektur
- **Player** - Spelardatabasen med alla spelare
- **Team** - Lag i systemet
- **TeamRoster** - Kopplingstabell som bestämmer vilka spelare som tillhör vilket lag

### Många-till-många relation
- ✅ En spelare kan vara med i flera lag samtidigt
- ✅ Ett lag kan ha många spelare
- ✅ Gemensam spelardatabas för alla lag

## Användning

### 1. Skapa ett nytt lag

1. Gå till fliken **"Lag"** i huvudmenyn
2. Fyll i lagnamn (obligatoriskt)
3. Valfritt: Lägg till kortnamn och lagfärg
4. Klicka på **"Skapa lag"**

### 2. Välj aktivt lag

- Klicka på ett lag i listan för att välja det som aktivt
- Det valda laget visas i "Aktivt lag"-rutan
- Detta lag används automatiskt i Trupp och Uppställning

### 3. Hantera trupp för ett lag

1. Se till att rätt lag är valt i **"Lag"**-fliken
2. Gå till **"Trupp"**-fliken
3. Det valda laget visas högst upp
4. **Tillgängliga spelare** - Visa alla spelare som INTE är i detta lags trupp
5. **Trupp** - Visar spelare som är med i detta lag
6. Lägg till/ta bort spelare för detta specifika lag

### 4. Spelardatabas

- **"Spelardatabas"** innehåller ALLA spelare oavsett lag
- Lägg till nya spelare här
- Spelare kan sedan läggas till i valfritt lags trupp

## Exempel: Hantera flera lag

### Scenario: Du har ett A-lag och ett Juniorlag

1. **Skapa lagen:**
   - Skapa "A-laget" 
   - Skapa "Juniorlag"

2. **Lägg till spelare i spelardatabasen:**
   - Gå till "Spelardatabas"
   - Lägg till alla spelare (både seniorer och juniorer)

3. **Bygg A-lagets trupp:**
   - Välj "A-laget" i Lag-fliken
   - Gå till Trupp-fliken
   - Lägg till erfarna spelare i truppen

4. **Bygg Juniorlagets trupp:**
   - Välj "Juniorlag" i Lag-fliken
   - Gå till Trupp-fliken
   - Lägg till juniorer i truppen

5. **Spelare i båda lagen:**
   - En spelare kan vara med i båda trupper
   - Lägg bara till samma spelare i bådas trupperna

## API för utvecklare

### TeamRepository

```csharp
// Hämta alla lag
var teams = await _teamRepository.GetAllTeamsAsync();

// Hämta ett lags trupp
var roster = await _teamRepository.GetTeamRosterAsync(teamId);

// Hämta tillgängliga spelare (som inte är i laget)
var available = await _teamRepository.GetAvailablePlayersAsync(teamId);

// Lägg till spelare i trupp
await _teamRepository.AddPlayerToRosterAsync(teamId, playerId);

// Ta bort spelare från trupp
await _teamRepository.RemovePlayerFromRosterAsync(teamId, playerId);

// Kolla om spelare är i trupp
bool inRoster = await _teamRepository.IsPlayerInRosterAsync(teamId, playerId);
```

### Skapa lag programmatiskt

```csharp
var team = new Team
{
    Name = "A-laget",
    ShortName = "A",
    TeamColor = "#FF0000",
    IsActive = true
};

var createdTeam = await _teamRepository.AddTeamAsync(team);
```

## Databasschema

### Teams-tabell
| Kolumn | Typ | Beskrivning |
|--------|-----|-------------|
| Id | int | Primärnyckel |
| Name | string | Lagnamn (obligatoriskt) |
| ShortName | string | Kortnamn (valfritt) |
| TeamColor | string | Lagfärg (valfritt) |
| LogoPath | string | Sökväg till logotyp |
| CreatedDate | DateTime | När laget skapades |
| IsActive | bool | Om laget är aktivt |
| Notes | string | Anteckningar |

### TeamRoster-tabell (Junction)
| Kolumn | Typ | Beskrivning |
|--------|-----|-------------|
| Id | int | Primärnyckel |
| TeamId | int | Foreign key till Teams |
| PlayerId | int | Foreign key till Players |
| JoinedDate | DateTime | När spelaren gick med i laget |
| IsActive | bool | Om spelaren är aktiv i truppen |

**Unique constraint:** (TeamId, PlayerId) - en spelare kan bara finnas en gång per lag

## Migration från äldre version

Om du uppgraderar från en version utan multi-team support:

### Automatisk migration

När du startar appen första gången skapas de nya tabellerna automatiskt:
- `Teams`
- `TeamRosters`

### Manuell datamigration (om du hade spelare i roster tidigare)

```csharp
// 1. Skapa ett standardlag
var defaultTeam = new Team { Name = "Mitt lag", IsActive = true };
await _teamRepository.AddTeamAsync(defaultTeam);

// 2. Flytta befintliga roster-spelare till det nya laget
var playersInRoster = await _playerRepository.GetActivePlayersAsync();
foreach (var player in playersInRoster.Where(p => p.IsInRoster))
{
    await _teamRepository.AddPlayerToRosterAsync(defaultTeam.Id, player.Id);
}
```

## Tips och tricks

### Best practices
1. **Namnge lag tydligt** - Använd beskrivande namn som "A-lag", "Juniorlag", etc.
2. **Använd kortnamn** - För enklare identifiering i listor
3. **Sätt lagfärg** - Hjälper visuellt skilja lag åt (framtida funktion)
4. **Regelbunden backup** - Säkerhetskopiera databasen innan stora ändringar

### Vanliga användningsfall

**Träningsgrupper:**
- Skapa olika lag för olika träningsgrupper
- Spelare kan vara med i flera grupper

**Säsongshantering:**
- Skapa nya lag för varje säsong
- Behåll historisk data för gamla lag

**Turneringar:**
- Skapa specifika turneringstrupper
- Dra från huvudtruppen

## Felsökning

### Problem: Kan inte se spelare i truppen
- ✅ Kontrollera att rätt lag är valt i Lag-fliken
- ✅ Kontrollera att spelaren har lagts till i DET lagets trupp

### Problem: Spelare syns inte som tillgänglig
- ✅ Spelaren kan redan vara i detta lags trupp
- ✅ Kontrollera att spelaren är aktiv i spelardatabasen

### Problem: Kan inte ta bort lag
- ✅ Se till att inga andra delar av systemet använder laget
- ✅ Kontrollera att du har rätt behörigheter

## Framtida funktioner

Planerade förbättringar:
- 📅 Lagstatistik och historik
- 👥 Roller i lag (kapten, assisterande, etc.)
- 📊 Jämförelse mellan lags prestationer  
- 🎨 Visuell färgkodning baserat på lagfärg
- 📱 Export/import av trupper
