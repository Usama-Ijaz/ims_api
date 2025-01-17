﻿using IMS.Models.User;
using Npgsql;
using IMS.Models;
using IMS.Core.Services;

namespace IMS.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly IUserContextService _userContextService;
        public UserRepository(NpgsqlConnection connection, IUserContextService userContextService)
        {
            _connection = connection;
            _userContextService = userContextService;
        }
        public async Task<UserModel> ValidateUser(UserLogin userLogin)
        {
            var user = new UserModel();
            int userId = 0;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * FROM fn_get_user(@email)", _connection);
                cmd.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Varchar, userLogin.Email);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int.TryParse(Convert.ToString(reader["UserId"]), out userId);
                    user.HashedPassword.passwordHash = Convert.ToString(reader["passwordHash"]);
                    user.HashedPassword.salt = Convert.ToString(reader["passwordSalt"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            user.UserId = userId;
            return user;
        }
        public async Task<int> RegisterUser(UserRegister userRegister, HashedPassword hashedPassword)
        {
            int userId = 0;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_insert_user(@Email, @Password, @PasswordSalt)", _connection);
                cmd.Parameters.AddWithValue("Email", NpgsqlTypes.NpgsqlDbType.Varchar, userRegister.Email);
                cmd.Parameters.AddWithValue("Password", NpgsqlTypes.NpgsqlDbType.Varchar, hashedPassword.passwordHash);
                cmd.Parameters.AddWithValue("PasswordSalt", NpgsqlTypes.NpgsqlDbType.Varchar, hashedPassword.salt);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    int.TryParse(Convert.ToString(result), out userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return userId;
        }
        public async Task<int> VerifyOtp(string otp)
        {
            int userId = _userContextService.GetUserId();
            int valid = -1;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_verify_otp(@UserId, @Otp)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("Otp", NpgsqlTypes.NpgsqlDbType.Varchar, otp);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    int.TryParse(Convert.ToString(result), out valid);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return valid;
        }
        public async Task<bool> UpdateAddress(UserAddress userAddress)
        {
            int userId = _userContextService.GetUserId();
            bool updated = false;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_update_user_address(@UserId, @Address1, @Address2, @City, @Country, @Dob)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("Address1", NpgsqlTypes.NpgsqlDbType.Varchar, userAddress.Address1);
                cmd.Parameters.AddWithValue("Address2", NpgsqlTypes.NpgsqlDbType.Varchar, userAddress.Address2);
                cmd.Parameters.AddWithValue("City", NpgsqlTypes.NpgsqlDbType.Varchar, userAddress.City);
                cmd.Parameters.AddWithValue("Country", NpgsqlTypes.NpgsqlDbType.Varchar, userAddress.Country);
                cmd.Parameters.AddWithValue("Dob", NpgsqlTypes.NpgsqlDbType.Date, userAddress.Dob.ToDateTime(new TimeOnly(0, 0)));

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    bool.TryParse(Convert.ToString(result), out updated);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return updated;
        }
        public async Task<bool> UpdateImage(UserImage userImage)
        {
            int userId = _userContextService.GetUserId();
            bool updated = false;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_update_user_image(@UserId, @Image)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("Image", NpgsqlTypes.NpgsqlDbType.Text, userImage.ImageBase64);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    bool.TryParse(Convert.ToString(result), out updated);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return updated;
        }
        public async Task<UserModel> GetUserProfile()
        {
            int userId = _userContextService.GetUserId();
            var user = new UserModel();
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * from fn_get_user_profile(@UserId)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user.UserId = userId;
                    user.Email = Convert.ToString(reader["email"]) ?? "";
                    user.Address.Address1 = Convert.ToString(reader["address1"]) ?? "";
                    user.Address.Address2 = Convert.ToString(reader["address2"]) ?? "";
                    user.Address.City = Convert.ToString(reader["city"]) ?? "";
                    user.Address.Country = Convert.ToString(reader["country"]) ?? "";
                    user.UserImage.ImageBase64 = Convert.ToString(reader["image"]) ?? "";
                    user.ProfileStatus = Convert.ToString(reader["profilestatus"]) ?? "";
                    user.ProfileCompletionStatus = bool.Parse(Convert.ToString(reader["profilecompletionstatus"]));
                    user.OtpVerified = bool.Parse(Convert.ToString(reader["otpverified"]));
                    user.CardDetailsEntered = bool.Parse(Convert.ToString(reader["carddetailsentered"]));
                    _ = DateTime.TryParse(Convert.ToString(reader["dob"]), out DateTime dob);
                    user.Dob = DateOnly.FromDateTime(dob);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            user.UserPreferences = await GetUserPreferences();
            return user;
        }
        public async Task<string> GetUserProfileStatus()
        {
            int userId = _userContextService.GetUserId();
            string status = "";
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_get_user_profile_status(@UserId)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    status = Convert.ToString(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return status;
        }
        public async Task<List<Preference>> GetAllPreferences()
        {
            var preferences = new List<Preference>();
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * from fn_get_preferences()", _connection);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var preference = new Preference
                    {
                        PreferenceId = int.Parse(Convert.ToString(reader["PreferenceId"])),
                        PreferenceName = Convert.ToString(reader["PreferenceName"])
                    };
                    preferences.Add(preference);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return preferences;
        }
        public async Task<List<UserPreference>> GetUserPreferences()
        {
            int userId = _userContextService.GetUserId();
            var userPreferences = new List<UserPreference>();
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * from fn_get_user_preferences(@UserId)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var preference = new UserPreference
                    {
                        PreferenceId = int.Parse(Convert.ToString(reader["PreferenceId"])),
                        PreferenceName = Convert.ToString(reader["PreferenceName"]),
                        PreferenceValue = bool.Parse(Convert.ToString(reader["PreferenceValue"]))
                    };
                    userPreferences.Add(preference);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return userPreferences;
        }
        public async Task<bool> UpdateUserPreferences(List<UpdateUserPreference> userPreferences)
        {
            bool updated = true;
            foreach (var userPreference in userPreferences) 
            {
                if (updated)
                {
                    updated = await UpdateUserPreference(userPreference);
                }
            }
            return updated;
        }
        private async Task<bool> UpdateUserPreference(UpdateUserPreference userPreference)
        {
            int userId = _userContextService.GetUserId();
            bool updated = true;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_update_user_preference(@UserId, @Id, @Value)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("Id", NpgsqlTypes.NpgsqlDbType.Integer, userPreference.PreferenceId);
                cmd.Parameters.AddWithValue("Value", NpgsqlTypes.NpgsqlDbType.Boolean, userPreference.PreferenceValue);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    //bool.TryParse(Convert.ToString(result), out updated);
                }
            }
            catch (Exception ex)
            {
                updated = false;
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return updated;
        }
        public async Task<bool> UpdateCardAddedStatus()
        {
            int userId = _userContextService.GetUserId();
            bool updated = false;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_update_user_card_status(@UserId)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    bool.TryParse(Convert.ToString(result), out updated);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
            return updated;
        }

    }
}
