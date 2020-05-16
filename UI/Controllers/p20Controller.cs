using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    ///MJ
    public class p20Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p20RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p20UnitBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p20Unit();              
                v.Rec.entity = "p20";
                if (Factory.CurrentUser.j03EnvironmentFlag == 2)
                {//klientský režim
                    v.Rec.p28ID = Factory.CurrentUser.p28ID;
                    v.Rec.p28Name = Factory.CurrentUser.p28Name;
                }
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p20RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p20Unit c = new BO.p20Unit();
                if (v.Rec.pid > 0) c = Factory.p20UnitBL.Load(v.Rec.pid);

                c.p20Code = v.Rec.p20Code;
                c.p20Name = v.Rec.p20Name;
                c.p28ID = v.Rec.p28ID;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p20UnitBL.Save(c);
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