using Mp3TagHelper.Model;
using System.IO;

namespace Mp3TagHelper.Business;

/// <summary>
/// Provides the functions for the interaction with the files
/// </summary>
internal static class DataManager
{
    /// <summary>
    /// Loads the files of the source directory
    /// </summary>
    /// <param name="source">The source</param>
    /// <returns>The list with the files</returns>
    public static async Task<(List<Mp3File> files, GeneralDetails generalDetails)> LoadFilesAsync(string source)
    {
        if (string.IsNullOrWhiteSpace(source) || !Directory.Exists(source))
            return ([], new GeneralDetails());

        var dir = new DirectoryInfo(source);
        var files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

        var tmpFiles = files.Where(w => w.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase) ||
                                        w.Extension.Equals(".flac", StringComparison.OrdinalIgnoreCase) ||
                                        w.Extension.Equals(".ogg", StringComparison.OrdinalIgnoreCase));

        return await Task.Run(() => ExtractFileData(tmpFiles));
    }

    /// <summary>
    /// Extracts the file data
    /// </summary>
    /// <param name="files">The list with the files</param>
    /// <returns>The list with the extracted data</returns>
    private static (List<Mp3File> files, GeneralDetails generalDetails) ExtractFileData(IEnumerable<FileInfo> files)
    {
        var fileResult = (from file in files
            let tagInfo = TagLib.File.Create(file.FullName)
            select new Mp3File(file)
            {
                Album = tagInfo.Tag.Album,
                TrackNumber = tagInfo.Tag.Track,
                Title = tagInfo.Tag.Title,
                Artists = ArrayToString(tagInfo.Tag.AlbumArtists),
                Composers = ArrayToString(tagInfo.Tag.Composers),
                Genres = ArrayToString(tagInfo.Tag.Genres),
                Comment = tagInfo.Tag.Comment,
                Copyright = tagInfo.Tag.Copyright ?? string.Empty,
                Year = tagInfo.Tag.Year,
                Duration = tagInfo.Properties.Duration,

            }).ToList();

        var generalDetails = new GeneralDetails
        {
            Album = GetStringValue(entry => entry.Album),
            Artists = GetStringValue(entry => entry.Artists),
            Composers = GetStringValue(entry => entry.Composers),
            Genres = GetStringValue(entry => entry.Genres),
            Year = GetIntValue(entry => entry.Year)
        };

        return (fileResult, generalDetails);

        string ArrayToString(IEnumerable<string> values)
        {
            return string.Join(", ", values.Where(w => !string.IsNullOrWhiteSpace(w)));
        }

        string GetStringValue(Func<Mp3File, string> selector)
        {
            var tmpData = fileResult.Select(selector).Distinct().ToList();
            return tmpData.Count == 1
                ? tmpData[0]
                : ArrayToString(tmpData);
        }

        uint GetIntValue(Func<Mp3File, uint> selector)
        {
            var tmpData = fileResult.Select(selector).Distinct().ToList();
            return tmpData.Count == 1
                ? tmpData[0]
                : tmpData.Min();
        }
    }

    /// <summary>
    /// Saves the general details
    /// </summary>
    /// <param name="files">The list with the files</param>
    /// <param name="details">The details</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveGeneralDetails(IEnumerable<Mp3File> files, GeneralDetails details)
    {
        foreach (var file in files)
        {
            await Task.Run(() => SaveFileDetails(file, details));
        }
    }

    /// <summary>
    /// Saves the file details
    /// </summary>
    /// <param name="mp3File">The mp3 file</param>
    /// <param name="details">The general details</param>
    private static void SaveFileDetails(Mp3File mp3File, GeneralDetails details)
    {
        var file = TagLib.File.Create(mp3File.Path);
        if (file == null)
            return;

        file.Tag.Album = details.Album;
        file.Tag.AlbumArtists = ToArray(details.Artists);
        file.Tag.Composers = ToArray(details.Composers);
        file.Tag.Genres = ToArray(details.Genres);
        file.Tag.Year = details.Year;
        file.Save();

        mp3File.SetDetails(details);

        return;

        static string[] ToArray(string value)
        {
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
        }
    }

    /// <summary>
    /// Saves the details of a single file
    /// </summary>
    /// <param name="mp3File">The file which should be saved</param>
    public static void SaveFileDetails(Mp3File mp3File)
    {
        var file = TagLib.File.Create(mp3File.Path);
        if (file == null)
            return;

        file.Tag.Title = mp3File.Title;
        file.Tag.Track = mp3File.TrackNumber;
        file.Tag.Album = mp3File.Album;
        file.Tag.AlbumArtists = ToArray(mp3File.Artists);
        file.Tag.Composers = ToArray(mp3File.Composers);
        file.Tag.Genres = ToArray(mp3File.Genres);
        file.Tag.Year = mp3File.Year;
        file.Tag.Comment = mp3File.Comment;
        file.Save();

        return;

        static string[] ToArray(string value)
        {
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
        }
    }
}