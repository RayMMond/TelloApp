
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using Tello.Core;
using Tello.Core.Network.UWP;

namespace Tello.Network.UWPTest
{
    [TestClass]
    public class WiFiManagerTest
    {
        static readonly Container container;
        static WiFiManagerTest()
        {
            container = new Container();
            //container.Register<TelloSettings, TelloSettings>(Lifestyle.Singleton);
            container.Register<INetworkSetting, NetworkSetting>(Lifestyle.Singleton);
            container.Register<IWiFiManager, WiFiManager>();

            container.Verify();
        }


        [TestMethod]
        [DataRow("WLAN")]
        public void InitializeWiFiAdapterAsyncTest(string adapterName)
        {
            var man = container.GetInstance<IWiFiManager>();
            man.Setting.WiFiAdapterName = adapterName;
            man.InitializeWiFiAdapterAsync().Wait();
        }

        [TestMethod]
        [DataRow("WLAN")]
        public void ConnectAysncTest(string adapterName)
        {
            var man = container.GetInstance<IWiFiManager>();
            man.Setting.WiFiAdapterName = adapterName;
            man.Setting.SSID = "TELLO-C3D147";
            man.Setting.Password = "";
            man.InitializeWiFiAdapterAsync().Wait();
            var succ = man.ConnectAsync();
            succ.Wait();
            Assert.IsTrue(succ.Result);
        }


    }
}
