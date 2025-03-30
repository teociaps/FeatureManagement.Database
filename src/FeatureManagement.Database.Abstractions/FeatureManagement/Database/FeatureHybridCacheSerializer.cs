#if NET9_0_OR_GREATER
using Microsoft.Extensions.Caching.Hybrid;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeatureManagement.Database;

/// <summary>
/// Custom serializer and deserializer for Feature objects.
/// </summary>
public class FeatureHybridCacheSerializer : IHybridCacheSerializer<Feature>
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerOptions.Default)
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    /// <summary>
    /// Deserializes a ReadOnlySequence of bytes to a Feature object.
    /// </summary>
    /// <param name="source">The ReadOnlySequence of bytes to deserialize.</param>
    /// <returns>The deserialized Feature object.</returns>
    public Feature Deserialize(ReadOnlySequence<byte> source)
    {
        var reader = new Utf8JsonReader(source);
        return JsonSerializer.Deserialize<Feature>(ref reader, _jsonOptions);
    }

    /// <summary>
    /// Serializes a Feature object to an IBufferWriter of bytes.
    /// </summary>
    /// <param name="value">The Feature object to serialize.</param>
    /// <param name="target">The IBufferWriter to write the serialized bytes to.</param>
    public void Serialize(Feature value, IBufferWriter<byte> target)
    {
        using var writer = new Utf8JsonWriter(target);
        JsonSerializer.Serialize(writer, value, _jsonOptions);
    }
}
#endif