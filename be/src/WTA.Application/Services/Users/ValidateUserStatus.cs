namespace WTA.Application.Services.Users;

public enum ValidateUserStatus
{
    Successful = 1,
    NotExist = 2,
    WrongPassword = 3,
    NotActive = 4,
    LockedOut = 5
}
