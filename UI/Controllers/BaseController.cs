﻿using System;
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
            //předání přihlášeného uživatele do Factory
            BO.RunningUser ru = (BO.RunningUser)HttpContext.RequestServices.GetService(typeof(BO.RunningUser));
            if (string.IsNullOrEmpty(ru.j03Login))
            {
                ru.j03Login = context.HttpContext.User.Identity.Name;
            }
            this.Factory= (BL.Factory)HttpContext.RequestServices.GetService(typeof(BL.Factory));

            if (Factory.CurrentUser.isclosed)
            {
                context.Result = new RedirectResult("~/Login/UserLogin");
            }
            if (Factory.CurrentUser.j03IsMustChangePassword && context.RouteData.Values["action"].ToString() != "ChangePassword")
            {

                context.Result = new RedirectResult("~/Home/ChangePassword");
                // RedirectToAction("ChangePassword", "Home");
            }

         

            //Příklad přesměrování stránky jinam:
            //context.Result = new RedirectResult("~/Home/Index");

        }
        //Test probíhá po spuštění každé Akce:
        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        {
            if (ModelState.IsValid == false)
            {
                var modelErrors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                        Factory.CurrentUser.AddMessage("Kontrola chyb: "+modelError.ErrorMessage);
                    }
                }
            }
            
            base.OnActionExecuted(context);
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