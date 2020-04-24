using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    
    public class p10Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p10PreviewViewModel();
            v.Rec = Factory.p10MasterProductBL.Load(pid);
            if (v.Rec == null) return RecNotFound(v);
            return View(v);

        }
        public IActionResult Record(int pid, bool isclone)
        {            
            var v = new Models.p10RecordViewModel();
            
            if (pid > 0)
            {
                v.Rec = Factory.p10MasterProductBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p10MasterProduct();
                v.Rec.entity = "p10";
            }
            InitToolbar_and_Combos(v);
           
            if (isclone) { v.Toolbar.MakeClone(); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p10RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p10MasterProduct c = new BO.p10MasterProduct();
                if (v.Rec.pid > 0) c = Factory.p10MasterProductBL.Load(v.Rec.pid);

                c.p10Code = v.Rec.p10Code;
                c.p10Name = v.Rec.p10Name;
                c.p10Memo = v.Rec.p10Memo;
                c.b02ID = v.Rec.b02ID;
                c.p13ID = v.Rec.p13ID;
                c.o12ID = v.Rec.o12ID;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p10MasterProductBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToActionPermanent("Index","TheGrid", new { pid = v.Rec.pid, entity = "p10" });
                    
                }
                
            }

            InitToolbar_and_Combos(v);
            this.Notify_RecNotSaved();
            return View(v);


        }

        private void InitToolbar_and_Combos(p10RecordViewModel v)
        {            
            v.ComboP13ID = new TheComboViewModel() { Entity = "p13MasterTpv", CallerIDValue = "Rec_p13ID", CallerIDText = "Rec_p13Name", SelectedValue = v.Rec.p13ID.ToString(), SelectedText = v.Rec.p13Name };
            v.ComboB02ID = new TheComboViewModel() { Entity = "b02Status", CallerIDValue = "Rec_b02ID", CallerIDText = "Rec_b02Name", SelectedValue = v.Rec.b02ID.ToString(), SelectedText = v.Rec.b02Name, Param1 = "p10" };
            v.ComboO12ID = new TheComboViewModel() { Entity = "o12Category", CallerIDValue = "Rec_o12ID", CallerIDText = "Rec_o12Name", SelectedValue = v.Rec.o12ID.ToString(), SelectedText = v.Rec.o12Name, Param1 = "p10" };

            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }
    }
}