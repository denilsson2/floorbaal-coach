using System.Collections.ObjectModel;
using System.Windows.Input;
using FloorballCoach.Data;
using FloorballCoach.Helpers;
using FloorballCoach.Models;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// Main ViewModel for the application
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private readonly IPlayerRepository _playerRepository;

        public MainViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            
            PlayerDatabaseViewModel = new PlayerDatabaseViewModel(_playerRepository);
            RosterViewModel = new RosterViewModel(_playerRepository);
            LineupViewModel = new LineupViewModel(_playerRepository);
            
            _currentViewModel = PlayerDatabaseViewModel;

            ShowPlayerDatabaseCommand = new RelayCommand(_ => CurrentViewModel = PlayerDatabaseViewModel);
            ShowRosterCommand = new RelayCommand(_ => CurrentViewModel = RosterViewModel);
            ShowLineupCommand = new RelayCommand(_ => CurrentViewModel = LineupViewModel);
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public PlayerDatabaseViewModel PlayerDatabaseViewModel { get; }
        public RosterViewModel RosterViewModel { get; }
        public LineupViewModel LineupViewModel { get; }

        public ICommand ShowPlayerDatabaseCommand { get; }
        public ICommand ShowRosterCommand { get; }
        public ICommand ShowLineupCommand { get; }
    }
}
