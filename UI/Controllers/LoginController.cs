using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UI.Models;


namespace UI.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult UserLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                TryLogout();
            }

            var v = new BO.LoggingUser();
            return View(v);
        }

        private async void TryLogout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("CookieAuthentication");



        }

        [HttpPost]
        public ActionResult UserLogin([Bind] BO.LoggingUser lu, string returnurl)
        {
            var f = new BL.Factory(lu.Login);
            if (f.CurrentUser == null)
            {
                lu.Message = "Přihlášení se nezdařilo - pravděpodobně chybné heslo nebo jméno!";
                return View(lu);
            }
            if (f.CurrentUser.isclosed)
            {
                lu.Message = "Uživatelský účet je uzavřený pro přihlašování!";
                return View(lu);
            }
            BO.j02Person cJ02 = f.j02PersonBL.LoadByLogin(lu.Login);
            var hasher = new BO.COM.PasswordHasher();
            var sul = lu.Login.ToUpper() + "+kurkuma+" + lu.Password+"+"+cJ02.pid.ToString() ;
            var overeni = hasher.VerifyHashedPassword(cJ02.j02PasswordHash, sul);
            if (overeni == BO.COM.PasswordVerificationResult.Failed)
            {
                lu.Message = "Ověření uživatele se nezdařilo - pravděpodobně chybné heslo nebo jméno!";                
                return View(lu);
            }

            //ověřený
            var userClaims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, lu.Login),
                new Claim("access_token","hovado1"),
                new Claim(ClaimTypes.Email, cJ02.j02Email)
                 };

            var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });



            //prodloužit expiraci cookie na CookieExpiresInHours hodin
            var xx = new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.Now.AddHours(lu.CookieExpiresInHours) };
            HttpContext.SignInAsync(userPrincipal, xx);


            if (returnurl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnurl);

            }



        }


    }
}