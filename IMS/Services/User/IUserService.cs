﻿using IMS.Models.User;

namespace IMS.Services.User
{
    public interface IUserService
    {
        Task<int> ValidateUser(UserLogin userLogin);
        Task<int> RegisterUser(UserRegister userRegister);
        Task<int> VerifyOtp(string otp);
        Task<bool> UpdateAddress(UserAddress userAddress);
        Task<bool> UpdateImage(UserImage userImage);
        Task<UserModel> GetUserProfile();
        Task<string> GetUserProfileStatus();
    }
}
