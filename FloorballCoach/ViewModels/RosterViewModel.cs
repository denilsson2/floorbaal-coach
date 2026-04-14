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
    /// ViewModel for managing the team roster - supports multiple teams
    /// </summary>
    public class RosterViewModel : ViewModelBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private Team? _currentTeam;
        private ObservableCollection<PlayerCardViewModel> _allPlayers;
        private ObservableCollection<PlayerCardViewModel> _availablePlayers;
        private ObservableCollection<PlayerCardViewModel> _rosterPlayers;
        private ObservableCollection<PlayerCardViewModel> _goalkeepers;
        private ObservableCollection<PlayerCardViewModel> _defenders;
        private ObservableCollection<PlayerCardViewModel> _centers;
        private ObservableCollection<PlayerCardViewModel> _forwards;

        public RosterViewModel(IPlayerRepository playerRepository, ITeamRepository teamRepository)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _allPlayers = new ObservableCollection<PlayerCardViewModel>();
            _availablePlayers = new ObservableCollection<PlayerCardViewModel>();
            _rosterPlayers = new ObservableCollection<PlayerCardViewModel>();
            _goalkeepers = new ObservableCollection<PlayerCardViewModel>();
            _defenders = new ObservableCollection<PlayerCardViewModel>();
            _centers = new ObservableCollection<PlayerCardViewModel>();
            _forwards = new ObservableCollection<PlayerCardViewModel>();

            AddToRosterCommand = new RelayCommand(async player => await AddToRoster(player as PlayerCardViewModel), _ => HasCurrentTeam);
            RemoveFromRosterCommand = new RelayCommand(async player => await RemoveFromRoster(player as PlayerCardViewModel), _ => HasCurrentTeam);
            EditPlayerCommand = new RelayCommand(async player => await EditPlayer(player as PlayerCardViewModel));
            DeletePlayerCommand = new RelayCommand(async player => await DeletePlayer(player as PlayerCardViewModel));
            SaveRosterCommand = new RelayCommand(async _ => await SaveRoster(), _ => HasCurrentTeam);
            RefreshCommand = new RelayCommand(async _ => await LoadPlayers(), _ => HasCurrentTeam);
        }

        public async Task InitializeAsync()
        {
            if (_currentTeam != null && !_allPlayers.Any())
            {
                await LoadPlayers();
            }
        }

        /// <summary>
        /// Set the current team to manage - called when team selection changes
        /// </summary>
        public async Task SetCurrentTeamAsync(Team? team)
        {
            _currentTeam = team;
            OnPropertyChanged(nameof(HasCurrentTeam));
            OnPropertyChanged(nameof(CurrentTeamName));
            
            if (team != null)
            {
                await LoadPlayers();
            }
            else
            {
                ClearCollections();
            }
        }

        private void ClearCollections()
        {
            AllPlayers.Clear();
            AvailablePlayers.Clear();
            RosterPlayers.Clear();
            Goalkeepers.Clear();
            Defenders.Clear();
            Centers.Clear();
            Forwards.Clear();
        }

        public bool HasCurrentTeam => _currentTeam != null;
        public string CurrentTeamName => _currentTeam?.Name ?? "Inget lag valt";

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
            if (_currentTeam == null) return;

            try
            {
                // Load players in this team's roster
                var rosterPlayers = await _teamRepository.GetTeamRosterAsync(_currentTeam.Id);
                
                // Load available players (not in this team's roster)
                var availablePlayers = await _teamRepository.GetAvailablePlayersAsync(_currentTeam.Id);

                AllPlayers.Clear();
                AvailablePlayers.Clear();
                RosterPlayers.Clear();
                Goalkeepers.Clear();
                Defenders.Clear();
                Centers.Clear();
                Forwards.Clear();

                // Add roster players
                foreach (var player in rosterPlayers.OrderBy(p => p.Position).ThenBy(p => p.LastName))
                {
                    var playerCard = new PlayerCardViewModel(player);
                    AllPlayers.Add(playerCard);
                    RosterPlayers.Add(playerCard);
                    
                    // Group by position
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

                // Add available players
                foreach (var player in availablePlayers.OrderBy(p => p.LastName))
                {
                    var playerCard = new PlayerCardViewModel(player);
                    AvailablePlayers.Add(playerCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av spelartrupp: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddToRoster(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null || _currentTeam == null)
                return;

            try
            {
                await _teamRepository.AddPlayerToRosterAsync(_currentTeam.Id, playerCard.Id);
                await LoadPlayers();
                MessageBox.Show($"{playerCard.FullName} har lagts till i {_currentTeam.Name}.", "Spelare tillagd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid tillägg av spelare: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RemoveFromRoster(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null || _currentTeam == null)
                return;

            var result = MessageBox.Show(
                $"Är du säker på att du vill ta bort {playerCard.FullName} från {_currentTeam.Name}?\n\nSpelaren finns kvar i databasen och kan läggas till igen.",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _teamRepository.RemovePlayerFromRosterAsync(_currentTeam.Id, playerCard.Id);
                await LoadPlayers();
                MessageBox.Show($"{playerCard.FullName} har tagits bort från {_currentTeam.Name}.", "Spelare borttagen", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid borttagning av spelare: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditPlayer(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            try
            {
                var dialog = new Views.AddEditPlayerDialog(playerCard.Player);
                if (dialog.ShowDialog() == true && dialog.Player != null)
                {
                    await _playerRepository.UpdatePlayerAsync(dialog.Player);
                    await LoadPlayers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid uppdatering av spelare: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeletePlayer(PlayerCardViewModel? playerCard)
        {
            if (playerCard == null)
                return;

            var result = MessageBox.Show(
                $"Är du säker på att du vill ta bort {playerCard.FullName} från databasen?\n\nSpelaren kommer tas bort från ALLA lag. Denna åtgärd kan inte ångras.",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _playerRepository.DeletePlayerAsync(playerCard.Id);
                    await LoadPlayers();
                    MessageBox.Show(
                        $"{playerCard.FullName} har tagits bort från databasen.",
                        "Spelare borttagen",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fel vid borttagning av spelare: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SaveRoster()
        {
            // This could be used for batch operations in the future
            await LoadPlayers();
            MessageBox.Show("Truppen har sparats!", "Sparat", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
