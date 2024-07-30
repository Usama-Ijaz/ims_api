namespace IMS.Services.Login
{
    public interface ILoginService
    {
        Task<string> GenerateJwtToken(int userId); 
    }
}
