using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p27Controller : BaseController
    {

        public IActionResult Index(int pid)
        {
            var v = new Models.p27PreviewViewModel();
            v.Rec = Factory.p27MszUnitBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                var tg = Factory.o51TagBL.GetTagging("p27", pid);
                v.Rec.TagHtml = tg.TagHtml;
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p27RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p27MszUnitBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p27", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;
            }
            else
            {
                v.Rec = new BO.p27MszUnit();
                v.Rec.entity = "p27";
                
            }

            RefreshState(v);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p27RecordViewModel v)
        {

            if (ModelState.IsValid)
            {
                BO.p27MszUnit c = new BO.p27MszUnit();
                if (v.Rec.pid > 0) c = Factory.p27MszUnitBL.Load(v.Rec.pid);

                c.p27Code = v.Rec.p27Code;
                c.p27Name = v.Rec.p27Name;
                c.p27Capacity = v.Rec.p27Capacity;
                c.p25ID_HW = v.Rec.p25ID_HW;
                c.p31ID = v.Rec.p31ID;



                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p27MszUnitBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p27", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            RefreshState(v);

            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(p27RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);


        }
    }
}