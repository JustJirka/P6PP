using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }
                if (int.TryParse(str, out int result))
                {
                    return result;
                }
                throw new JsonException($"Unable to convert \"{str}\" to an integer.");
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            throw new JsonException($"Unexpected token parsing int?. Token: {reader.TokenType}");
        }
        catch (FormatException ex)
        {
            throw new JsonException("Invalid format for Int32.", ex);
        }
        catch (Exception ex)
        {
            throw new JsonException("An unexpected error occurred during JSON deserialization.", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
