// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database;

/// <summary>
/// Represents settings associated with a feature.
/// </summary>
public class FeatureSettings
{
    /// <summary>
    /// The unique identifier of the feature settings.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// The filter type used for enabling the feature.
    /// </summary>
    public FeatureFilterType FilterType { get; set; }

    /// <summary>
    /// The parameters associated with the feature settings.
    /// </summary>
    public string Parameters { get; set; }

    /// <summary>
    /// The feature associated with the settings.
    /// </summary>
    public virtual Feature Feature { get; set; }
}
