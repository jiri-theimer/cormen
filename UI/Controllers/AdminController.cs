using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    public class AdminController : BaseController
    {

        //WORKFLOW STAV
        public IActionResult B02(int pid, bool isclone)
        {
            var v = new Models.b02RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.b02StatusBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.b02Status();
            }
          
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.ControllorName = "B02";
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult B02(Models.b02RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.b02Status c = new BO.b02Status();
                if (v.Rec.pid > 0) c = Factory.b02StatusBL.Load(v.Rec.pid);

                c.b02Code = v.Rec.b02Code;
                c.b02Name = v.Rec.b02Name;
                c.b02Entity = v.Rec.b02Entity;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.b02StatusBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Grid", new { pid = v.Rec.pid });
                }
                else
                {
                    return this.StopPage(false, "Chyba");
                }
            }
            else
            {
                v.Toolbar = new MyToolbarViewModel(v.Rec);
                v.Toolbar.ControllorName = "B02";

                return View(v);
            }
        }


        ///KATEGORIE
        public IActionResult O12(int pid, bool isclone)
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

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.ControllorName = "O12";
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult O12(Models.o12RecordViewModel v)
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
                    return RedirectToAction("Grid", new { pid = v.Rec.pid });
                }
                else
                {
                    return this.StopPage(false, "Chyba");
                }
            }
            else
            {
                v.Toolbar = new MyToolbarViewModel(v.Rec);
                v.Toolbar.ControllorName = "O12";

                return View(v);
            }
        }

        

    }
}