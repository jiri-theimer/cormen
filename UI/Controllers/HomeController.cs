using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
          
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");

            return View();

        }
        public IActionResult MyProfile()
        {
            var v = new MyProfileViewModel();
            v.Rec = Factory.j02PersonBL.Load(Factory.CurrentUser.pid);
            v.CurrentUser = Factory.CurrentUser;
            return View(v);
        }
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }
        [HttpPost]
        public IActionResult ChangePassword(Models.ChangePasswordViewModel v)
        {
            var cJ02 = Factory.j02PersonBL.Load(Factory.CurrentUser.pid);
            var lu = new BO.LoggingUser();
            var ret = lu.ValidateChangePassword(v.NewPassword, v.CurrentPassword, v.VerifyPassword,cJ02);
            if (ret.Flag == BO.ResultEnum.Success)
            {                                
                cJ02.j02PasswordHash = lu.Pwd2Hash(v.NewPassword, cJ02);
                if (Factory.j02PersonBL.Save(cJ02) > 0)
                {
                    v.Notify("Heslo bylo změněno.", "info");
                    return RedirectToAction("Index");
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);
                }                
            }
            else
            {
                v.Notify(ret.Message);                
            }
            return View(v);

        }


        [AllowAnonymous]        
        public IActionResult Index()
        {
            
            var v = new HomeViewModel();
            
            return View(v);
        }

        [AllowAnonymous]
        public IActionResult Client()
        {

            var v = new HomeViewModel();
           
            return View(v);
        }
        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
             
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
