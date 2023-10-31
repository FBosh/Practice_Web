using System.ComponentModel.DataAnnotations;

namespace Practice_Web.Models.Entity
{
    public class User
    {
        public User()
        {
            IsEnabled = true;
        }

        public User(User user)
        {
            ID = user.ID;
            Account = user.Account;
            Password = user.Password;
            IsEnabled = user.IsEnabled;
        }

        public User(object[] properties)
        {
            if (properties[0] != null) ID = (int)properties[0];
            if (properties[1] != null) Account = properties[1].ToString();
            if (properties[2] != null) Password = properties[2].ToString();
            if (properties[3] != null) IsEnabled = (bool)properties[3];
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(3), MaxLength(32)]
        public string Account { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"[\S]{8,32}", ErrorMessage = "Password must contain 8~32 characters")]
        [MinLength(8), MaxLength(32)]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password mismatch")]
        //public string ConfirmPassword { get; set; }

        public bool IsEnabled { get; set; }

        public enum UserProperty
        {
            ID, Account, Password, IsEnabled
        }

        public object[] GetProperties() => new object[] { ID, Account, Password, IsEnabled };

        public bool HasNullInProperties => Account == null || Password == null;

        public bool IsValid => ID > 0 && Account != null && Password != null;
    }
}
