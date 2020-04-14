using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    public class b02Controller : BaseController
    {

        //WORKFLOW STAV
        public IActionResult Record(int pid, bool isclone)
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
                v.Rec.entity = "b02";
            }
          
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.b02RecordViewModel v)
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
                    return RedirectToActionPermanent("Index", "TheGrid", new { pid = v.Rec.pid, entity = "b02" });
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);
                }
               
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Notify("Záznam zatím nebyl uložen.", "warning");
            return View(v);
        }


        

        

    }
}