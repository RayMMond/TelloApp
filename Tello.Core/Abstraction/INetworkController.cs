namespace Tello.Core.Network
{
    public interface INetworkController
    {
        ITelloClient Client { get; }
    }
}