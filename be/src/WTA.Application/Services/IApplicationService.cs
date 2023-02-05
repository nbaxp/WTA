namespace WTA.Application.Services;

public interface IApplicationService<T> where T : class
{
    T Get(Guid id);
    T Create(T entity);
    int Update(T entity);
    int Delete(Guid id);
}
