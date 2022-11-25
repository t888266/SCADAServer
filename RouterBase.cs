public interface ICustomRouter
{
    string BaseRouteUrl { get; set; }
    void AddRouter(WebApplication app);
}
#nullable disable
public abstract class RouterBase : ICustomRouter
{
    public RouterBase(string baseRouteUrl)
    {
        BaseRouteUrl = baseRouteUrl;
    }

    public string BaseRouteUrl { get; set; }
    public abstract void AddRouter(WebApplication app);
}
