namespace SCADAServer.Routers.UserRouter.Models
{
#nullable disable
    public interface IUserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public interface IUserData
    {
        public string Username { get; set; }
        public string UserKey { get; set; }
    }
    public class UserLoginModel : IUserLogin
    {
        public UserLoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserDataModel : IUserData
    {
        public UserDataModel(string username, string userKey)
        {
            Username = username;
            UserKey = userKey;
        }

        public string Username { get; set; }
        public string UserKey { get; set; }
    }
    public class UserModel : IUserLogin, IUserData
    {
        public UserModel(string email, string password, string username, string userKey)
        {
            Email = email;
            Password = password;
            Username = username;
            UserKey = userKey;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string UserKey { get; set; }
    }
}
