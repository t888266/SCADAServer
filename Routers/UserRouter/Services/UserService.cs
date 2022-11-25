namespace SCADAServer.Routers.UserRouter.Services
{
    using SCADAServer.Routers.UserRouter.Models;
    public interface IUserService
    {
        Task<UserModel> SignIn(IUserLogin userLogin);
        Task<bool> SignUp(UserModel user);
        Task<string> GetUserKeyByMail(string email);
        Task<bool> UpdateEmail(string email, string userKey);
        Task UpdatePassword(string password, string userKey);
        Task UpdateUsername(string userName, string userKey);
    }
#nullable disable
    public class UserService : IUserService
    {
        DBContext dBContext;

        public UserService(DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<bool> SignUp(UserModel user)
        {
            if(await dBContext.User.AddUser(user))
            {
                return true;
            }
            return false;
        }
        public async Task<UserModel> SignIn(IUserLogin userLogin)
        {
            return await dBContext.User.GetUserModel(userLogin);
        }

        public async Task<bool> UpdateEmail(string email, string userKey)
        {
            return await dBContext.User.UpdateEmail(email,userKey);
        }

        public async Task UpdatePassword(string password,string userKey)
        {
            await dBContext.User.UpdatePassword(password,userKey);
        }

        public async Task UpdateUsername(string userName,string userKey)
        {
            await dBContext.User.UpdateUsername(userName,userKey);
        }
        public async Task<string> GetUserKeyByMail(string email)
        {
            return await dBContext.User.GetUserKeyByMail(email);
        }
    }
}
