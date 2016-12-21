﻿using LegnicaIT.JwtAuthServer.Interfaces;
using Microsoft.Extensions.Logging;

namespace LegnicaIT.JwtAuthServer.Helpers
{
    public class JwtLogger : IJwtLogger
    {
        protected readonly ILogger<JwtLogger> _logger;

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }
    }
}
