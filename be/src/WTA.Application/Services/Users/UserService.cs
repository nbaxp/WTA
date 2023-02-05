using Microsoft.Extensions.Options;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Components;
using WTA.Application.Abstractions.Data;
using WTA.Application.Authentication;
using WTA.Application.Domain.System;

namespace WTA.Application.Services.Users;

[Service<IUserService>]
public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IdentityOptions _identityOptions;

    public UserService(IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IOptions<IdentityOptions> options,
        ITokenService tokenService)
    {
        this._userRepository = userRepository;
        this._passwordHasher = passwordHasher;
        this._identityOptions = options.Value;
    }

    public ValidateUserResult ValidateUser(LoginModel model)
    {
        var result = new ValidateUserResult();
        var user = _userRepository.Query().FirstOrDefault(o => o.UserName == model.UserName);
        if (user != null)
        {
            result.User = user;
            if (SupportsUserLockout(user))
            {
                if (user.LockoutEnd.HasValue)
                {
                    if (user.LockoutEnd.Value >= DateTimeOffset.UtcNow)
                    {
                        result.Status = ValidateUserStatus.LockedOut;
                    }
                    else
                    {
                        user.AccessFailedCount = 0;
                        user.LockoutEnd = null;
                        UpdateUser();
                    }
                }
            }
            if (user.PasswordHash == _passwordHasher.HashPassword(model.Password, user.SecurityStamp!))
            {
                result.Status = ValidateUserStatus.Successful;
            }
            else
            {
                result.Status = ValidateUserStatus.WrongPassword;
                if (SupportsUserLockout(user))
                {
                    if (user.AccessFailedCount + 1 < _identityOptions.MaxFailedAccessAttempts)
                    {
                        user.AccessFailedCount += 1;
                    }
                    else
                    {
                        user.LockoutEnd = DateTimeOffset.UtcNow.Add(_identityOptions.DefaultLockoutTimeSpan);
                        user.AccessFailedCount = 0;
                        result.Status = ValidateUserStatus.LockedOut;
                    }
                    UpdateUser();
                }
            }
        }
        else
        {
            result.Status = ValidateUserStatus.NotExist;
        }
        return result;
    }

    public User? GetUser(string userName)
    {
        return this._userRepository.AsNoTracking().FirstOrDefault(o => o.UserName == userName);
    }

    public List<Role> GetRoles(string userName)
    {
        return this._userRepository.AsNoTracking().SelectMany(o => o.UserRoles).Select(o => o.Role).ToList();
    }

    private bool SupportsUserLockout(User user)
    {
        return _identityOptions.SupportsUserLockout && user.LockoutEnabled;
    }

    private void UpdateUser()
    {
        this._userRepository.SaveChangesAsync();
    }

    public void SignIn(string userName, bool rememberMe)
    {
        throw new NotImplementedException();
    }
}
