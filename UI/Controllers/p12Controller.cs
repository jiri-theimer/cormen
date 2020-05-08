using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p12Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p12PreviewViewModel();


            v.Rec = Factory.p12ClientTpvBL.Load(pid);

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
            if (!Factory.CurrentUser.TestPermission(BO.UserPermFlag.ClientAdmin))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p12RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p12ClientTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
                var mq = new BO.myQuery("p15ClientOper");
                mq.p12id = v.Rec.pid;
                v.lisP15 = Factory.p15ClientOperBL.GetList(mq).ToList();
                for (var i = 0; i < v.lisP15.Count(); i++)
                {
                    v.lisP15[i].TempRecGuid = BO.BAS.GetGuid();
                    v.lisP15[i].TempRecDisplay = "table-row";
                }

            }
            else
            {
                
                v.Rec = new BO.p12ClientTpv();
                v.Rec.entity = "p12";
                v.lisP15 = new List<BO.p15ClientOper>();
                v.lisP15.Add(new BO.p15ClientOper() { TempRecDisplay = "table-row", TempRecGuid = BO.BAS.GetGuid() });
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.Toolbar.MakeClone();
                for (var i = 0; i < v.lisP15.Count(); i++)
                {
                    v.lisP15[i].p15ID = 0;
                    v.lisP15[i].pid = 0;
                }
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p12RecordViewModel v, string rec_oper, string rec_guid)
        {
            if (rec_oper != null)
            {
                if (rec_oper == "add")
                {
                    v.lisP15.Add(new BO.p15ClientOper() { TempRecDisplay = "table-row", TempRecGuid = BO.BAS.GetGuid(), p15RowNum = v.lisP15.Count() + 1 });

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
                BO.p12ClientTpv c = new BO.p12ClientTpv();
                if (v.Rec.pid > 0) c = Factory.p12ClientTpvBL.Load(v.Rec.pid);

                c.p12Code = v.Rec.p12Code;
                c.p12Name = v.Rec.p12Name;
                c.p12Memo = v.Rec.p12Memo;               
               
                v.Rec.pid = Factory.p12ClientTpvBL.Save(c, v.lisP15);
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