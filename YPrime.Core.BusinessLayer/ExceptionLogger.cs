using System.IO;
using Serilog;
using Serilog.Events;

namespace YPrime.Core.BusinessLayer
{
    public static class ExceptionLogger
    {
        private static ILogger _logger = null;
        private static readonly string path = Directory.GetCurrentDirectory();

        public static ILogger InitLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerConfiguration()
                       .MinimumLevel.Verbose()
                       .WriteTo.Async(a => a.RollingFile($"{path}\\logs\\ExceptionLog.log", restrictedToMinimumLevel: LogEventLevel.Error))
                       .WriteTo.Async(a => a.RollingFile($"{path}\\logs\\DebugLog.log", LogEventLevel.Debug))
                        .CreateLogger();
                }
                return _logger;
            }
            set { _logger = value; }
        }

        public static ILogger CreateLogger() => InitLogger;
    }
}
