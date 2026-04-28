using DotNet.IXC.ORM.Api;
using DotNet.IXC.ORM.Api.Records;
using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM;


/// <summary>
/// A classe <c>IxcOrm</c> implementa métodos que geram uma query de busca e herda o comportamento da classe
/// <see cref="RequestEmitter"/>, para disponibilizar, através da mesma instância, os métodos que executam requisições HTTP
/// para a API do IXC Provedor.
/// <para>
/// Essa classe manipula as classes de ordenação, paginação e de construção de parâmetros, além de gerar um JSON compatível com
/// a query de busca da API do IXC Provedor.
/// </para>
/// </summary>
/// <remarks>
/// Autor: Felipe S. Carmo <br/>
/// Versão: 1.2.0 <br/>
/// Desde: 2026-04-28
/// </remarks>
public class IxcOrm(string table) : RequestEmitter(table)
{
    private readonly List<Parameter> parameters = [];


    private Ordering ordering = Ordering.AscBy(table, "id");
    private Pagination pagination = Pagination.Default;
    private Parameter.Builder parameterBuilder = Parameter.NewBuilder(table);


    /// <summary>
    /// Compara a string "expected" com a query gerada pela classe, ignorando espaços em branco e quebras de linha.
    /// Retorna true se as strings forem equivalentes, ou false caso contrário.
    /// </summary>
    /// <param name="expected">A string esperada para comparação com a query gerada.</param>
    /// <returns>True se as strings forem equivalentes, ou false caso contrário.</returns>
    public bool ValidateQuery(string expected)
    {
        string normalizedExpected = expected
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");

        string actual = GetQueryAsJsonString();

        return normalizedExpected.Equals(actual);
    }


    /// <summary>
    /// Define a paginação na query de consulta.
    /// </summary>
    /// <param name="pagination">Um objeto <see cref="Pagination"/> com as configurações de paginação.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm WithPagination(int page, int rows)
    {
        pagination = new Pagination(page, rows);
        return this;
    }


    /// <summary>
    /// Define como a API do IXC Provedor deverá ordenar os dados retornados pela busca.
    /// </summary>
    /// <param name="sortName">O campo da tabela que será usado para ordenar os registros retornados.</param>
    /// <param name="sortOrder">O tipo de ordenação (asc | desc).</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm OrderBy(string sortName, IxcOrmSort sortOrder)
    {
        ordering = (sortOrder == IxcOrmSort.Desc)
            ? Ordering.DescBy(Table, sortName)
            : Ordering.AscBy(Table, sortName);

        SetupQuery(GetQueryAsJsonString());

        return this;
    }


    /// <summary>
    /// Inicia um novo objeto de parâmetro para a propriedade <c>grid_param</c> da query.
    /// </summary>
    /// <param name="column">O campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm Where(string column)
    {
        parameterBuilder.WithType(column);
        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (L) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm Like(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Like)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (=) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm Exactly(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Equals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (!=) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm Not(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Not)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (&lt;) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm LessThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (&lt;=) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm LessThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (&gt;) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm GreaterThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Adiciona o operador de comparação (&gt;=) e o valor a ser filtrado no objeto de parâmetro iniciado por 
    /// <see cref="Where(string)"/>.
    /// </summary>
    /// <param name="value">O valor do campo da tabela que será usado como filtro na busca.</param>
    /// <returns>A própria instância de <see cref="IxcOrm"/>.</returns>
    public IxcOrm GreaterThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <summary>
    /// Sobrescreve a chamada para <see cref="RequestEmitter.EmitRequest(HttpMethod, CancellationToken?)"/>.
    /// Envia a requisição para a API do IXC Provedor e retorna o conteúdo em uma string.
    /// </summary>
    /// <param name="method">O método HTTP (GET, POST, PUT, DELETE).</param>
    /// <param name="cancellationToken">Token de cancelamento opcional.</param>
    /// <returns>O conteúdo da resposta em uma string.</returns>
    /// <exception cref="DotNet.IXC.ORM.Exceptions.IxcOrmRequestException">
    /// Lançada quando ocorre uma falha na comunicação com o servidor, na requisição HTTP.
    /// Ou quando a API responde com um status code diferente de 200 (OK).
    /// </exception>
    protected override async Task<string> EmmitRequest(HttpMethod method, CancellationToken? cancellationToken = null)
    {
        string result = await base.EmmitRequest(method, cancellationToken);

        parameterBuilder = Parameter.NewBuilder(Table);
        parameters.Clear();
        SetupQuery(string.Empty);

        return result;
    }


    private void PrepareQuery()
    {
        parameters.Add(parameterBuilder.Build());
        parameterBuilder = Parameter.NewBuilder(Table);
        SetupQuery(GetQueryAsJsonString());
    }


    private string GetQueryAsJsonString()
    {
        string queryPropsAsJson = (
            $"""
            "qtype":"{Table}",
            "query":"",
            "oper":"",
            "page":"{pagination.Page}",
            "rp":"{pagination.Rows}",
            "sortname":"{ordering.SortName}",
            "sortorder":"{ordering.SortOrder.Value()}"
            """
        ).Replace("\r", "")
         .Replace("\n", "")
         .Replace(" ", "");

        string gridParamsContent = string.Join(",", parameters.Select(param => param.ToString()));
        string gridParamAsJson = $"\"grid_param\":\"[{gridParamsContent}]\"";

        return $"{{{queryPropsAsJson},{gridParamAsJson}}}";
    }
}
