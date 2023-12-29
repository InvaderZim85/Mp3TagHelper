using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3TagHelper.Model;

/// <summary>
/// Provides the general details
/// </summary>
internal class GeneralDetails
{
    /// <summary>
    /// Gets or sets the album name
    /// </summary>
    public string Album { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the artist list (separated by a comma)
    /// </summary>
    public string Artists { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the composers
    /// </summary>
    public string Composers { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the genre
    /// </summary>
    public string Genres { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release year
    /// </summary>
    public uint Year { get; set; }

    /// <summary>
    /// Gets the value which indicates if the entry is empty
    /// </summary>
    public bool IsEmpty => !string.IsNullOrWhiteSpace(Album) &&
                           !string.IsNullOrWhiteSpace(Artists) &&
                           !string.IsNullOrWhiteSpace(Composers) &&
                           !string.IsNullOrWhiteSpace(Genres) &&
                           Year != 0;
}