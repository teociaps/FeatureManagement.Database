// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Represents a feature with its associated settings.
/// </summary>
public class Feature
{
    /// <summary>
    /// The unique identifier of the feature.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique name of the feature.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Describes whether any or all conditions in a set should be required to be true.
    /// </summary>
    public RequirementType RequirementType { get; set; }

    /// <summary>
    /// All settings associated with the feature.
    /// </summary>
    public ICollection<FeatureSettings> Settings { get; set; }
}

// TODO: softdelete + auditing?