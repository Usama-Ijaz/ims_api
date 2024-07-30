using IMS.Models;

namespace IMS.Core.Services
{
    public interface IHashingService
    {
        HashedPassword HashPassword(string passwordText);
        bool VerifyPassword(string enteredPassword, string storedSaltBase64, string storedHashBase64);
    }
}
