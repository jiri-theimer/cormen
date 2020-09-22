using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p52Controller : BaseController
    {
        public IActionResult BatchInsertByP11(string p11ids)
        {
            //hromadné založení položek objednávky z výběru produktů
            var v = new p52BatchInsertByP11() { p11ids = p11ids };
            if (BO.BAS.ConvertString2ListInt(p11ids).Count==0)
            {
                return this.StopPage(true, "p11ids missing.");
            }
            var arr = BO.BAS.ConvertString2ListInt(v.p11ids);

            RefreshState_BatchInsertByP11(v);
            return View(v);
        }
        [HttpPost]
        public IActionResult BatchInsertByP11(p52BatchInsertByP11 v, string oper)
        {
            RefreshState_BatchInsertByP11(v);
            if (ModelState.IsValid)
            {
                if (v.SelectedP51ID==0)
                {
                    this.AddMessage("Musíte vybrat objednávku.");return View(v);
                }
                if (v.p52DateNeeded == null)
                {
                    this.AddMessage("Musíte zadat Datum potřeby."); return View(v);
                }
                if (v.lisP52.Where(p => p.p52UnitsCount <= 0).Count() > 0)
                {
                    this.AddMessage("Množství položky musí být větší než nula."); return View(v);
                }
                int x = 0;
                foreach(var c in v.lisP52)
                {
                    c.p52DateNeeded = v.p52DateNeeded;
                    c.p51ID = v.SelectedP51ID;                   
                    x+=Factory.p52OrderItemBL.Save(c);
                }
                if (x>0)
                {
                    v.SetJavascript_CallOnLoad(v.SelectedP51ID);
                }

            }
            return View(v);
        }

        private void RefreshState_BatchInsertByP11(p52BatchInsertByP11 v)
        {
            if (v.lisP52 == null)
            {
                v.lisP52 = new List<BO.p52OrderItem>();
                var arr = BO.BAS.ConvertString2ListInt(v.p11ids);
                foreach (int intP11ID in arr)
                {
                    var recP11 = Factory.p11ClientProductBL.Load(intP11ID);
                    var c = new BO.p52OrderItem() { p11ID = intP11ID, p11Code = recP11.p11Code, p11Name = recP11.p11Name,p52UnitsCount=recP11.p11Davka };
                    v.lisP52.Add(c);
                }
            }
        }

        //položka objednávky
        public IActionResult Record(int pid, int p51id, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p52RecordViewModelcs();
            if (pid > 0)
            {
                v.Rec = Factory.p52OrderItemBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p52OrderItem();
                v.Rec.entity = "p52";
                v.Rec.p51ID = p51id;

            }
            v.RecP51 = Factory.p51OrderBL.Load(v.Rec.p51ID);

            v.Toolbar = new MyToolbarViewModel(v.Rec) { IsToArchive = false };
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p52RecordViewModelcs v, string rec_oper)
        {
            if (rec_oper == "postback")
            {
                v.RecP51 = Factory.p51OrderBL.Load(v.Rec.p51ID);
                if (v.Rec.p11ID > 0)
                {
                    var cP11 = Factory.p11ClientProductBL.Load(v.Rec.p11ID);
                    v.Rec.p20Code = cP11.p20Code;
                    v.Rec.p11RecalcUnit2Kg = cP11.p11RecalcUnit2Kg;
                }
                
                v.Toolbar = new MyToolbarViewModel(v.Rec) { IsToArchive = false };
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p52OrderItem c = new BO.p52OrderItem();
                if (v.Rec.pid > 0) c = Factory.p52OrderItemBL.Load(v.Rec.pid);

                c.p52Code = v.Rec.p52Code;
                c.p51ID = v.Rec.p51ID;
                c.p11ID = v.Rec.p11ID;
                c.p52UnitsCount = v.Rec.p52UnitsCount;
                c.p52DateNeeded = v.Rec.p52DateNeeded;

                v.Rec.pid = Factory.p52OrderItemBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.p51ID);
                    return View(v);
                }


            }

            v.Toolbar = new MyToolbarViewModel(v.Rec) { IsToArchive = false };


            this.Notify_RecNotSaved();
            return View(v);

        }

      
    }
}