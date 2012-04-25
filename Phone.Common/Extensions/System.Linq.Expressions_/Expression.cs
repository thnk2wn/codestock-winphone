using System;
using System.Linq.Expressions;

namespace Phone.Common.Extensions.System.Linq.Expressions_
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName<T, T2>(Expression<Func<T, T2>> expression)
        {
            return expression.GetMemberName();
        }

        public static string GetMemberName(this Expression expr)
        {
            if (expr.NodeType == ExpressionType.Lambda)
                return GetMemberName(((LambdaExpression)expr).Body);
            if (expr.NodeType == ExpressionType.MemberAccess)
                return ((MemberExpression)expr).Member.Name;
            throw new NotImplementedException(expr.NodeType + " is unsupported.");
        }
    }
}
