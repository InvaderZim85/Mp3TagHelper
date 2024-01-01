using System.Collections.ObjectModel;
using System.IO;

namespace Mp3TagHelper.Common;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Converts the <see cref="IEnumerable{T}"/> into a <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    /// <param name="source">The source data</param>
    /// <returns>The <see cref="ObservableCollection{T}"/></returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    /// <summary>
    /// Converts the size of the file into a readable format (bytes, KB, MB, GB)
    /// </summary>
    /// <param name="fileLength">The file length (aka size)</param>
    /// <param name="divider">The divider. If the value is 0, it will be automatically set to 1024 (optional)</param>
    /// <param name="addBytes"><see langword="true"/> to add the bytes to the end, otherwise <see langword="false"/></param>
    /// <returns>The converted size</returns>
    public static string ConvertToFileSize(this long fileLength, int divider = 1024, bool addBytes = false)
    {
        if (divider == 0)
            divider = 1024;

        var result = fileLength switch
        {
            var size when size < divider => $"{size:N0} Bytes",
            var size when size >= divider && size < Math.Pow(divider, 2) => $"{size / divider:N2} KB",
            var size when size >= Math.Pow(divider, 2) && size < Math.Pow(divider, 3) =>
                $"{size / Math.Pow(divider, 2):N2} MB",
            var size when size >= Math.Pow(divider, 3) && size <= Math.Pow(divider, 4) => $"{size / Math.Pow(divider, 3):N2} GB",
            var size when size >= Math.Pow(divider, 4) => $"{size / Math.Pow(divider, 4)} TB",
            _ => fileLength.ToString()
        };

        return addBytes ? $"{result} ({fileLength:N0} Bytes)" : result;
    }
}