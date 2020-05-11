using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p51Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p51PreviewViewModel();
            v.Rec = Factory.p51OrderBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p51RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p51OrderBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p51Order();
                v.Rec.entity = "p51";
                v.Rec.p51Code = Factory.CBL.EstimateRecordCode("p51");
                v.Rec.p51Date = DateTime.Today;
                v.Rec.p51DateDelivery = DateTime.Today.AddDays(10).AddSeconds(-1);
            }

            RefreshState(v);


            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p51Code = Factory.CBL.EstimateRecordCode("p51"); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p51RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p51Order c = new BO.p51Order();
                if (v.Rec.pid > 0) c = Factory.p51OrderBL.Load(v.Rec.pid);

                c.p51IsDraft = v.Rec.p51IsDraft;
                c.p51Code = v.Rec.p51Code;
                c.p51Name = v.Rec.p51Name;
                c.p51Memo = v.Rec.p51Memo;
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;
                c.p51CodeByClient = v.Rec.p51CodeByClient;
                c.p26ID = v.Rec.p26ID;
                c.p51Date = v.Rec.p51Date;
                c.p51DateDelivery = v.Rec.p51DateDelivery;


                v.Rec.pid = Factory.p51OrderBL.Save(c);
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

        private void RefreshState(p51RecordViewModel v)
        {
            v.OrderItems = Factory.p51OrderBL.GetList_OrderItems(v.Rec.pid);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }


        //položka objednávky
        public IActionResult Record_Item(int pid,int p51id)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p52RecordViewModelcs();
            if (pid > 0)
            {
                v.Rec = Factory.p51OrderBL.LoadOrderItem(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p52OrderItem();
                v.Rec.entity = "p52";
                v.Rec.p51ID = p51id;
                
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec) { IsClone = false, IsToArchive = false,IsRefresh=false };

            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record_Item(Models.p52RecordViewModelcs v)
        {
            if (ModelState.IsValid)
            {
                BO.p52OrderItem c = new BO.p52OrderItem();
                if (v.Rec.pid > 0) c = Factory.p51OrderBL.LoadOrderItem(v.Rec.pid);
                
                c.p52Code = v.Rec.p52Code;
                c.p51ID = v.Rec.p51ID;
                c.p11ID = v.Rec.p11ID;
                c.p52UnitsCount = v.Rec.p52UnitsCount;


                v.Rec.pid = Factory.p51OrderBL.SaveOrderItem(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Record", new { pid = v.Rec.p51ID });
                    //var v2 = new Models.p51RecordViewModel();
                    //v2.Rec = Factory.p51OrderBL.Load(v.Rec.p51ID);
                    //return View(v2);
                }


            }

            v.Toolbar = new MyToolbarViewModel(v.Rec) { IsClone = false, IsToArchive = false, IsRefresh=false };
            

            this.Notify_RecNotSaved();
            return View(v);

        }


    }
}