using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p41Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p41PreviewViewModel();
            v.Rec = Factory.p41TaskBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                return View(v);
            }
            

        }

        public IActionResult Create(int p52id)
        {
            var v = new p41CreateViewModel();
            v.lisTasks = new List<BO.p41Task>();


            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p41RecordViewModel();
            v.Rec = Factory.p41TaskBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            RefreshState(v);


            if (isclone)
            {
                
                v.Toolbar.MakeClone();
                v.Rec.p41Code = Factory.CBL.EstimateRecordCode("p41");
            }



            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p41RecordViewModel v)
        {
           
            if (ModelState.IsValid)
            {
                BO.p41Task c = new BO.p41Task();
                if (v.Rec.pid > 0) c = Factory.p41TaskBL.Load(v.Rec.pid);

                c.p52ID = v.Rec.p52ID;
                c.b02ID = v.Rec.b02ID;
                c.p27ID = v.Rec.p27ID;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p41IsDraft = v.Rec.p41IsDraft;
                c.p41Code = v.Rec.p41Code;
                c.p41Name = v.Rec.p41Name;
                c.p41Memo = v.Rec.p41Memo;
                c.p41StockCode = v.Rec.p41StockCode;

                c.p41PlanStart = v.Rec.p41PlanStart;
                c.p41PlanEnd = v.Rec.p41PlanEnd;
                c.p41PlanUnitsCount = v.Rec.p41PlanUnitsCount;


                v.Rec.pid = Factory.p41TaskBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            RefreshState(v);

            this.Notify_RecNotSaved();
            return View(v);

        }


        private void RefreshState(p41RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
            v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }

    }
}