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
                v.Rec.ValidFrom = DateTime.Today;
                v.Rec.ValidUntil = new DateTime(3000, 1, 1);
                v.Rec.entity = "p21";
            }

            RefreshState(v);

            
                        
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
                c.b02ID = v.ComboB02ID.SelectedValue;
                c.p28ID = v.ComboP28ID.SelectedValue;

                c.ValidFrom = v.PlatnostOd.SelectedDate;
                c.ValidUntil = v.PlatnostDo.SelectedDate;


                v.Rec.pid = Factory.p21LicenseBL.Save(c,BO.BAS.ConvertString2ListInt(v.p10IDs));
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p21" });
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
            
            if (Request.Method == "GET")    //myCombo má vstupní parametry modelu v hidden polích a proto se v POST vše dostane na server
            {
                v.ComboP28ID = new MyComboViewModel() { Entity = "p28Company", SelectedText = v.Rec.p28Name, SelectedValue = v.Rec.p28ID };
                v.ComboB02ID = new MyComboViewModel() { Entity = "b02Status", SelectedText = v.Rec.b02Name, SelectedValue = v.Rec.b02ID, Param1 = "p21" };

                v.ComboSelectP10ID = new MyComboViewModel() { Entity = "p10MasterProduct", PlaceHolder = "Přidat do licence Master produkt..." };

               
                v.PlatnostOd = new MyDateViewModel() {SelectedDate = v.Rec.ValidFrom };
                v.PlatnostDo = new MyDateViewModel() { SelectedDate = v.Rec.ValidUntil };
            }
                
            v.ComboSelectP10ID.Event_After_ChangeValue = "handle_append_product";
            v.ComboB02ID.ViewFlag = 2;
        }



    }
}