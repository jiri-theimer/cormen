using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BL.RunningApp _app;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,BL.RunningApp app)
        {
            _logger = logger;
            _app = app;
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            var c = new BO.RunningUser() { j03Login = "treti@marktime.cz" };
            BL.Factory f = new BL.Factory(c, _app);
            return f.CurrentUser.FullName;
        }

        [HttpGet]
        [Route("pokus")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
