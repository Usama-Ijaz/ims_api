using IMS.Repositories.User;

namespace IMS.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<int> ValidateUser(IMS.Models.User.User user)
        {
            return await _userRepository.ValidateUser(user);
        }
    }
}
