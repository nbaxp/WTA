using WTA.Application.Domain.System;

namespace WTA.Application.Services;

public interface ITestService<Entity>
{
    IList<User> Test();
}
