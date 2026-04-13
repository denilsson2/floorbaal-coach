using System.Windows.Controls;
using System.Windows.Input;
using FloorballCoach.ViewModels;

namespace FloorballCoach.Views
{
    /// <summary>
    /// Interaction logic for PlayerDatabaseView.xaml
    /// </summary>
    public partial class PlayerDatabaseView : UserControl
    {
        public PlayerDatabaseView()
        {
            InitializeComponent();
        }

        private void PlayerCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Border border && 
                border.DataContext is PlayerCardViewModel playerCard &&
                DataContext is PlayerDatabaseViewModel viewModel)
            {
                viewModel.SelectedPlayer = playerCard;
            }
        }

        private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && 
                button.DataContext is PlayerCardViewModel playerCard &&
                DataContext is PlayerDatabaseViewModel viewModel)
            {
                viewModel.SelectedPlayer = playerCard;
                viewModel.EditPlayerCommand.Execute(null);
            }
        }
    }
}
