using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Core
{
    public class TelloLogging : ILoggerFactory
    {
        private readonly ILoggerFactory defaultFactory;

        public TelloLogging()
        {
            defaultFactory = new LoggerFactory()
                .AddDebug(minLevel: LogLevel.Debug);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            defaultFactory.AddProvider(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return defaultFactory.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            defaultFactory.Dispose();
        }
    }
}
