namespace SCADAServer.Routers.DeviceRouter.Models
{
#nullable disable
    public class DeviceModel
    {
        public int DeviceID { get; set; }
        public string DeviceKey { get; set; }
        public string DeviceName { get; set; }
    }
    public class DeviceConfig
    {
        public string TypeVibration { get; set; }
        public bool IsRecord { get; set; }
        public float WarnValue { get;set; }
        public float StopValue { get; set; }
    }
    public class DeviceSavedData
    {
        public string TypeVibration { get;set; }
        public DateTime TimeSaved { get; set; }
        public float Temperature { get; set; }
        public float XAcc { get; set; }
        public float YAcc { get; set; }
        public float ZAcc { get; set; }
    }
}
