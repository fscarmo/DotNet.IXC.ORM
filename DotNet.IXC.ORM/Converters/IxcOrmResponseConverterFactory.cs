using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM.Converters;


public class IxcOrmResponseConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(IxcOrmResponse<>);
    }


    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type type = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(IxcOrmResponseConverter<>).MakeGenericType(type);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}
