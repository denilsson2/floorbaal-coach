using FloorballCoach.Data;
using FloorballCoach.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FloorballCoach.Services
{
    /// <summary>
    /// Service for seeding the database with sample data
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly FloorballDbContext _context;

        public DatabaseSeeder(FloorballDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Only seed if database is empty
            if (_context.Players.Any())
                return;

            var players = new[]
            {
                // 2008 Players
                new Player { FirstName = "Moses", LastName = "Bernhardsson", Position = Position.Center, JerseyNumber = 1, DateOfBirth = new DateTime(2008, 6, 15), IsActive = true },
                new Player { FirstName = "Benjamin", LastName = "Bernhardsson", Position = Position.Goalkeeper, JerseyNumber = 30, DateOfBirth = new DateTime(2008, 6, 15), IsActive = true },
                new Player { FirstName = "Leo", LastName = "Fransson", Position = Position.Center, JerseyNumber = 2, DateOfBirth = new DateTime(2008, 4, 10), IsActive = true },
                new Player { FirstName = "Hugo", LastName = "Ström", Position = Position.Forward, JerseyNumber = 3, DateOfBirth = new DateTime(2008, 8, 22), IsActive = true },
                new Player { FirstName = "Noah", LastName = "Härenstam", Position = Position.Forward, JerseyNumber = 4, DateOfBirth = new DateTime(2008, 3, 18), IsActive = true },
                
                // 2009 Players
                new Player { FirstName = "Einar", LastName = "Melin", Position = Position.Defender, JerseyNumber = 5, DateOfBirth = new DateTime(2009, 1, 25), IsActive = true },
                new Player { FirstName = "Hilding", LastName = "Wallmo", Position = Position.Center, JerseyNumber = 6, DateOfBirth = new DateTime(2009, 7, 12), IsActive = true },
                new Player { FirstName = "Sigge", LastName = "Forsdahl", Position = Position.Forward, JerseyNumber = 7, DateOfBirth = new DateTime(2009, 9, 8), IsActive = true },
                new Player { FirstName = "Eddie", LastName = "Blychert", Position = Position.Forward, JerseyNumber = 8, DateOfBirth = new DateTime(2009, 5, 14), IsActive = true },
                new Player { FirstName = "Filip", LastName = "Olofsson", Position = Position.Forward, JerseyNumber = 9, DateOfBirth = new DateTime(2009, 11, 20), IsActive = true },
                new Player { FirstName = "Eric", LastName = "Davidsson", Position = Position.Forward, JerseyNumber = 10, DateOfBirth = new DateTime(2009, 2, 28), IsActive = true },
                new Player { FirstName = "Lucas", LastName = "Olsson", Position = Position.Defender, JerseyNumber = 11, DateOfBirth = new DateTime(2009, 6, 5), IsActive = true },
                new Player { FirstName = "Melvin", LastName = "Vikström", Position = Position.Center, JerseyNumber = 12, DateOfBirth = new DateTime(2009, 10, 17), IsActive = true },
                new Player { FirstName = "Elliot", LastName = "Brännhult", Position = Position.Forward, JerseyNumber = 13, DateOfBirth = new DateTime(2009, 4, 9), IsActive = true },
                new Player { FirstName = "Emrik", LastName = "Palmér", Position = Position.Forward, JerseyNumber = 14, DateOfBirth = new DateTime(2009, 8, 23), IsActive = true },
                new Player { FirstName = "Lucas", LastName = "Hadodo", Position = Position.Forward, JerseyNumber = 15, DateOfBirth = new DateTime(2009, 12, 30), IsActive = true },
                new Player { FirstName = "Alex", LastName = "Florin", Position = Position.Defender, JerseyNumber = 16, DateOfBirth = new DateTime(2009, 3, 11), IsActive = true },
                new Player { FirstName = "Ivar", LastName = "Sandström", Position = Position.Forward, JerseyNumber = 17, DateOfBirth = new DateTime(2009, 7, 26), IsActive = true },
                new Player { FirstName = "Simon", LastName = "Lagerqvist", Position = Position.Defender, JerseyNumber = 18, DateOfBirth = new DateTime(2009, 11, 3), IsActive = true },
                
                // 2010 Players
                new Player { FirstName = "Filip", LastName = "Logius", Position = Position.Forward, JerseyNumber = 19, DateOfBirth = new DateTime(2010, 1, 15), IsActive = true },
                new Player { FirstName = "Isak", LastName = "Barkestam", Position = Position.Defender, JerseyNumber = 20, DateOfBirth = new DateTime(2010, 5, 20), IsActive = true },
                new Player { FirstName = "Vilmer", LastName = "Widtfelt", Position = Position.Center, JerseyNumber = 21, DateOfBirth = new DateTime(2010, 9, 12), IsActive = true },
                new Player { FirstName = "Isac", LastName = "Josefsson Korutschka", Position = Position.Defender, JerseyNumber = 22, DateOfBirth = new DateTime(2010, 2, 8), IsActive = true },
                new Player { FirstName = "Gösta", LastName = "Tholin", Position = Position.Goalkeeper, JerseyNumber = 31, DateOfBirth = new DateTime(2010, 6, 25), IsActive = true },
                new Player { FirstName = "Nils", LastName = "Levander", Position = Position.Defender, JerseyNumber = 23, DateOfBirth = new DateTime(2010, 10, 14), IsActive = true },
                new Player { FirstName = "Anton", LastName = "Kero", Position = Position.Forward, JerseyNumber = 24, DateOfBirth = new DateTime(2010, 3, 30), IsActive = true },
                new Player { FirstName = "Oliver", LastName = "Karlström", Position = Position.Forward, JerseyNumber = 25, DateOfBirth = new DateTime(2010, 7, 18), IsActive = true },
                new Player { FirstName = "Gunnar", LastName = "Svärd", Position = Position.Forward, JerseyNumber = 26, DateOfBirth = new DateTime(2010, 11, 22), IsActive = true },
                new Player { FirstName = "Edvin", LastName = "Unoson", Position = Position.Forward, JerseyNumber = 27, DateOfBirth = new DateTime(2010, 4, 5), IsActive = true },
                new Player { FirstName = "Wilmer", LastName = "Jyrkin", Position = Position.Goalkeeper, JerseyNumber = 32, DateOfBirth = new DateTime(2010, 8, 16), IsActive = true },
                new Player { FirstName = "Erik", LastName = "Tullebo", Position = Position.Defender, JerseyNumber = 28, DateOfBirth = new DateTime(2010, 12, 10), IsActive = true },
                new Player { FirstName = "Vide", LastName = "Dyrkell", Position = Position.Defender, JerseyNumber = 29, DateOfBirth = new DateTime(2010, 1, 28), IsActive = true },
                new Player { FirstName = "Walter", LastName = "Alderhammar", Position = Position.Forward, JerseyNumber = 33, DateOfBirth = new DateTime(2010, 5, 7), IsActive = true },
                new Player { FirstName = "Folke", LastName = "Björklund", Position = Position.Defender, JerseyNumber = 34, DateOfBirth = new DateTime(2010, 9, 19), IsActive = true },
                new Player { FirstName = "Jonathan", LastName = "Melkstam", Position = Position.Defender, JerseyNumber = 35, DateOfBirth = new DateTime(2010, 2, 14), IsActive = true },
                new Player { FirstName = "Pontus", LastName = "Andersson", Position = Position.Center, JerseyNumber = 36, DateOfBirth = new DateTime(2010, 6, 21), IsActive = true },
                new Player { FirstName = "Helmer", LastName = "Almqvist", Position = Position.Forward, JerseyNumber = 37, DateOfBirth = new DateTime(2010, 10, 3), IsActive = true },
                new Player { FirstName = "Emil", LastName = "Holmkvist", Position = Position.Forward, JerseyNumber = 38, DateOfBirth = new DateTime(2010, 3, 17), IsActive = true },
                new Player { FirstName = "Wiggo", LastName = "Holmkvist", Position = Position.Forward, JerseyNumber = 39, DateOfBirth = new DateTime(2010, 7, 29), IsActive = true },
                new Player { FirstName = "Alfred", LastName = "Sandell", Position = Position.Goalkeeper, JerseyNumber = 33, DateOfBirth = new DateTime(2010, 11, 11), IsActive = true },
                
                // 2011 Players
                new Player { FirstName = "Bob", LastName = "Lindberg", Position = Position.Forward, JerseyNumber = 40, DateOfBirth = new DateTime(2011, 4, 8), IsActive = true },
                new Player { FirstName = "Anton", LastName = "Skogeryd", Position = Position.Forward, JerseyNumber = 41, DateOfBirth = new DateTime(2011, 8, 24), IsActive = true }
            };

            await _context.Players.AddRangeAsync(players);
            await _context.SaveChangesAsync();
        }
    }
}
