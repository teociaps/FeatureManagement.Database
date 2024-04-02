// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database;

/// <summary>
/// Represents a feature with its associated settings.
/// </summary>
public class Feature
{
    /// <summary>
    /// The unique identifier of the feature.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// The unique name of the feature.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The settings associated with the feature.
    /// </summary>
    public virtual IEnumerable<FeatureSettings> Settings { get; set; }
}

// TODO: softdelete + auditing