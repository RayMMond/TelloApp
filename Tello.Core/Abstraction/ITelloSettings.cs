namespace Tello.Core
{
    public interface ITelloSettings
    {
        string ConfigurationFile { get; }
        INetworkSetting Network { get; set; }

        void Load(string dir);
        bool Save();
    }
}