using SCADAServer.EspVsApp.Connection;
using System.Net.WebSockets;

namespace SCADAServer.EspVsApp
{
    public abstract class WSMiddleware<T>
    {
        protected readonly RequestDelegate _next;
        protected T WSConnection { get; set; }
        public WSMiddleware(RequestDelegate next, T wSConnection)
        {
            _next = next;
            WSConnection = wSConnection;
        }
        public abstract Task InvokeAsync(HttpContext context);
        protected async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, 
        byte[]> handleMessage, Action handleException)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
                     CancellationToken.None);
                    handleMessage.Invoke(result, buffer);
                }
                catch
                {
                    Console.WriteLine($"Unexpected error from {socket}");
                    handleException.Invoke();
                }
            }
        }
    }
}
