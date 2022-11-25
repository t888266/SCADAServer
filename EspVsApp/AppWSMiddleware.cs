using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SCADAServer.EspVsApp.Connection;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SCADAServer.EspVsApp
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AppWSMiddleware : WSMiddleware<AppWSConnection>
    {
        public AppWSMiddleware(RequestDelegate next, AppWSConnection wSConnection) : base(next, wSConnection)
        {
        }

        public async override Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            string deviceKey = context.Request.Query["deviceKey"];
            string typeVibration = context.Request.Query["typeVibration"];
            string token = context.Request.Query["token"];
            if (!String.IsNullOrEmpty(deviceKey) && !String.IsNullOrEmpty(typeVibration)&& !String.IsNullOrEmpty(token))
            {
                var ID = (deviceKey: deviceKey, typeVibration: typeVibration, token: token);
                WSConnection.OnConnected(ID, socket);
                await Receive(socket, async (result, buf) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine(token);
                        await WSConnection.OnReceiveMessage((deviceKey:deviceKey,typeVibration:typeVibration), socket, result, buf);
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await WSConnection.OnDisconnected(ID, socket);
                        return;
                    }
                },
                async () =>
                {
                    await WSConnection.OnDisconnected(ID, socket);
                });
            }
        }
    }

}
