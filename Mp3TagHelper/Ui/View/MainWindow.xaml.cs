using System.Windows;
using MahApps.Metro.Controls;
using Mp3TagHelper.Ui.ViewModel;

namespace Mp3TagHelper.Ui.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    /// <summary>
    /// Creates a new instance of the <see cref="MainWindow"/>
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Occurs when the form was loaded
    /// </summary>
    /// <param name="sender">The <see cref="MainWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.InitViewModel();
    }
}