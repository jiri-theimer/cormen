using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p21Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p21PreviewViewModel();
            v.Rec = Factory.p21LicenseBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                v.IsPossible2UpdateClientProducts = this.TestIfUserEditor(true, false);
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p21RecordViewModel();
            
            if (pid > 0)
            {
                v.Rec = Factory.p21LicenseBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                var mq = new BO.myQuery("p10");
                mq.p21id = pid;                
                v.p10IDs = string.Join(",", Factory.p10MasterProductBL.GetList(mq).Select(p => p.pid));
            }
            else
            {
                v.Rec = new BO.p21License();
                v.Rec.ValidFrom = DateTime.Today;
                v.Rec.ValidUntil = new DateTime(3000, 1, 1);
                v.Rec.p21Code = Factory.CBL.EstimateRecordCode("p21");
                v.Rec.entity = "p21";
            }

            RefreshState(v);

            
                        
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p21Code = Factory.CBL.EstimateRecordCode("p21"); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p21RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p21License c = new BO.p21License();
                if (v.Rec.pid > 0) c = Factory.p21LicenseBL.Load(v.Rec.pid);

                c.p21Code = v.Rec.p21Code;
                c.p21Name = v.Rec.p21Name;
                c.p21Memo = v.Rec.p21Memo;
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;
                c.o12ID = v.Rec.o12ID;
                c.p21Price = v.Rec.p21Price;

                c.ValidFrom = v.Rec.ValidFrom;
                c.ValidUntil = v.Rec.ValidUntil;


                v.Rec.pid = Factory.p21LicenseBL.Save(c,BO.BAS.ConvertString2ListInt(v.p10IDs));
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


        


        public BO.Result CreateClientProducts(int p21id)
        {
            
            return Factory.p21LicenseBL.CreateClientProducts(p21id);
            
            
        }

        private void RefreshState(p21RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
           
                
            
        }



    }
}