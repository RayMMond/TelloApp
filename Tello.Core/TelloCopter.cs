using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tello.Core.Network;

namespace Tello.Core
{
    public class TelloCopter
    {
        private NetworkController networkController;
        private ILoggerFactory loggerFactory;
        private ILogger logger;


        public TelloCopter(ILoggerFactory loggerFactory,
              TelloSettings settings,
              NetworkController controller)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger = loggerFactory.CreateLogger<TelloCopter>();
            Settings = settings;
            networkController = controller;
        }





        public TelloSettings Settings { get; private set; }





    }
}
