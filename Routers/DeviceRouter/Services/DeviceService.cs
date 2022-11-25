using SCADAServer.Routers.DeviceRouter.Models;

namespace SCADAServer.Routers.DeviceRouter.Services
{
    public interface IDeviceService
    {
        Task<bool> AddDevice(string userKey, DeviceModel deviceModel);
        Task<bool> RenameDevice(string deviceKey,string deviceName);
        Task<bool> RemoveDevice(string deviceKey);
        Task<bool> AddDeviceConfig(string deviceKey,DeviceConfig deviceConfig);
        Task<bool> UpdateDeviceConfig(string deviceKey,string currentTypeVibration, 
        DeviceConfig deviceConfig);
        Task<bool> ChangeRecordState(string deviceKey, string typeVibration, bool isRecord);
        IAsyncEnumerable<DeviceConfig> GetDeviceConfig(string deviceKey);
        Task<bool> RemoveDeviceConfig(string deviceKey, string typeVibration);
        IAsyncEnumerable<DeviceModel> GetListDevice(string userKey);
    }
    public class DeviceService : IDeviceService
    {
        DBContext dbContext;

        public DeviceService(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<bool> AddDevice(string userKey,DeviceModel deviceModel)
        {
            return dbContext.Device.AddDevice(userKey,deviceModel);
        }

        public IAsyncEnumerable<DeviceModel> GetListDevice(string userKey)
        {
            return dbContext.Device.GetListDevice(userKey);
        }

        public async Task<bool> RemoveDevice(string deviceKey)
        {
            return await dbContext.Device.RemoveDevice(deviceKey);
        }
        public async Task<bool> AddDeviceConfig(string deviceKey, DeviceConfig deviceConfig)
        {
            return await dbContext.Device.AddDeviceConfig(deviceKey, deviceConfig);
        }

        public async Task<bool> UpdateDeviceConfig(string deviceKey,
         string currentTypeVibration, DeviceConfig deviceConfig)
        {
            return await dbContext.Device.UpdateDeviceConfig(
                deviceKey,currentTypeVibration, deviceConfig);
        }
        public async Task<bool> ChangeRecordState(string deviceKey, 
        string typeVibration,bool isRecord)
        {
            return await dbContext.Device.ChangeRecordState(deviceKey, typeVibration,isRecord);
        }
        public IAsyncEnumerable<DeviceConfig> GetDeviceConfig(string deviceKey)
        {
            return dbContext.Device.GetDeviceConfig(deviceKey);
        }
        public async Task<bool> RemoveDeviceConfig(string deviceKey, string typeVibration)
        {
            return await dbContext.Device.RemoveDeviceConfig(deviceKey, typeVibration);
        }

        public async Task<bool> RenameDevice(string deviceKey,string deviceName)
        {
            return await dbContext.Device.RenameDevice(deviceKey, deviceName);
        }
    }
}
