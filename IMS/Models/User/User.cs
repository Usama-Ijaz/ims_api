namespace IMS.Models.User
{
    public class UserModel
    {
        public UserModel() 
        {
            HashedPassword = new HashedPassword();
        }
        public int UserId { get; set; }
        public string Email { get; set; }
        public HashedPassword HashedPassword { get; set; }
    }
}
