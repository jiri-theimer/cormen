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

        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");
            
            return View();

        }

     

        public string getHTML_FontStyleMenu()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 1; i <= 4; i++)
            {
                string s = "Malé písmo";
                if (i == 2) s = "Výchozí velikost písma";
                if (i == 3) s = "Větší";
                if (i == 4) s = "Velké";
                if (Factory.CurrentUser.j03FontStyleFlag == i) s += "&#10004;";
                sb.AppendLine(string.Format("<div ><a class='nav-link' href='javascript: save_fontstyle_menu({0})'>{1}</a></div>", i,s));
            }                        
            return sb.ToString();
        }
        
        public BO.Result SaveCurrentUserFontStyle(int fontstyleflag)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03FontStyleFlag = fontstyleflag;
            Factory.j03UserBL.Save(c);
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


        public string getHTML_CurrentUserMenu()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<a class='nav-link' href='/Home/MyProfile'>Můj profil</a>");
            sb.Append("<a class='nav-link' href='/Home/ChangePassword'>Změnit přístupové heslo</a>");
            sb.Append("<hr/><a class='nav-link' href='/Home/logout'>Odhlásit se</a>");

            return sb.ToString();
        }
        public string getHTML_MainMenu_New()
        {
            var sb = new System.Text.StringBuilder();
            if (Factory.CurrentUser.j03EnvironmentFlag == 2)
            {
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p41/record');\">Výrobní zakázka</a>");
                sb.Append("<hr/>");
            }
            sb.Append("<a class='nav-link' href=\"javascript:_window_open('/o23/record');\">Dokument</a>");
            sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p28/record');\">Klient</a>");
            sb.Append("<a class='nav-link' href=\"javascript:_window_open('/j02/record');\">Osoba/Uživatel</a>");
            sb.Append("<hr/>");
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p10/record');\">Master produkt</a>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p13/record');\">TPV</a>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p21/record');\">Licence</a>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/p26/record');\">Stroj</a>");

                sb.Append("<hr/>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/b02/record');\">Workflow stav</a>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/o12/record');\">Kategorie</a>");
                sb.Append("<a class='nav-link' href=\"javascript:_window_open('/j04/record');\">Aplikační role</a>");
            }
            

            return sb.ToString();
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
        public IActionResult MyProfile()
        {
            
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
