using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p19Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.p19RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p19MaterialBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p19Material();
                v.Rec.entity = "p19";

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p19RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p19Material c = new BO.p19Material();
                if (v.Rec.pid > 0) c = Factory.p19MaterialBL.Load(v.Rec.pid);

                c.p19Code = v.Rec.p19Code;
                c.p19Name = v.Rec.p19Name;
                c.p19Memo = v.Rec.p19Memo;
                c.p20ID = v.Rec.p20ID;
                c.o12ID = v.Rec.o12ID;
                c.p19DefaultOperParam = v.Rec.p19DefaultOperParam;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p19MaterialBL.Save(c);
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