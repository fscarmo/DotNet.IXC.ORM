using DotNet.IXC.ORM.Api;
using DotNet.IXC.ORM.Api.Records;
using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM;


/// <summary>
/// A classe <c>IxcOrm</c> implementa métodos que geram uma query de busca, utilizando o filtro da grid da API do IXC
/// Provedor. Ela também herda o comportamento da classe <see cref="RequestEmitter"/>, para disponibilizar, através da
/// mesma instância, os métodos que manipulam e executam as requisições HTTP.
/// <para>
/// Essa classe manipula as classes de ordenação, paginação e de construção de parâmetros, além de gerar um JSON
/// compatível com a query de busca da API do IXC Provedor.
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


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="ValidateQuery"]/*'/>
    public bool ValidateQuery(string expected)
    {
        string normalizedExpected = expected
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");

        string actual = GetQueryAsJsonString();

        return normalizedExpected.Equals(actual);
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="WithPagination"]/*'/>
    public IxcOrm WithPagination(int page, int rows)
    {
        pagination = new Pagination(page, rows);
        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="OrderBy"]/*'/>
    public IxcOrm OrderBy(string sortName, IxcOrmSort sortOrder)
    {
        ordering = (sortOrder == IxcOrmSort.Desc)
            ? Ordering.DescBy(Table, sortName)
            : Ordering.AscBy(Table, sortName);

        SetupQuery(GetQueryAsJsonString());

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="Where"]/*'/>
    public IxcOrm Where(string column)
    {
        parameterBuilder.WithType(column);
        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="Like"]/*'/>
    public IxcOrm Like(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Like)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="Exactly"]/*'/>
    public IxcOrm Exactly(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Equals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="Not"]/*'/>
    public IxcOrm Not(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Not)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="LessThan"]/*'/>
    public IxcOrm LessThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="LessThanOrEqual"]/*'/>
    public IxcOrm LessThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="GreaterThan"]/*'/>
    public IxcOrm GreaterThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="GreaterThanOrEqual"]/*'/>
    public IxcOrm GreaterThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    /// <include file='Docs/IxcOrm.xml' path='docs/member[@name="EmmitRequest"]/*'/>
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
