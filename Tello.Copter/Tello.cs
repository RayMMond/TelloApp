using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tello.Core;

namespace Tello.Core
{
    public class Tello
    {
        private IConnectionController networkController;
        private ILogger logger;


        public Tello(ILoggerFactory loggerFactory,
              TelloSettings settings,
              IConnectionController controller)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<Tello>();
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            networkController = controller ?? throw new ArgumentNullException(nameof(controller));
        }





        public TelloSettings Settings { get; private set; }





    }
}
