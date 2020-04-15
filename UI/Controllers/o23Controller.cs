using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    public class o23Controller : BaseController
{
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.o23RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o23DocBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.o23Doc();
                v.Rec.entity = "o23";
            }
            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbxB02");
            v.ComboB02ID.Param1 = "o23";
            v.ComboO12ID = new MyComboViewModel("o12", v.Rec.o12ID.ToString(), v.Rec.o12Name, "cbxO12");
            v.ComboO12ID.Param1 = "o23";
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o23RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o23Doc c = new BO.o23Doc();
                if (v.Rec.pid > 0) c = Factory.o23DocBL.Load(v.Rec.pid);

                c.o23Code = v.Rec.o23Code;
                c.o23Name = v.Rec.o23Name;
                c.o23Entity = v.Rec.o23Entity;
                c.o23Memo = v.Rec.o23Memo;
                c.o23Date = v.Rec.o23Date;
                c.b02ID = BO.BAS.InInt(v.ComboB02ID.SelectedValue);
                c.o12ID = BO.BAS.InInt(v.ComboO12ID.SelectedValue);                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o23DocBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToActionPermanent("Index", "TheGrid", new { pid = v.Rec.pid, entity = "o23" });
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);
                }
            }
            
            
            v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbxB02");
            v.ComboB02ID.Param1 = "o23";
            v.ComboO12ID = new MyComboViewModel("o12", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbxO12");
            v.ComboO12ID.Param1 = "o23";
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Notify("Záznam zatím nebyl uložen.", "warning");
            return View(v);
        }
    }
}