namespace Tello.Core
{
    public interface IConnectionController
    {
        ITelloClient Client { get; }

        IWiFiManager WiFi { get; }
    }
}