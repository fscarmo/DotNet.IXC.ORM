using System.Text.Json.Serialization;

namespace DotNet.IXC.ORM.Test.Models;


public class Client
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("razao")]
    public string Razao { get; set; } = string.Empty;
}
