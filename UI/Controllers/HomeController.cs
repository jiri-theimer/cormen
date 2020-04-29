using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UI.Models;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;


namespace UI.Controllers
{

    

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogDebug("Začíná HomeController ");
            _logger.LogInformation("Začíná HomeController. ");
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");
            
            return View();

        }

        public IActionResult TestujGrid()
        {
            
            return View();
        }
        public IActionResult Test1()
        {

            return View();
        }



        public IActionResult MyProfile()
        {
            _logger.LogDebug("Jsem v MyProfile. ");
            _logger.LogInformation("Jsem v MyProfile. ");
            var v = new MyProfileViewModel();
            v.Rec = Factory.j02PersonBL.Load(Factory.CurrentUser.pid);
            v.CurrentUser = Factory.CurrentUser;
            return View(v);
        }
        public IActionResult ChangePassword()
        {
            var v = new ChangePasswordViewModel();
            if (Factory.CurrentUser.j03IsMustChangePassword)
            {
                Factory.CurrentUser.AddMessage("Administrátor nastavil, že si musíte změnit přihlašovací heslo.", "info");
            }
            return View(v);
        }
        [HttpPost]
        public IActionResult ChangePassword(Models.ChangePasswordViewModel v)
        {
            var cJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            var lu = new BO.LoggingUser();
            var ret = lu.ValidateChangePassword(v.NewPassword, v.CurrentPassword, v.VerifyPassword,cJ03);
            if (ret.Flag == BO.ResultEnum.Success)
            {
                cJ03.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, cJ03);
                cJ03.j03IsMustChangePassword = false;
                if (Factory.j03UserBL.Save(cJ03) > 0)
                {
                    Factory.CurrentUser.AddMessage("Heslo bylo změněno.", "info");
                    return RedirectToAction("Index");
                }
                             
            }
            else
            {
                Factory.CurrentUser.AddMessage(ret.Message);            
            }
            return View(v);

        }


       
        public IActionResult Index()
        {
            
            var v = new HomeViewModel();
            
            return View(v);
        }

       
        public IActionResult Client()
        {

            var v = new HomeViewModel();
           
            return View(v);
        }
        

    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var v = new ErrorViewModel() { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            var errFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (errFeature != null)
            {
                v.Error = errFeature.Error;
            }
            
            
            
            var path = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (path != null)
            {
                v.OrigFullPath = path.Path;
            }
            
            

            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusFeature != null)
            {
                v.OrigFullPath = statusFeature.OriginalPath;
                
            }

            v.OrigFullPath += HttpContext.Request.QueryString;
            _logger.LogError(v.Error,"Stala se chyba, byl jsem tu");
            
            _logger.LogCritical("Stala se kritická chyba, byl jsem tu");


            return View(v);
        }
    }
}
