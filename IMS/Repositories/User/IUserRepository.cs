﻿using IMS.Models;
using IMS.Models.User;

namespace IMS.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserModel> ValidateUser(UserLogin userLogin);
        Task<int> RegisterUser(UserRegister userRegister, HashedPassword hashedPassword);
    }
}
