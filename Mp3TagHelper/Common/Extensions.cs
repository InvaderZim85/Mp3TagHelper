using System.Collections.ObjectModel;

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
}