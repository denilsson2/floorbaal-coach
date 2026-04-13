using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FloorballCoach.Models;

namespace FloorballCoach.Data
{
    /// <summary>
    /// Repository implementation for player database operations
    /// </summary>
    public class PlayerRepository : IPlayerRepository
    {
        private readonly FloorballDbContext _context;

        public PlayerRepository(FloorballDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _context.Players
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<List<Player>> GetPlayersByPositionAsync(Position position)
        {
            return await _context.Players
                .Where(p => p.Position == position && p.IsActive)
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<Player> AddPlayerAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<bool> DeletePlayerAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Player>> GetActivePlayersAsync()
        {
            return await _context.Players
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
    }
}
