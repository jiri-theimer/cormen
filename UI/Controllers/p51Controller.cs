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
                v.OrderItems = Factory.p52OrderItemBL.GetList(pid);
                return View(v);
            }

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
                v.Rec.p51Date = DateTime.Now;
                v.Rec.p51DateDelivery = DateTime.Today.AddDays(10).AddSeconds(-1);
                v.NewItems = new List<BO.p52OrderItem>();
                v.NewItems.Add(new BO.p52OrderItem());
            }

            RefreshState(v);


            if (isclone)
            {                
                v.NewItems = new List<BO.p52OrderItem>();
                foreach(var c in Factory.p52OrderItemBL.GetList(v.Rec.pid))
                {
                    v.NewItems.Add(new BO.p52OrderItem() { p11ID = c.p11ID, p11Name = c.p11Name, p52UnitsCount = c.p52UnitsCount });
                }
                v.Toolbar.MakeClone();
                v.Rec.p51Code = Factory.CBL.EstimateRecordCode("p51");
            }
                


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p51RecordViewModel v, string rec_oper)
        {
            if (rec_oper == "newitem")
            {
                if (v.NewItems == null) v.NewItems = new List<BO.p52OrderItem>();
                v.NewItems.Add(new BO.p52OrderItem() { p51ID=v.Rec.p51ID});
                
                v.Toolbar = new MyToolbarViewModel(v.Rec);
                return View(v);
            }
            

           
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
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p51CodeByClient = v.Rec.p51CodeByClient;
                //c.p26ID = v.Rec.p26ID;
                c.p51Date = v.Rec.p51Date;
                c.p51DateDelivery = v.Rec.p51DateDelivery;


                v.Rec.pid = Factory.p51OrderBL.Save(c,v.NewItems);
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
            
            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }


        


    }
}