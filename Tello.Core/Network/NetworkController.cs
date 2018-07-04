using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tello.Core.Network
{
    public class NetworkController : INetworkController
    {
        private INetworkSetting setting;

        public NetworkController(INetworkSetting setting,
            ITelloClient telloClient)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            Client = telloClient ?? throw new ArgumentNullException(nameof(telloClient));


        }


        public ITelloClient Client { get; }
    }
}
