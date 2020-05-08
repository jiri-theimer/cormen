using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using UI.Models;

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

            if (Factory.CurrentUser==null || Factory.CurrentUser.isclosed)
            {
                context.Result = new RedirectResult("~/Login/UserLogin");
                return;
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

        public bool TestIfUserEditor(bool bolTestMasterSide,bool bolTestClientSide)
        {
            if (bolTestMasterSide && Factory.CurrentUser.j03EnvironmentFlag==1 && Factory.CurrentUser.TestPermission(BO.UserPermFlag.MasterAdmin)==true)
            {
                return true;
            }
            if (bolTestClientSide && Factory.CurrentUser.j03EnvironmentFlag==2 && Factory.CurrentUser.TestPermission(BO.UserPermFlag.ClientAdmin) == true)
            {
                return true;
            }
            return false;
        }
        public bool TestIfRecordEditable(int intRecJ02ID_Owner=0, int intRecP28ID=0)
        {
            if (intRecP28ID == 0 && intRecJ02ID_Owner == 0) return false;
            if (intRecP28ID==0 && intRecJ02ID_Owner > 0)
            {
                //zjistit svázané p28ID pro intRecJ02ID_Owner v testovaném záznamu
                BO.COM.GetInteger c = Factory.j02PersonBL.LoadPersonalP28ID(intRecJ02ID_Owner);
                if (c == null) return false;
                intRecP28ID = c.Value;
            }
            if (Factory.CurrentUser.j03EnvironmentFlag == 1 && Factory.CurrentUser.TestPermission(BO.UserPermFlag.MasterAdmin)){
                return true; //master admin může vše
            }
         
            if (Factory.CurrentUser.j03EnvironmentFlag == 2 && intRecP28ID == Factory.CurrentUser.p28ID)
            {
                if (Factory.CurrentUser.TestPermission(BO.UserPermFlag.ClientAdmin))
                {
                    return true;    //client admin může editovat
                }
            }
            return false;
            
        }
        public IActionResult StopPage(bool bolModal,string strMessage)
        {
            var v = new StopPageViewModel() { Message = strMessage, IsModal = bolModal };

            return View("_StopPage",v);
        }
        public IActionResult StopPageEdit(bool bolModal)
        {
            return (StopPage(bolModal, "Nemáte oprávnění editovat tento záznam."));            
        }
        public IActionResult StopPageCreate(bool bolModal)
        {
            return (StopPage(bolModal, "Nemáte oprávnění zakládat tento druh záznamu."));
        }
        public IActionResult StopPageCreateEdit(bool bolModal)
        {
            return (StopPage(bolModal, "Nemáte oprávnění zakládat nebo editovat tento druh záznamu."));
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