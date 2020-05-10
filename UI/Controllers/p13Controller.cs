
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p13Controller : BaseController
    {
        

        public IActionResult Index(int pid)
        {
            var v = new Models.p13PreviewViewModel();
           

            v.Rec = Factory.p13MasterTpvBL.Load(pid);     
           
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
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p13RecordViewModel();
           
            if (pid > 0)
            {
                v.Rec = Factory.p13MasterTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("p14MasterOper");
                mq.p13id = v.Rec.pid;
                v.lisP14 = Factory.p14MasterOperBL.GetList(mq).ToList();
                for (var i = 0; i < v.lisP14.Count(); i++)
                {
                    v.lisP14[i].TempRecGuid = BO.BAS.GetGuid();
                    v.lisP14[i].TempRecDisplay = "table-row";
                }
            }
            else
            {
                v.Rec = new BO.p13MasterTpv();
                v.Rec.entity = "p13";
                v.lisP14 = new List<BO.p14MasterOper>();
                v.lisP14.Add(new BO.p14MasterOper() { TempRecDisplay = "table-row", TempRecGuid = BO.BAS.GetGuid() });
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) {
                v.Toolbar.MakeClone();
                for (var i = 0; i < v.lisP14.Count(); i++)
                {
                    v.lisP14[i].p14ID = 0;
                    v.lisP14[i].pid = 0;
                }
            }
            

            

            return View(v);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p13RecordViewModel v,string rec_oper,string rec_guid)
        {
            if (rec_oper != null)
            {
                if (rec_oper == "add")
                {
                    if (v.lisP14 == null) v.lisP14 = new List<BO.p14MasterOper>();
                    v.lisP14.Add(new BO.p14MasterOper() {TempRecDisplay="table-row", TempRecGuid = BO.BAS.GetGuid(),p14RowNum=v.lisP14.Count()+1 });
                    
                }
                if (rec_oper == "postback")
                {
                    //pouze postback
                    

                }

                v.Toolbar = new MyToolbarViewModel(v.Rec);

                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p13MasterTpv c = new BO.p13MasterTpv();
                if (v.Rec.pid > 0) c = Factory.p13MasterTpvBL.Load(v.Rec.pid);

                c.p25ID = v.Rec.p25ID;
                c.p13Code = v.Rec.p13Code;
                c.p13Name = v.Rec.p13Name;
                c.p13Memo = v.Rec.p13Memo;

                //c.ValidUntil = v.Toolbar.GetValidUntil(c);
                //c.ValidFrom = v.Toolbar.GetValidFrom(c);
                int x = 1;
                foreach(var row in v.lisP14.OrderBy(p => p.p14RowNum))
                {
                    row.p14RowNum = x;  //narovnat rownum na postupku od jedničky
                    x += 1;
                }
               


                v.Rec.pid = Factory.p13MasterTpvBL.Save(c, v.lisP14);
                
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
