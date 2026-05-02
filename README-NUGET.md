Esse ORM foi criado com o intuito de facilitar o consumo de dados da API oficial do [IXC Provedor](https://ixcsoft.com/ixc-provedor).\
Essa biblioteca não faz parte das bibliotecas oficiais da [IXCsoft](https://ixcsoft.com/) e foi desenvolvida de forma independente e sem fins lucrativos.


### :arrow_down: Download


```bash
dotnet add package IXC.ORM
```

Após o download, certifique-se de que a dependência foi adicionada ao seu .csproj...

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    ...
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="IXC.ORM" Version="1.0.5" />
    ...
  </ItemGroup>
  ...
```


### :desktop_computer: Variáveis de Ambiente


> No `appsettings.json` ou `appsettings.Development.json`.
```json
{
  "IxcOrm": {
    "AccessToken": "token gerado dentro do IXC",
    "ServerDomain": "dominiodoseuixc.com.br"
  }
}
```

> No `docker-compose.yaml`.
```yaml
services:
  sua-aplicacao:
    build:
      context: .
      dockerfile: Dockerfile
    image: sua-imagem-docker:0.0.0
    environment:
      # Obtendo as variáveis a partir de um .env
      - IxcOrm__AccessToken=${IXC_ACCESS_TOKEN}
      - IxcOrm__ServerDomain=${IXC_SERVER_DOMAIN}
      ...
```

> Siga o exemplo abaixo para carregar o ambiente da biblioteca em tempo de execução.
```csharp
using DotNet.IXC.ORM.Config; // Sem isso, builder.Services.AddIxcOrmEnvironment(config => {}); não funciona!!!

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIxcOrmEnvironment(config =>
{
    var accessToken = builder.Configuration.GetValue<string>("IxcOrm:AccessToken")
        ?? throw new InvalidOperationException("A variável IxcOrm:AccessToken não está configurada.");

    var serverDomain = builder.Configuration.GetValue<string>("IxcOrm:ServerDomain")
        ?? throw new InvalidOperationException("A variável IxcOrm:ServerDomain não está configurada.");

    config.SetupAccessToken(accessToken);
    config.SetupServerDomain(serverDomain);
});
```


### :man_technologist: Implementação


> Convertendo resultados com o auxílio da classe `IxcRecord`.
```csharp
using DotNet.IXC.ORM;
using System.Text.Json.Serialization;

public class Cliente : IxcRecord
{
    [JsonPropertyName("razao")]
    public string Razao { get; set; } = string.Empty;

    [JsonPropertyName("cnpj_cpf")]
    public string CnpjCpf { get; set; } = string.Empty;
}
```

> Utilizando a classe `IxcOrm` para poder enviar as requisições ao o seu IXC Provedor.
```csharp
using DotNet.IXC.ORM;

public class ClienteOrm : IxcOrm
{
    public static ClienteOrm NewOrm()
    {
        return new ClienteOrm();
    }

    private ClienteOrm() : base("cliente") { } // Define a tabela "cliente" como alvo das consultas.

    public async Task<List<Cliente>?> FindByCnpjOrCpf(string cnpjOrCpf)
    {
        IxcOrmResponse<Cliente> response = await Where("cnpj_cpf")
            .Like(cnpjOrCpf)
            .GetAsync<Cliente>();

        if (response is null || response.Total == 0)
        {
            return null;
        }
        
        return response.Records;
    }
}
```

> Ciar uma classe derivada de `IxcOrm`, como no exemplo acima, permitirá que você a utilize como no exemplo abaixo...
```csharp
// EXEMPLO DE COMO IMPLEMENTAR UM POSSÍVEL MÉTODO "GET" EM UM CONTROLLER DA SUA API

[HttpGet("{cnpjOrCpf}")]
public async Task<IActionResult> FindByCnpjOrCpf([FromRoute] string cnpjOrCpf)
{
    using var orm = ClienteOrm.NewOrm();
    
    try
    {
        var clientes = await orm.FindByCnpjOrCpf(cnpjOrCpf);

        if (clientes is null)
        {
            return NotFound();
        }

        return Ok(clientes);
    }
    catch (IxcOrmResponseException e)
    {
        return Problem(detail: e.Message, statusCode: 500);
    }
    catch (IxcOrmRequestException e)
    {
        return Problem(detail: e.Message, statusCode: 400);
    }
}
```


> [!TIP]
> Você não é obrigado a seguir à risca os exemplos acima... ☝🏻\
> Eles servem apenas como orientação sobre como utilizar esta biblioteca!


# Contribuições


Contribuições são sempre bem-vindas!\
Se você conhece uma maneira melhor de fazer algo, por favor, me avise!\
Ou sinta-se a vontade para enviar um novo PR!

At.te,\
<b>Felipe S. Carmo</b>.