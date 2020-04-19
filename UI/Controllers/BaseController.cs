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
            if (Factory == null)
            {
                Factory = new BL.Factory(context.HttpContext.User.Identity.Name);
                if (Factory.CurrentUser.isclosed)
                {
                    context.Result = new RedirectResult("~/Login/UserLogin");
                }
                if (Factory.CurrentUser.j02IsMustChangePassword && context.RouteData.Values["action"].ToString() !="ChangePassword")
                {
                    
                    context.Result = new RedirectResult("~/Home/ChangePassword");
                    // RedirectToAction("ChangePassword", "Home");
                }
            }

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
            v.Notify("Hledaný záznam neexistuje!");
            return View(v);
        }
        
    }
}