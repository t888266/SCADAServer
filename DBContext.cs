using Microsoft.OpenApi.Any;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using SCADAServer.Routers.UserRouter.Models;
using SCADAServer.Routers.DeviceRouter.Models;
using System.Reflection;

namespace SCADAServer
{
#nullable disable
    public class DBContext
    {
        public class UserDBContext
        {
            string connectionString;

            public UserDBContext(string connectionString)
            {
                this.connectionString = connectionString;
            }

            public async Task<bool> IsEmailExists(string email)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[IsEmailExists]";
                    var para = new DynamicParameters();
                    para.Add("@email", email);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, para, 
                    commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> IsUserKeyExists(string userKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[IsUserKeyExists]";
                    var para = new DynamicParameters();
                    para.Add("@userKey", userKey);
                    para.Add("@state",dbType:DbType.Int32,direction:
                    ParameterDirection.ReturnValue);
                    Console.WriteLine(userKey+" "+await connection.ExecuteScalarAsync<int>
                    (procedure, para, commandType: CommandType.StoredProcedure));
                    await connection.ExecuteScalarAsync<int>(procedure, para, commandType: 
                    CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<UserModel> GetUserModel(IUserLogin userLogin)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[GetUser]";
                    var para = new DynamicParameters();
                    para.Add("@email", userLogin.Email);
                    para.Add("@password", userLogin.Password);
                    UserDataModel userData = await connection.QueryFirstOrDefaultAsync
                    <UserDataModel>(procedure, para, commandType: CommandType.StoredProcedure);
                    if (userData != null)
                    {
                        return new UserModel(userLogin.Email, userLogin.Password,
                         userData.Username, userData.UserKey);
                    }
                    return null;
                }
            }
            public async Task<string> GetUserKeyByMail(string email)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[GetUserKeyByMail]";
                    var para = new DynamicParameters();
                    para.Add("@email", email);
                    return await connection.QueryFirstOrDefaultAsync<string>(procedure, 
                    para, commandType: CommandType.StoredProcedure);
                }
            }
            public async Task<bool> AddUser(UserModel userModel)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[AddUser]";
                    var para = new DynamicParameters();
                    para.Add("@userName", userModel.Username);
                    para.Add("@email", userModel.Email);
                    para.Add("@password", userModel.Password);
                    para.Add("@userKey", userModel.UserKey);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, para, 
                    commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            #region  Update Method
            public async Task<bool> UpdateEmail(string email,string userKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[UpdateEmail]";
                    var para = new DynamicParameters();
                    para.Add("@email", email);
                    para.Add("@userKey", userKey);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, para,
                     commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task UpdatePassword(string password, string userKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[UpdatePassword]";
                    var para = new DynamicParameters();
                    para.Add("@password", password);
                    para.Add("@userKey", userKey);
                    await connection.ExecuteScalarAsync<int>(procedure, para,
                     commandType: CommandType.StoredProcedure);
                }
            }
            public async Task UpdateUsername(string username, string userKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[UpdateUsername]";
                    var para = new DynamicParameters();
                    para.Add("@username", username);
                    para.Add("@userKey", userKey);
                    await connection.ExecuteScalarAsync<int>(procedure, para, 
                    commandType: CommandType.StoredProcedure);
                }
            }
            #endregion
        }
        public class DeviceDBContext
        {
            string connectionString;

            public DeviceDBContext(string connectionString)
            {
                this.connectionString = connectionString;
            }
            public async Task<bool> IsDeviceKeyExists(string deviceKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[IsDeviceKeyExists]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, 
                    para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> AddDevice(string userKey,DeviceModel deviceModel)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[AddDevice]";
                    var para = new DynamicParameters();
                    para.Add("@userKey", userKey);
                    para.Add("@deviceID", deviceModel.DeviceID);
                    para.Add("@deviceKey", deviceModel.DeviceKey);
                    para.Add("@deviceName", deviceModel.DeviceName);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, 
                    para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> RenameDevice(string deviceKey,string deviceName)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[RenameDevice]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@deviceName", deviceName);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, 
                    para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> RemoveDevice(string deviceKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[RemoveDevice]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure,
                     para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async IAsyncEnumerable<DeviceModel>GetListDevice(string userKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[GetListDevice]";
                    var para = new DynamicParameters();
                    para.Add("@userKey", userKey);
                    foreach(DeviceModel model in await
                     connection.QueryAsync<DeviceModel>(procedure, para,
                      commandType: CommandType.StoredProcedure))
                    {
                        yield return model;
                    }
                }
            }
            public async IAsyncEnumerable<DeviceConfig> GetDeviceConfig(string deviceKey)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[GetDeviceConfig]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    foreach (DeviceConfig config in await 
                    connection.QueryAsync<DeviceConfig>
                    (procedure, para, commandType: CommandType.StoredProcedure))
                    {
                        yield return config;
                    }
                }
            }
            public async Task<bool> AddDeviceConfig(string deviceKey,DeviceConfig deviceConfig)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[AddDeviceConfig]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@typeVibration", deviceConfig.TypeVibration);
                    para.Add("@isRecord", deviceConfig.IsRecord);
                    para.Add("@warnValue", deviceConfig.WarnValue);
                    para.Add("@stopValue", deviceConfig.StopValue);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure,
                     para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> UpdateDeviceConfig(string deviceKey,
            string currentTypeVibration, DeviceConfig deviceConfig)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[UpdateDeviceConfig]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@currentTypeVibration", currentTypeVibration);
                    para.Add("@updatedTypeVibration", deviceConfig.TypeVibration);
                    para.Add("@isRecord", deviceConfig.IsRecord);
                    para.Add("@warnValue", deviceConfig.WarnValue);
                    para.Add("@stopValue", deviceConfig.StopValue);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure,
                     para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> ChangeRecordState(string deviceKey, 
            string typeVibration,bool isRecord)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[ChangeRecordState]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@typeVibration", typeVibration);
                    para.Add("@isRecord", (isRecord)?1:0);
                    para.Add("@state", dbType: DbType.Int32, direction:
                     ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure, 
                    para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async Task<bool> RemoveDeviceConfig(string deviceKey, string typeVibration)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[RemoveDeviceConfig]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@typeVibration", typeVibration);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure,
                     para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
            public async IAsyncEnumerable<DeviceSavedData> 
            GetDeviceData(string deviceKey, string typeVibration)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[GetDeviceData]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@typeVibration", typeVibration);
                    foreach (DeviceSavedData data in await connection.QueryAsync
                    <DeviceSavedData>(procedure, para, commandType: CommandType.StoredProcedure))
                    {
                        yield return data;
                    }
                }
            }
            public async Task<bool> SaveDeviceData(string deviceKey, 
            string typeVibration, DeviceSavedData deviceData)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string procedure = "[SaveDeviceData]";
                    var para = new DynamicParameters();
                    para.Add("@deviceKey", deviceKey);
                    para.Add("@typeVibration", typeVibration);
                    para.Add("@timeSaved", deviceData.TimeSaved);
                    para.Add("@temperature", deviceData.Temperature);
                    para.Add("@xAcc", deviceData.XAcc);
                    para.Add("@yAcc", deviceData.YAcc);
                    para.Add("@zAcc", deviceData.ZAcc);
                    para.Add("@state", dbType: DbType.Int32, direction: 
                    ParameterDirection.ReturnValue);
                    await connection.ExecuteScalarAsync<int>(procedure,
                     para, commandType: CommandType.StoredProcedure);
                    return para.Get<int>("@state") == 1;
                }
            }
        }
        public UserDBContext User { get; }
        public DeviceDBContext Device { get; }
        public string ConnectionString { get; set; }
        public DBContext()
        {
            Initial();
            User = new UserDBContext(ConnectionString);
            Device = new DeviceDBContext(ConnectionString);
        }
        void Initial()
        {
            var connectStringBuilder = new SqlConnectionStringBuilder();
            connectStringBuilder["Server"] = "SQL8001.site4now.net";
            connectStringBuilder["Database"] = "db_a8f6fa_scadadb";
            connectStringBuilder["UID"] = "db_a8f6fa_scadadb_admin";
            connectStringBuilder["PWD"] = "#12345689Hiep";
            ConnectionString = connectStringBuilder.ToString();
        }
    }
}
