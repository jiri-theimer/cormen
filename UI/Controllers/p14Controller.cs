using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p14Controller : BaseController
    {
        public IActionResult Record(int pid,int p13id, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }


            var v = new Models.p14RecordViewModel();
            
            if (pid > 0)
            {
                v.Rec = Factory.p14MasterOperBL.Load(pid);

                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                v.RecP13 = Factory.p13MasterTpvBL.Load(v.Rec.p13ID);


            }
            else
            {
                v.Rec = new BO.p14MasterOper();
                v.RecP13 = Factory.p13MasterTpvBL.Load(p13id);
                v.Rec.p13ID = p13id;
                v.Rec.entity = "p14";
                

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.AllowArchive = false;
            if (isclone)
            {
                v.Toolbar.MakeClone();

            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p14RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p14MasterOper c = new BO.p14MasterOper();
                if (v.Rec.pid > 0) c = Factory.p14MasterOperBL.Load(v.Rec.pid);

                c.p13ID = v.Rec.p13ID;
                c.p19ID = v.Rec.p19ID;
                c.p18ID = v.Rec.p18ID;
                c.p14RowNum = v.Rec.p14RowNum;
                c.p14OperParam = v.Rec.p14OperParam;
                c.p14OperNum = v.Rec.p14OperNum;
                c.p14UnitsCount = v.Rec.p14UnitsCount;
                c.p14Name = v.Rec.p14Name;
                c.p14DurationOper = v.Rec.p14DurationOper;
                c.p14DurationPreOper = v.Rec.p14DurationPreOper;
                c.p14DurationPostOper = v.Rec.p14DurationPostOper;

               

                v.Rec.pid = Factory.p14MasterOperBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            v.RecP13 = Factory.p13MasterTpvBL.Load(v.Rec.p13ID);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.AllowArchive = false;
            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}