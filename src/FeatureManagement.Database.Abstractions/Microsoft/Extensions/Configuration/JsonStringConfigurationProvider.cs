// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// Load implementation code from https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.Json/src/JsonConfigurationFileParser.cs

using System.Diagnostics;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// A JSON string based <see cref="ConfigurationProvider"/>.
/// </summary>
/// <param name="jsonString">The JSON string to be converted into <see cref="IConfiguration"/>.</param>
internal sealed class JsonStringConfigurationProvider(string jsonString) : ConfigurationProvider
{
    private readonly string _jsonString = jsonString;
    private readonly Dictionary<string, string> _data = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _paths = new();

    /// <summary>
    /// Loads the JSON.
    /// </summary>
    /// <exception cref="FormatException">Thrown if any JSON parser error occurs.</exception>
    public override void Load()
    {
        try
        {
            var jsonDocumentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            using (JsonDocument doc = JsonDocument.Parse(_jsonString, jsonDocumentOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException($"Invalid top level JSON element: {doc.RootElement.ValueKind}");
                }
                VisitObjectElement(doc.RootElement);
            }

            Data = _data;
        }
        catch (JsonException ex)
        {
            throw new FormatException("Invalid JSON format in configuration string.", ex);
        }
    }

    #region Private

    private void VisitObjectElement(JsonElement element)
    {
        var isEmpty = true;

        foreach (JsonProperty property in element.EnumerateObject())
        {
            isEmpty = false;
            EnterContext(property.Name);
            VisitValue(property.Value);
            ExitContext();
        }

        SetNullIfElementIsEmpty(isEmpty);
    }

    private void VisitArrayElement(JsonElement element)
    {
        int index = 0;

        foreach (JsonElement arrayElement in element.EnumerateArray())
        {
            EnterContext(index.ToString());
            VisitValue(arrayElement);
            ExitContext();
            index++;
        }

        SetNullIfElementIsEmpty(isEmpty: index == 0);
    }

    private void SetNullIfElementIsEmpty(bool isEmpty)
    {
        if (isEmpty && _paths.Count > 0)
        {
            _data[_paths.Peek()] = null;
        }
    }

    private void VisitValue(JsonElement value)
    {
        Debug.Assert(_paths.Count > 0);

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitObjectElement(value);
                break;

            case JsonValueKind.Array:
                VisitArrayElement(value);
                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                string key = _paths.Peek();
                if (_data.ContainsKey(key))
                {
                    throw new FormatException($"Key is duplicated: {key}");
                }
                _data[key] = value.ToString();
                break;

            default:
                throw new FormatException($"Unsupported JSON token: {value.ValueKind}");
        }
    }

    private void EnterContext(string context) =>
        _paths.Push(_paths.Count > 0 ?
                _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

    private void ExitContext() => _paths.Pop();

    #endregion Private
}