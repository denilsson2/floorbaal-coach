using System;
using System.Windows;
using System.Windows.Controls;
using FloorballCoach.Models;

namespace FloorballCoach.Views
{
    /// <summary>
    /// Interaction logic for AddEditPlayerDialog.xaml
    /// </summary>
    public partial class AddEditPlayerDialog : Window
    {
        public Player? Player { get; private set; }

        public AddEditPlayerDialog(Player? existingPlayer = null)
        {
            InitializeComponent();
            
            if (existingPlayer != null)
            {
                LoadPlayer(existingPlayer);
                Title = "Edit Player";
            }
            else
            {
                Title = "Add Player";
                DateOfBirthPicker.SelectedDate = DateTime.Now.AddYears(-25);
            }
        }

        private void LoadPlayer(Player player)
        {
            FirstNameTextBox.Text = player.FirstName;
            LastNameTextBox.Text = player.LastName;
            JerseyNumberTextBox.Text = player.JerseyNumber.ToString();
            
            // Set position
            foreach (ComboBoxItem item in PositionComboBox.Items)
            {
                if (item.Tag.ToString() == player.Position.ToString())
                {
                    PositionComboBox.SelectedItem = item;
                    break;
                }
            }

            DateOfBirthPicker.SelectedDate = player.DateOfBirth;
            GamesPlayedTextBox.Text = player.GamesPlayed.ToString();
            GoalsTextBox.Text = player.Goals.ToString();
            AssistsTextBox.Text = player.Assists.ToString();
            PenaltiesTextBox.Text = player.Penalties.ToString();
            NotesTextBox.Text = player.Notes ?? string.Empty;
            IsActiveCheckBox.IsChecked = player.IsActive;
            
            Player = player;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameTextBox.Focus();
                return;
            }

            if (!int.TryParse(JerseyNumberTextBox.Text, out int jerseyNumber) || jerseyNumber < 0)
            {
                MessageBox.Show("Please enter a valid jersey number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                JerseyNumberTextBox.Focus();
                return;
            }

            if (PositionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a position.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PositionComboBox.Focus();
                return;
            }

            if (!DateOfBirthPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date of birth.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DateOfBirthPicker.Focus();
                return;
            }

            // Parse statistics
            if (!int.TryParse(GamesPlayedTextBox.Text, out int gamesPlayed) || gamesPlayed < 0)
                gamesPlayed = 0;
            
            if (!int.TryParse(GoalsTextBox.Text, out int goals) || goals < 0)
                goals = 0;
            
            if (!int.TryParse(AssistsTextBox.Text, out int assists) || assists < 0)
                assists = 0;
            
            if (!int.TryParse(PenaltiesTextBox.Text, out int penalties) || penalties < 0)
                penalties = 0;

            // Get position
            var selectedPosition = ((ComboBoxItem)PositionComboBox.SelectedItem).Tag.ToString();
            Position position = selectedPosition switch
            {
                "Goalkeeper" => Position.Goalkeeper,
                "Center" => Position.Center,
                "Forward" => Position.Forward,
                "Defender" => Position.Defender,
                _ => Position.Forward
            };

            // Create or update player
            if (Player == null)
            {
                Player = new Player();
            }

            Player.FirstName = FirstNameTextBox.Text.Trim();
            Player.LastName = LastNameTextBox.Text.Trim();
            Player.JerseyNumber = jerseyNumber;
            Player.Position = position;
            Player.DateOfBirth = DateOfBirthPicker.SelectedDate.Value;
            Player.GamesPlayed = gamesPlayed;
            Player.Goals = goals;
            Player.Assists = assists;
            Player.Penalties = penalties;
            Player.Notes = string.IsNullOrWhiteSpace(NotesTextBox.Text) ? null : NotesTextBox.Text.Trim();
            Player.IsActive = IsActiveCheckBox.IsChecked ?? true;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
