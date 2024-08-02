namespace IMS.Services.Email
{
    public interface IEmailService
    {
        Task<int> SendEmail(string email);
    }
}
