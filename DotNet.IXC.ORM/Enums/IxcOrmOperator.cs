namespace DotNet.IXC.ORM.Enums;


public enum IxcOrmOperator
{
    Equals,
    Not,
    Like,
    LessThan,
    LessThanEquals,
    GreaterThan,
    GreaterThanEquals
}


public static class IxcOrmOperatorExtensions
{
    public static string Value(this IxcOrmOperator oper)
    {
        return oper switch
        {
            IxcOrmOperator.Equals => "=",
            IxcOrmOperator.Not => "!=",
            IxcOrmOperator.Like => "L",
            IxcOrmOperator.LessThan => "<",
            IxcOrmOperator.LessThanEquals => "<=",
            IxcOrmOperator.GreaterThan => ">",
            IxcOrmOperator.GreaterThanEquals => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(oper), oper, null)
        };
    }
}
