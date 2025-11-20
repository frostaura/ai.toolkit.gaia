using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrostAura.MCP.Gaia.Converters;

/// <summary>
/// Generic JSON converter for enums that serializes to string and supports reading both string and integer values for backward compatibility
/// </summary>
/// <typeparam name="T">The enum type</typeparam>
public class EnumStringJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    /// <summary>
    /// Reads enum from JSON, supporting both string and integer values for backward compatibility
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">Serializer options</param>
    /// <returns>Enum value</returns>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                // Handle string values (new format)
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return GetDefaultValue(); // Default fallback
                
                if (Enum.TryParse<T>(stringValue, true, out var parsedEnum))
                    return parsedEnum;
                
                // If parsing fails, return default
                return GetDefaultValue();
                
            case JsonTokenType.Number:
                // Handle integer values (legacy format for backward compatibility)
                var intValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(T), intValue))
                    return (T)(object)intValue;
                
                // If not a valid enum value, return default
                return GetDefaultValue();
                
            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} when reading {typeof(T).Name}");
        }
    }

    /// <summary>
    /// Writes enum to JSON as a string value
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The enum value to write</param>
    /// <param name="options">Serializer options</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Always write as string for consistency and readability
        writer.WriteStringValue(value.ToString());
    }

    /// <summary>
    /// Gets the default value for the enum (first defined value)
    /// </summary>
    /// <returns>Default enum value</returns>
    private static T GetDefaultValue()
    {
        var values = Enum.GetValues<T>();
        return values.Length > 0 ? values[0] : default;
    }
}
