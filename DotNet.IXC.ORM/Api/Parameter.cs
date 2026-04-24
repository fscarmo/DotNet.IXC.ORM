using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM.Api;


public class Parameter
{
    public static Builder NewBuilder(string table)
    {
        return new Builder(table);
    }


    private readonly string table;
    private readonly IxcOrmOperator oper;
    private readonly object val;


    private Parameter(Builder builder)
    {
        table = builder.Type;
        oper = builder.Operator;
        val = builder.Value;
    }


    public override string ToString()
    {
        return "{" +
                    $"\\\"TB\\\":\\\"{this.table}\\\"," +
                    $"\\\"OP\\\":\\\"{this.oper.Value()}\\\"," +
                    $"\\\"P\\\":\\\"{this.val}\\\"" +
               "}";
    }


    public class Builder(string table)
    {
        public string Table { get; private set; } = table;
        public IxcOrmOperator Operator { get; private set; } = IxcOrmOperator.Equals;
        public string Type { get; private set; } = table;
        public Object Value { get; private set; } = new();


        public void WithType(string type)
        {
            Type = $"{Table}.{type}";
        }


        public Builder WithOperator(IxcOrmOperator? oper)
        {
            Operator = oper ?? IxcOrmOperator.Equals;
            return this;
        }


        public Builder WithValue(object value)
        {
            if (value != null)
                Value = value;
            return this;
        }


        public Parameter Build()
        {
            return new Parameter(this);
        }
    }
}
