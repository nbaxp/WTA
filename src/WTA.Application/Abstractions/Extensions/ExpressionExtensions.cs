using System.Linq.Expressions;

namespace WTA.Application.Abstractions.Extensions;

public static class ExpressionExtensions
{
  public static string? GetName<T>(this Expression<Func<T, object?>> expression)
  {
    return ((expression.Body as UnaryExpression)?.Operand as MemberExpression)?.Member.Name;
  }
}
