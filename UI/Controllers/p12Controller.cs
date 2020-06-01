using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p12Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p12PreviewViewModel();


            v.Rec = Factory.p12ClientTpvBL.Load(pid);

            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                if (v.Rec.p13ID_Master > 0)
                {
                    v.RecP13 = Factory.p13MasterTpvBL.Load(v.Rec.p13ID_Master);
                }
                v.RecP21 = Factory.p21LicenseBL.Load(v.Rec.p21ID);
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(false, true))
            {
                return this.StopPageCreateEdit(true);
            }
           
            var v = new Models.p12RecordViewModel();

            if (pid > 0)
            {                

                v.Rec = Factory.p12ClientTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                BO.p21License cP21 = Factory.p21LicenseBL.Load(v.Rec.p21ID);
                if (isclone == false && v.Rec.p13ID_Master>0)
                {
                    return this.StopPage(true, "Recepturu s Master vzorem nelze upravovat.<hr>Zkopírujte si ji do nové receptury, kterou můžete upravovat.");
                }
                if (cP21.p21PermissionFlag != BO.p21PermENUM.Independent2Master)
                {
                    return this.StopPage(true, string.Format("S licencí typu {2} [{0} - {1}]  nemáte oprávnění zakládat vlastní receptury.", cP21.p21Code,cP21.p21Name,cP21.PermFlagAlias));
                }

                

            }
            else
            {
                
                v.Rec = new BO.p12ClientTpv();
                v.Rec.entity = "p12";
                if (Factory.p21LicenseBL.GetList(new BO.myQuery("p21License")).Where(p => p.p21PermissionFlag == BO.p21PermENUM.Independent2Master).Count() == 0)
                {
                    Factory.CurrentUser.AddMessage("Systém nepovolí uložit vlastní recepturu, protože ani jedna z vašich licencí k tomu nemá oprávnění.", "warning");
                }

            }


            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.Toolbar.MakeClone();
                v.Rec.p12Code += "-COPY";
                
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p12RecordViewModel v)
        {
           
            if (ModelState.IsValid)
            {
                BO.p12ClientTpv c = new BO.p12ClientTpv();
                if (v.Rec.pid > 0) c = Factory.p12ClientTpvBL.Load(v.Rec.pid);

                c.p12Code = v.Rec.p12Code;
                c.p12Name = v.Rec.p12Name;
                c.p12Memo = v.Rec.p12Memo;
                c.p21ID = v.Rec.p21ID;
                c.p25ID = v.Rec.p25ID;
                

                v.Rec.pid = Factory.p12ClientTpvBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            this.Notify_RecNotSaved();
            return View(v);
        }


       
       
    }
}