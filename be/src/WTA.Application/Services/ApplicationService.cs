using WTA.Application.Abstractions.Components;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;

namespace WTA.Application.Services;

public class ApplicationService<T> : IApplicationService<T> where T : class
{
    private readonly IRepository<T> _respository;

    public ApplicationService(IRepository<T> repository)
    {
        this._respository = repository;
    }

    public T Create(T entity)
    {
        throw new NotImplementedException();
    }

    public int Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public T Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public int Update(T entity)
    {
        throw new NotImplementedException();
    }

    //public IList<T> Test()
    //{
    //    //var users = _respository.Query()
    //    //  .Include(o => o.UserRoles)
    //    //  .ThenInclude(o => o.Role)
    //    //  .ThenInclude(o => o.RolePermissions)
    //    //  .ThenInclude(o => o.Permission)
    //    //  //.Where("UserName == @0", "super")
    //    //  .Where(@"UserName == ""super""")
    //    //  .OrderBy("UserName,Id desc")
    //    //  .ToList();
    //    return _respository.Query().ToList();
    //}
}

[Service<IApplicationService<User>>]
public class UserService : ApplicationService<User>
{
    public UserService(IRepository<User> repository) : base(repository)
    {
    }
}
