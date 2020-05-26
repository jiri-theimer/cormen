﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p52Controller : BaseController
    {
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