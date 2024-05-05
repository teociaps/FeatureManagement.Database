// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Represents JSON string as an <see cref="IConfigurationSource"/>.
/// </summary>
/// <param name="jsonString">The JSON string to be converted into <see cref="IConfiguration"/>.</param>
internal sealed class JsonStringConfigurationSource(string jsonString) : IConfigurationSource
{
    private readonly string _jsonString = jsonString;

    /// <inheritdoc/>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new JsonStringConfigurationProvider(_jsonString);
    }
}