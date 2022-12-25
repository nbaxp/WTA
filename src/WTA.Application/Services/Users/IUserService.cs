using WTA.Application.Domain.System;

namespace WTA.Application.Services.Users;

public interface IUserService
{
  ValidateUserResult ValidateUser(LoginModel model);

  User? GetUser(string userName);

  List<Role> GetRoles(string userName);
}
