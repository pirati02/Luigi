using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace User.Api.Service;

public interface IPasswordGenerator
{
    string Generate(string password);
}

public class PasswordGenerator : IPasswordGenerator
{
    public string Generate(string password)
    {
        var salt = Encoding.UTF8.GetBytes("test_sult");

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
}