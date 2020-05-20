using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace UI
{
    public class TheRobot : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TheRobot> _logger;
        private Timer _timer;
        private readonly BL.RunningApp _app;

        public TheRobot(ILogger<TheRobot> logger,BL.RunningApp runningapp)
        {
            _logger = logger;
            _app = runningapp;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(20));   //každých 20 sekund
            
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            BO.RunningUser c = new BO.RunningUser() { j03Login = "admin@marktime.cz" };

            var f = new BL.Factory(c, _app);
            string strPING = f.CurrentUser.j03PingTimestamp.ToString();
            

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}, factory user: "+ f.CurrentUser.FullName+", last ping: "+strPING, count);

            try
            {
                var xx = new RedirectResult("~/Login/UserLogin");
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error, error: " + ex.Message);
            }
            
            

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
