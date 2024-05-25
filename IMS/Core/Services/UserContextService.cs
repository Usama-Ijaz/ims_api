namespace IMS.Core.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContextService(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int GetUserId()
        {
            var userId = 0;
            if (_httpContextAccessor.HttpContext is not null)
            {
                int.TryParse(_httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "UserId").Value, out userId);
            }
            return userId;
        }
    }
}
