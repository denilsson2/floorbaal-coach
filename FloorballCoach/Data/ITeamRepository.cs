using System.Collections.Generic;
using System.Threading.Tasks;
using FloorballCoach.Models;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Repository interface for managing team operations
    /// </summary>
    public interface ITeamRepository
    {
        Task<List<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task<List<Team>> GetActiveTeamsAsync();
        Task<Team> AddTeamAsync(Team team);
        Task<Team> UpdateTeamAsync(Team team);
        Task<bool> DeleteTeamAsync(int id);
        
        // Roster management
        Task<List<Player>> GetTeamRosterAsync(int teamId);
        Task<List<Player>> GetAvailablePlayersAsync(int teamId);
        Task<bool> AddPlayerToRosterAsync(int teamId, int playerId);
        Task<bool> RemovePlayerFromRosterAsync(int teamId, int playerId);
        Task<bool> IsPlayerInRosterAsync(int teamId, int playerId);
    }
}
