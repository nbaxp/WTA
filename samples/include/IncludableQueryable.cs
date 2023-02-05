using System.Collections;
using System.Linq.Expressions;
using WTA.Application.Abstractions.Include;

namespace WTA.Infrastructure.Data;

public class IncludableQueryable<TEntity, TProperty> : IIncludable<TEntity, TProperty>
{
    public IQueryable<TEntity> Query { get; private set; }

    public IncludableQueryable(IQueryable<TEntity> query)
    {
        this.Query = query;
    }

    public Type ElementType => Query.ElementType;

    public Expression Expression => Query.Expression;

    public IQueryProvider Provider => Query.Provider;

    public IEnumerator<TEntity> GetEnumerator()
    {
        return Query.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
