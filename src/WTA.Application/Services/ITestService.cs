using WTA.Application.Domain.Users;

namespace WTA.Application.Services;

public interface ITestService<Entity>
{
  IList<User> Test();
}
