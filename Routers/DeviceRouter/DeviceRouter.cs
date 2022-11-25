using Microsoft.AspNetCore.Mvc;
using SCADAServer.Routers.DeviceRouter.Services;
using SCADAServer.Routers.DeviceRouter.Models;

namespace SCADAServer.Routers.DeviceRouter
{
    public class DeviceRouter : RouterBase
    {
        public DeviceRouter(string baseRouteUrl) : base(baseRouteUrl)
        {
        }
        public override void AddRouter(WebApplication app)
        {
            app.MapPost($"/{BaseRouteUrl}/addDevice", async (
                /*[FromForm]string userKey,*/ [FromServices]
                 IDeviceService deviceService,IDeviceAuthService 
                 deviceAuthService, HttpContext context) =>
            {
                string userKey = context.Request.Form["userKey"].ToString();
                DeviceModel model = new DeviceModel();
                model.DeviceID = int.Parse(context.Request.Form["deviceID"].ToString());
                model.DeviceName = context.Request.Form["deviceName"].ToString();
                do
                {
                    model.DeviceKey = Helper.GetUniqueKey(16);
                } while (await deviceAuthService.IsDeviceKeyExists(model.DeviceKey));
                if(await deviceService.AddDevice(userKey,model))
                {
                    return Results.Ok(model);
                }
                return Results.BadRequest();
            });
            app.MapDelete($"/{BaseRouteUrl}/removeDevice", async 
            ([FromQuery] string deviceKey, [FromServices] IDeviceService deviceService) =>
            {
                if (await deviceService.RemoveDevice(deviceKey))
                {
                    return Results.Ok();
                }
                return Results.NotFound();
            });
            app.MapGet($"/{BaseRouteUrl}/getListDevice", 
            (/*[FromForm]string userKey,*/ [FromServices]
            IDeviceService deviceService, HttpContext context) =>
            {
                string userKey = context.Request.Form["userKey"].ToString();
                return  deviceService.GetListDevice(userKey);
            });
            app.MapPost($"/{BaseRouteUrl}/addDeviceConfig/{{deviceKey}}",
             async ([FromRoute]string deviceKey, 
             [FromBody]DeviceConfig config, [FromServices] IDeviceService deviceService) =>
            {
                if(await deviceService.AddDeviceConfig(deviceKey, config))
                {
                    return Results.Ok();
                }
                return Results.BadRequest();
            });
            app.MapGet($"/{BaseRouteUrl}/getDeviceConfig/{{deviceKey}}",
             async ([FromRoute] string deviceKey, [FromServices]
              IDeviceService deviceService) =>
            {
                return deviceService.GetDeviceConfig(deviceKey);
            });
            app.MapDelete($"/{BaseRouteUrl}/removeDeviceConfig",
             async ([FromQuery] string deviceKey,
              string typeVibration, [FromServices] IDeviceService deviceService) =>
            {
                if (await deviceService.RemoveDeviceConfig(deviceKey, typeVibration))
                {
                    return Results.Ok();
                }
                return Results.NotFound();
            });
            app.MapPut($"/{BaseRouteUrl
            }/updateDeviceConfig/{{deviceKey}}/{{currentTypeVibration}}", 
            async ([FromRoute] string deviceKey,
            string currentTypeVibration, [FromBody] DeviceConfig config, 
            [FromServices] IDeviceService deviceService) =>
            {
                if (await deviceService.UpdateDeviceConfig(deviceKey,currentTypeVibration,config))
                {
                    return Results.Ok();
                }
                return Results.BadRequest();
            });
            app.MapPut($"/{BaseRouteUrl
            }/changeRecordState/{{deviceKey}}/{{currentTypeVibration}}", 
            async ([FromRoute] string deviceKey,
             string currentTypeVibration, [FromServices]
              IDeviceService deviceService, HttpContext context) =>
            {
                if(bool.TryParse(context.Request.Form["isRecord"].ToString(),out bool isRecord))
                {
                    if (await deviceService.ChangeRecordState(deviceKey,
                     currentTypeVibration, isRecord))
                    {
                        return Results.Ok();
                    }
                }
                return Results.BadRequest();
            });
            app.MapPut($"/{BaseRouteUrl}/renameDevice/{{deviceKey}}", 
            async ([FromRoute] string deviceKey, 
            IDeviceService deviceService, HttpContext context) =>
            {
                string deviceName = context.Request.Form["deviceName"].ToString();
                if (await deviceService.RenameDevice(deviceKey, deviceName)) 
                {
                    return Results.Ok();
                }
                return Results.BadRequest();
            });
        }
    }
}
