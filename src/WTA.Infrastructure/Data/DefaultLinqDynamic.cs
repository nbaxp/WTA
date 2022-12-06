using System.Linq.Dynamic.Core;

namespace WTA.Infrastructure.Data;

public class DefaultLinqDynamic : Application.Abstractions.Data.ILinqDynamic
{
  public IQueryable<T> Where<T>(IQueryable<T> source, string queryString, params object[] args)
  {
    return source.Where(queryString, args);
  }

  public IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args)
  {
    return source.OrderBy(ordering, args);
  }
}
