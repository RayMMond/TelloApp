namespace Tello.Core
{
    public interface ITelloController
    {
        IConnectionController ConnectionController { get; }
        TelloSettings Settings { get; }
    }
}