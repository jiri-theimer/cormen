using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p19Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }

            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p19RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p19MaterialBL.Load(pid);

                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p19", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;


                if (Factory.CurrentUser.j03EnvironmentFlag == 2 && v.Rec.p28ID != Factory.CurrentUser.p28ID)
                {
                    return this.StopPage(true, "V režimu [CLIENT] nemáte oprávnění editovat tuto kartu materiálu");
                }

            }
            else
            {
                v.Rec = new BO.p19Material();
                v.Rec.entity = "p19";
                v.Rec.p19Code = Factory.CBL.EstimateRecordCode("p19");
                if (Factory.CurrentUser.j03EnvironmentFlag == 2)
                {//klientský režim
                    v.Rec.p28ID = Factory.CurrentUser.p28ID;
                    v.Rec.p28Name = Factory.CurrentUser.p28Name;
                }
                

            }


            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p19Code = Factory.CBL.EstimateRecordCode("p19"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p19RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p19Material c = new BO.p19Material();
                if (v.Rec.pid > 0) c = Factory.p19MaterialBL.Load(v.Rec.pid);

                c.p19Code = v.Rec.p19Code;
                c.p19Name = v.Rec.p19Name;
                c.p19Memo = v.Rec.p19Memo;
                c.p20ID = v.Rec.p20ID;
                
                c.p28ID = v.Rec.p28ID;

                c.p19Supplier = v.Rec.p19Supplier;
                c.p19Intrastat = v.Rec.p19Intrastat;
                c.p19NameAlias = v.Rec.p19NameAlias;
                c.p19ITSINC = v.Rec.p19ITSINC;
                c.p19ITSCAS = v.Rec.p19ITSCAS;
                c.p19ITSEINECS = v.Rec.p19ITSEINECS;

                c.p19Lang1 = v.Rec.p19Lang1;
                c.p19Lang2 = v.Rec.p19Lang2;
                c.p19Lang3 = v.Rec.p19Lang3;
                c.p19Lang4 = v.Rec.p19Lang4;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p19MaterialBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p19", v.Rec.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}