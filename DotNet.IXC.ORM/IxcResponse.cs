using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM;


public class IxcResponse<T> where T : class
{
    private static string? GetHtmlContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;

        var document = new HtmlDocument();
        document.LoadHtml(content);

        bool hasHtml = document.DocumentNode
            .Descendants()
            .Any(n => n.NodeType == HtmlNodeType.Element);

        if (!hasHtml)
            return null;

        return document.DocumentNode.InnerText;
    }


    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };


    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("registros")]
    public List<T> Records { get; set; } = [];


    public IxcResponse() { }


    public IxcResponse(string content)
    {
        string? htmlContent = GetHtmlContent(content);
        if (htmlContent != null)
        {
            LoadResponseAsError(htmlContent);
        }
        else
        {
            LoadResponseAsSuccess(content);
        }
    }


    private void LoadResponseAsError(string content)
    {
        Type = "error";
        Message = content;
        Page = 0;
        Total = 0;
    }


    private void LoadResponseAsSuccess(string content)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<IxcResponse<T>>(content, options);

            Type = deserialized?.Type ?? string.Empty;
            Message = deserialized?.Message ?? string.Empty;
            Page = deserialized?.Page ?? 0;
            Total = deserialized?.Total ?? 0;
            Records = deserialized?.Records ?? [];
        }
        catch (JsonException e)
        {
            throw new IxcOrmResponseException("Falha ao desserializar a resposta do IXC.", e);
        }
    }
}
