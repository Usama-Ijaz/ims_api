namespace IMS.Services.Login
{
    public interface ILoginService
    {
        Task<string> GenerateJwtToken(IMS.Models.User.User user); 
    }
}
