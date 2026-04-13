# Snabbguide - Floorball Coach

## Kom igång på 5 minuter

### 1. Öppna och kör applikationen

```powershell
cd C:\git\coach\FloorballCoach
dotnet run
```

### 2. Första starten
När du startar första gången får du:
- 16 exempelspelare i databasen
- 2 målvakter
- 3 centers
- 6 forwards
- 6 backar

### 3. Utforska spelardatabasen
1. Klicka på **"Player Database"** (startsidan)
2. Se alla dina spelare med statistik
3. Använd sökrutan för att filtrera

### 4. Lägg till en ny spelare
1. Klicka på **"Add Player"**
2. Fyll i uppgifterna:
   - Förnamn och efternamn (obligatoriskt)
   - Tröjnummer (obligatoriskt)
   - Position (obligatoriskt)
   - Födelsedatum (obligatoriskt)
   - Statistik (valfritt)
3. Klicka **"Save"**

### 5. Redigera en spelare
1. Klicka på en spelare i listan
2. Klicka **"Edit Player"**
3. Ändra uppgifterna
4. Klicka **"Save"**

### 6. Ta bort en spelare
1. Klicka på en spelare
2. Klicka **"Delete Player"**
3. Bekräfta borttagningen

### 7. Skapa en matchuppställning
1. Klicka på **"Lineup"** i navigationen
2. Dra spelare från listan till vänster
3. Släpp dem på önskade positioner:
   - **Målvakter**: Starting och Backup
   - **Kedja 1, 2, 3**: Varje kedja har 5 positioner
     - Center (C)
     - Left Forward (LF)
     - Right Forward (RF)
     - Left Back (LB)
     - Right Back (RB)
4. Lägg till bänkspelare vid behov
5. Klicka **"Save Lineup"** när du är klar

### 8. Rensa uppställningen
Klicka **"Clear Lineup"** för att börja om från början.

## Tips & Tricks

### Sök snabbt
- Skriv spelarnamn eller tröjnummer i sökfältet
- Filtrering sker automatiskt när du skriver

### Drag-and-Drop tips
- Håll ned vänster musknapp på en spelare
- Dra till önskad position
- Släpp för att placera spelaren
- Drop-zonen blir blå när du kan släppa

### Positionsförklaring
- **Goalkeeper**: Målvakt
- **Center**: Centerforward (oftast spelfördelaren)
- **Forward**: Ytterforward (vänster/höger)
- **Back**: Back/försvarare (vänster/höger)

### Statistik
- **Points** = Mål + Assist (beräknas automatiskt)
- **Age** = Beräknas från födelsedatum

## Databas-plats
Databasen sparas här:
```
C:\Users\[DittAnvändarnamn]\AppData\Local\FloorballCoach\floorball.db
```

## Vanliga frågor

**Q: Kan jag använda samma spelare flera gånger?**
A: För närvarande placeras spelaren på den position du drar till. Framtida version kan stödja flera kedjor med samma spelare.

**Q: Hur sparar jag uppställningar?**
A: Klicka "Save Lineup". Uppställningar sparas i databasen och kan laddas senare (funktionen för att ladda uppställningar kommer i framtida version).

**Q: Kan jag skriva ut uppställningen?**
A: Utskriftsfunktion kommer i framtida version.

**Q: Kan jag importera spelare från Excel?**
A: Import-funktion planeras för framtida version.

**Q: Hur säkerhetskopierar jag databasen?**
A: Kopiera filen `floorball.db` från AppData/Local/FloorballCoach till en säker plats.

## Nästa steg

Efter att du lärt känna grundfunktionerna kan du:
1. Lägga till alla dina riktiga spelare
2. Uppdatera statistik efter varje match
3. Experimentera med olika uppställningar
4. Förbereda dig inför matcher

## Behöver du hjälp?

Kolla in den fullständiga README.md för mer information om:
- Projektstruktur
- Tekniska detaljer
- Framtida funktioner
- Utvecklingsmöjligheter

---

**Lycka till med ditt lag! 🏑**
