using Microsoft.Extensions.Logging;
using SimpleInjector;
using System;
using Tello.Core;
using Tello.Core.Network;
using Xunit;

namespace Tello.CoreTest
{
    public class NetworkControllerTest
    {

        static Container container;
        static NetworkControllerTest()
        {
            container = new Container();
            container.Register<ILoggerFactory, TelloLogging>(Lifestyle.Singleton);
            container.Register<INetworkSetting, NetworkSetting>(Lifestyle.Singleton);
            container.Register<ITelloClient, TelloClient>(Lifestyle.Transient);
            container.Register<ITelloSettings, TelloSettings>(Lifestyle.Singleton);
            container.Register<INetworkController, NetworkController>();

            container.Verify(VerificationOption.VerifyAndDiagnose);
        }



        [Fact]
        public void Test1()
        {
            var settings = container.GetInstance<ITelloSettings>();
            settings.Load(@"F:\gitcode\TelloApp\Tello.Core");
            settings.Network.IPv4 = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("192.168.10.1"), 8889);
            settings.Save();
            var controller = container.GetInstance<INetworkController>();
        }
    }
}
