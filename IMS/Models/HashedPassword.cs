namespace IMS.Models
{
    public class HashedPassword
    {
        public string salt {  get; set; }
        public string passwordHash { get; set; }
    }
}
