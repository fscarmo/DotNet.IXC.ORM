using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM;


public sealed class IxcOrmResponse<T> : IxcResponse where T : class
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("registros")]
    public List<T> Records { get; set; } = [];


    public IxcOrmResponse() : base() { }


    public IxcOrmResponse(string content) : base(content)
    {
        string? htmlContent = GetHtmlContent(content);
        if (htmlContent != null)
            LoadContentAsError();
        else
            LoadContentAsSuccess(content);
    }


    private void LoadContentAsError()
    {
        Page = 0;
        Total = 0;
        Records = [];
    }


    private void LoadContentAsSuccess(string content)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<IxcOrmResponse<T>>(content, base.options);
            Page = deserialized?.Page ?? 0;
            Total = deserialized?.Total ?? 0;
            Records = deserialized?.Records ?? [];
        }
        catch (JsonException e)
        {
            throw new IxcOrmResponseException("Falha ao desserializar o conteúdo da resposta do IXC.", e);
        }
    }
}
