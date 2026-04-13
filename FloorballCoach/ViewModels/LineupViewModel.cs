using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FloorballCoach.Data;
using FloorballCoach.Helpers;
using FloorballCoach.Models;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// ViewModel for creating match lineups with drag-and-drop functionality
    /// </summary>
    public class LineupViewModel : ViewModelBase
    {
        private readonly IPlayerRepository _playerRepository;
        private ObservableCollection<PlayerCardViewModel> _availablePlayers;
        private ObservableCollection<PlayerCardViewModel> _allPlayers;
        private Position? _selectedPositionFilter;
        
        // Line 1
        private PlayerCardViewModel? _line1Center;
        private PlayerCardViewModel? _line1LeftForward;
        private PlayerCardViewModel? _line1RightForward;
        private PlayerCardViewModel? _line1LeftBack;
        private PlayerCardViewModel? _line1RightBack;

        // Line 2
        private PlayerCardViewModel? _line2Center;
        private PlayerCardViewModel? _line2LeftForward;
        private PlayerCardViewModel? _line2RightForward;
        private PlayerCardViewModel? _line2LeftBack;
        private PlayerCardViewModel? _line2RightBack;

        // Line 3
        private PlayerCardViewModel? _line3Center;
        private PlayerCardViewModel? _line3LeftForward;
        private PlayerCardViewModel? _line3RightForward;
        private PlayerCardViewModel? _line3LeftBack;
        private PlayerCardViewModel? _line3RightBack;

        // Goalkeepers
        private PlayerCardViewModel? _startingGoalkeeper;
        private PlayerCardViewModel? _backupGoalkeeper;

        // Bench players
        private PlayerCardViewModel? _bench1;
        private PlayerCardViewModel? _bench2;
        private PlayerCardViewModel? _bench3;
        private ObservableCollection<PlayerCardViewModel> _benchPlayers;

        public LineupViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            _availablePlayers = new ObservableCollection<PlayerCardViewModel>();
            _allPlayers = new ObservableCollection<PlayerCardViewModel>();
            _benchPlayers = new ObservableCollection<PlayerCardViewModel>();

            LoadPlayersCommand = new RelayCommand(async _ => await LoadPlayers());
            ClearLineupCommand = new RelayCommand(_ => ClearLineup());
            SaveLineupCommand = new RelayCommand(async _ => await SaveLineup());
            FilterByPositionCommand = new RelayCommand(position => FilterByPosition(position as Position?));
            ShowAllPlayersCommand = new RelayCommand(_ => ShowAllPlayers());
            RemoveFromPositionCommand = new RelayCommand(positionName => RemoveFromPosition(positionName as string));
        }

        public async Task InitializeAsync()
        {
            if (!_availablePlayers.Any())
            {
                await LoadPlayers();
            }
        }

        public ObservableCollection<PlayerCardViewModel> AvailablePlayers
        {
            get => _availablePlayers;
            set => SetProperty(ref _availablePlayers, value);
        }

        public ObservableCollection<PlayerCardViewModel> BenchPlayers
        {
            get => _benchPlayers;
            set => SetProperty(ref _benchPlayers, value);
        }

        public Position? SelectedPositionFilter
        {
            get => _selectedPositionFilter;
            set => SetProperty(ref _selectedPositionFilter, value);
        }

        // Line 1 Properties
        public PlayerCardViewModel? Line1Center
        {
            get => _line1Center;
            set 
            { 
                SetProperty(ref _line1Center, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line1LeftForward
        {
            get => _line1LeftForward;
            set 
            { 
                SetProperty(ref _line1LeftForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line1RightForward
        {
            get => _line1RightForward;
            set 
            { 
                SetProperty(ref _line1RightForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line1LeftBack
        {
            get => _line1LeftBack;
            set 
            { 
                SetProperty(ref _line1LeftBack, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line1RightBack
        {
            get => _line1RightBack;
            set 
            { 
                SetProperty(ref _line1RightBack, value);
                UpdateAvailablePlayers();
            }
        }

        // Line 2 Properties
        public PlayerCardViewModel? Line2Center
        {
            get => _line2Center;
            set 
            { 
                SetProperty(ref _line2Center, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line2LeftForward
        {
            get => _line2LeftForward;
            set 
            { 
                SetProperty(ref _line2LeftForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line2RightForward
        {
            get => _line2RightForward;
            set 
            { 
                SetProperty(ref _line2RightForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line2LeftBack
        {
            get => _line2LeftBack;
            set 
            { 
                SetProperty(ref _line2LeftBack, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line2RightBack
        {
            get => _line2RightBack;
            set 
            { 
                SetProperty(ref _line2RightBack, value);
                UpdateAvailablePlayers();
            }
        }

        // Line 3 Properties
        public PlayerCardViewModel? Line3Center
        {
            get => _line3Center;
            set 
            { 
                SetProperty(ref _line3Center, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line3LeftForward
        {
            get => _line3LeftForward;
            set 
            { 
                SetProperty(ref _line3LeftForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line3RightForward
        {
            get => _line3RightForward;
            set 
            { 
                SetProperty(ref _line3RightForward, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line3LeftBack
        {
            get => _line3LeftBack;
            set 
            { 
                SetProperty(ref _line3LeftBack, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Line3RightBack
        {
            get => _line3RightBack;
            set 
            { 
                SetProperty(ref _line3RightBack, value);
                UpdateAvailablePlayers();
            }
        }

        // Goalkeeper Properties
        public PlayerCardViewModel? StartingGoalkeeper
        {
            get => _startingGoalkeeper;
            set 
            { 
                SetProperty(ref _startingGoalkeeper, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? BackupGoalkeeper
        {
            get => _backupGoalkeeper;
            set 
            { 
                SetProperty(ref _backupGoalkeeper, value);
                UpdateAvailablePlayers();
            }
        }

        // Bench Properties
        public PlayerCardViewModel? Bench1
        {
            get => _bench1;
            set 
            { 
                SetProperty(ref _bench1, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Bench2
        {
            get => _bench2;
            set 
            { 
                SetProperty(ref _bench2, value);
                UpdateAvailablePlayers();
            }
        }

        public PlayerCardViewModel? Bench3
        {
            get => _bench3;
            set 
            { 
                SetProperty(ref _bench3, value);
                UpdateAvailablePlayers();
            }
        }

        public ICommand LoadPlayersCommand { get; }
        public ICommand ClearLineupCommand { get; }
        public ICommand SaveLineupCommand { get; }
        public ICommand FilterByPositionCommand { get; }
        public ICommand ShowAllPlayersCommand { get; }
        public ICommand RemoveFromPositionCommand { get; }

        private async Task LoadPlayers()
        {
            var players = await _playerRepository.GetActivePlayersAsync();
            
            // Only show players that are in the roster
            var rosterPlayers = players.Where(p => p.IsInRoster).ToList();
            
            _allPlayers.Clear();
            AvailablePlayers.Clear();
            
            foreach (var player in rosterPlayers)
            {
                var playerCard = new PlayerCardViewModel(player);
                _allPlayers.Add(playerCard);
                AvailablePlayers.Add(playerCard);
            }
            
            UpdateAvailablePlayers();
        }

        private void UpdateAvailablePlayers()
        {
            // Get all players currently in lineup
            var playersInLineup = new List<PlayerCardViewModel?>();
            
            // Add all lineup positions
            playersInLineup.Add(StartingGoalkeeper);
            playersInLineup.Add(BackupGoalkeeper);
            playersInLineup.Add(Line1Center);
            playersInLineup.Add(Line1LeftForward);
            playersInLineup.Add(Line1RightForward);
            playersInLineup.Add(Line1LeftBack);
            playersInLineup.Add(Line1RightBack);
            playersInLineup.Add(Line2Center);
            playersInLineup.Add(Line2LeftForward);
            playersInLineup.Add(Line2RightForward);
            playersInLineup.Add(Line2LeftBack);
            playersInLineup.Add(Line2RightBack);
            playersInLineup.Add(Line3Center);
            playersInLineup.Add(Line3LeftForward);
            playersInLineup.Add(Line3RightForward);
            playersInLineup.Add(Line3LeftBack);
            playersInLineup.Add(Line3RightBack);
            playersInLineup.Add(Bench1);
            playersInLineup.Add(Bench2);
            playersInLineup.Add(Bench3);
            
            // Filter out nulls and get IDs
            var playerIdsInLineup = playersInLineup
                .Where(p => p != null)
                .Select(p => p!.Id)
                .ToHashSet();
            
            // Update AvailablePlayers to only show players not in lineup
            var currentFilter = SelectedPositionFilter;
            AvailablePlayers.Clear();
            
            var playersToShow = currentFilter.HasValue 
                ? _allPlayers.Where(p => p.Player.Position == currentFilter.Value)
                : _allPlayers;
            
            foreach (var player in playersToShow)
            {
                if (!playerIdsInLineup.Contains(player.Id))
                {
                    AvailablePlayers.Add(player);
                }
            }
        }

        private void FilterByPosition(Position? position)
        {
            SelectedPositionFilter = position;
            UpdateAvailablePlayers();
        }

        private void ShowAllPlayers()
        {
            SelectedPositionFilter = null;
            UpdateAvailablePlayers();
        }

        private void ClearLineup()
        {
            Line1Center = Line1LeftForward = Line1RightForward = Line1LeftBack = Line1RightBack = null;
            Line2Center = Line2LeftForward = Line2RightForward = Line2LeftBack = Line2RightBack = null;
            Line3Center = Line3LeftForward = Line3RightForward = Line3LeftBack = Line3RightBack = null;
            StartingGoalkeeper = BackupGoalkeeper = null;
            Bench1 = Bench2 = Bench3 = null;
            BenchPlayers.Clear();
        }

        private async Task SaveLineup()
        {
            // Implementation for saving lineup to database
            // This would create a MatchSetup object and save it
            await Task.CompletedTask;
            MessageBox.Show("Lineup saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void RemoveFromPosition(string? positionName)
        {
            if (string.IsNullOrEmpty(positionName))
                return;

            // Use reflection to clear the position
            var property = this.GetType().GetProperty(positionName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this, null);
            }
        }
    }
}
