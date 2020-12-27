using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hass_desktop_service.StateDetectors.Windows.Fullscreen;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hass_desktop_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly UserNotificationStateDetector _userNotificationStateDetector;

        public Worker(ILogger<Worker> logger, UserNotificationStateDetector userNotificationStateDetector)
        {
            _logger = logger;
            this._userNotificationStateDetector = userNotificationStateDetector;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Notificationstate: {this._userNotificationStateDetector.GetState()}");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
