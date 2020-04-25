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

        public IActionResult Index(int pid)
        {
            var v = new Models.p26PreviewViewModel();
            v.Rec = Factory.p26MszBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.p26RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p26MszBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p26Msz();
                v.Rec.entity = "p26";
            }

            RefreshState(v);

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
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;

                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p26MszBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p26" });
                }
               
                
            }

            RefreshState(v);

            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(p26RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.ComboP28ID = new TheComboViewModel() { Entity = "p28Company", CallerIDValue = "Rec_p28ID", CallerIDText = "Rec_p28Name", SelectedText = v.Rec.p28Name, SelectedValue = v.Rec.p28ID.ToString() };
            v.ComboB02ID = new TheComboViewModel() { Entity = "b02Status", CallerIDValue = "Rec_b02ID", CallerIDText = "Rec_b02Name", SelectedText = v.Rec.b02Name, SelectedValue = v.Rec.b02ID.ToString(), Param1 = "p26" };

        }
    }
}