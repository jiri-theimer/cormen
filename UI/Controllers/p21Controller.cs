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
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
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
                v.Rec.entity = "p21";
            }

            v.TheComboP28ID = new TheComboViewModel() { Entity = "p28Company",CallerIDValue= "Rec_p28ID", CallerIDText="Rec_p28Name",SelectedValue= v.Rec.p28ID.ToString(),SelectedText= v.Rec.p28Name };
            v.TheComboB02 = new TheComboViewModel() { Entity = "b02Status", CallerIDValue = "Rec_b02ID", CallerIDText = "Rec_b02Name", SelectedValue = v.Rec.b02ID.ToString(), SelectedText = v.Rec.b02Name };


            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbx2");
            v.ComboB02ID.Param1 = "p21";
            v.ComboSelectP10ID = new MyComboViewModel("p10","", "Přidat do licence master produkt...", "cbxProduct");
            v.ComboSelectP10ID.OnChange_Event = "handle_append_product";
            
            
            

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }


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
               
                c.ValidFrom = v.Rec.ValidFrom;
                var d = new DateTime(3000,1,1);
                if (v.Rec.ValidUntil != null) d = Convert.ToDateTime((v.Rec.ValidUntil));
                c.ValidUntil = d.AddDays(1).AddMinutes(-1);

                v.Rec.pid = Factory.p21LicenseBL.Save(c,BO.BAS.ConvertString2ListInt(v.p10IDs));
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p21" });
                }
                          
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
           
            v.TheComboP28ID = new TheComboViewModel() { Entity = "p28Company", CallerIDValue = "Rec_p28ID", CallerIDText = "Rec_p28Name",SelectedText=v.Rec.p28Name,SelectedValue=v.Rec.p28ID.ToString() };
            v.TheComboB02 = new TheComboViewModel() { Entity = "b02Status", CallerIDValue = "Rec_b02ID", CallerIDText = "Rec_b02Name", SelectedText = v.Rec.b02Name, SelectedValue = v.Rec.b02ID.ToString() };

            v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbx2");
            v.ComboB02ID.Param1 = "p21";
            v.ComboSelectP10ID = new MyComboViewModel("p10", "", "Přidat Master produkt", "cbx3");
            v.ComboSelectP10ID.OnChange_Event = "handle_append_product";
            this.Notify_RecNotSaved();
            return View(v);


        }

       
        public BO.Result CreateClientProducts(int p21id)
        {
            
            return Factory.p21LicenseBL.CreateClientProducts(p21id);
            
            
        }
        
        
        
    }
}