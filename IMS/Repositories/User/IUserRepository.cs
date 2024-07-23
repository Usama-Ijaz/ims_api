namespace IMS.Repositories.User
{
    public interface IUserRepository
    {
        Task<int> ValidateUser(IMS.Models.User.User user);
    }
}
