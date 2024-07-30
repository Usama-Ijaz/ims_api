﻿using IMS.Models.User;
using Npgsql;
using IMS.Models;

namespace IMS.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;
        public UserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
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
    }
}
