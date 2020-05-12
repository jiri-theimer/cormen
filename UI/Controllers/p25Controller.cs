using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p25Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p25RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p25MszTypeBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p25MszType();
                v.Rec.p25Code = Factory.CBL.EstimateRecordCode("p25");
                v.Rec.entity = "p25";

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) {
                v.Toolbar.MakeClone();
                v.Rec.p25Code = Factory.CBL.EstimateRecordCode("p25");
            }
            else
            {
                if (v.Rec.pid > 0)
                {
                    var mq = new BO.myQuery("p18OperCode");
                    mq.p25id = v.Rec.pid;
                    v.lisP18 = Factory.p18OperCodeBL.GetList(mq);
                }
                
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p25RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p25MszType c = new BO.p25MszType();
                if (v.Rec.pid > 0) c = Factory.p25MszTypeBL.Load(v.Rec.pid);

                c.p25Code = v.Rec.p25Code;
                c.p25Name = v.Rec.p25Name;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p25MszTypeBL.Save(c);
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

        public BO.Result Copy_p18OperCode(int p25id_dest,int p25id_source)
        {
            bool b = Factory.p25MszTypeBL.Copy_p18OperCode(p25id_dest, p25id_source);
            return new BO.Result(b);
        }
    }
}