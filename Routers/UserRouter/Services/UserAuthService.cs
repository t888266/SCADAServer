using MailSender;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
namespace SCADAServer.Routers.UserRouter.Services
{
    public interface IUserAuthService
    {
        string GetToken();
        Task SendRegVerifyCode(string token, MailContent mailContent);
        Task SendRstVerifyCode(string token, MailContent mailContent);
        Task<bool> IsMailExists(string mail);
        Task<bool> IsUserKeyExists(string userKey);
        bool IsCodeValid(int code, string token,TypeCode typeCode);
    }
    public class UserAuthService : IUserAuthService
    {
        DBContext dBContext;
        CodeVerify codeVerify;
        IBGMailSender mailSender;
        public UserAuthService(DBContext dBContext, CodeVerify codeVerify,IBGMailSender mailSender)
        {
            this.dBContext = dBContext;
            this.codeVerify = codeVerify;
            this.mailSender = mailSender;
        }
        public bool IsCodeValid(int code, string token,TypeCode typeCode)
        {
            return codeVerify.CheckCode(code, token,typeCode);
        }
        public string GetToken()
        {
            return Helper.GetUniqueKey(32);
        }
        public async Task<bool> IsMailExists(string mail)
        {
            return await dBContext.User.IsEmailExists(mail);
  
        }

        public async Task<bool> IsUserKeyExists(string userKey)
        {
            return await dBContext.User.IsUserKeyExists(userKey);
        }
        public async Task SendRegVerifyCode(string token, MailContent mailContent)
        {

            int randomCode = codeVerify.VerifyCodeFor(token,TypeCode.REG);
            codeVerify.DeleteVerifyCodeAfter(randomCode, token, 59990, TypeCode.REG);
            mailContent.Subject = "Verify your email address";
            mailContent.Body = $@"Almost done, @{mailContent.DisplayName}! To complete your sign up, we just need to verify your email address by using the code below:
{randomCode}
Please use it within 1 minutes.";
            await mailSender.SendMail(mailContent);
        }

        public async Task SendRstVerifyCode(string token, MailContent mailContent)
        {
            int randomCode = codeVerify.VerifyCodeFor(token, TypeCode.RST);
            codeVerify.DeleteVerifyCodeAfter(randomCode, token, 179990, TypeCode.RST);
            mailContent.Subject = "Reset your password";
            mailContent.Body = $@"Someone attempted to reset the password for your account,  If that person was you, using the code below to complete your action:
{randomCode}
Please use it within 3 minutes.";
            await mailSender.SendMail(mailContent);
        }
    }
}
