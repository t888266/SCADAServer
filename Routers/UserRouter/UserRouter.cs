using MailSender;
using Microsoft.AspNetCore.Mvc;
using SCADAServer.Routers.UserRouter.Services;
using SCADAServer.Routers.UserRouter.Models;
using TypeCode = SCADAServer.Routers.UserRouter.Services.TypeCode;
namespace SCADAServer.Routers.UserRouter
{
#nullable disable
    public class UserRouter : RouterBase
    {
        public UserRouter(string baseRouteUrl) : base(baseRouteUrl)
        {
        }
        public override void AddRouter(WebApplication app)
        {
            //Get token for this session app
            app.MapGet($"/{BaseRouteUrl}/getToken", (IUserAuthService userAuthService) =>
            {
                return Results.Ok(userAuthService.GetToken());
            });
            //Send verification code to the email address for sign up action
            //Form binding not working for NET 6.
            app.MapPost($"/{BaseRouteUrl}/getRegCode", async
             (/*[FromForm]string token, string username, string email,*/
              [FromServices] IUserAuthService userAuthService, HttpContext context) =>
            {
                string email = context.Request.Form["email"].ToString();
                if (await userAuthService.IsMailExists(email))
                {
                    return Results.Conflict();
                }
                string token = context.Request.Form["token"].ToString();
                string username = context.Request.Form["username"].ToString();
                MailContent content = new MailContent();
                content.To = email;
                content.DisplayName = username;
                await userAuthService.SendRegVerifyCode(token, content);
                return Results.Ok();
            });
            //Sign up with the code have been recieved from mail
            app.MapPost($"/{BaseRouteUrl}/signUp/{{code}}", async 
            ([FromRoute] int code, [FromBody] UserModel user, 
            IUserService userService, IUserAuthService userAuthService) =>
            {
                //User key is the token....
                if (userAuthService.IsCodeValid(code, user.UserKey, TypeCode.REG))
                {
                    do
                    {
                        user.UserKey = Helper.GetUniqueKey(16);
                    } while (await userAuthService.IsUserKeyExists(user.UserKey));
                    if (await userService.SignUp(user))
                    {
                        return Results.Ok();
                    }
                }
                return Results.BadRequest();
            });
            //Change part of data of account
            app.MapPut($"/{BaseRouteUrl}/update/{{type}}", async 
            ([FromRoute] string type, /*[FromForm]string data, 
            string userKey,*/ IUserService userService, HttpContext context) =>
            {
                string data = context.Request.Form["data"].ToString();
                string userKey = context.Request.Form["userKey"].ToString();
                if (type.Equals("email"))
                {
                    if(!await userService.UpdateEmail(data, userKey))
                    {
                        return Results.Conflict();
                    }
                }
                else if (type.Equals("password"))
                {
                    await userService.UpdatePassword(data, userKey);
                }
                else if (type.Equals("username"))
                {
                    await userService.UpdateUsername(data, userKey);
                }
                return Results.Ok();
            });
            app.MapPost($"/{BaseRouteUrl}/signIn", async ([FromBody]
             UserLoginModel userLogin, IUserService userService) =>
            {
                UserModel user = await userService.SignIn(userLogin);
                return user != null ? Results.Ok(user) : Results.NotFound();
            });
            app.MapPost($"/{BaseRouteUrl}/getRstCode", 
            async (/*[FromForm] string token, string email,*/
             IUserAuthService userAuthService, HttpContext context) =>
            {
                string email = context.Request.Form["email"].ToString();
                if (!await userAuthService.IsMailExists(email))
                {
                    return Results.NotFound();
                }
                string token = context.Request.Form["token"].ToString();
                MailContent content = new MailContent();
                content.To = email;
                await userAuthService.SendRstVerifyCode(token, content);
                return Results.Ok();
            });
            app.MapPost($"/{BaseRouteUrl}/verifyRstCode/{{code}}",
             async ([FromRoute] int code, /*[FromForm] string token, string email,*/ 
             IUserService userService, IUserAuthService userAuthService, HttpContext context) =>
            {
                string token = context.Request.Form["token"].ToString();
                string email = context.Request.Form["email"].ToString();
                if (userAuthService.IsCodeValid(code, token, TypeCode.RST))
                {
                    return Results.Ok(await userService.GetUserKeyByMail(email));
                }
                return Results.BadRequest();
            });
        }
    }
}
