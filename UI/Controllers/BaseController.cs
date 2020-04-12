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
            }

            //Příklad přesměrování stránky jinam:
            //context.Result = new RedirectResult("~/Home/Index");

        }
        
       
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

        
        
    }
}