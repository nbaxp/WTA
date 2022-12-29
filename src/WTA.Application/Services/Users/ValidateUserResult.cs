using WTA.Application.Domain.System;

namespace WTA.Application.Services.Users;

public class ValidateUserResult
{
    public ValidateUserStatus Status { get; set; }
    public User? User { get; set; }
}
