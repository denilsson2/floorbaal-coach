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
    /// ViewModel for managing teams and team selection
    /// </summary>
    public class TeamManagementViewModel : ViewModelBase
    {
        private readonly ITeamRepository _teamRepository;
        private ObservableCollection<Team> _teams;
        private Team? _selectedTeam;
        private string _newTeamName = string.Empty;
        private string _newTeamShortName = string.Empty;
        private string _newTeamColor = string.Empty;

        public TeamManagementViewModel(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
            _teams = new ObservableCollection<Team>();

            AddTeamCommand = new RelayCommand(async _ => await AddTeam(), _ => CanAddTeam());
            DeleteTeamCommand = new RelayCommand(async team => await DeleteTeam(team as Team), team => team is Team);
            SelectTeamCommand = new RelayCommand(team => SelectTeam(team as Team), team => team is Team);
            RefreshCommand = new RelayCommand(async _ => await LoadTeams());
        }

        public async Task InitializeAsync()
        {
            await LoadTeams();
        }

        private async Task LoadTeams()
        {
            try
            {
                var teams = await _teamRepository.GetAllTeamsAsync();
                Teams.Clear();
                foreach (var team in teams)
                {
                    Teams.Add(team);
                }

                // Auto-select first team if none selected
                if (SelectedTeam == null && Teams.Any())
                {
                    SelectedTeam = Teams.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid laddning av lag: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddTeam()
        {
            try
            {
                var team = new Team
                {
                    Name = NewTeamName,
                    ShortName = string.IsNullOrWhiteSpace(NewTeamShortName) ? null : NewTeamShortName,
                    TeamColor = string.IsNullOrWhiteSpace(NewTeamColor) ? null : NewTeamColor,
                    IsActive = true
                };

                var addedTeam = await _teamRepository.AddTeamAsync(team);
                Teams.Add(addedTeam);

                // Select the newly added team
                SelectedTeam = addedTeam;

                // Clear form
                NewTeamName = string.Empty;
                NewTeamShortName = string.Empty;
                NewTeamColor = string.Empty;

                MessageBox.Show($"Laget '{addedTeam.Name}' har skapats!", "Lag skapat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid skapande av lag: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddTeam()
        {
            return !string.IsNullOrWhiteSpace(NewTeamName);
        }

        private async Task DeleteTeam(Team? team)
        {
            if (team == null) return;

            var result = MessageBox.Show(
                $"Är du säker på att du vill ta bort laget '{team.Name}'?\n\nDetta kommer ta bort truppen men inte spelarna från databasen.",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var success = await _teamRepository.DeleteTeamAsync(team.Id);
                if (success)
                {
                    Teams.Remove(team);
                    if (SelectedTeam?.Id == team.Id)
                    {
                        SelectedTeam = Teams.FirstOrDefault();
                    }
                    MessageBox.Show($"Laget '{team.Name}' har tagits bort.", "Lag borttaget", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fel vid borttagning av lag: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectTeam(Team? team)
        {
            if (team != null)
            {
                SelectedTeam = team;
            }
        }

        // Properties
        public ObservableCollection<Team> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        public Team? SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                if (SetProperty(ref _selectedTeam, value))
                {
                    OnPropertyChanged(nameof(HasSelectedTeam));
                    // Notify other ViewModels that the team has changed
                    TeamChanged?.Invoke(this, value);
                }
            }
        }

        public bool HasSelectedTeam => SelectedTeam != null;

        public string NewTeamName
        {
            get => _newTeamName;
            set => SetProperty(ref _newTeamName, value);
        }

        public string NewTeamShortName
        {
            get => _newTeamShortName;
            set => SetProperty(ref _newTeamShortName, value);
        }

        public string NewTeamColor
        {
            get => _newTeamColor;
            set => SetProperty(ref _newTeamColor, value);
        }

        // Commands
        public ICommand AddTeamCommand { get; }
        public ICommand DeleteTeamCommand { get; }
        public ICommand SelectTeamCommand { get; }
        public ICommand RefreshCommand { get; }

        // Event for notifying other ViewModels when team changes
        public event EventHandler<Team?>? TeamChanged;
    }
}
