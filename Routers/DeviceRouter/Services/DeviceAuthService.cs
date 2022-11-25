using System.Security.Cryptography;

namespace SCADAServer.Routers.DeviceRouter.Services
{
    public interface IDeviceAuthService
    {
        Task<bool> IsDeviceKeyExists(string deviceKey);
    }
    public class DeviceAuthService : IDeviceAuthService
    {
        DBContext dbContext;

        public DeviceAuthService(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> IsDeviceKeyExists(string deviceKey)
        {
            return await dbContext.Device.IsDeviceKeyExists(deviceKey);
        }
    }
}
