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
    /// <para>
    /// Used for custom filter types not defined in the <see cref="FeatureFilterType"/> enum.
    /// </para>
    /// <para>
    /// It's optional and defaults to <see langword="null"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If <see cref="FilterType"/> is set to <see cref="FeatureFilterType.Custom"/>, you must provide a value for
    /// <see cref="CustomFilterTypeName"/>. This string should represent the name of your custom filter type.
    /// Otherwise, the provided <see cref="FilterType"/> from the <see cref="FeatureFilterType"/> enum will
    /// be used for feature flags logics.
    /// </remarks>
    public string CustomFilterTypeName
    {
        get => _customFilterTypeName;
        set
        {
            ValidateCustomFilterType(_filterType, value);
            _customFilterTypeName = value;
        }
    }

    private string _customFilterTypeName;

    /// <summary>
    /// The filter type used for enabling the feature.
    /// </summary>
    public FeatureFilterType FilterType
    {
        get => _filterType;
        set
        {
            ValidateCustomFilterType(value, _customFilterTypeName);
            _filterType = value;
        }
    }

    private FeatureFilterType _filterType;


    /// <summary>
    /// The parameters associated with the feature settings.
    /// </summary>
    public string Parameters { get; set; }

    /// <summary>
    /// The feature associated with the settings.
    /// </summary>
    public virtual Feature Feature { get; set; }

    /// <summary>
    /// Returns the appropriate filter type name, considering custom filters.
    /// </summary>
    /// <returns>The filter type name.</returns>
    internal string GetFilterType() =>
        FilterType is FeatureFilterType.Custom ? CustomFilterTypeName : FilterType.ToString();

    private void ValidateCustomFilterType(FeatureFilterType filterType, string customFilterTypeName)
    {
        if (string.IsNullOrWhiteSpace(customFilterTypeName) && filterType is FeatureFilterType.Custom)
        {
            throw new ArgumentException($"{nameof(CustomFilterTypeName)} is required when {nameof(FilterType)} is set to {nameof(FeatureFilterType.Custom)}.");
        }
    }
}