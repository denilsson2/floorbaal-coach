using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FloorballCoach.Models;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Repository implementation for team database operations
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        private readonly FloorballDbContext _context;

        public TeamRepository(FloorballDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TeamRosters)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Team>> GetActiveTeamsAsync()
        {
            return await _context.Teams
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Team> AddTeamAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<Team> UpdateTeamAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        // Roster Management
        public async Task<List<Player>> GetTeamRosterAsync(int teamId)
        {
            return await _context.TeamRosters
                .Where(tr => tr.TeamId == teamId && tr.IsActive)
                .Include(tr => tr.Player)
                .Select(tr => tr.Player!)
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<List<Player>> GetAvailablePlayersAsync(int teamId)
        {
            var rosterPlayerIds = await _context.TeamRosters
                .Where(tr => tr.TeamId == teamId && tr.IsActive)
                .Select(tr => tr.PlayerId)
                .ToListAsync();

            return await _context.Players
                .Where(p => p.IsActive && !rosterPlayerIds.Contains(p.Id))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<bool> AddPlayerToRosterAsync(int teamId, int playerId)
        {
            // Check if already exists
            var existing = await _context.TeamRosters
                .FirstOrDefaultAsync(tr => tr.TeamId == teamId && tr.PlayerId == playerId);

            if (existing != null)
            {
                // If exists but inactive, reactivate
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    await _context.SaveChangesAsync();
                }
                return true;
            }

            // Add new roster entry
            var teamRoster = new TeamRoster
            {
                TeamId = teamId,
                PlayerId = playerId,
                IsActive = true
            };

            _context.TeamRosters.Add(teamRoster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePlayerFromRosterAsync(int teamId, int playerId)
        {
            var teamRoster = await _context.TeamRosters
                .FirstOrDefaultAsync(tr => tr.TeamId == teamId && tr.PlayerId == playerId);

            if (teamRoster == null)
                return false;

            // Soft delete - just mark as inactive
            teamRoster.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsPlayerInRosterAsync(int teamId, int playerId)
        {
            return await _context.TeamRosters
                .AnyAsync(tr => tr.TeamId == teamId && tr.PlayerId == playerId && tr.IsActive);
        }
    }
}
