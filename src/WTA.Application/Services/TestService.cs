using Microsoft.EntityFrameworkCore;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;

namespace WTA.Application.Services;

public class TestService : ITestService<User>
{
    private readonly IRepository<User> _respository;

    public TestService(IRepository<User> repository)
    {
        this._respository = repository;
    }

    public IList<User> Test()
    {
        var users = _respository.Query()
          .Include(o => o.UserRoles)
          .ThenInclude(o => o.Role)
          .ThenInclude(o => o.RolePermissions)
          .ThenInclude(o => o.Permission)
          //.Where("UserName == @0", "super")
          .Where(@"UserName == ""super""")
          .OrderBy("UserName,Id desc")
          .ToList();
        return users;
    }
}
