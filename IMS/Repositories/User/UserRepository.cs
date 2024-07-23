using Npgsql;

namespace IMS.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlConnection _connection;
        public UserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> ValidateUser(IMS.Models.User.User user)
        {
            int userId = 0;
            try
            {
                await _connection.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM stp_getUser(@email, @password)", _connection))
                {
                    cmd.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Varchar, user.Email);
                    cmd.Parameters.AddWithValue("password", NpgsqlTypes.NpgsqlDbType.Varchar, user.Password);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int.TryParse(Convert.ToString(reader["UserId"]), out userId);
                        }
                    }
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
