namespace IMS.Repositories.Email
{
    public interface IEmailRepository
    {
        Task<int> InsertOtp(string otp, int userId);
    }
}
