﻿using System;
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

        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");
            
            return View();

        }

        public BO.Result SaveCurrentUserFontStyle(int fontstyleflag)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03FontStyleFlag = fontstyleflag;
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }

        public BO.Result UpdateCurrentUserPing(BO.j92PingLog c)
        {
            var uaParser = UAParser.Parser.GetDefault();
            UAParser.ClientInfo client_info = uaParser.Parse(c.j92BrowserUserAgent);
            c.j92BrowserOS = client_info.OS.Family + " " + client_info.OS.Major;
            c.j92BrowserFamily = client_info.UA.Family + " " + client_info.UA.Major;
            c.j92BrowserDeviceFamily = client_info.Device.Family;
            
            Factory.j03UserBL.UpdateCurrentUserPing(c);
            
            return new BO.Result(false);
        }

        public BO.Result StartStopLiveChat(int flag)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            if (flag == 1)
            {
                c.j03LiveChatTimestamp = DateTime.Now;   //zapnout smartsupp
            }
            else
            {
                c.j03LiveChatTimestamp = null;   //vypnout smartsupp
            }
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }
        public IActionResult LiveChat()
        {
            
            return View();
        }


       
        
        public BO.Result ToggleEnvironment()
        {
            if (Factory.CurrentUser.TestPermission(BO.UserPermFlag.MasterReader) || Factory.CurrentUser.TestPermission(BO.UserPermFlag.MasterAdmin)){
                var cU = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
                if (Factory.CurrentUser.j03EnvironmentFlag == 1)
                {
                    cU.j03EnvironmentFlag = 2;
                }
                else
                {
                    cU.j03EnvironmentFlag = 1;
                }
                Factory.j03UserBL.Save(cU);
            }
            return new BO.Result(false);
        }

        public IActionResult About()
        {
            
            return View();
        }
        public IActionResult MyProfile()
        {           
            var v = new MyProfileViewModel();
            v.userAgent = Request.Headers["User-Agent"];
            
            var uaParser = UAParser.Parser.GetDefault();
            v.client_info = uaParser.Parse(v.userAgent);
            
            v.Rec = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
            v.CurrentUser = Factory.CurrentUser;
            if (v.CurrentUser.j03GridSelectionModeFlag == 1)
            {
                v.IsGridClipboard = true;
            }
            v.EmailAddres = v.Rec.j02Email;
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(Models.MyProfileViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.j02Person c = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
                c.j02Email = v.EmailAddres;
                if (Factory.j02PersonBL.Save(c) > 0)
                {
                    BO.j03User cUser = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
                    if (v.IsGridClipboard == true)
                    {
                        cUser.j03GridSelectionModeFlag = 1;
                    }
                    else
                    {
                        cUser.j03GridSelectionModeFlag = 0;
                    }
                    Factory.j03UserBL.Save(cUser);
                    Factory.CurrentUser.AddMessage("Změny uloženy", "info");
                }

            }

            return MyProfile();
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
         
            return View(v);
        }
    }
}
