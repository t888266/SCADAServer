using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SCADAServer.EspVsApp.Connection;

namespace SCADAServer.EspVsApp
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class EspWSMiddelware:WSMiddleware<EspWSConnection>
    {
        public EspWSMiddelware(RequestDelegate next, EspWSConnection wSConnection) : 
        base(next, wSConnection)
        {
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            string deviceKey = context.Request.Query["deviceKey"];
            string typeVibration = context.Request.Query["typeVibration"];
            if (!String.IsNullOrEmpty(deviceKey) && !String.IsNullOrEmpty(typeVibration))
            {
                var ID = (deviceKey: deviceKey, typeVibration: typeVibration);
                WSConnection.OnConnected(ID, socket);
                await Receive(socket, async (result, buf) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await WSConnection.OnReceiveMessage(ID, socket, result, buf);
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await WSConnection.OnDisconnected(ID, socket);
                        return;
                    }
                },
                async ()=>
                {
                    await WSConnection.OnDisconnected(ID, socket);
                });
            }
        }
    }
}
