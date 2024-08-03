namespace IMS.Models.User
{
    public class UserModel
    {
        public UserModel() 
        {
            HashedPassword = new HashedPassword();
            Address = new UserAddress();
            UserImage = new UserImage();
        }
        public int UserId { get; set; }
        public string Email { get; set; }
        public HashedPassword HashedPassword { get; set; }
        public UserAddress Address { get; set; }
        public UserImage UserImage { get; set; }
        public string ProfileStatus { get; set; }
    }
}
