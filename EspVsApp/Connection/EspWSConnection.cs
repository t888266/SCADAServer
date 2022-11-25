using System.Net.WebSockets;
using System.Text;

namespace SCADAServer.EspVsApp.Connection
{
    public class EspWSConnection
    {
        IEspWSConnectionManger EspWSConnectionManger { get; set; }
        IAppWSGetter AppWSGetter { get; set; }
        public EspWSConnection(WSConnectionManager wsConnectionManager)
        {
            EspWSConnectionManger = wsConnectionManager as IEspWSConnectionManger;
            AppWSGetter = wsConnectionManager as IAppWSGetter;
        }
        public virtual void OnConnected((string deviceKey, string typeVibration) id,
         WebSocket socket)
        {
            EspWSConnectionManger.AddSocket(id, socket);
        }
        public virtual async Task OnDisconnected((string deviceKey, string typeVibration) id,
         WebSocket socket)
        {
            await EspWSConnectionManger.RemoveSocket(id, socket);
        }
        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task OnReceiveMessage((string deviceKey, string typeVibration) id, 
        WebSocket socket, WebSocketReceiveResult result, byte[] buf)
        {
            IEnumerable<WebSocket> wss = AppWSGetter.GetByID(id);
            if(wss.Count()>0)
            {
                await Parallel.ForEachAsync(AppWSGetter.GetByID(id), async (ws, token) =>
                {
                    await SendMessageAsync(ws, Encoding.UTF8.GetString(buf, 0, result.Count));
                });
            }
        }
    }
}
