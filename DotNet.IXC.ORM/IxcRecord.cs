using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM;


public abstract class IxcRecord
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; set; }


    public string ToJsonString()
    {
        Type type = GetType();
        try
        {
            return JsonSerializer.Serialize(this, type);
        }
        catch (Exception e)
        {
            throw new JsonException($"Falha ao serializar o objeto do tipo {type.Name}.", e);
        }
    }
}
