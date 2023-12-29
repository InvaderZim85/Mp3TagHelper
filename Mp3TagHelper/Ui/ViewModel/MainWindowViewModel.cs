using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Mp3TagHelper.Business;
using Mp3TagHelper.Model;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;
using Mp3TagHelper.Common;

namespace Mp3TagHelper.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the directory which contains the MP3 files
    /// </summary>
    [ObservableProperty]
    private string _sourceDir = string.Empty;

    /// <summary>
    /// Gets or sets the list with the files
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Mp3File> _files = [];

    /// <summary>
    /// Gets or sets the selected file
    /// </summary>
    [ObservableProperty]
    private Mp3File? _selectedFile;

    /// <summary>
    /// Gets or sets the general details
    /// </summary>
    [ObservableProperty]
    private GeneralDetails _generalDetails = new();

    /// <summary>
    /// Browses for a file
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task BrowseDirAsync()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select the directory with the MP3 files",
            DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic)
        };

        if (dialog.ShowDialog() != true)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while loading the MP3 information.");

        try
        {
            SourceDir = dialog.FolderName;

            var (files, generalDetails) = await DataManager.LoadFilesAsync(dialog.FolderName);
            Files = files.ToObservableCollection();
            GeneralDetails = generalDetails;
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Load);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Saves the general details
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveGeneralDetailsAsync()
    {
        if (Files.Count == 0)
            return;

        if (GeneralDetails.IsEmpty)
        {
            var result = await ShowQuestionAsync("Empty details", "Do you really want to remove all details?", "Yes",
                "No");
            if (result != MessageDialogResult.Affirmative)
                return;
        }

        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the MP3 information.");

        try
        {
            await DataManager.SaveGeneralDetails(Files, GeneralDetails);
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Load);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Saves the file details
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveFileDetailsAsync()
    {
        if (SelectedFile == null)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the MP3 information.");

        try
        {
            await Task.Run(() => DataManager.SaveFileDetails(SelectedFile));
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Load);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }
}