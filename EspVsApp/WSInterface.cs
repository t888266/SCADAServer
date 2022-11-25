using System.Net.WebSockets;

namespace SCADAServer.EspVsApp
{
    public interface IAppWSConnectionManger
    {
        void AddSocket((string deviceKey, string typeVibration, string token) id,
         WebSocket socket);
        Task RemoveSocket((string deviceKey, string typeVibration, string token) id,
         WebSocket socket);
    }
    public interface IEspWSConnectionManger
    {
        void AddSocket((string deviceKey, string typeVibration) id, WebSocket ws);
        Task RemoveSocket((string deviceKey, string typeVibration) id, WebSocket ws);
    }
    public interface IAppWSGetter
    {
        IEnumerable<WebSocket> GetByID((string deviceKey, string typeVibration) id);
        IEnumerable<WebSocket> GetAll();
        public IEnumerable<WebSocket> GetByIDExcept((string deviceKey, string 
        typeVibration) id,WebSocket ws);
    }
    public interface IEspWSGetter
    {
        WebSocket GetByID((string deviceKey, string typeVibration) id);
        IEnumerable<WebSocket> GetAll();
    }
}
