using NLog;
using System;

namespace LechebnikProject.Helpers
{
    public static class Logger
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public static void LogError(string message, Exception ex = null)
        {
            if (ex != null)
            {
                _logger.Error(ex, message);
            }
            else
            {
                _logger.Error(message);
            }
        }

        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }
    }
}