using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tello.Core;

namespace Tello.Core
{
    public class TelloController : ITelloController
    {
        private ILogger logger;


        public TelloController(ILoggerFactory loggerFactory,
              TelloSettings settings,
              IConnectionController controller)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<TelloController>();
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ConnectionController = controller ?? throw new ArgumentNullException(nameof(controller));

        }

        public IConnectionController ConnectionController { get; }



        public TelloSettings Settings { get; private set; }



    }
}
