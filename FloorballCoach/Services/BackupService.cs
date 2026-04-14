using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FloorballCoach.Data;
using FloorballCoach.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FloorballCoach.Services
{
    /// <summary>
    /// Service for backing up and restoring database data
    /// </summary>
    public class BackupService
    {
        private readonly FloorballDbContext _context;

        public BackupService(FloorballDbContext context)
        {
            _context = context;
        }

        public class BackupData
        {
            public List<Player> Players { get; set; } = new();
            public List<Team> Teams { get; set; } = new();
            public List<TeamRoster> TeamRosters { get; set; } = new();
            public DateTime BackupDate { get; set; }
            public string Version { get; set; } = "1.0";
        }

        /// <summary>
        /// Export all data to JSON file
        /// </summary>
        public async Task<string> ExportToFileAsync(string filePath)
        {
            var data = new BackupData
            {
                BackupDate = DateTime.Now,
                Players = await _context.Players.ToListAsync(),
                Teams = await _context.Teams.ToListAsync(),
                TeamRosters = await _context.TeamRosters.ToListAsync()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(filePath, json);

            return filePath;
        }

        /// <summary>
        /// Import data from JSON file
        /// </summary>
        public async Task<int> ImportFromFileAsync(string filePath, bool clearExisting = false)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Backup-filen hittades inte", filePath);

            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var data = JsonSerializer.Deserialize<BackupData>(json, options);
            if (data == null)
                throw new InvalidDataException("Ogiltig backup-fil");

            int importedCount = 0;

            // Clear existing data if requested
            if (clearExisting)
            {
                _context.TeamRosters.RemoveRange(_context.TeamRosters);
                _context.Teams.RemoveRange(_context.Teams);
                _context.Players.RemoveRange(_context.Players);
                await _context.SaveChangesAsync();
            }

            // Import players
            foreach (var player in data.Players)
            {
                // Reset ID to let database assign new ones if clearing
                if (clearExisting)
                    player.Id = 0;

                if (!_context.Players.Any(p => p.FirstName == player.FirstName && 
                                               p.LastName == player.LastName && 
                                               p.DateOfBirth == player.DateOfBirth))
                {
                    _context.Players.Add(player);
                    importedCount++;
                }
            }
            await _context.SaveChangesAsync();

            // Import teams
            var teamIdMap = new Dictionary<int, int>(); // Old ID -> New ID
            foreach (var team in data.Teams)
            {
                var oldId = team.Id;
                if (clearExisting)
                    team.Id = 0;

                if (!_context.Teams.Any(t => t.Name == team.Name))
                {
                    _context.Teams.Add(team);
                    await _context.SaveChangesAsync(); // Save to get new ID
                    teamIdMap[oldId] = team.Id;
                    importedCount++;
                }
                else
                {
                    var existing = await _context.Teams.FirstAsync(t => t.Name == team.Name);
                    teamIdMap[oldId] = existing.Id;
                }
            }

            // Import team rosters
            foreach (var roster in data.TeamRosters)
            {
                // Map old team ID to new team ID
                if (teamIdMap.ContainsKey(roster.TeamId))
                {
                    roster.TeamId = teamIdMap[roster.TeamId];
                }

                // Skip if player data is missing
                if (roster.Player == null)
                    continue;

                // Find player by name and date of birth
                var firstName = roster.Player.FirstName;
                var lastName = roster.Player.LastName;
                var dateOfBirth = roster.Player.DateOfBirth;

                var player = await _context.Players
                    .FirstOrDefaultAsync(p => p.FirstName == firstName &&
                                             p.LastName == lastName &&
                                             p.DateOfBirth == dateOfBirth);

                if (player != null)
                {
                    roster.PlayerId = player.Id;
                    roster.Id = 0; // Let database assign new ID

                    if (!await _context.TeamRosters.AnyAsync(tr => tr.TeamId == roster.TeamId && 
                                                                    tr.PlayerId == roster.PlayerId))
                    {
                        _context.TeamRosters.Add(roster);
                        importedCount++;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return importedCount;
        }

        /// <summary>
        /// Get default backup directory
        /// </summary>
        public static string GetDefaultBackupDirectory()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var backupPath = Path.Combine(documentsPath, "FloorballCoach", "Backups");
            Directory.CreateDirectory(backupPath);
            return backupPath;
        }

        /// <summary>
        /// Generate default backup filename
        /// </summary>
        public static string GenerateBackupFileName()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"FloorballCoach_Backup_{timestamp}.json";
        }
    }
}
