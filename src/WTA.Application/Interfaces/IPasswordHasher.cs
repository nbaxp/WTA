namespace WTA.Application.Interfaces;

public interface IPasswordHasher
{
  string CreateSalt();

  string HashPassword(string password, string salt);
}
