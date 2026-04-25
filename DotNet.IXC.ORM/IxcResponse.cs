using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM;


public class IxcResponse<T>(string content)
{
    private readonly string content = content;


    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
