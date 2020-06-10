using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    
    public class ApiController : ControllerBase
    {
        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return DateTime.Now.ToString();

        }

        [HttpGet]
        [Route("test/{pid:int}")]
        public BO.p41Task TestRecord(int pid)
        {
            var c = new BO.p41Task() { p41ID = pid, p41Name = "hovado" };

            return c;
            
        }

        
    }
}