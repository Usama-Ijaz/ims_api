using IMS.Models.User;

namespace IMS.Services.User
{
    public interface IUserService
    {
        Task<int> ValidateUser(UserLogin userLogin);
        Task<int> RegisterUser(UserRegister userRegister);
    }
}
