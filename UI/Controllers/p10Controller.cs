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
            if (v.Rec == null) v.Notify("Hledaný záznam neexistuje!");
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
                    v.Notify("Hledaný záznam neexistuje!");
                    return View(v);
                }

            }
            else
            {
                v.Rec = new BO.p10MasterProduct();
                v.Rec.entity = "p10";
            }

            v.ComboP13ID = new MyComboViewModel("p13", v.Rec.p13ID.ToString(), v.Rec.p13Name, "cbx1");
            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbx2");
            v.ComboB02ID.Param1 = "p10";
            v.ComboO12ID = new MyComboViewModel("o12", v.Rec.o12ID.ToString(), v.Rec.o12Name, "cbx3");
            v.ComboO12ID.Param1 = "p10";

            v.Toolbar = new MyToolbarViewModel(v.Rec);
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
                c.b02ID = BO.BAS.InInt(v.ComboB02ID.SelectedValue);
                c.p13ID = BO.BAS.InInt(v.ComboP13ID.SelectedValue);
                c.o12ID = BO.BAS.InInt(v.ComboO12ID.SelectedValue);

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p10MasterProductBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToActionPermanent("Index","TheGrid", new { pid = v.Rec.pid, entity = "p10" });
                    
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);                    
                }
            }
          
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.ComboP13ID = new MyComboViewModel("p13", v.ComboP13ID.SelectedValue, v.ComboP13ID.SelectedText, "cbx1");
            v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbx2");
            v.ComboB02ID.Param1 = "p10";
            v.ComboO12ID = new MyComboViewModel("o12", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbx3");
            v.ComboO12ID.Param1 = "p10";
            v.Notify("Záznam zatím nebyl uložen.","warning");
            return View(v);


        }
    }
}