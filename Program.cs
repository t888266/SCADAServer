using SCADAServer.EspVsApp;
using SCADAServer.EspVsApp.Connection;
using SCADAServer;
using MailSender;
using SCADAServer.Routers.UserRouter;
using SCADAServer.Routers.UserRouter.Services;
using SCADAServer.Routers.DeviceRouter;
using SCADAServer.Routers.DeviceRouter.Services;
#nullable disable
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IMailSender, MailSender.MailSender>();
builder.Services.AddSingleton<IBGMailSender,MailSenderBackgroundService>();
builder.Services.AddHostedService((sp)=>sp.GetService(typeof(IBGMailSender)) as MailSenderBackgroundService);
builder.Services.AddSingleton<DBContext>();
builder.Services.AddSingleton<CodeVerify>();
builder.Services.AddTransient<IUserAuthService,UserAuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDeviceAuthService, DeviceAuthService>();
builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddScoped<ICustomRouter>((sp) => new UserRouter("user"));
builder.Services.AddScoped<ICustomRouter>((sp) => new DeviceRouter("device"));
builder.Services.AddSingleton<WSConnectionManager>();
builder.Services.AddSingleton<EspWSConnection>();
builder.Services.AddSingleton<AppWSConnection>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWebSockets();
//app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    app.MapEspWS("/ws/esp", scope.ServiceProvider.GetService<EspWSConnection>());
    app.MapAppWS("/ws/app", scope.ServiceProvider.GetService<AppWSConnection>());
    var router = scope.ServiceProvider.GetServices<ICustomRouter>();
    foreach(ICustomRouter customRouter in router)
    {
        customRouter.AddRouter(app);
    }
}
app.Run();