using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Mp3TagHelper.Business;
using Mp3TagHelper.Common;
using Mp3TagHelper.Model;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;

namespace Mp3TagHelper.Ui.ViewModel;

/// <summary>
/// Interaction logic for <see cref="View.MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The default window title
    /// </summary>
    private const string _defaultWindowTitle = "Mp3 Tag Helper";

    /// <summary>
    /// The instance of the media player
    /// </summary>
    private readonly MediaPlayer _player = new();

    /// <summary>
    /// Contains the value which indicates if the player is currently playing (needed for the pause function)
    /// </summary>
    private bool _playerIsPlaying;

    /// <summary>
    /// The timer for the playback
    /// </summary>
    private readonly DispatcherTimer _timer = new();

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
    /// Gets or sets the information
    /// </summary>
    [ObservableProperty]
    private string _info = "No directory selected";

    /// <summary>
    /// Gets or sets the file information
    /// </summary>
    [ObservableProperty]
    private string _fileInfo = "Files";

    /// <summary>
    /// Gets or sets the play info
    /// </summary>
    [ObservableProperty]
    private string _playInfo = "00:00 / 00:00";

    /// <summary>
    /// Gets or sets the value which indicates if the play button is enabled
    /// </summary>
    [ObservableProperty]
    private bool _buttonPlayEnabled;

    /// <summary>
    /// Gets or sets the value which indicates if the pause button is enabled
    /// </summary>
    [ObservableProperty]
    private bool _buttonPauseEnabled;

    /// <summary>
    /// Gets or sets the value which indicates if the stop button is enabled
    /// </summary>
    [ObservableProperty]
    private bool _buttonStopEnabled;

    /// <summary>
    /// Gets or sets the max. value of the slider
    /// </summary>
    [ObservableProperty]
    private long _sliderMax = 100;

    /// <summary>
    /// Gets or sets the current position of the slider
    /// </summary>
    [ObservableProperty]
    private long _sliderPosition;

    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "Mp3 Tag Helper";

    /// <summary>
    /// Occurs when the state of the <see cref="ButtonPlayEnabled"/> value was changed
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnButtonPlayEnabledChanged(bool value)
    {
        ButtonStopEnabled = !value;
        ButtonPauseEnabled = !value;
    }

    /// <summary>
    /// Occurs when the user selects a file
    /// </summary>
    /// <param name="value">The file which was selected</param>
    partial void OnSelectedFileChanged(Mp3File? value)
    {
        ButtonPlayEnabled = value != null;
    }

    /// <summary>
    /// Init the view model
    /// </summary>
    public void InitViewModel()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += (_, _) =>
        {
            PlayInfo = $@"{_player.Position:mm\:ss} / {_player.NaturalDuration.TimeSpan:mm\:ss}";

            // Set the slider
            SliderMax = _player.NaturalDuration.TimeSpan.Ticks;
            SliderPosition = _player.Position.Ticks;
        };
    }

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

        Info = $"Source directory: {dialog.FolderName}";

        var controller = await ShowProgressAsync("Please wait", "Please wait while loading the MP3 information.");

        try
        {
            SourceDir = dialog.FolderName;

            var (files, generalDetails) = await DataManager.LoadFilesAsync(dialog.FolderName);
            FileInfo = $"{files.Count} files loaded";
            Files = files.ToObservableCollection();
            GeneralDetails = generalDetails;
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Load);
            Info = "Error while loading data";
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

        var controller = await ShowProgressAsync("Please wait", "Please wait while saving the information.");

        try
        {
            await DataManager.SaveGeneralDetails(Files, GeneralDetails);
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Load);
            Info = "Error while saving data";
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

    /// <summary>
    /// Starts the playback
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task StartPlaybackAsync()
    {
        if (SelectedFile == null)
            return;

        ButtonPlayEnabled = false;

        try
        {
            WindowTitle = $"{_defaultWindowTitle} [Playing - {SelectedFile.Name}]";

            // Check if the player is already playing (pause menu)
            if (_playerIsPlaying)
            {
                _player.Play();
                return;
            }

            _player.Open(new Uri(SelectedFile.Path));

            // Start the timer
            _timer.Start();

            // Play the song
            _player.Play();

            _playerIsPlaying = true;
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Custom, "An error has occurred while playing the song.");
        }
    }

    /// <summary>
    /// Pauses the playback
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task PausePlaybackAsync()
    {
        if (SelectedFile == null)
            return;

        ButtonPlayEnabled = true;

        try
        {
            _player.Pause();

            WindowTitle = $"{_defaultWindowTitle} [Pause - {SelectedFile.Name}]";
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Custom, "An error has occurred while pausing the song.");
        }
    }

    /// <summary>
    /// Stops the playback
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task StopPlaybackAsync()
    {
        ButtonPlayEnabled = true;

        try
        {
            _player.Stop();
            _timer.Stop();
            SliderPosition = 0;
            PlayInfo = "00:00 / 00:00";

            _playerIsPlaying = false;

            WindowTitle = _defaultWindowTitle;
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync(ex, ErrorType.Custom, "An error has occurred while stopping the song.");
        }
    }
}