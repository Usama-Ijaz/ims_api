using IMS.Core.Services;
using IMS.Models.User;
using IMS.Repositories.User;

namespace IMS.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashingService _hashingService;
        public UserService(IUserRepository userRepository, IHashingService hashingService)
        {
            _userRepository = userRepository;
            _hashingService = hashingService;
        }
        public async Task<int> ValidateUser(UserLogin userLogin)
        {
            UserModel userModel = await _userRepository.ValidateUser(userLogin);
            if (userModel == null || userModel.UserId <= 0) 
            {
                return -1;
            }
            bool validPassword = _hashingService.VerifyPassword(userLogin.Password, userModel.HashedPassword.salt, userModel.HashedPassword.passwordHash);
            if (!validPassword) return -2;
            return userModel.UserId;
        }
        public async Task<int> RegisterUser(UserRegister userRegister)
        {
            var hashedPassword = _hashingService.HashPassword(userRegister.Password);
            return await _userRepository.RegisterUser(userRegister, hashedPassword);
        }
    }
}
