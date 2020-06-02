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
                var tg = Factory.o51TagBL.GetTagging("p26", pid);
                v.Rec.TagHtml = tg.TagHtml;
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true,true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p26RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p26MszBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p26", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

            }
            else
            {
                v.Rec = new BO.p26Msz();
                v.Rec.entity = "p26";
                v.Rec.p26Code = Factory.CBL.EstimateRecordCode("p26");
            }

            RefreshState(v);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p26Code = Factory.CBL.EstimateRecordCode("p26"); }


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
                c.p25ID = v.Rec.p25ID;
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;
                
                c.p31ID = v.Rec.p31ID;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p26MszBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p26", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
               
                
            }

            RefreshState(v);

            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(p26RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
           

        }
    }
}