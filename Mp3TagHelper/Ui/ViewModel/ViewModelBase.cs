using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using Serilog;

namespace Mp3TagHelper.Ui.ViewModel;

/// <summary>
/// Provides the basic functions for a view model
/// </summary>
internal class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Provides different error types
    /// </summary>
    protected enum ErrorType
    {
        /// <summary>
        /// Generic error message
        /// </summary>
        General,

        /// <summary>
        /// Error which has occurred during a load process
        /// </summary>
        Load,

        /// <summary>
        /// Error which has occurred during a save process
        /// </summary>
        Save,

        /// <summary>
        /// A custom error message
        /// </summary>
        Custom
    }

    /// <summary>
    /// Shows a message
    /// </summary>
    /// <param name="title">The title of the message</param>
    /// <param name="message">The message</param>
    /// <returns>The awaitable task</returns>
    protected Task ShowMessageAsync(string title, string message)
    {
        return DialogCoordinator.Instance.ShowMessageAsync(this, title, message);
    }

    /// <summary>
    /// Shows an error message according to the given error type
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <param name="type">The error type</param>
    /// <param name="message">
    /// The error message which should be shown.
    /// <para />
    /// <b>Note:</b> This message will only be used when the <paramref name="type"/> is set to <see cref="ErrorType.Custom"/>
    /// </param>
    /// <returns>The awaitable task</returns>
    protected Task ShowErrorMessageAsync(Exception ex, ErrorType type, string message = "")
    {
        var tmpMessage = type switch
        {
            ErrorType.General => "An error has occurred.",
            ErrorType.Load => "An error has occurred while loading the data.",
            ErrorType.Save => "An error has occurred while saving the data",
            ErrorType.Custom => message,
            _ => "An error has occured."
        };

        // Log the error
        Log.Error(ex, message);

        return DialogCoordinator.Instance.ShowMessageAsync(this, "Error", tmpMessage);
    }

    /// <summary>
    /// Shows a progress dialog
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message</param>
    /// <returns>The progress dialog</returns>
    protected async Task<ProgressDialogController> ShowProgressAsync(string title, string message)
    {
        var controller = await DialogCoordinator.Instance.ShowProgressAsync(this, title, message);
        controller.SetIndeterminate();

        return controller;
    }

    /// <summary>
    /// Shows a question dialog with two buttons
    /// </summary>
    /// <param name="title">The title of the dialog</param>
    /// <param name="message">The message of the dialog</param>
    /// <param name="okButtonText">The text of the ok button (optional)</param>
    /// <param name="cancelButtonText">The text of the cancel button (optional)</param>
    /// <returns>The dialog result</returns>
    protected async Task<MessageDialogResult> ShowQuestionAsync(string title, string message, string okButtonText = "OK",
        string cancelButtonText = "Cancel")
    {
        var result = await DialogCoordinator.Instance.ShowMessageAsync(this, title, message,
            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AffirmativeButtonText = okButtonText,
                NegativeButtonText = cancelButtonText
            });

        return result;
    }
}