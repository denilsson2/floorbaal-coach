using System.Windows.Controls;

namespace FloorballCoach.Views
{
    /// <summary>
    /// Interaction logic for TeamManagementView.xaml
    /// </summary>
    public partial class TeamManagementView : UserControl
    {
        public TeamManagementView()
        {
            InitializeComponent();
            
            // Initialize the ViewModel when the view is loaded
            Loaded += async (s, e) =>
            {
                if (DataContext is ViewModels.TeamManagementViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            };
        }
    }
}
