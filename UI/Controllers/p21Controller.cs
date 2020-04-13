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
        public IActionResult Preview(int pid)
        {
            var v = new Models.p21PreviewViewModel();
            v.Rec = Factory.p21LicenseBL.Load(pid);
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
            var v = new Models.p21RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p21LicenseBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.p21License();

            }

            v.ComboP28ID = new MyComboViewModel("p28", v.Rec.p28ID.ToString(), v.Rec.p28Name, "cbx1");
            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbx2");
            v.ComboB02ID.Param1 = "p21";
            v.ComboSelectP10ID = new MyComboViewModel("p10","", "Přidat produkt...", "cbx3");
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
                c.b02ID = BO.BAS.InInt(v.ComboB02ID.SelectedValue);
                c.p28ID = BO.BAS.InInt(v.ComboP28ID.SelectedValue);

                c.ValidFrom = v.Rec.ValidFrom;
                var d = new DateTime(3000,1,1);
                if (v.Rec.ValidUntil != null) d = Convert.ToDateTime((v.Rec.ValidUntil));
                c.ValidUntil = d.AddDays(1).AddMinutes(-1);

                v.Rec.pid = Factory.p21LicenseBL.Save(c);
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
                v.ComboP28ID = new MyComboViewModel("p28", v.ComboP28ID.SelectedValue, v.ComboP28ID.SelectedText, "cbx1");
                v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbx2");
                v.ComboB02ID.Param1 = "p21";
                v.ComboSelectP10ID = new MyComboViewModel("p10", "", "Přidat produkt...", "cbx3");
                v.ComboSelectP10ID.OnChange_Event = "handle_append_product";

                return View(v);
            }


        }
    }
}