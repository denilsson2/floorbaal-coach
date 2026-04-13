using System.Collections.Generic;
using System.Threading.Tasks;
using FloorballCoach.Models;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Repository interface for managing player database operations
    /// </summary>
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllPlayersAsync();
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<List<Player>> GetPlayersByPositionAsync(Position position);
        Task<Player> AddPlayerAsync(Player player);
        Task<Player> UpdatePlayerAsync(Player player);
        Task<bool> DeletePlayerAsync(int id);
        Task<List<Player>> GetActivePlayersAsync();
    }
}
