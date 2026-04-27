using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM;


public class IxcResponse
{
    public static string CreateResponseContentError(string error)
    {
        return JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { "type", "error" },
            { "message", error }
        });
    }


    protected static string? GetHtmlContent(string content)
    {
        if (content.StartsWith("<div style=\""))
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document.DocumentNode.InnerText;
        }

        return null;
    }


    protected readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };


    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;


    public IxcResponse() { }


    public IxcResponse(string content)
    {
        string? htmlContent = GetHtmlContent(content);
        if (htmlContent != null)
            LoadStatusAsError(htmlContent);
        else
            LoadStatusAsSuccess(content);
    }


    protected void LoadStatusAsError(string content)
    {
        Type = "error";
        Message = content;
    }


    protected void LoadStatusAsSuccess(string content)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<IxcResponse>(content, options);

            Type = deserialized?.Type ?? string.Empty;
            Message = deserialized?.Message ?? string.Empty;
        }
        catch (JsonException e)
        {
            throw new IxcOrmResponseException("Falha ao desserializar o status e a mensagem da resposta do IXC.", e);
        }
    }
}
