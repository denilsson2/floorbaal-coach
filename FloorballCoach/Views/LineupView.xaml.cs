using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FloorballCoach.ViewModels;
using FloorballCoach.Models;

namespace FloorballCoach.Views
{
    /// <summary>
    /// Interaction logic for LineupView.xaml
    /// </summary>
    public partial class LineupView : UserControl
    {
        private PlayerCardViewModel? _draggedPlayer;

        public LineupView()
        {
            InitializeComponent();
            Loaded += LineupView_Loaded;
        }

        private async void LineupView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LineupViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && DataContext is LineupViewModel viewModel)
            {
                var tag = button.Tag as string;
                
                if (tag == "All")
                {
                    viewModel.ShowAllPlayersCommand.Execute(null);
                }
                else if (Enum.TryParse<Position>(tag, out var position))
                {
                    viewModel.FilterByPositionCommand.Execute(position);
                }
            }
        }

        private void PlayerCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Border border)
            {
                var player = border.DataContext as PlayerCardViewModel;
                if (player != null)
                {
                    _draggedPlayer = player;
                    DragDrop.DoDragDrop(border, player, DragDropEffects.Move);
                }
            }
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(200, 230, 255));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243));
            }
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border && DataContext is LineupViewModel viewModel)
            {
                // Reset visual state
                border.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));

                if (_draggedPlayer == null)
                    return;

                var targetPosition = border.Tag as string;
                if (string.IsNullOrEmpty(targetPosition))
                    return;

                // Assign player to the correct position using reflection
                var property = viewModel.GetType().GetProperty(targetPosition);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(viewModel, _draggedPlayer);
                }

                _draggedPlayer = null;
            }
        }

        private void DropZone_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && DataContext is LineupViewModel viewModel)
            {
                var targetPosition = border.Tag as string;
                if (string.IsNullOrEmpty(targetPosition))
                    return;

                // Get the current player in this position
                var property = viewModel.GetType().GetProperty(targetPosition);
                if (property != null && property.CanRead)
                {
                    var currentPlayer = property.GetValue(viewModel) as PlayerCardViewModel;
                    if (currentPlayer != null)
                    {
                        // Clear the position
                        if (property.CanWrite)
                        {
                            property.SetValue(viewModel, null);
                        }
                    }
                }
            }
        }
    }
}
