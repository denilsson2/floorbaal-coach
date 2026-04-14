using System.Collections.ObjectModel;
using System.Windows.Input;
using FloorballCoach.Data;
using FloorballCoach.Helpers;
using FloorballCoach.Models;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// Main ViewModel for the application - manages navigation and team selection
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public MainViewModel(
            TeamManagementViewModel teamManagementViewModel,
            PlayerDatabaseViewModel playerDatabaseViewModel,
            RosterViewModel rosterViewModel,
            LineupViewModel lineupViewModel,
            BackupViewModel backupViewModel)
        {
            TeamManagementViewModel = teamManagementViewModel;
            PlayerDatabaseViewModel = playerDatabaseViewModel;
            RosterViewModel = rosterViewModel;
            LineupViewModel = lineupViewModel;
            BackupViewModel = backupViewModel;
            
            _currentViewModel = TeamManagementViewModel;

            // Subscribe to team changes
            TeamManagementViewModel.TeamChanged += OnTeamChanged;

            ShowTeamManagementCommand = new RelayCommand(_ => CurrentViewModel = TeamManagementViewModel);
            ShowPlayerDatabaseCommand = new RelayCommand(_ => CurrentViewModel = PlayerDatabaseViewModel);
            ShowRosterCommand = new RelayCommand(_ => CurrentViewModel = RosterViewModel);
            ShowLineupCommand = new RelayCommand(_ => CurrentViewModel = LineupViewModel);
            ShowBackupCommand = new RelayCommand(_ => CurrentViewModel = BackupViewModel);
        }

        private async void OnTeamChanged(object? sender, Team? team)
        {
            // Update RosterViewModel and LineupViewModel with the selected team
            await RosterViewModel.SetCurrentTeamAsync(team);
            // LineupViewModel might also need team context in the future
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public TeamManagementViewModel TeamManagementViewModel { get; }
        public PlayerDatabaseViewModel PlayerDatabaseViewModel { get; }
        public RosterViewModel RosterViewModel { get; }
        public LineupViewModel LineupViewModel { get; }
        public BackupViewModel BackupViewModel { get; }

        public ICommand ShowTeamManagementCommand { get; }
        public ICommand ShowPlayerDatabaseCommand { get; }
        public ICommand ShowRosterCommand { get; }
        public ICommand ShowLineupCommand { get; }
        public ICommand ShowBackupCommand { get; }
    }
}
