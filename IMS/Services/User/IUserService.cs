namespace IMS.Services.User
{
    public interface IUserService
    {
        Task<int> ValidateUser(IMS.Models.User.User user);
    }
}
