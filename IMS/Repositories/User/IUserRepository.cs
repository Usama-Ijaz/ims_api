using IMS.Models;
using IMS.Models.User;

namespace IMS.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserModel> ValidateUser(UserLogin userLogin);
        Task<int> RegisterUser(UserRegister userRegister, HashedPassword hashedPassword);
        Task<int> VerifyOtp(string otp);
        Task<bool> UpdateAddress(UserAddress userAddress);
        Task<bool> UpdateImage(UserImage userImage);
    }
}
