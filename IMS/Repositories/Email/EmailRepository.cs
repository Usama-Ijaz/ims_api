using IMS.Models.User;
using IMS.Models;
using Npgsql;
using IMS.Core.Services;

namespace IMS.Repositories.Email
{
    public class EmailRepository : IEmailRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly IUserContextService _userContextService;
        public EmailRepository(NpgsqlConnection connection, IUserContextService userContextService)
        {
            _connection = connection;
            _userContextService = userContextService;
        }
        public async Task<int> InsertOtp(string otp)
        {
            int userId = _userContextService.GetUserId();
            int pkId = 0;
            try
            {
                await _connection.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT fn_insert_otp(@UserId, @Otp)", _connection);
                cmd.Parameters.AddWithValue("UserId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                cmd.Parameters.AddWithValue("Otp", NpgsqlTypes.NpgsqlDbType.Varchar, otp);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    int.TryParse(Convert.ToString(result), out pkId);
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
            return pkId;
        }

    }
}
