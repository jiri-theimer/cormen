using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;


namespace UI.Controllers    
{
    [Authorize]
    public class BaseController : Controller
    {
        
        public BL.Factory Factory;
        

        //Test probíhá před spuštěním každé Akce!
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            BO.RunningUser ru = (BO.RunningUser)HttpContext.RequestServices.GetService(typeof(BO.RunningUser));
            if (string.IsNullOrEmpty(ru.j02Login))
            {
                ru.j02Login = context.HttpContext.User.Identity.Name;
            }
            this.Factory= (BL.Factory)HttpContext.RequestServices.GetService(typeof(BL.Factory));

            if (Factory.CurrentUser.isclosed)
            {
                context.Result = new RedirectResult("~/Login/UserLogin");
            }
            if (Factory.CurrentUser.j02IsMustChangePassword && context.RouteData.Values["action"].ToString() != "ChangePassword")
            {

                context.Result = new RedirectResult("~/Home/ChangePassword");
                // RedirectToAction("ChangePassword", "Home");
            }

            //BL.Factory2 f = (BL.Factory2)HttpContext.RequestServices.GetService(typeof(BL.Factory2));
            //if (f.CurrentUser == null)
            //{
            //    f.SetCurrentUser(context.HttpContext.User.Identity.Name);
            //}


            //helper.WriteMessage("Nazdar, jsem v OnActionExecuting", context.HttpContext.User.Identity.Name);

            //if (Factory == null)
            //{
            //    Factory = new BL.Factory(context.HttpContext.User.Identity.Name);

            //    if (Factory.CurrentUser.isclosed)
            //    {
            //        context.Result = new RedirectResult("~/Login/UserLogin");
            //    }
            //    if (Factory.CurrentUser.j02IsMustChangePassword && context.RouteData.Values["action"].ToString() !="ChangePassword")
            //    {

            //        context.Result = new RedirectResult("~/Home/ChangePassword");
            //        // RedirectToAction("ChangePassword", "Home");
            //    }

            //}

            //Příklad přesměrování stránky jinam:
            //context.Result = new RedirectResult("~/Home/Index");

        }
        //Test probíhá po spuštění každé Akce:
        //public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        //{

        //    base.OnActionExecuted(context);
        //}


        //public MotherController(IHttpContextAccessor context)
        //{



        //    //Factory = new BL.Factory(context.HttpContext.User.Identity.Name);                                    
        //}


        public RedirectToActionResult StopPage(bool bolModal,string strMessage)
        {
            if (bolModal)
            {
                return RedirectToAction("StopModal", "Common", new { message = strMessage });
            }
            else
            {
                return RedirectToAction("Stop", "Common", new { message = strMessage });
            }
        }

        
        public ViewResult RecNotFound(UI.Models.BaseViewModel v)
        {
            Factory.CurrentUser.AddMessage("Hledaný záznam neexistuje!","error");            
            return View(v);
        }

        public void Notify_RecNotSaved()
        {
            Factory.CurrentUser.AddMessage("Záznam zatím nebyl uložen.", "warning");
        }
        
    }
}