namespace IMS.Models.User
{
    public class UserPreference
    {
        public int PreferenceId { get; set; }
        public string PreferenceName { get; set; } = string.Empty;
        public bool PreferenceValue { get; set; }
    }
    public class Preference
    {
        public int PreferenceId { get; set;}
        public string PreferenceName { get; set;} = string.Empty;
    }
    public class UpdateUserPreference
    {
        public int PreferenceId { get; set;}
        public bool PreferenceValue { get; set; }
    }
}
