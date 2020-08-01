using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class b03Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.b03RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.b03StatusGroupBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.b03StatusGroup();
                v.Rec.entity = "b03";
            }
            var mq = new BO.myQuery("b02");
            if (v.Rec.pid > 0)
            {
                mq.b03id = v.Rec.pid;
                v.SelectedB02IDs = Factory.b02StatusBL.GetList(mq).Select(p => p.pid).ToList();
            }

            mq = new BO.myQuery("b02");
            v.lisB02 = Factory.b02StatusBL.GetList(mq).Where(p=>p.b02Entity=="p41" || p.b02Entity=="p51").OrderBy(p=>p.b02Entity).ThenBy(p=>p.b02Ordinary).ToList();

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.b03RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.b03StatusGroup c = new BO.b03StatusGroup();
                if (v.Rec.pid > 0) c = Factory.b03StatusGroupBL.Load(v.Rec.pid);


                c.b03Name = v.Rec.b03Name;             
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.b03StatusGroupBL.Save(c,v.SelectedB02IDs);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            var mq = new BO.myQuery("b02");
            v.lisB02 = Factory.b02StatusBL.GetList(mq).Where(p => p.b02Entity == "p41" || p.b02Entity == "p51").OrderBy(p => p.b02Entity).ThenBy(p => p.b02Ordinary).ToList();

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}