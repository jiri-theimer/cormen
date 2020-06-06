using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p18Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }

            
            var v = new Models.p18RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p18OperCodeBL.Load(pid);

                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p18", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;



            }
            else
            {
                v.Rec = new BO.p18OperCode();
                v.Rec.entity = "p18";
                v.Rec.p18Code = Factory.CBL.EstimateRecordCode("p18");

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p18Code = Factory.CBL.EstimateRecordCode("p18"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p18RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p18OperCode c = new BO.p18OperCode();
                if (v.Rec.pid > 0) c = Factory.p18OperCodeBL.Load(v.Rec.pid);

                c.p18Code = v.Rec.p18Code;
                c.p18Name = v.Rec.p18Name;
                c.p18Memo = v.Rec.p18Memo;
                c.p25ID = v.Rec.p25ID;
                c.p19ID = v.Rec.p19ID;
                c.p18Flag = v.Rec.p18Flag;
                c.p18UnitsCount = v.Rec.p18UnitsCount;
                c.p18DurationPreOper = v.Rec.p18DurationPreOper;
                c.p18DurationOper = v.Rec.p18DurationOper;
                c.p18DurationPostOper = v.Rec.p18DurationPostOper;

                c.p18Lang1 = v.Rec.p18Lang1;
                c.p18Lang2 = v.Rec.p18Lang2;
                c.p18Lang3 = v.Rec.p18Lang3;
                c.p18Lang4 = v.Rec.p18Lang4;

                v.Rec.pid = Factory.p18OperCodeBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p18", v.Rec.pid, v.TagPids);
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