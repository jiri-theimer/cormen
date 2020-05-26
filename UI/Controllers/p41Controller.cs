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
            v.Tasks = new List<BO.p41Task>();
            v.lisP27 = new List<BO.p27MszUnit>();

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.p41CreateViewModel v, string rec_oper,int p27id)
        {

            if (rec_oper == "p51id_change")
            {
                v.p52ID = 0;
                v.p52Code = "";
            }
            if (v.p51ID > 0)
            {
                v.RecP51 = Factory.p51OrderBL.Load(v.p51ID);
            }
            if (v.p52ID > 0)
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.p52ID);
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
            }
            if (v.p26ID > 0)
            {
                v.RecP26 = Factory.p26MszBL.Load(v.p26ID);
                var mq = new BO.myQuery("p27MszUnit");
                mq.p26id = v.p26ID;
                v.lisP27 = Factory.p27MszUnitBL.GetList(mq).ToList();
            }
            else
            {
                v.lisP27 = new List<BO.p27MszUnit>();
            }
            if (rec_oper == "newitem")
            {
                if (v.Tasks == null) v.Tasks = new List<BO.p41Task>();
                var c = new BO.p41Task();
                c.p41PlanStart = DateTime.Now.AddHours(1);
                c.p41PlanEnd = DateTime.Now.AddHours(2);
                if (p27id > 0)
                {
                    c.p27ID = p27id;
                    c.p27Name = Factory.p27MszUnitBL.Load(p27id).p27Name;
                }
                if (v.RecP52 != null)
                {
                    c.p52ID = v.RecP52.pid;
                    c.p52Code = v.RecP52.p52Code;
                    //c.p41Name = v.RecP52.p11Name + " [" + v.RecP52.p11Code + "]";
                }
                v.Tasks.Add(c);

                return View(v);
            }
            if (rec_oper == "clear")
            {
                v.Tasks = new List<BO.p41Task>();
            }
            if (rec_oper == "delete")
            {
                //došlo k virtuálnímu odstranění řádku zakázky - pouze postback
            }
            if (rec_oper == "postback")     //pouze postback
            {

            }
            if (v.Tasks == null)
            {
                v.Tasks = new List<BO.p41Task>();
            }

            if (ModelState.IsValid)
            {

                if (rec_oper == "save")
                {
                    int x = Factory.p41TaskBL.SaveBatch(v.Tasks);
                    if (x > 0)
                    {

                        v.SetJavascript_CallOnLoad(0, "p41");
                        return View(v);
                    }
                }



            }

            
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
            RefreshState_Record(v);


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

            RefreshState_Record(v);

            this.Notify_RecNotSaved();
            return View(v);

        }


        private void RefreshState_Record(p41RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
            v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);

        }

    }
}