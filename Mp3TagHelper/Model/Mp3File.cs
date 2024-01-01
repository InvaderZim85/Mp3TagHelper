using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Mp3TagHelper.Common;
using ZimLabs.CoreLib;

namespace Mp3TagHelper.Model;

/// <summary>
/// Represents a MP3 file
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="Mp3File"/> and sets the basic file infos
/// </remarks>
/// <param name="file">The original file</param>
internal partial class Mp3File(FileInfo file) : ObservableObject
{
    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public string Name { get; } = file.Name;

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    public string Path { get; } = file.FullName;

    /// <summary>
    /// Gets the size of the file
    /// </summary>
    public string Size { get; } = file.Length.ConvertToFileSize();

    /// <summary>
    /// Gets the file length
    /// </summary>
    public long Length { get; } = file.Length;

    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the track number
    /// </summary>
    public uint TrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the album name
    /// </summary>
    [ObservableProperty]
    private string _album = string.Empty;

    /// <summary>
    /// Gets or sets the artist list (separated by a comma)
    /// </summary>
    [ObservableProperty]
    private string _artists = string.Empty;

    /// <summary>
    /// Gets or sets the composers
    /// </summary>
    [ObservableProperty]
    private string _composers = string.Empty;

    /// <summary>
    /// Gets or sets the genre
    /// </summary>
    [ObservableProperty]
    private string _genres = string.Empty;

    /// <summary>
    /// Gets or sets the comment
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the copyright
    /// </summary>
    public string Copyright { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release year
    /// </summary>
    [ObservableProperty]
    private uint _year;

    /// <summary>
    /// Gets or sets the duration of the MP3
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets the duration (only needed for the data grid)
    /// </summary>
    public string DurationView => Duration.ToString("hh\\:mm\\:ss");

    /// <summary>
    /// Sets the details
    /// </summary>
    /// <param name="details"></param>
    public void SetDetails(GeneralDetails details)
    {
        Album = details.Album;
        Artists = details.Artists;
        Composers = details.Composers;
        Genres = details.Genres;
        Year = details.Year;
    }
}