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
    /// ViewModel for managing the player database
    /// </summary>
    public class PlayerDatabaseViewModel : ViewModelBase
    {
        private readonly IPlayerRepository _playerRepository;
        private ObservableCollection<PlayerCardViewModel> _players;
        private PlayerCardViewModel? _selectedPlayer;
        private string _searchText = string.Empty;

        public PlayerDatabaseViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            _players = new ObservableCollection<PlayerCardViewModel>();

            AddPlayerCommand = new RelayCommand(async _ => await AddPlayer());
            EditPlayerCommand = new RelayCommand(async _ => await EditPlayer(), _ => SelectedPlayer != null);
            DeletePlayerCommand = new RelayCommand(async _ => await DeletePlayer(), _ => SelectedPlayer != null);
            RefreshCommand = new RelayCommand(async _ => await LoadPlayers());

            _ = LoadPlayers();
        }

        public ObservableCollection<PlayerCardViewModel> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        public PlayerCardViewModel? SelectedPlayer
        {
            get => _selectedPlayer;
            set => SetProperty(ref _selectedPlayer, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = LoadPlayers();
                }
            }
        }

        public ICommand AddPlayerCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand DeletePlayerCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadPlayers()
        {
            var players = await _playerRepository.GetActivePlayersAsync();
            
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                players = players.Where(p => 
                    p.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.JerseyNumber.ToString().Contains(SearchText))
                    .ToList();
            }

            Players.Clear();
            foreach (var player in players)
            {
                Players.Add(new PlayerCardViewModel(player));
            }
        }

        private async Task AddPlayer()
        {
            var dialog = new Views.AddEditPlayerDialog();
            if (dialog.ShowDialog() == true && dialog.Player != null)
            {
                await _playerRepository.AddPlayerAsync(dialog.Player);
                await LoadPlayers();
            }
        }

        private async Task EditPlayer()
        {
            if (SelectedPlayer == null)
                return;

            var dialog = new Views.AddEditPlayerDialog(SelectedPlayer.Player);
            if (dialog.ShowDialog() == true && dialog.Player != null)
            {
                await _playerRepository.UpdatePlayerAsync(dialog.Player);
                await LoadPlayers();
            }
        }

        private async Task DeletePlayer()
        {
            if (SelectedPlayer == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to delete {SelectedPlayer.FullName}?\n\nThis action cannot be undone.",
                "Confirm Delete",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                await _playerRepository.DeletePlayerAsync(SelectedPlayer.Id);
                await LoadPlayers();
                System.Windows.MessageBox.Show(
                    $"{SelectedPlayer.FullName} has been removed from the roster.",
                    "Player Deleted",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
