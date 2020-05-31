using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class o51Controller : BaseController
    {
        ///štítky
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o51RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o51TagBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            else
            {
                v.Rec = new BO.o51Tag();
                v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51");
                v.Rec.entity = "o51";

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o51RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o51Tag c = new BO.o51Tag();
                if (v.Rec.pid > 0) c = Factory.o51TagBL.Load(v.Rec.pid);

                c.o51Code = v.Rec.o51Code;
                c.o51Name = v.Rec.o51Name;
                c.o51Entity = v.Rec.o51Entity;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o51TagBL.Save(c);
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