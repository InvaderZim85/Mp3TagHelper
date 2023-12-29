using Serilog;
using Serilog.Events;
using System.IO;
using System.Windows;

namespace Mp3TagHelper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Occurs when the app is starting
    /// </summary>
    /// <param name="sender">The <see cref="App"/></param>
    /// <param name="e">The startup event arguments</param>
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        InitLogger(e.Args);
    }

    /// <summary>
    /// Init the logger
    /// </summary>
    /// <param name="args">The arguments</param>
    private static void InitLogger(IEnumerable<string> args)
    {
        var minLevel = args.Contains("debug", StringComparer.OrdinalIgnoreCase)
            ? LogEventLevel.Debug
            : LogEventLevel.Information;

        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(minLevel)
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "log_.log"), outputTemplate: template,
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}