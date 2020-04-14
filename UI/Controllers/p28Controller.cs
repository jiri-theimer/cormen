using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p28Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p28PreviewViewModel();

            v.Rec = Factory.p28CompanyBL.Load(pid);
            if (v.Rec == null)
            {
                return this.StopPage(false, "Hledaný záznam neexistuje!");
            }
            else
            {
                return View(v);
            }
            

        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.p28RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p28CompanyBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.p28Company();
                v.Rec.entity = "p28";

            }

            
            v.Country1 = new MyAutoCompleteViewModel(328, v.Rec.p28Country1,"Stát");
            v.Country1.DropDownWidth = "300px";
            v.Country2 = new MyAutoCompleteViewModel(328, v.Rec.p28Country2, "Stát", "pop2");
            v.Country2.DropDownWidth = "300px";
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) { v.Toolbar.MakeClone(); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p28RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p28Company c = new BO.p28Company();
                if (v.Rec.pid > 0) c = Factory.p28CompanyBL.Load(v.Rec.pid);

                c.p28Code = v.Rec.p28Code;
                c.p28Name = v.Rec.p28Name;
                c.p28ShortName = v.Rec.p28ShortName;
                c.p28RegID = v.Rec.p28RegID;
                c.p28VatID = v.Rec.p28VatID;
                c.p28Street1 = v.Rec.p28Street1;
                c.p28City1 = v.Rec.p28City1;
                c.p28PostCode1 = v.Rec.p28PostCode1;
                c.p28Country1 = v.Country1.SelectedText;
                c.p28Street2 = v.Rec.p28Street2;
                c.p28City2 = v.Rec.p28City2;
                c.p28PostCode2 = v.Rec.p28PostCode2;
                c.p28Country2 = v.Country2.SelectedText;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p28CompanyBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p28" });                    

                }
                else
                {
                    return this.StopPage(false, "Chyba");
                }
            }
            else
            {
                v.Toolbar = new MyToolbarViewModel(v.Rec);                
                v.Country1 = new MyAutoCompleteViewModel(328, v.Country1.SelectedText,"Stát");
                v.Country1.DropDownWidth = "300px";
                v.Country2 = new MyAutoCompleteViewModel(328, v.Country1.SelectedText, "Stát","pop2");
                v.Country2.DropDownWidth = "300px";
                return View(v);
            }


        }
    }
}