using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class o12Controller : BaseController
    {
        ///KATEGORIE
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.o12RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o12CategoryBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.o12Category();
                v.Rec.entity = "o12";
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o12RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o12Category c = new BO.o12Category();
                if (v.Rec.pid > 0) c = Factory.o12CategoryBL.Load(v.Rec.pid);

                c.o12Code = v.Rec.o12Code;
                c.o12Name = v.Rec.o12Name;
                c.o12Entity = v.Rec.o12Entity;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o12CategoryBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToActionPermanent("Index", "TheGrid", new { pid = v.Rec.pid, entity = "o12" });
                }
                else
                {
                    return this.StopPage(false, "Chyba");
                }
            }
            else
            {
                v.Toolbar = new MyToolbarViewModel(v.Rec);
                

                return View(v);
            }
        }
    }
}