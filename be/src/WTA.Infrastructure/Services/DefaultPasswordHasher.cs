using System.Security.Cryptography;
using System.Text;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Components;

namespace WTA.Infrastructure.Services;

[Service<IPasswordHasher>]
public class DefaultPasswordHasher : IPasswordHasher
{
    public string CreateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var buff = new byte[5];
        rng.GetBytes(buff);
        return Convert.ToBase64String(buff);
    }

    public string HashPassword(string password, string salt)
    {
        var data = Encoding.UTF8.GetBytes(string.Concat(password, salt));
        var algorithm = CryptoConfig.CreateFromName("SHA256") as HashAlgorithm;
        return BitConverter.ToString(algorithm!.ComputeHash(data)).Replace("-", string.Empty);
    }
}
