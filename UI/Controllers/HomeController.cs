﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
          
        }


        
        [AllowAnonymous]        
        public IActionResult Index()
        {
            
            var v = new HomeViewModel();
            //v.Notify(String.Format("Testovací info zpráva, aktuální čas: {0}", DateTime.Now.ToString()), "info");
            //v.Notify(String.Format("Testovací warning zpráva, aktuální čas: {0}", DateTime.Now.ToString()), "warning");
            //v.Notify(String.Format("Testovací error zpráva, aktuální čas: {0}", DateTime.Now.ToString()), "error");





            return View(v);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
             
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
