﻿using IMS.Core.Services;
using IMS.Models.User;
using IMS.Repositories.User;
using System.Collections.Generic;

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
        public async Task<int> VerifyOtp(string otp)
        {
            return await _userRepository.VerifyOtp(otp);
        }
        public async Task<bool> UpdateAddress(UserAddress userAddress)
        {
            return await _userRepository.UpdateAddress(userAddress);
        }
        public async Task<bool> UpdateImage(UserImage userImage)
        {
            return await _userRepository.UpdateImage(userImage);
        }
        public async Task<UserModel> GetUserProfile()
        {
            return await _userRepository.GetUserProfile();
        }
        public async Task<string> GetUserProfileStatus()
        {
            return await _userRepository.GetUserProfileStatus();
        }
        public async Task<List<Preference>> GetAllPreferences()
        {
            return await _userRepository.GetAllPreferences();
        }
        public async Task<bool> UpdateUserPreferences(List<UpdateUserPreference> userPreferences)
        {
            return await _userRepository.UpdateUserPreferences(userPreferences);
        }
        public async Task<bool> UpdateCardAddedStatus()
        {
            return await _userRepository.UpdateCardAddedStatus();
        }
    }
}
