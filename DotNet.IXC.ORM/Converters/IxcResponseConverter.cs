using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM.Converters;


public class IxcResponseConverter : JsonConverter<IxcResponse>
{
    public static void ReadBaseProperty(ref Utf8JsonReader reader, string key, IxcResponse response, JsonSerializerOptions options)
    {
        switch (key)
        {
            case "type":
            case "tipo":
                response.Type = reader.GetString() ?? string.Empty;
                break;

            case "message":
            case "mensagem":
                response.Message = reader.GetString() ?? string.Empty;
                break;

            default:
                reader.Skip();
                break;
        }
    }


    public override IxcResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Erro na conversão da resposta. O conversor não conseguiu iniciar a conversão.");

        var response = new IxcResponse();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return response;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Falha na conversão da resposta. Nome da propriedade não encontrado.");

            string key = reader.GetString()?.ToLowerInvariant() ?? string.Empty;
            reader.Read();

            ReadBaseProperty(ref reader, key, response, options);
        }

        throw new JsonException("Erro na conversão da resposta. O conversor não conseguiu encerrar a conversão.");
    }


    public override void Write(Utf8JsonWriter writer, IxcResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", value.Type);
        writer.WriteString("message", value.Message);
        writer.WriteEndObject();
    }
}
