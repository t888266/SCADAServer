using SCADAServer.EspVsApp;
using SCADAServer.EspVsApp.Connection;

namespace SCADAServer
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder MapAppWS(this IApplicationBuilder builder, string path, AppWSConnection appWSConnection)
        {
            return builder.Map(path, (builder) => builder.UseMiddleware<AppWSMiddleware>(appWSConnection));
        }
        public static IApplicationBuilder MapEspWS(this IApplicationBuilder builder, string path, EspWSConnection espWSConnection)
        {
            return builder.Map(path, (builder) => builder.UseMiddleware<EspWSMiddelware>(espWSConnection));
        }
    }
}
