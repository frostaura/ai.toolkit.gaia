using System.Text.Json;
using System.Text.Json.Serialization;
using FrostAura.MCP.Gaia.Enums;

namespace FrostAura.MCP.Gaia.Converters;

/// <summary>
/// Custom JSON converter for TaskStatus enum that serializes to string and supports reading both string and integer values for backward compatibility
/// </summary>
public class TaskStatusJsonConverter : JsonConverter<Enums.TaskStatus>
{
    /// <summary>
    /// Reads TaskStatus from JSON, supporting both string and integer values for backward compatibility
    /// </summary>
    /// <param name="reader">The JSON reader</param>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">Serializer options</param>
    /// <returns>TaskStatus enum value</returns>
    public override Enums.TaskStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                // Handle string values (new format)
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return Enums.TaskStatus.Todo; // Default fallback
                
                if (Enum.TryParse<Enums.TaskStatus>(stringValue, true, out var parsedStatus))
                    return parsedStatus;
                
                // If parsing fails, return default
                return Enums.TaskStatus.Todo;
                
            case JsonTokenType.Number:
                // Handle integer values (legacy format for backward compatibility)
                var intValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(Enums.TaskStatus), intValue))
                    return (Enums.TaskStatus)intValue;
                
                // If not a valid enum value, return default
                return Enums.TaskStatus.Todo;
                
            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} when reading TaskStatus");
        }
    }

    /// <summary>
    /// Writes TaskStatus to JSON as a string value
    /// </summary>
    /// <param name="writer">The JSON writer</param>
    /// <param name="value">The TaskStatus value to write</param>
    /// <param name="options">Serializer options</param>
    public override void Write(Utf8JsonWriter writer, Enums.TaskStatus value, JsonSerializerOptions options)
    {
        // Always write as string for consistency and readability
        writer.WriteStringValue(value.ToString());
    }
}
