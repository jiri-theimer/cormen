using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p44Controller : BaseController
    {
        public IActionResult Record(int pid, int p41id, bool isclone)
        {
            if (!this.TestIfUserEditor(false, true))
            {
                return this.StopPageCreateEdit(true);
            }


            var v = new Models.p44RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p44TaskOperPlanBL.Load(pid);

                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
               
                v.RecP18 = Factory.p18OperCodeBL.Load(v.Rec.p18ID);
                if (v.RecP18.p18Flag == 1)
                {
                    return this.StopPage(true, "Technologické operace nelze v plánu upravovat.");
                }
               
                if (isclone)
                {
                    v.Rec.p44RowNum += 1;
                }

            }
            else
            {
                v.Rec = new BO.p44TaskOperPlan();
                v.RecP18 = new BO.p18OperCode();
               
                v.Rec.p41ID = p41id;
                v.Rec.entity = "p44";


            }

            RefreshState(ref v);

            if (isclone)
            {
                v.Toolbar.MakeClone();

            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p44RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p44TaskOperPlan c = new BO.p44TaskOperPlan();
                if (v.Rec.pid > 0) c = Factory.p44TaskOperPlanBL.Load(v.Rec.pid);

                c.p41ID = v.Rec.p41ID;
                c.p19ID = v.Rec.p19ID;
                c.p18ID = v.Rec.p18ID;
                c.p44RowNum = v.Rec.p44RowNum;
                c.p44OperParam = v.Rec.p44OperParam;
                c.p44OperNum = v.Rec.p44OperNum;
                c.p44MaterialUnitsCount = v.Rec.p44MaterialUnitsCount;

                c.p44Start = v.Rec.p44Start;                
                c.p44TotalDurationOperMin = v.Rec.p44TotalDurationOperMin;
             

                v.Rec.pid = Factory.p44TaskOperPlanBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }
            RefreshState(ref v);
           
            if (v.Rec.p18ID > 0)
            {
                v.RecP18 = Factory.p18OperCodeBL.Load(v.Rec.p18ID);
            }
            else
            {
                v.RecP18 = new BO.p18OperCode();
            }
           
            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(ref p44RecordViewModel v)
        {
            v.RecP41 = Factory.p41TaskBL.Load(v.Rec.p41ID);
            v.RecP27 = Factory.p27MszUnitBL.Load(v.RecP41.p27ID);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.AllowArchive = false;
        }


        public BO.p44TaskOperPlan inhale_p44_record(int p18id,int p41id)
        {
            var ret = new BO.p44TaskOperPlan() { p41ID = p41id, p18ID = p18id };
            var recP18= Factory.p18OperCodeBL.Load(p18id);
            var recP41 = Factory.p41TaskBL.Load(p41id);
            ret.p44MaterialUnitsCount = recP18.p18UnitsCount;
            ret.p44TotalDurationOperMin = recP18.p18DurationPreOper + recP18.p18DurationPostOper + ret.p44MaterialUnitsCount*recP41.p41PlanUnitsCount*recP18.p18DurationOper;
            ret.p19ID = recP18.p19ID;
            ret.Material = recP18.p19Name;
            


            return ret;
        }
    }
}