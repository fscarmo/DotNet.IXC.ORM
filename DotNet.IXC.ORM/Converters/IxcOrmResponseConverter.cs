using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM.Converters;


public class IxcOrmResponseConverter<T> : JsonConverter<IxcOrmResponse<T>> where T : IxcRecord
{
    public override IxcOrmResponse<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Erro na conversão do ORM. O conversor não conseguiu iniciar a conversão.");

        var response = new IxcOrmResponse<T>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return response;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Falha na conversão do ORM. Nome da propriedade não encontrado.");

            string key = reader.GetString()?.ToLowerInvariant() ?? string.Empty;
            reader.Read();

            switch (key)
            {
                case "type":
                case "tipo":
                    response.Type = reader.GetString() ?? "success";
                    break;

                case "message":
                case "mensagem":
                    response.Message = reader.GetString() ?? string.Empty;
                    break;

                case "page":
                case "pagina":
                    response.Page = IxcResponseConverter.ReadInt32Safely(ref reader);
                    break;

                case "total":
                    response.Total = IxcResponseConverter.ReadInt32Safely(ref reader);
                    break;

                case "records":
                case "registros":
                    response.Records = JsonSerializer.Deserialize<List<T>>(ref reader, options) ?? [];
                    break;
            }
        }

        throw new JsonException("Erro na conversão do ORM. O conversor não conseguiu encerrar a conversão.");
    }


    public override void Write(Utf8JsonWriter writer, IxcOrmResponse<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", value.Type);
        writer.WriteString("message", value.Message);
        writer.WriteNumber("page", value.Page);
        writer.WriteNumber("total", value.Total);

        writer.WriteStartArray("registros");
        foreach (var record in value.Records)
        {
            JsonSerializer.Serialize(writer, record, options);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
