using System.Text.Json;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM;


public sealed class IxcOrmResponse<T> : IxcResponse where T : class
{
    public int Page { get; set; }

    public int Total { get; set; }

    public List<T> Records { get; set; } = [];


    public IxcOrmResponse() : base() { }


    public IxcOrmResponse(string content) : base()
    {
        string? htmlContent = GetHtmlContent(content);
        if (htmlContent != null)
            LoadContentAsError(htmlContent);
        else
            LoadContentAsSuccess(content);
    }


    private void LoadContentAsError(string content)
    {
        Type = "error";
        Message = content;
        Page = 0;
        Total = 0;
        Records = [];
    }


    private void LoadContentAsSuccess(string content)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<IxcOrmResponse<T>>(content, base.options);
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
