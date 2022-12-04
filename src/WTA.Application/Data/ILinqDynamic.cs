namespace WTA.Application.Data;

public interface ILinqDynamic
{
  IQueryable<T> Where<T>(IQueryable<T> source, string queryString, object[] args);

  IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args);
}
