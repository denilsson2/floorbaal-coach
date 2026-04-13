using FloorballCoach.Models;
using FloorballCoach.Helpers;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// ViewModel for displaying a player card (used for drag-and-drop)
    /// </summary>
    public class PlayerCardViewModel : ViewModelBase
    {
        private Player _player;

        public PlayerCardViewModel(Player player)
        {
            _player = player;
            UpdateProperties();
        }

        public Player Player
        {
            get => _player;
            set
            {
                if (SetProperty(ref _player, value))
                {
                    UpdateProperties();
                }
            }
        }

        private int _id;
        private string _fullName = string.Empty;
        private string _position = string.Empty;
        private int _jerseyNumber;
        private int _age;
        private int _goals;
        private int _assists;
        private int _points;
        private string? _imagePath;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public int JerseyNumber
        {
            get => _jerseyNumber;
            set => SetProperty(ref _jerseyNumber, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public int Goals
        {
            get => _goals;
            set => SetProperty(ref _goals, value);
        }

        public int Assists
        {
            get => _assists;
            set => SetProperty(ref _assists, value);
        }

        public int Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private void UpdateProperties()
        {
            Id = Player.Id;
            FullName = Player.FullName;
            Position = Player.Position.ToString();
            JerseyNumber = Player.JerseyNumber;
            Age = Player.Age;
            Goals = Player.Goals;
            Assists = Player.Assists;
            Points = Player.Points;
            ImagePath = Player.ImagePath;
        }
    }
}
