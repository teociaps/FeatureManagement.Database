// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database;

/// <summary>
/// Represents different types of feature filters used for enabling features.
/// </summary>
public enum FeatureFilterType
{
    /// <summary>
    /// Feature is always enabled.
    /// </summary>
    AlwaysOn,

    /// <summary>
    /// Feature is enabled based on a percentage.
    /// </summary>
    Percentage,

    /// <summary>
    /// Feature is enabled within a specified time window.
    /// </summary>
    TimeWindow,

    /// <summary>
    /// Feature is enabled based on targeting criteria.
    /// </summary>
    Targeting
}