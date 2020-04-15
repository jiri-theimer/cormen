﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p13Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p13PreviewViewModel();
            v.Rec = Factory.p13MasterTpvBL.Load(pid);
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
            var v = new Models.p13RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p13MasterTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p13MasterTpv();
                v.Rec.entity = "p13";
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
                BO.p13MasterTpv c = new BO.p13MasterTpv();
                if (v.Rec.pid > 0) c = Factory.p13MasterTpvBL.Load(v.Rec.pid);

                c.p13Code = v.Rec.p13Code;
                c.p13Name = v.Rec.p13Name;
                c.p13Memo = v.Rec.p13Memo;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p13MasterTpvBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "p13" });
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);
                }
                
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);

            return View(v);
        }
    }
}