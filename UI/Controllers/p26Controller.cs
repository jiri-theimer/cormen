using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p26Controller : BaseController
    {
       

        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.p26RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p26MszBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.p26Msz();

            }

            v.ComboP28ID = new MyComboViewModel("p28", v.Rec.p28ID.ToString(), v.Rec.p28Name,"cbx1");
            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbx2");
            v.ComboB02ID.Param1 = "p26";

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p26RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p26Msz c = new BO.p26Msz();
                if (v.Rec.pid > 0) c = Factory.p26MszBL.Load(v.Rec.pid);

                c.p26Code = v.Rec.p26Code;
                c.p26Name = v.Rec.p26Name;
                c.p26Memo = v.Rec.p26Memo;
                c.b02ID = BO.BAS.InInt(v.ComboB02ID.SelectedValue);
                c.p28ID = BO.BAS.InInt(v.ComboP28ID.SelectedValue);

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p26MszBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p26" });
                }
                else
                {
                    return this.StopPage(false, "Chyba");
                }
            }
            else
            {
                v.Toolbar = new MyToolbarViewModel(v.Rec);
                v.ComboP28ID = new MyComboViewModel("p28", v.ComboP28ID.SelectedValue, v.ComboP28ID.SelectedText, "cbx1");
                v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbx2");
                v.ComboB02ID.Param1 = "p26";

                return View(v);
            }


        }
    }
}