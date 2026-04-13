using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FloorballCoach.Data;
using FloorballCoach.Helpers;
using FloorballCoach.Models;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// ViewModel for managing the team roster
    /// </summary>
    public class RosterViewModel : ViewModelBase
    {
        private readonly IPlayerRepository _playerRepository;
        private ObservableCollection<PlayerCardViewModel> _allPlayers;
        private ObservableCollection<PlayerCardViewModel> _availablePlayers;
        private ObservableCollection<PlayerCardViewModel> _rosterPlayers;
        private ObservableCollection<PlayerCardViewModel> _goalkeepers;
        private ObservableCollection<PlayerCardViewModel> _defenders;
        private ObservableCollection<PlayerCardViewModel> _centers;
        private ObservableCollection<PlayerCardViewModel> _forwards;

        public RosterViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            _allPlayers = new ObservableCollection<PlayerCardViewModel>();
            _availablePlayers = new ObservableCollection<PlayerCardViewModel>();
            _rosterPlayers = new ObservableCollection<PlayerCardViewModel>();
            _goalkeepers = new ObservableCollection<PlayerCardViewModel>();
            _defenders = new ObservableCollection<PlayerCardViewModel>();
            _centers = new ObservableCollection<PlayerCardViewModel>();
            _forwards = new ObservableCollection<PlayerCardViewModel>();

            AddToRosterCommand = new RelayCommand(async player => await AddToRoster(player as PlayerCardViewModel));
            RemoveFromRosterCommand = new RelayCommand(async player => await RemoveFromRoster(player as PlayerCardViewModel));
            EditPlayerCommand = new RelayCommand(async player => await EditPlayer(player as PlayerCardViewModel));
            DeletePlayerCommand = new RelayCommand(async player => await DeletePlayer(player as PlayerCardViewModel));
            SaveRosterCommand = new RelayCommand(async _ => await SaveRoster());
            RefreshCommand = new RelayCommand(async _ => await LoadPlayers());
        }

        public async Task InitializeAsync()
        {
            if (!_allPlayers.Any())
            {
                await LoadPlayers();
            }
        }

        public ObservableCollection<PlayerCardViewModel> AllPlayers
        {
            get => _allPlayers;
            set => SetProperty(ref _allPlayers, value);
        }

        public ObservableCollection<PlayerCardViewModel> AvailablePlayers
        {
            get => _availablePlayers;
            set => SetProperty(ref _availablePlayers, value);
        }

        public ObservableCollection<PlayerCardViewModel> RosterPlayers
        {
            get => _rosterPlayers;
            set => SetProperty(ref _rosterPlayers, value);
        }

        public ObservableCollection<PlayerCardViewModel> Goalkeepers
        {
            get => _goalkeepers;
            set => SetProperty(ref _goalkeepers, value);
        }

        public ObservableCollection<PlayerCardViewModel> Defenders
        {
            get => _defenders;
            set => SetProperty(ref _defenders, value);
        }

        public ObservableCollection<PlayerCardViewModel> Centers
        {
            get => _centers;
            set => SetProperty(ref _centers, value);
        }

        public ObservableCollection<PlayerCardViewModel> Forwards
        {
            get => _forwards;
            set => SetProperty(ref _forwards, value);
        }

        public ICommand AddToRosterCommand { get; }
        public ICommand RemoveFromRosterCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand DeletePlayerCommand { get; }
        public ICommand SaveRosterCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadPlayers()
        {
            var players = await _playerRepository.GetActivePlayersAsync();

            AllPlayers.Clear();
            AvailablePlayers.Clear();
            RosterPlayers.Clear();
            Goalkeepers.Clear();
            Defenders.Clear();
            Centers.Clear();
            Forwards.Clear();

            foreach (var player in players.OrderBy(p => p.RosterOrder))
            {
                var playerCard = new PlayerCardViewModel(player);
                AllPlayers.Add(playerCard);

                if (player.IsInRoster)
                {
                    RosterPlayers.Add(playerCard);
                    
                    // Group by position in the order: Goalkeepers, Defenders, Centers, Forwards
                    switch (player.Position)
                    {
                        case Position.Goalkeeper:
                            Goalkeepers.Add(playerCard);
                            break;
                        case Position.Defender:
                            Defenders.Add(playerCard);
                            break;
                        case Position.Center:
                            Centers.Add(playerCard);
                            break;
                        case Position.Forward:
                            Forwards.Add(playerCard);
                            break;
                    }
                }
                else
                {
                    // Only show players not in roster as available
                    AvailablePlayers.Add(playerCard);
                }
            }
        }

        private async Task AddToRoster(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            playerCard.Player.IsInRoster = true;
            
            // Set RosterOrder based on current count in that position
            var playersInPosition = await _playerRepository.GetActivePlayersAsync();
            var maxOrder = playersInPosition
                .Where(p => p.IsInRoster && p.Position == playerCard.Player.Position)
                .Select(p => p.RosterOrder)
                .DefaultIfEmpty(0)
                .Max();
            
            playerCard.Player.RosterOrder = maxOrder + 1;
            
            await _playerRepository.UpdatePlayerAsync(playerCard.Player);
            await LoadPlayers();
        }

        private async Task RemoveFromRoster(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            playerCard.Player.IsInRoster = false;
            await _playerRepository.UpdatePlayerAsync(playerCard.Player);
            await LoadPlayers();
        }

        private async Task EditPlayer(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            var dialog = new Views.AddEditPlayerDialog(playerCard.Player);
            if (dialog.ShowDialog() == true && dialog.Player != null)
            {
                await _playerRepository.UpdatePlayerAsync(dialog.Player);
                await LoadPlayers(); // Reload to ensure player appears in correct position group
            }
        }

        private async Task DeletePlayer(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to delete {playerCard.FullName} from the database?\n\nThis action cannot be undone.",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                await _playerRepository.DeletePlayerAsync(playerCard.Id);
                await LoadPlayers();
                System.Windows.MessageBox.Show(
                    $"{playerCard.FullName} has been deleted from the database.",
                    "Player Deleted",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }

        public async void ReorderPlayers(PlayerCardViewModel draggedPlayer, PlayerCardViewModel targetPlayer)
        {
            if (draggedPlayer == null || targetPlayer == null || draggedPlayer == targetPlayer)
                return;

            // Make sure they're in the same position group
            if (draggedPlayer.Player.Position != targetPlayer.Player.Position)
                return;

            var collection = draggedPlayer.Player.Position switch
            {
                Position.Goalkeeper => Goalkeepers,
                Position.Defender => Defenders,
                Position.Center => Centers,
                Position.Forward => Forwards,
                _ => null
            };

            if (collection == null)
                return;

            int draggedIndex = collection.IndexOf(draggedPlayer);
            int targetIndex = collection.IndexOf(targetPlayer);

            if (draggedIndex == -1 || targetIndex == -1)
                return;

            // Reorder in the collection
            collection.Move(draggedIndex, targetIndex);

            // Update RosterOrder in database
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].Player.RosterOrder = i;
                await _playerRepository.UpdatePlayerAsync(collection[i].Player);
            }
        }

        private async Task SaveRoster()
        {
            try
            {
                // All changes are already saved when adding/removing players
                // This is just a confirmation
                System.Windows.MessageBox.Show(
                    $"Roster saved successfully!\n\nTotal players in roster: {RosterPlayers.Count}\n" +
                    $"Goalkeepers: {Goalkeepers.Count}\n" +
                    $"Defenders: {Defenders.Count}\n" +
                    $"Centers: {Centers.Count}\n" +
                    $"Forwards: {Forwards.Count}",
                    "Roster Saved",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Error saving roster: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            await Task.CompletedTask;
        }
    }
}
