using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p13Controller : BaseController
    {
        public IActionResult Preview(int pid)
        {
            var v = new Models.p13PreviewViewModel();
            v.Rec = Factory.p13TpvBL.Load(pid);
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
            var v = new Models.p13RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p13TpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return this.StopPage(false, "Hledaný záznam neexistuje!");
                }

            }
            else
            {
                v.Rec = new BO.p13Tpv();

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p13RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p13Tpv c = new BO.p13Tpv();
                if (v.Rec.pid > 0) c = Factory.p13TpvBL.Load(v.Rec.pid);

                c.p13Code = v.Rec.p13Code;
                c.p13Name = v.Rec.p13Name;
                c.p13Memo = v.Rec.p13Memo;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p13TpvBL.Save(c);
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

                return View(v);
            }
        }
    }
}