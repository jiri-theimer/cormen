using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class o53Controller : BaseController
    {
        ///KATEGORIE
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o53RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o53TagGroupBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.o53TagGroup();                
                v.Rec.entity = "o53";
               
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);            
           
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o53RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o53TagGroup c = new BO.o53TagGroup();
                if (v.Rec.pid > 0) c = Factory.o53TagGroupBL.Load(v.Rec.pid);

                
                c.o53Name = v.Rec.o53Name;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o53TagGroupBL.Save(c);
                if (v.Rec.pid > 0)
                {
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