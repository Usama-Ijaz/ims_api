using IMS.Models;

namespace IMS.Services.Login
{
    public interface ILoginService
    {
        Task<JwtToken> GenerateJwtToken(int userId); 
    }
}
