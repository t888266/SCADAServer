using System.Collections.Concurrent;
using System.Net.WebSockets;
namespace SCADAServer.EspVsApp
{
    public enum TypeConnection
    {
        EspConnection,
        AppClient,
    }
#nullable disable
    public class WSConnectionManager:IAppWSConnectionManger,IAppWSGetter,IEspWSConnectionManger,IEspWSGetter
    {
        ConcurrentDictionary<(string deviceKey, string typeVibration), WebSocket> espConnections = new ConcurrentDictionary<(string deviceKey, string typeVibration), WebSocket>();
        ConcurrentDictionary<(string deviceKey, string typeVibration, string token), WebSocket>appConnections = new ConcurrentDictionary<(string deviceKey, string typeVibration, string token), WebSocket>();
        public void AddSocket((string deviceKey, string typeVibration) id, WebSocket ws)
        {
            Console.WriteLine("ADD ESP");
            espConnections.TryAdd(id, ws);
        }
        public async Task RemoveSocket((string deviceKey, string typeVibration) id, WebSocket ws)
        {
            WebSocket socket;
            if (espConnections.TryRemove(id, out socket))
            {
                Console.WriteLine(socket.State);
                if(socket.State==WebSocketState.CloseReceived)
                {
                    Console.WriteLine("REMOVE ESP");
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Esp {id.deviceKey}-{id.typeVibration} closed!", CancellationToken.None);
                    socket.Dispose();
                }
            }
        }
        public void AddSocket((string deviceKey, string typeVibration, string token) id, WebSocket ws)
        {
            Console.WriteLine("ADD APP");
            appConnections.TryAdd(id, ws);
        }
        public async Task RemoveSocket((string deviceKey, string typeVibration, string token) id, WebSocket ws)
        {
            Console.WriteLine("REMOVE APP");
            WebSocket socket;
            if (appConnections.TryRemove(id, out socket))
            {
                if(socket.State == WebSocketState.CloseReceived)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"App {id.deviceKey}-{id.typeVibration}-{id.token} closed!", CancellationToken.None);
                    socket.Dispose();
                }
            }
        }

        IEnumerable<WebSocket> IAppWSGetter.GetByID((string deviceKey, string typeVibration) id)
        {
            return appConnections.Where((kp) => kp.Key.Item1.Equals(id.deviceKey) && kp.Key.typeVibration.Equals(id.typeVibration))
                                .Select(kp => kp.Value);
        }
        public IEnumerable<WebSocket> GetByIDExcept((string deviceKey, string typeVibration) id, WebSocket ws)
        {
            return appConnections.Where((kp) => kp.Key.Item1.Equals(id.deviceKey) && kp.Key.typeVibration.Equals(id.typeVibration) && kp.Value != ws)
                                .Select(kp => kp.Value);
        }
        IEnumerable<WebSocket> IAppWSGetter.GetAll()
        {
            return appConnections.Values;
        }

        WebSocket IEspWSGetter.GetByID((string deviceKey, string typeVibration) id)
        {
            return espConnections.FirstOrDefault((kp) => kp.Key.Equals(id)).Value;
        }

        IEnumerable<WebSocket> IEspWSGetter.GetAll()
        {
            return espConnections.Values;
        }
    }
}
