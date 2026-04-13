using System.Windows.Controls;
using FloorballCoach.ViewModels;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace FloorballCoach.Views
{
    /// <summary>
    /// Interaction logic for RosterView.xaml
    /// </summary>
    public partial class RosterView : UserControl
    {
        private Point _dragStartPoint;
        private bool _isDragging = false;
        
        public RosterView()
        {
            InitializeComponent();
            Loaded += RosterView_Loaded;
        }

        private async void RosterView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RosterViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }

        private void PlayerCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                return;
            }
            
            if (sender is System.Windows.FrameworkElement element && 
                element.DataContext is PlayerCardViewModel playerCard &&
                DataContext is RosterViewModel viewModel)
            {
                if (viewModel.EditPlayerCommand.CanExecute(playerCard))
                {
                    viewModel.EditPlayerCommand.Execute(playerCard);
                }
            }
        }

        private void RosterPlayerCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _isDragging = false;
        }

        private void RosterPlayerCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // If we didn't drag, treat it as a click to edit
            if (!_isDragging && sender is System.Windows.FrameworkElement element && 
                element.DataContext is PlayerCardViewModel playerCard &&
                DataContext is RosterViewModel viewModel)
            {
                if (viewModel.EditPlayerCommand.CanExecute(playerCard))
                {
                    viewModel.EditPlayerCommand.Execute(playerCard);
                }
            }
            
            _isDragging = false;
        }

        private void RosterPlayerCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
            {
                Point currentPosition = e.GetPosition(null);
                Vector diff = _dragStartPoint - currentPosition;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;
                    
                    if (sender is FrameworkElement element && element.DataContext is PlayerCardViewModel playerCard)
                    {
                        DragDrop.DoDragDrop(element, playerCard, DragDropEffects.Move);
                    }
                }
            }
        }

        private void RosterPlayerCard_Drop(object sender, DragEventArgs e)
        {
            if (sender is System.Windows.Controls.Border dropTarget && 
                dropTarget.DataContext is PlayerCardViewModel targetPlayer &&
                e.Data.GetData(typeof(PlayerCardViewModel)) is PlayerCardViewModel draggedPlayer &&
                DataContext is RosterViewModel viewModel)
            {
                // Reset visual feedback
                dropTarget.Background = GetOriginalBackground(targetPlayer);
                
                // Reorder players
                viewModel.ReorderPlayers(draggedPlayer, targetPlayer);
            }
            
            _isDragging = false;
        }

        private void RosterPlayerCard_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is System.Windows.Controls.Border element)
            {
                element.Background = new SolidColorBrush(Color.FromArgb(100, 33, 150, 243));
            }
        }

        private void RosterPlayerCard_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is System.Windows.Controls.Border element && element.DataContext is PlayerCardViewModel playerCard)
            {
                element.Background = GetOriginalBackground(playerCard);
            }
        }

        private Brush GetOriginalBackground(PlayerCardViewModel playerCard)
        {
            return playerCard.Player.Position switch
            {
                Models.Position.Goalkeeper => new SolidColorBrush(Color.FromRgb(227, 242, 253)), // #E3F2FD
                Models.Position.Defender => new SolidColorBrush(Color.FromRgb(252, 228, 236)),   // #FCE4EC
                Models.Position.Center => new SolidColorBrush(Color.FromRgb(255, 248, 225)),     // #FFF8E1
                Models.Position.Forward => new SolidColorBrush(Color.FromRgb(232, 245, 233)),    // #E8F5E9
                _ => new SolidColorBrush(Colors.White)
            };
        }
    }
}
