using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotNet.IXC.ORM.Test.Models;


public class Customer : IxcRecord
{
    [JsonPropertyName("razao")]
    public string Razao { get; set; } = string.Empty;

    [JsonPropertyName("cnpj_cpf")]
    public string CnpjCpf { get; set; } = string.Empty;
}
