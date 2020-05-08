using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p11Controller : BaseController
    {
        public IActionResult Index(int pid)
        {

            var v = new Models.p11PreviewViewModel();
            v.Rec = Factory.p11ClientProductBL.Load(pid);
            if (v.Rec == null) return RecNotFound(v);
            return View(v);

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!Factory.CurrentUser.TestPermission(BO.UserPermFlag.ClientAdmin))
            {
                return this.StopPageCreateEdit(true);
            }

            var v = new Models.p11RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p11ClientProductBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                

            }
            else
            {

                v.Rec = new BO.p11ClientProduct();
                v.Rec.entity = "p11";
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);


            if (isclone) { v.Toolbar.MakeClone(); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p11RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p11ClientProduct c = new BO.p11ClientProduct();
                if (v.Rec.pid > 0) c = Factory.p11ClientProductBL.Load(v.Rec.pid);

                c.p11Code = v.Rec.p11Code;
                c.p11Name = v.Rec.p11Name;
                c.p11Memo = v.Rec.p11Memo;
                c.b02ID = v.Rec.b02ID;
                c.p12ID = v.Rec.p12ID;
                c.p11UnitPrice = v.Rec.p11UnitPrice;
                c.p20ID = v.Rec.p20ID;
                
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p11ClientProductBL.Save(c);
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