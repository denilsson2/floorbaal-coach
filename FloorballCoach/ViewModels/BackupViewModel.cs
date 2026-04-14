using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using FloorballCoach.Data;
using FloorballCoach.Helpers;
using FloorballCoach.Services;
using Microsoft.Win32;
using System.Windows;

namespace FloorballCoach.ViewModels
{
    /// <summary>
    /// ViewModel for backup and restore operations
    /// </summary>
    public class BackupViewModel : ViewModelBase
    {
        private readonly BackupService _backupService;
        private string _statusMessage = string.Empty;
        private bool _isProcessing = false;

        public BackupViewModel(BackupService backupService)
        {
            _backupService = backupService;

            ExportBackupCommand = new RelayCommand(async _ => await ExportBackup());
            ImportBackupCommand = new RelayCommand(async _ => await ImportBackup());
            QuickBackupCommand = new RelayCommand(async _ => await QuickBackup());
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public ICommand ExportBackupCommand { get; }
        public ICommand ImportBackupCommand { get; }
        public ICommand QuickBackupCommand { get; }

        private async Task ExportBackup()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Exporterar...";

                var saveDialog = new SaveFileDialog
                {
                    Title = "Spara backup",
                    Filter = "JSON filer (*.json)|*.json|Alla filer (*.*)|*.*",
                    FileName = BackupService.GenerateBackupFileName(),
                    InitialDirectory = BackupService.GetDefaultBackupDirectory()
                };

                if (saveDialog.ShowDialog() == true)
                {
                    await _backupService.ExportToFileAsync(saveDialog.FileName);
                    StatusMessage = $"✓ Backup sparad: {Path.GetFileName(saveDialog.FileName)}";
                    MessageBox.Show(
                        $"Backup har sparats till:\n{saveDialog.FileName}",
                        "Export lyckades",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    StatusMessage = "Export avbruten";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Fel vid export: {ex.Message}";
                MessageBox.Show(
                    $"Ett fel uppstod vid export:\n{ex.Message}",
                    "Export misslyckades",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ImportBackup()
        {
            try
            {
                var result = MessageBox.Show(
                    "Importera backup?\n\nVarning: Detta kommer att lägga till data från backupen.\n\nVill du radera befintlig data först?",
                    "Bekräfta import",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    StatusMessage = "Import avbruten";
                    return;
                }

                bool clearExisting = result == MessageBoxResult.Yes;

                var openDialog = new OpenFileDialog
                {
                    Title = "Välj backup-fil",
                    Filter = "JSON filer (*.json)|*.json|Alla filer (*.*)|*.*",
                    InitialDirectory = BackupService.GetDefaultBackupDirectory()
                };

                if (openDialog.ShowDialog() == true)
                {
                    IsProcessing = true;
                    StatusMessage = "Importerar...";

                    int imported = await _backupService.ImportFromFileAsync(openDialog.FileName, clearExisting);
                    
                    StatusMessage = $"✓ {imported} poster importerade";
                    MessageBox.Show(
                        $"Import lyckades!\n\n{imported} poster har importerats.",
                        "Import klar",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Suggest restart
                    var restartResult = MessageBox.Show(
                        "Vill du starta om applikationen för att se den importerade datan?",
                        "Starta om?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (restartResult == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Environment.ProcessPath ?? "FloorballCoach.exe");
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    StatusMessage = "Import avbruten";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Fel vid import: {ex.Message}";
                MessageBox.Show(
                    $"Ett fel uppstod vid import:\n{ex.Message}",
                    "Import misslyckades",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task QuickBackup()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Skapar snabb backup...";

                var backupDir = BackupService.GetDefaultBackupDirectory();
                var fileName = BackupService.GenerateBackupFileName();
                var filePath = Path.Combine(backupDir, fileName);

                await _backupService.ExportToFileAsync(filePath);

                StatusMessage = $"✓ Snabb backup skapad: {fileName}";
                MessageBox.Show(
                    $"Backup har sparats till:\n{filePath}",
                    "Snabb backup klar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Fel vid snabb backup: {ex.Message}";
                MessageBox.Show(
                    $"Ett fel uppstod:\n{ex.Message}",
                    "Backup misslyckades",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
