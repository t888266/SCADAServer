using System;
using System.Net.WebSockets;
using System.Text;

namespace SCADAServer.EspVsApp.Connection
{
    public class AppWSConnection
    {
        IAppWSConnectionManger AppWSConnectionManger { get; set; }
        IEspWSGetter EspWSGetter { get; set; }
        IAppWSGetter AppWSGetter { get; set; }
        public AppWSConnection(WSConnectionManager wsConnectionManager)
        {
            AppWSConnectionManger = wsConnectionManager as IAppWSConnectionManger;
            EspWSGetter = wsConnectionManager as IEspWSGetter;
            AppWSGetter = wsConnectionManager as IAppWSGetter;
        }
        public virtual async void OnConnected((string deviceKey,string typeVibration,string token) id, WebSocket socket)
        {
            AppWSConnectionManger.AddSocket(id, socket);
            var roomID = (deviceKey: id.deviceKey, typeVibration: id.typeVibration);
            WebSocket espWS = EspWSGetter.GetByID(roomID);
            if (espWS == null)
            {
                await Parallel.ForEachAsync(AppWSGetter.GetByID(roomID), async (ws, token) =>
                {
                    await SendMessageAsync(ws, "E-No Esp Available");
                });
            }
        }
        public virtual async Task OnDisconnected((string deviceKey,string typeVibration,string token) id, WebSocket socket)
        {
            await AppWSConnectionManger.RemoveSocket(id, socket);
        }
        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task OnReceiveMessage((string deviceKey, string typeVibration) id, WebSocket socket, WebSocketReceiveResult result, byte[] buf)
        {
            WebSocket espWS = EspWSGetter.GetByID(id);
            if (espWS != null)
            {
                string mess = Encoding.UTF8.GetString(buf, 0, result.Count);
                await SendMessageAsync(EspWSGetter.GetByID(id), mess);
                var appList = AppWSGetter.GetByIDExcept(id, socket);
                if (appList.Count() > 0)
                {
                    if(TypeMessageHelper.GetTypeMessage(mess)==TypeMessage.Command)
                    {
                        await Parallel.ForEachAsync(AppWSGetter.GetByIDExcept(id, socket), async (ws, token) =>
                        {
                            await SendMessageAsync(ws, mess);
                        });
                    }
                }

            }
        }
    }
}
