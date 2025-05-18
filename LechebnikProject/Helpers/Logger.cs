using NLog;
using System;

namespace LechebnikProject.Helpers
{
    /// <summary>
    /// Класс для логирования событий и ошибок.
    /// </summary>
    public static class Logger
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Логирует ошибку с исключением.
        /// </summary>
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

        /// <summary>
        /// Логирует информационное сообщение.
        /// </summary>
        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }
    }
}